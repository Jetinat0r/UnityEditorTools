using UnityEngine;
using UnityEditor;
using System;

public class NavMeshBakerWindow : EditorWindow
{
    [MenuItem("Tools/NavMeshBakerWindow")]
    public static void ShowWindow()
    {
        GetWindow<NavMeshBakerWindow>("NavMeshBakerWindow");
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit Settings for Bake");
        if (GUILayout.Button("Open Settings"))
        {
            SettingsService.OpenProjectSettings("Project/NavMeshBaker");
        }

        EditorGUILayout.Space();
        GUILayout.Label("Bake in Current Scene/Prefab");
        if (GUILayout.Button("Bake Current View"))
        {
            NavMeshBaker.BakeOpenView((NavMeshBakerSettings)NavMeshBakerSettings.GetOrCreateSerializedSettings().targetObject, out string _sceneError);
        }
    }
}
