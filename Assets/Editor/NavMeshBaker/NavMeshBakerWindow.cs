using UnityEngine;
using UnityEditor;

public class NavMeshBakerWindow : EditorWindow
{
    [MenuItem("Tools/GlobalNavMeshBakerWindow")]
    public static void ShowWindow()
    {
        GetWindow<NavMeshBakerWindow>("GlobalNavMeshBakerWindow");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bake Settings");
        //GUILayout.Toggle(,);

        if(GUILayout.Button("Bake All Nav Meshes"))
        {
            Debug.Log("Pressed!");
            
        }

    }
}
