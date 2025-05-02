using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        /* Prefabs; disabled bc they're broken
         * TODO: Fix
        string[] _allPrefabPaths = AssetDatabase.FindAssets("t:prefab");
        foreach (string _prefabGuids in _allPrefabPaths)
        {
            string _prefabPath = AssetDatabase.GUIDToAssetPath(_prefabGuids);
            GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPath);

            int _numNavMeshesBuilt = BuildNavMeshes(_prefab, true);
            Debug.Log($"Built {_numNavMeshesBuilt} in Prefab {_prefab.name} ({_prefabPath})");
        }
        */


        foreach(EditorBuildSettingsScene _sceneInfo in EditorBuildSettings.scenes)
        {
            //Ignore scenes not in build
            if (_sceneInfo.enabled)
            {
                Scene _newScene = EditorSceneManager.OpenScene(_sceneInfo.path, OpenSceneMode.Single);
                
                int _numNavMeshesBuilt = BuildNavMeshes(_newScene.GetRootGameObjects(), true);
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

    public int BuildNavMeshes(NavMeshSurface[] _navMeshSurfaces)
    {
        int _numNavMeshesBuilt = 0;
        foreach (NavMeshSurface _navMeshSurface in _navMeshSurfaces)
        {
            _navMeshSurface.BuildNavMesh();
            _numNavMeshesBuilt++;
        }

        return _numNavMeshesBuilt;
    }

    public int BuildNavMeshes(GameObject _rootGameObject, bool _includeInactiveChildren)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_rootGameObject, _includeInactiveChildren);
        return BuildNavMeshes(_navMeshSurfaces);
    }

    public int BuildNavMeshes(GameObject[] _rootGameObjects, bool _includeInactiveChildren)
    {
        NavMeshSurface[] _navMeshSurfaces = FindNavMeshSurfaces(_rootGameObjects, _includeInactiveChildren);
        return BuildNavMeshes(_navMeshSurfaces);
    }
}
