using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Unity.AI.Navigation;
using Unity.AI.Navigation.Editor;
using UnityEngine.AI;
using System.Threading;
using System.IO;

class PreBuildNavMeshBaker : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport _report)
    {
        Debug.Log("PreBuildNavMeshBaker.OnPreprocessBuild for target " + _report.summary.platform + " at path " + _report.summary.outputPath);
        
        Scene _activeScene = EditorSceneManager.GetActiveScene();
        string _activeScenePath = _activeScene.path;
        //We run this first because it is the last chance to canel these actions
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            throw new BuildFailedException("Couldn't build navmeshes, user cancelled saving operation!");
        }

        string[] _allPrefabPaths = AssetDatabase.FindAssets("t:prefab");
        foreach (string _prefabGuid in _allPrefabPaths)
        {
            string _prefabAssetPath = AssetDatabase.GUIDToAssetPath(_prefabGuid);
            //GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabAssetPath);

            //PrefabStage _prefabStage = PrefabStageUtility.OpenPrefab(_prefabAssetPath, null, PrefabStage.Mode.InIsolation);
            //Debug.LogWarning($"STAGE: {_prefabStage.name} {_prefabStage.assetPath}");
            //StageUtility.GoToStage(_prefabStage, false);

            /*
            GameObject _prefab = _prefabStage.prefabContentsRoot;
            _prefab.GetComponent<NavMeshSurface>()?.BuildNavMesh();
            Debug.LogWarning($"PATH: {AssetDatabase.GetAssetPath(_prefab.GetComponent<NavMeshSurface>().navMeshData.GetInstanceID())}");
            //int _numNavMeshesBuilt = BuildNavMeshes(_prefab, true);
            //Debug.Log($"Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabAssetPath})");

            
            PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabAssetPath, out bool _savedSuccessfully);
            if (!_savedSuccessfully)
            {
                Debug.LogError($"Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})");
            }
            */

            //StageUtility.GoBackToPreviousStage();
            //EditorUtility.SetDirty(_prefabStage.prefabContentsRoot);

            //Debug.Log($"Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabPath})");
            //return;
            /*
            Scene _prefabScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            _prefab.sce

            int _numNavMeshesBuilt = BuildNavMeshes(_prefab, true);
            Debug.Log($"Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabAssetPath})");

            PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabAssetPath, out bool _savedSuccessfully);
            if (!_savedSuccessfully)
            {
                Debug.LogError($"Failed to save changes to prefab {_prefab.name} ({_prefabAssetPath})");
            }
            */

            //return;

            /*
            
            //Scene _prefabScene = EditorSceneManager.NewPreviewScene();
            //Scene _prefabScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            //StageUtility.GoToStage(_prefabScene, false);
            string _prefabPath = AssetDatabase.GUIDToAssetPath(_prefabGuid);
            GameObject _prefab = PrefabUtility.LoadPrefabContents(_prefabPath);
            //PrefabUtility.LoadPrefabContentsIntoPreviewScene(_prefabPath, _prefabScene);
            Stage _prefabStage = StageUtility.GetStage(_prefab);
            StageUtility.GoToStage(_prefabStage, false);
            //GameObject _prefab = _prefabScene.GetRootGameObjects()[0];

            //Scene _prefabScene = EditorSceneManager.OpenScene(_prefab.scene.path, OpenSceneMode.Single);
            //GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPath);
            //GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPath);
            Debug.Log($"Loaded prefab into: {_prefab.scene.name}");

            int _numNavMeshesBuilt = BuildNavMeshes(_prefab, true);
            Debug.Log($"Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabPath})");
            GameObject _new = new();
            _new.transform.SetParent(_prefab.transform);
            //Save nav mesh to prefab
            PrefabUtility.SavePrefabAsset(_prefab, out bool _savedSuccessfully);
            //PrefabUtility.SaveAsPrefabAsset(_prefab, _prefabPath, out bool _savedSuccessfully);
            if (!_savedSuccessfully)
            {
                Debug.LogError($"Failed to save changes to prefab {_prefab.name} ({_prefabPath})");
            }
            
            */

            //Clean up memory
            //PrefabUtility.UnloadPrefabContents(_prefab);
            //StageUtility.GoBackToPreviousStage();
            /*
            if (!EditorSceneManager.ClosePreviewScene(_prefabScene))
            {
                Debug.LogError($"Couldn't close prefab preview scene!");
            }
            */
        }

        //return;

        foreach(EditorBuildSettingsScene _sceneInfo in EditorBuildSettings.scenes)
        {
            //Ignore scenes not in build
            if (_sceneInfo.enabled)
            {
                Scene _newScene = EditorSceneManager.OpenScene(_sceneInfo.path, OpenSceneMode.Single);
                
                int _numNavMeshesBuilt = BuildNavMeshes(_sceneInfo.path, _newScene.GetRootGameObjects(), true);
                Debug.Log($"Built {_numNavMeshesBuilt} navmeshes in Scene {_newScene.name}");

                if (!EditorSceneManager.SaveScene(_newScene))
                {
                    Debug.LogError($"Couldn't save changes to {_newScene.name} ({_newScene.path}");
                }
            }
        }

        //Reopen initial active scene
        Scene _reOpenedInitialScene = EditorSceneManager.OpenScene(_activeScenePath, OpenSceneMode.Single);
        if (!_reOpenedInitialScene.IsValid())
        {
            Debug.LogWarning($"Couldn't reopen initial active scene!");
        }
    }

    public NavMeshSurface[] FindNavMeshSurfaces(GameObject _rootGameObject, bool _includeInactiveChildren)
    {
        return _rootGameObject.GetComponentsInChildren<NavMeshSurface>(_includeInactiveChildren);
    }

    public NavMeshSurface[] FindNavMeshSurfaces(GameObject[] _rootGameObjects, bool _includeInactiveChildren)
    {
        List<NavMeshSurface> _navMeshSurfaces = new();
        foreach(GameObject _rootGameObject in _rootGameObjects)
        {
            _navMeshSurfaces.AddRange(FindNavMeshSurfaces(_rootGameObject, _includeInactiveChildren));
        }

        return _navMeshSurfaces.ToArray();
    }

    public int BuildNavMeshes(string _rootAssetPath, NavMeshSurface[] _navMeshSurfaces)
    {
        int _numNavMeshesBuilt = 0;
        foreach (NavMeshSurface _navMeshSurface in _navMeshSurfaces)
        {
            string _navMeshDataPath;
            bool _hasExistingNavMesh;
            if(_navMeshSurface.navMeshData == null)
            {
                _hasExistingNavMesh = false;

                string _assetFolder = Path.GetDirectoryName(_rootAssetPath);
                string _assetName = Path.GetFileNameWithoutExtension(_rootAssetPath);
                string _assetExtension = Path.GetExtension(_rootAssetPath);

                if(_assetExtension.ToLower() == ".prefab")
                {
                    //If we're a prefab, we don't put this in a folder
                    _navMeshDataPath = _assetFolder + $"NavMesh-";
                }
                else
                {
                    //Ensure target directory exists
                    if(!AssetDatabase.IsValidFolder(_assetFolder + "/" + _assetName))
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

            if(_navMeshDataPath == string.Empty)
            {
                Debug.LogError($"Couldn't get nav mesh data asset path!");
                continue;
            }

            _navMeshSurface.BuildNavMesh();

            if(_navMeshSurface.navMeshData != null)
            {
                //Use existing path if one already exists, but recreate if it doesn't
                string _newAssetPath = _hasExistingNavMesh ? _navMeshDataPath : _navMeshDataPath + $"{_navMeshSurface.name}.asset";
                AssetDatabase.CreateAsset(_navMeshSurface.navMeshData, _newAssetPath);

                //We've got a prefab!
                if(PrefabUtility.IsPartOfPrefabInstance(_navMeshSurface))
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

    public int BuildNavMeshes(string _rootAssetPath, GameObject _rootGameObject, bool _includeInactiveChildren)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_rootGameObject, _includeInactiveChildren);
        return BuildNavMeshes(_rootAssetPath, _navMeshSurfaces);
    }

    public int BuildNavMeshes(string _rootAssetPath, GameObject[] _rootGameObjects, bool _includeInactiveChildren)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_rootGameObjects, _includeInactiveChildren);
        return BuildNavMeshes(_rootAssetPath, _navMeshSurfaces);
    }
}
