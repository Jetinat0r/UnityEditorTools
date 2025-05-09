using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Unity.AI.Navigation;
using System.IO;

public static class NavMeshBaker
{
    public static bool BakeAll(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        Scene _activeScene = EditorSceneManager.GetActiveScene();
        string _activeScenePath = _activeScene.path;
        //We run this first because it is the last chance to canel these actions
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            _errorMsg = "Couldn't build navmeshes, user cancelled saving operation!";
            return false;
        }

        //We bake prefabs before scenes because scenes can contain prefabs
        //  We do NOT account for recursive prefabs because I don't want to do a topological sort
        //  Though honestly with how nav meshes work, I don't even think it matters which one is first :P
        if(!BakePrefabs(_settings, out string _prefabErrorMsg))
        {
            Debug.LogError(_prefabErrorMsg);
        }

        if(!BakeBuildScenes(_settings, out string _buildScenesErrorMsg))
        {
            Debug.LogError(_buildScenesErrorMsg);
        }

        //Reopen initial active scene
        Scene _reOpenedInitialScene = EditorSceneManager.OpenScene(_activeScenePath, OpenSceneMode.Single);
        if (!_reOpenedInitialScene.IsValid())
        {
            Debug.LogWarning($"Couldn't reopen initial active scene!");
        }

        return true;
    }

    public static bool BakeBuildScenes(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        //Bake meshes for scenes
        foreach (EditorBuildSettingsScene _sceneInfo in EditorBuildSettings.scenes)
        {
            //Ignore scenes not in build
            if (_sceneInfo.enabled)
            {
                if(!BakeScene(_settings, _sceneInfo.path, out string _sceneErrorMsg))
                {
                    //TODO: Handle errors
                }
            }
        }

        return true;
    }

    public static bool BakeAllScenes(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;
        //TODO: This
        return true;
    }

    public static bool BakeScenes(NavMeshBakerSettings _settings, string[] _scenePaths, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        foreach(string _scenePath in _scenePaths)
        {
            if(!BakeScene(_settings, _scenePath, out string _sceneErrorMsg))
            {
                //TODO: Handle errors
            }
        }

        return true;
    }

    public static bool BakeScene(NavMeshBakerSettings _settings, string _scenePath, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        Scene _newScene = EditorSceneManager.OpenScene(_scenePath, OpenSceneMode.Single);

        int _numNavMeshesBuilt = BuildNavMeshes(_settings, _scenePath, _newScene.GetRootGameObjects());
        Debug.Log($"Built {_numNavMeshesBuilt} navmeshes in Scene {_newScene.name}");

        if (!EditorSceneManager.SaveScene(_newScene))
        {
            Debug.LogError($"Couldn't save changes to {_newScene.name} ({_newScene.path}");
        }

        return true;
    }

    public static bool BakePrefabs(NavMeshBakerSettings _settings, out string _errorMsg)
    {
        _errorMsg = string.Empty;

        //Bake meshes for prefabs
        string[] _allPrefabPaths = AssetDatabase.FindAssets("t:prefab");
        foreach (string _prefabGuid in _allPrefabPaths)
        {
            string _prefabAssetPath = AssetDatabase.GUIDToAssetPath(_prefabGuid);
            GameObject _prefab = PrefabUtility.LoadPrefabContents(_prefabAssetPath);

            int _numNavMeshesBuilt = BuildNavMeshes(_settings, _prefabAssetPath, _prefab);
            Debug.Log($"Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabAssetPath})");

            //Save nav mesh to prefab
            PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabAssetPath, out bool _savedSuccessfully);
            //PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabPath, out bool _savedSuccessfully);
            if (!_savedSuccessfully)
            {
                Debug.LogError($"Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})");
            }

            //Clean up memory
            PrefabUtility.UnloadPrefabContents(_prefab);
        }

        return true;
    }

    public static NavMeshSurface[] FindNavMeshSurfaces(NavMeshBakerSettings _settings, GameObject _rootGameObject)
    {
        return _rootGameObject.GetComponentsInChildren<NavMeshSurface>();
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

    public static int BuildNavMeshes(NavMeshBakerSettings _settings, string _rootAssetPath, NavMeshSurface[] _navMeshSurfaces)
    {
        int _numNavMeshesBuilt = 0;
        foreach (NavMeshSurface _navMeshSurface in _navMeshSurfaces)
        {
            string _navMeshDataPath;
            bool _hasExistingNavMesh;
            if (_navMeshSurface.navMeshData == null)
            {
                _hasExistingNavMesh = false;

                string _assetFolder = Path.GetDirectoryName(_rootAssetPath);
                string _assetName = Path.GetFileNameWithoutExtension(_rootAssetPath);
                string _assetExtension = Path.GetExtension(_rootAssetPath);

                if (_assetExtension.ToLower() == ".prefab")
                {
                    //If we're a prefab, we don't put this in a folder
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
                _hasExistingNavMesh = true;

                _navMeshDataPath = AssetDatabase.GetAssetPath(_navMeshSurface.navMeshData);
            }

            if (_navMeshDataPath == string.Empty)
            {
                Debug.LogError($"Couldn't get nav mesh data asset path!");
                continue;
            }

            _navMeshSurface.BuildNavMesh();

            if (_navMeshSurface.navMeshData != null)
            {
                //Use existing path if one already exists, but recreate if it doesn't
                string _newAssetPath = _hasExistingNavMesh ? _navMeshDataPath : _navMeshDataPath + $"{_navMeshSurface.name}.asset";
                AssetDatabase.CreateAsset(_navMeshSurface.navMeshData, _newAssetPath);

                //We've got a prefab!
                if (PrefabUtility.IsPartOfPrefabInstance(_navMeshSurface))
                {
                    //Prefab instances don't like to save, so we do this to save to individual instances
                    //  This behavior does make them a bit easier to ignore if we want though :)
                    EditorUtility.SetDirty(_navMeshSurface);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_navMeshSurface);
                }
                //The below line doesn't seem to be necessary
                //_navMeshSurface.navMeshData = AssetDatabase.LoadAssetAtPath<NavMeshData>(_newAssetPath);
                _numNavMeshesBuilt++;
            }
            else
            {
                Debug.LogError($"Failed to build nav mesh for {_navMeshSurface.name} ({_rootAssetPath})!");
            }
        }

        return _numNavMeshesBuilt;
    }

    public static int BuildNavMeshes(NavMeshBakerSettings _settings, string _rootAssetPath, GameObject _rootGameObject)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_settings, _rootGameObject);
        return BuildNavMeshes(_settings, _rootAssetPath, _navMeshSurfaces);
    }

    public static int BuildNavMeshes(NavMeshBakerSettings _settings, string _rootAssetPath, GameObject[] _rootGameObjects)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_settings, _rootGameObjects);
        return BuildNavMeshes(_settings, _rootAssetPath, _navMeshSurfaces);
    }
}
