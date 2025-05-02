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
        
        string[] _prefabs = AssetDatabase.FindAssets("t:prefab");
        foreach (string _prefab in _prefabs)
        {
            Debug.Log($"Prefab Path: {_prefab}");
        }

        Scene _activeScene = EditorSceneManager.GetActiveScene();

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            throw new BuildFailedException("Couldn't build navmeshes, user cancelled saving operation!");
        }
        else
        {
        }

        /*
        if(!EditorSceneManager.CloseScene(_activeScene, false))
        {
            throw new BuildFailedException($"Couldn't close active scene!");
        }
        */

        foreach(EditorBuildSettingsScene _scene in EditorBuildSettings.scenes)
        {
            //Ignore scenes not in build
            if (_scene.enabled)
            {
                if(_activeScene.path == _scene.path)
                {
                    Debug.Log("Hit duplicate scene!");

                    foreach (GameObject _obj in _activeScene.GetRootGameObjects())
                    {
                        Debug.Log($"Object: {_obj}");
                    }

                    return;
                }
                Debug.Log($"Loading Scene: {_scene.path}");
                Scene _newScene = EditorSceneManager.OpenScene(_scene.path, OpenSceneMode.AdditiveWithoutLoading);
                
                foreach(GameObject _obj in _newScene.GetRootGameObjects())
                {
                    Debug.Log($"Object: {_obj}");
                }

                if(EditorSceneManager.CloseScene(_newScene, false))
                {
                    Debug.Log("Successfully closed scene!");
                }
                else
                {
                    Debug.Log("Couldn't close scene!");
                }
            }

            
        }

        
    }
}
