using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GlobalNavMeshBakerWindow : EditorWindow
{
    [MenuItem("Tools/GlobalNavMeshBakerWindow")]
    public static void ShowWindow()
    {
        GetWindow<GlobalNavMeshBakerWindow>("GlobalNavMeshBakerWindow");
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
