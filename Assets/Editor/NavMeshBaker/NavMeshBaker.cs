using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;
using System.IO;
using System.Runtime.InteropServices;

public static class NavMeshBaker
{
    //Bakes all Scenes and Prefabs (settings allowing)
    public static bool FullBake(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        if(_settings == null)
        {
            Debug.LogWarning("WHY");
            _errorMsg = "WHY";
            return false;
        }

        _errorMsg = string.Empty;

        PrefabStage _currentOpenPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        string _currentOpenPrefabPath = _currentOpenPrefabStage != null ? _currentOpenPrefabStage.assetPath : string.Empty;
        Scene _activeScene = EditorSceneManager.GetActiveScene();
        string _activeScenePath = _activeScene.path;
        //We run this first because it is the last chance to canel these actions
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogError("[NavMeshBaker] Couldn't build navmeshes, user cancelled saving operation");
            _errorMsg = "Couldn't build navmeshes, user cancelled saving operation";
            return false;
        }

        //We bake prefabs before scenes because scenes can contain prefabs
        //  We do NOT account for recursive prefabs because I don't want to do a topological sort
        if (_settings.BakePrefabsOnFullBake)
        {
            if(!BakePrefabs(_settings, out string _prefabErrorMsg))
            {
                Debug.LogError($"[NavMeshBaker] Failed to Bake Prefabs: {_prefabErrorMsg}");

                if (_settings.StopBakeOnError)
                {
                    _errorMsg = $"Failed to Bake Prefabs: {_prefabErrorMsg}";
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("[NavMeshBaker] Skipped Prefabs in Full Bake");
        }

        if (_settings.BakeScenesOnFullBake)
        {
            if (_settings.BakeOnlyBuildListScenes)
            {
                if(!BakeBuildScenes(_settings, out string _buildScenesErrorMsg))
                {
                    Debug.LogError($"[NavMeshBaker] Failed to Bake Build Scenes: {_buildScenesErrorMsg}");

                    if (_settings.StopBakeOnError)
                    {
                        _errorMsg = $"Failed to Bake Build Scenes: {_buildScenesErrorMsg}";
                        return false;
                    }
                }
            }
            else
            {
                if (!BakeAllAssetScenes(_settings, out string _buildScenesErrorMsg))
                {
                    Debug.LogError($"[NavMeshBaker] Failed to Bake All Scenes: {_buildScenesErrorMsg}");

                    if (_settings.StopBakeOnError)
                    {
                        _errorMsg = $"Failed to Bake All Scenes: {_buildScenesErrorMsg}";
                        return false;
                    }
                }
            }
        }
        else
        {
            Debug.Log("[NavMeshBaker] Skipped Scenes in Full Bake");
        }

        //NOTE: I believe this does not need to be done because building inherently reloads scenes properly
        //Before we can reopen our initial active scene, we have to open up a new one to ensure a clean reload
        //EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        //Reopen initial active scene
        Scene _reOpenedInitialScene = EditorSceneManager.OpenScene(_activeScenePath, OpenSceneMode.Single);
        if (!_reOpenedInitialScene.IsValid())
        {
            Debug.LogWarning($"[NavMeshBaker] Couldn't reopen initial active scene!");
        }

        //NOTE: This does not seem to work. I imagine it has something to do with how builds work
        //      interfering with these operations
        //NOTE: Normal builds also experience this issue, where building kicks you out of prefab view.
        //      I believe that is "intended" behavior from Unity >:(
        //Reopen initial active scene and prefab, if applicable
        /*
        if (_currentOpenPrefabPath != string.Empty)
        {
            //EditorSceneManager.OpenScene(_currentOpenPrefabPath);
            PrefabStageUtility.OpenPrefab(_currentOpenPrefabPath);

            if(PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                Debug.LogWarning($"[NavMeshBaker] Couldn't reopen initial active prefab!");
            }
        }
        */

        return true;
    }

    public static bool BakeAllScenes(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = "";

        Scene _activeScene = EditorSceneManager.GetActiveScene();
        string _activeScenePath = _activeScene.path;
        //We run this first because it is the last chance to canel these actions
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogError("[NavMeshBaker] Couldn't build navmeshes, user cancelled saving operation");
            _errorMsg = "Couldn't build navmeshes, user cancelled saving operation";
            return false;
        }

        if (_settings.BakeOnlyBuildListScenes)
        {
            if (!BakeBuildScenes(_settings, out string _buildScenesErrorMsg))
            {
                Debug.LogError($"[NavMeshBaker] Failed to Bake Build Scenes: {_buildScenesErrorMsg}");

                if (_settings.StopBakeOnError)
                {
                    _errorMsg = $"Failed to Bake Build Scenes: {_buildScenesErrorMsg}";
                    return false;
                }
            }
        }
        else
        {
            if (!BakeAllAssetScenes(_settings, out string _buildScenesErrorMsg))
            {
                Debug.LogError($"[NavMeshBaker] Failed to Bake All Scenes: {_buildScenesErrorMsg}");

                if (_settings.StopBakeOnError)
                {
                    _errorMsg = $"Failed to Bake All Scenes: {_buildScenesErrorMsg}";
                    return false;
                }
            }
        }

        //Reopen initial active scene
        Scene _reOpenedInitialScene = EditorSceneManager.OpenScene(_activeScenePath, OpenSceneMode.Single);
        if (!_reOpenedInitialScene.IsValid())
        {
            Debug.LogWarning($"[NavMeshBaker] Couldn't reopen initial active scene!");
        }

        return true;
    }

    public static bool BakeBuildScenes(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        //Bake meshes for scenes
        foreach (EditorBuildSettingsScene _sceneInfo in EditorBuildSettings.scenes)
        {
            //Only bake scenes that go in the build unless the settings say to build them all
            if (!_settings.BakeOnlyEnabledBuildListScenes || _sceneInfo.enabled)
            {
                if(!BakeScene(_settings, _sceneInfo.path, out string _sceneErrorMsg))
                {
                    Debug.LogError($"[NavMeshBaker] Could not bake Scene {_sceneInfo.path}: {_sceneErrorMsg}");

                    if (_settings.StopBakeOnError)
                    {
                        _errorMsg = $"Could not bake Scene {_sceneInfo.path}: {_sceneErrorMsg}";
                        return false;
                    }
                }
            }
        }

        return true;
    }

    //Tries to find and bake Nav Meshes for every .scene file in the project
    public static bool BakeAllAssetScenes(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        //Bake meshes for prefabs
        string[] _allSceneGuids = AssetDatabase.FindAssets("t:scene");
        string[] _allScenePaths = new string[_allSceneGuids.Length];

        for(int i = 0; i < _allSceneGuids.Length; i++)
        {
            _allScenePaths[i] = AssetDatabase.GUIDToAssetPath(_allSceneGuids[i]);
        }

        if(!BakeScenes(_settings, _allScenePaths, out _errorMsg))
        {
            Debug.LogError($"[NavMeshBaker] Failed to bake all scenes: {_errorMsg}");

            if (_settings.StopBakeOnError)
            {
                _errorMsg = $"Failed to bake all scenes: {_errorMsg}";
                return false;
            }
        }

        return true;
    }

    public static bool BakeScenes(NavMeshBakerSettings _settings, string[] _scenePaths, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        foreach(string _scenePath in _scenePaths)
        {
            if(!BakeScene(_settings, _scenePath, out string _sceneErrorMsg))
            {
                Debug.LogError($"[NavMeshBaker] Error while baking scene [{_scenePath}]: {_sceneErrorMsg}");

                if (_settings.StopBakeOnError)
                {
                    _errorMsg = $"Error while baking scene [{_scenePath}]: {_sceneErrorMsg}";
                    return false;
                }
            }
        }

        return true;
    }

    public static bool BakeScene(NavMeshBakerSettings _settings, string _scenePath, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        Scene _newScene = EditorSceneManager.OpenScene(_scenePath, OpenSceneMode.Single);

        int _numNavMeshesBuilt = BuildNavMeshes(_settings, _scenePath, _newScene.GetRootGameObjects(), out string _buildErrorMsg);
        Debug.Log($"[NavMeshBaker] Built {_numNavMeshesBuilt} navmeshes in Scene {_newScene.name}");
        if (_buildErrorMsg != string.Empty)
        {
            Debug.LogError($"[NavMeshBaker] Exited building in Prefab {_newScene.name} early: {_buildErrorMsg}");
            _errorMsg = $"Exited building in Prefab {_newScene.name} early: {_buildErrorMsg}";
            return false;
        }

        if (!EditorSceneManager.SaveScene(_newScene))
        {
            Debug.LogError($"[NavMeshBaker] Couldn't save changes to {_newScene.name} ({_newScene.path})");

            if (_settings.StopBakeOnError)
            {
                _errorMsg = $"Couldn't save changes to {_newScene.name} ({_newScene.path})";
                return false;
            }
        }

        return true;
    }

    //Determines if a prefab or scene is open, and bakes accordingly
    public static bool BakeOpenView(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        if(PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            return BakeOpenPrefab(_settings, out _errorMsg);
        }
        else
        {
            return BakeOpenScene(_settings, out _errorMsg);
        }
    }

    //Bakes the currently open and focused scene
    public static bool BakeOpenScene(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        Scene _openScene = EditorSceneManager.GetActiveScene();

        int _numNavMeshesBuilt = BuildNavMeshes(_settings, _openScene.path, _openScene.GetRootGameObjects(), out string _buildErrorMsg);
        Debug.Log($"[NavMeshBaker] Built {_numNavMeshesBuilt} navmeshes in Scene {_openScene.name}");
        if (_buildErrorMsg != string.Empty)
        {
            Debug.LogError($"[NavMeshBaker] Exited building in Prefab {_openScene.name} early: {_buildErrorMsg}");
            _errorMsg = $"Exited building in Prefab {_openScene.name} early: {_buildErrorMsg}";
            return false;
        }

        if (!EditorSceneManager.SaveScene(_openScene))
        {
            Debug.LogError($"[NavMeshBaker] Couldn't save changes to {_openScene.name} ({_openScene.path})");

            if (_settings.StopBakeOnError)
            {
                _errorMsg = $"Couldn't save changes to {_openScene.name} ({_openScene.path})";
                return false;
            }
        }

        EditorSceneManager.OpenScene(_openScene.path, OpenSceneMode.Single);

        return true;
    }

    public static bool BakePrefabs(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        //Bake meshes for prefabs
        string[] _allPrefabGuids = AssetDatabase.FindAssets("t:prefab");
        foreach (string _prefabGuid in _allPrefabGuids)
        {
            string _prefabAssetPath = AssetDatabase.GUIDToAssetPath(_prefabGuid);
            GameObject _prefab = PrefabUtility.LoadPrefabContents(_prefabAssetPath);

            int _numNavMeshesBuilt = BakeNavMeshes(_settings, _prefabAssetPath, _prefab, out string _buildErrorMsg);
            Debug.Log($"[NavMeshBaker] Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabAssetPath})");
            if(_buildErrorMsg != string.Empty)
            {
                Debug.LogError($"[NavMeshBaker] Exited building in Prefab {_prefab.name} ({_prefabAssetPath}) early: {_buildErrorMsg}");
                _errorMsg = $"Exited building in Prefab {_prefab.name} ({_prefabAssetPath}) early: {_buildErrorMsg}";
                return false;
            }

            //Save nav mesh to prefab
            PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabAssetPath, out bool _savedSuccessfully);
            //PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabPath, out bool _savedSuccessfully);
            if (!_savedSuccessfully)
            {
                Debug.LogError($"[NavMeshBaker] Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})");

                if (_settings.StopBakeOnError)
                {
                    _errorMsg = $"Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})";
                    return false;
                }
            }

            //Clean up memory
            PrefabUtility.UnloadPrefabContents(_prefab);
        }

        return true;
    }

    //Bakes the currently open and focused prefab
    public static bool BakeOpenPrefab(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        Scene _openScene = EditorSceneManager.GetActiveScene();
        string _openScenePath = _openScene.path;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            //I genuinely have no idea why it won't just let me modify the prefab, but you *have* to reload
            //  the scene to see the changes in prefab instances; otherwise it looks like it failed!
            //  Since we have to reload the scene, we have to save it first or face data loss
            Debug.LogError($"[NavMeshBaker] Couldn't save changes to {_openScene.name} ({_openScenePath}). Can't safely continue bake operation without data loss!");

            if (_settings.StopBakeOnError)
            {
                _errorMsg = $"Couldn't save changes to {_openScene.name} ({_openScenePath}). Can't safely continue bake operation without data loss!";
                return false;
            }
        }

        PrefabStage _prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if( _prefabStage == null)
        {
            Debug.LogError($"[NavMeshBaker] No currently open Prefab to bake");
            _errorMsg = $"No currently open Prefab to bake";
            return false;
        }
        GameObject _prefabRoot = _prefabStage.prefabContentsRoot;
        string _prefabAssetPath = _prefabStage.assetPath;

        StageUtility.GoBackToPreviousStage();
        GameObject _prefab = PrefabUtility.LoadPrefabContents(_prefabAssetPath);

        int _numNavMeshesBuilt = BakeNavMeshes(_settings, _prefabAssetPath, _prefab, out string _buildErrorMsg);
        Debug.Log($"[NavMeshBaker] Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabAssetPath})");
        if (_buildErrorMsg != string.Empty)
        {
            Debug.LogError($"[NavMeshBaker] Exited building in Prefab {_prefab.name} ({_prefabAssetPath}) early: {_buildErrorMsg}");
            _errorMsg = $"Exited building in Prefab {_prefab.name} ({_prefabAssetPath}) early: {_buildErrorMsg}";
            return false;
        }

        //Save nav mesh to prefab
        PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabAssetPath, out bool _savedSuccessfully);
        //PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabPath, out bool _savedSuccessfully);
        if (!_savedSuccessfully)
        {
            Debug.LogError($"[NavMeshBaker] Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})");

            if (_settings.StopBakeOnError)
            {
                _errorMsg = $"Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})";
                return false;
            }
        }

        //Clean up memory
        PrefabUtility.UnloadPrefabContents(_prefab);
        //PrefabUtility.UnloadPrefabContents(_prefabRoot);

        //In a frustrating turn of events, you have to reload the active scene to immediately see changes in the prefab
        //  But to reload the scene fully, you have to open a different one
        //  And we still want to reopen the prefab of course
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        if(_openScenePath != string.Empty)
        {
            EditorSceneManager.OpenScene(_openScenePath, OpenSceneMode.Single);
        }
        PrefabStageUtility.OpenPrefab(_prefabAssetPath);

        return true;
    }

    public static NavMeshSurface[] FindNavMeshSurfaces(NavMeshBakerSettings _settings, GameObject _rootGameObject)
    {
        return _rootGameObject.GetComponentsInChildren<NavMeshSurface>((_settings.BakeNavMeshSurfacesOnInactiveObjects || _settings.BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive));
    }

    public static NavMeshSurface[] FindNavMeshSurfaces(NavMeshBakerSettings _settings, GameObject[] _rootGameObjects)
    {
        List<NavMeshSurface> _navMeshSurfaces = new();
        foreach (GameObject _rootGameObject in _rootGameObjects)
        {
            _navMeshSurfaces.AddRange(FindNavMeshSurfaces(_settings, _rootGameObject));
        }

        return _navMeshSurfaces.ToArray();
    }

    public static int BakeNavMeshes(NavMeshBakerSettings _settings, string _rootAssetPath, NavMeshSurface[] _navMeshSurfaces, out string _errorMsg)
    {
        _errorMsg = "";

        int _numNavMeshesBuilt = 0;
        foreach (NavMeshSurface _navMeshSurface in _navMeshSurfaces)
        {
            //Check if the we can bake this surface based on its parent's active status
            //  I could DeMorgan's this, but it's more clear this way
            if(!(_navMeshSurface.gameObject.activeInHierarchy ||
                _settings.BakeNavMeshSurfacesOnInactiveObjects ||
                (_settings.BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive && _navMeshSurface.gameObject.activeSelf)))
            {
                //Current settings say we can't bake this NavMeshSurface
                continue;
            }

            //Check if we can bake this surface based on the componen'ts active status
            if(!(_navMeshSurface.enabled || _settings.BakeInactiveNavMeshSurfaceComponents))
            {
                //Current settings say we can't bake this NavMeshSurface
                continue;
            }

            string _navMeshDataPath;
            bool _hasExistingNavMesh;

            string _assetExtension = Path.GetExtension(_rootAssetPath);
            bool _isPrefabAsset = _assetExtension.ToLower() == ".prefab";

            if (_navMeshSurface.navMeshData == null)
            {
                _hasExistingNavMesh = false;

                string _assetFolder = Path.GetDirectoryName(_rootAssetPath);
                string _assetName = Path.GetFileNameWithoutExtension(_rootAssetPath);
                //string _assetExtension = Path.GetExtension(_rootAssetPath);

                if (_isPrefabAsset && !_settings.ForcePrefabBakeIntoFolder)
                {
                    //If we're a prefab, and handling prefabs like vanilla NavMesh, we don't put this in a folder
                    _navMeshDataPath = _assetFolder + $"/NavMesh-";
                }
                else
                {
                    //Ensure target directory exists
                    if (!AssetDatabase.IsValidFolder(_assetFolder + "/" + _assetName))
                    {
                        AssetDatabase.CreateFolder(_assetFolder, _assetName);
                    }
                    //If we're not a prefab (e.g. a scene) it goes in a folder
                    _navMeshDataPath = _assetFolder + "/" + _assetName + "/NavMesh-";
                }
            }
            else
            {
                //Since this NavMeshSurface already has a mesh, we should skip it if the setting is true
                if (_settings.OnlyBakeMissingNavMeshes)
                {
                    continue;
                }

                _hasExistingNavMesh = true;

                _navMeshDataPath = AssetDatabase.GetAssetPath(_navMeshSurface.navMeshData);
            }

            if (_navMeshDataPath == string.Empty)
            {
                Debug.LogError($"[NavMeshBaker] Couldn't get nav mesh data asset path!");
                continue;
            }

            //Check if we've got a prefab
            //  Can't reuse _isPrefabAsset because scenes can have prefab instances
            bool _isPartOfPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(_navMeshSurface);
            //If we're part of a prefab and not a prefab asset and we shouldn't bake prefab instances in scenes, skip!
            if (_isPartOfPrefabInstance && !_isPrefabAsset && !_settings.BakePrefabInstancesInScenes)
            {
                continue;
            }

            //If we're part of a prefab and not a prefab asset and we shouldn bake prefab instances in scenes:
            //  If override is true, always bake
            //  If override is false, bake only if the mesh has no data
            if (_isPartOfPrefabInstance && !_isPrefabAsset && (!_settings.OverrideExistingPrefabInstanceBakesInScenes && _navMeshSurface.navMeshData != null))
            {
                continue;
            }

            //If we're a nested prefab instance, we might not want to bake. Check!
            if (_isPartOfPrefabInstance && _settings.IgnoreNavMeshSurfacesInNestedPrefabInstances)
            {
                //If the root of this prefab instance's parent is part of a prefab or prefab instance, that means it's nested, and we shouldn't bake it
                GameObject _prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(_navMeshSurface.gameObject);
                if (_prefabRoot.transform.parent != null && PrefabUtility.IsPartOfAnyPrefab(_prefabRoot.transform.parent))
                {
                    continue;
                }
            }

            //Bake our breand new navmesh!
            _navMeshSurface.BuildNavMesh();

            if (_navMeshSurface.navMeshData != null)
            {
                //Use existing path if one already exists, but recreate if it doesn't
                string _newAssetPath = _hasExistingNavMesh ? _navMeshDataPath : _navMeshDataPath + $"{_navMeshSurface.name}.asset";
                AssetDatabase.CreateAsset(_navMeshSurface.navMeshData, _newAssetPath);

                //The below line doesn't seem to be necessary
                //_navMeshSurface.navMeshData = AssetDatabase.LoadAssetAtPath<NavMeshData>(_newAssetPath);

                //Prefabs need special handling
                if (_isPartOfPrefabInstance)
                {
                    //Prefab instances don't like to save, so we do this to save to individual instances
                    //  This behavior does make them a bit easier to ignore if we want though, as seen right above the bake itself
                    EditorUtility.SetDirty(_navMeshSurface);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_navMeshSurface);
                }
                _numNavMeshesBuilt++;
            }
            else
            {
                Debug.LogError($"[NavMeshBaker] Failed to build nav mesh for {_navMeshSurface.name} ({_rootAssetPath})");

                if (_settings.StopBakeOnError)
                {
                    _errorMsg = $"Failed to build nav mesh for {_navMeshSurface.name} ({_rootAssetPath})";
                    return _numNavMeshesBuilt;
                }
            }
        }

        return _numNavMeshesBuilt;
    }

    public static int BakeNavMeshes(NavMeshBakerSettings _settings, string _rootAssetPath, GameObject _rootGameObject, out string _errorMsg)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_settings, _rootGameObject);
        return BakeNavMeshes(_settings, _rootAssetPath, _navMeshSurfaces, out _errorMsg);
    }

    public static int BuildNavMeshes(NavMeshBakerSettings _settings, string _rootAssetPath, GameObject[] _rootGameObjects, out string _errorMsg)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_settings, _rootGameObjects);
        return BakeNavMeshes(_settings, _rootAssetPath, _navMeshSurfaces, out _errorMsg);
    }
}
