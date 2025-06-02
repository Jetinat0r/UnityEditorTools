using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NavMeshBakerSettings : ScriptableObject
{
    //Constant string keys to access NavMeshBaker's settings in ProjectPreferences
    public static class StringIdentifiers
    {
        //HANDLED BY STOP_BAKE_ON_ASSET_SAVE_FAILURE
        //Whether or not to continue with other navmeshes when an error is encountered with one navmesh
        //public const string IGNORE_BAKE_ERRORS = "IgnoreBakeErrors";

        //Whether or not to perform a full bake of NavMeshes on Build
        public const string BAKE_ON_BUILD = "BakeOnBuild";
        //Whether or not to bake NavMeshSurfaces in scenes on full build (and prebuild)
        public const string BAKE_SCENES_ON_FULL_BAKE = "BakeScenesOnFullBake";
        //Whether or not to bake NavMeshSurfaces in prefabs on full bake (and prebuild)
        public const string BAKE_PREFABS_ON_FULL_BAKE = "BakePrefabsOnFullBake";

        //Whether or not to only bake for NavMeshSurfaces that don't have an associated NavMesh
        public const string ONLY_BAKE_MISSING_NAV_MESHES = "OnlyBakeMissingNavMeshes";

        //Stops baking operations as soon as any error occurs
        public const string STOP_BAKE_ON_ERROR = "StopBakeOnError";

        //Whether or not the bake nav mesh surfaces if they are disabled
        public const string BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS = "BakeNavMeshSurfacesOnInactiveObjects";
        //Override applied only if BakeNavMeshSurfacesOnInactiveObjects is false to bake a surface on an object if it is enabled and its parent is not
        public const string BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE = "BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive";

        //Whether or not to bake disabled NavMeshSurface components
        //  Will only bake if the GameObject it is attached to is enabled or is allowed to bake by the above settings
        public const string BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS = "BakeInactiveNavMeshSurfaceComponents";
        
        //Bake only scenes in the build list or every scene
        public const string BAKE_ONLY_BUILD_LIST_SCENES = "BakeOnlyBuildListScenes";
        //When only baking build list scenes, whether or not to ignore disabled scenes in the list
        public const string BAKE_ONLY_ENABLED_BUILD_LIST_SCENES = "BakeOnlyEnabledBuildListScenes";

        //Whether or not to bake prefab instances within scenes
        //  If override is false, only bakes nav mesh data to prefabs instances with missing navmesh data
        public const string BAKE_PREFAB_INSTANCES_IN_SCENES = "BakePrefabInstancesInScenes";
        //If baking prefab instances in scenes, whether or not to replace a bake if it already has one
        //  Ignored if OnlyBakeMissingNavMeshes is true and the prefab instance has a NavMesh
        public const string OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES = "OverrideExistingPrefabInstanceBakesInScenes";

        //Makes prefab bakes behave like scenes, where baked NavMeshes are placed inside a folder next to the prefab
        //  Ignored if the prefab's NavMeshes already have bakes
        public const string FORCE_PREFAB_BAKE_INTO_FOLDER = "ForcePrefabBakeIntoFolder";
        //Whether or not to ignore NavMeshSurfaces in prefab instances nested inside prefabs
        public const string IGNORE_NAV_MESH_SURFACES_IN_NESTED_PREFAB_INSTANCES = "IgnoreNavMeshSurfacesInNestedPrefabInstances";
    }

    //Constants that represent the default values for each setting
    public static class Defaults
    {
        //HANDLED BY STOP_BAKE_ON_ASSET_SAVE_FAILURE
        //Whether or not to continue with other navmeshes when an error is encountered with one navmesh
        //public const bool IGNORE_BAKE_ERRORS = false;

        //Whether or not to perform a full bake of NavMeshes on Build
        public const bool BAKE_ON_BUILD = true;
        //Whether or not to bake NavMeshSurfaces in scenes on full bake (and prebuild)
        public const bool BAKE_SCENES_ON_FULL_BAKE = true;
        //Whether or not to bake NavMeshSurfaces in prefabs on full bake (and prebuild)
        public const bool BAKE_PREFABS_ON_FULL_BAKE = true;

        //Whether or not to only bake for NavMeshSurfaces that don't have an associated NavMesh
        public const bool ONLY_BAKE_MISSING_NAV_MESHES = false;

        //Stops baking operations as soon as any error occurs
        public const bool STOP_BAKE_ON_ERROR = true;

        //Whether or not the bake nav mesh surfaces if they are disabled
        public const bool BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS = false;
        //Override applied only if BakeNavMeshSurfacesOnInactiveObjects is false to bake a surface on an object if it is enabled and its parent is not
        public const bool BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE = true;

        //Whether or not to bake disabled NavMeshSurface components
        //  Will only bake if the GameObject it is attached to is enabled or is allowed to bake by the above settings
        public const bool BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS = false;

        //Bake only scenes in the build list or every scene
        public const bool BAKE_ONLY_BUILD_LIST_SCENES = true;
        //When only baking build list scenes, whether or not to ignore disabled scenes in the list
        public const bool BAKE_ONLY_ENABLED_BUILD_LIST_SCENES = true;

        //Whether or not to bake prefab instances within scenes
        //  If override is false, only bakes nav mesh data to prefabs instances with missing navmesh data
        public const bool BAKE_PREFAB_INSTANCES_IN_SCENES = false;
        //If baking prefab instances in scenes, whether or not to replace a bake if it already has one
        //  Ignored if OnlyBakeMissingNavMeshes is true and the prefab instance has a NavMesh
        public const bool OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES = true;

        //Makes prefab bakes behave like scenes, where baked NavMeshes are placed inside a folder next to the prefab
        //  Ignored if the prefab's NavMeshes already have bakes
        public const bool FORCE_PREFAB_BAKE_INTO_FOLDER = true;
        //Whether or not to ignore NavMeshSurfaces in prefab instances nested inside prefabs
        public const bool IGNORE_NAV_MESH_SURFACES_IN_NESTED_PREFAB_INSTANCES = true;
    }

    //Constant path for saved settings asset
    public const string NAV_MESH_BAKER_SETTINGS_PATH = "Assets/Editor/NavMeshBakerSettings.asset";

    //HANDLED BY STOP_BAKE_ON_ASSET_SAVE_FAILURE
    //[Tooltip("Whether or not to continue with other navmeshes when an error is encountered with one navmesh")]
    //public const bool IgnoreBakeErrors = Defaults.IGNORE_BAKE_ERRORS;

    [Tooltip("Whether or not to perform a full bake of NavMeshes on Build")]
    public bool BakeOnBuild = Defaults.BAKE_ON_BUILD;
    [Tooltip("Whether or not to bake NavMeshSurfaces in scenes on full bake (and prebuild)")]
    public bool BakeScenesOnFullBake = Defaults.BAKE_SCENES_ON_FULL_BAKE;
    [Tooltip("Whether or not to bake NavMeshSurfaces in prefabs on full bake (and prebuild)")]
    public bool BakePrefabsOnFullBake = Defaults.BAKE_PREFABS_ON_FULL_BAKE;

    [Tooltip("Whether or not to only bake for NavMeshSurfaces that don't have an associated NavMesh")]
    public bool OnlyBakeMissingNavMeshes = Defaults.ONLY_BAKE_MISSING_NAV_MESHES;

    [Tooltip("Stops baking operations as soon as any error occurs")]
    public bool StopBakeOnError = Defaults.STOP_BAKE_ON_ERROR;

    [Tooltip("Whether or not the bake nav mesh surfaces if they are disabled")]
    public bool BakeNavMeshSurfacesOnInactiveObjects = Defaults.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS;
    [Tooltip("Override applied only if BakeNavMeshSurfacesOnInactiveObjects is false to bake a surface on an object if it is enabled and its parent is not")]
    public bool BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive = Defaults.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE;

    [Tooltip("Whether or not to bake disabled NavMeshSurface components\n" +
                "Will only bake if the GameObject it is attached to is enabled or is allowed to bake by the above settings")]
    public bool BakeInactiveNavMeshSurfaceComponents = Defaults.BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS;

    [Tooltip("Bake only scenes in the build list or every scene")]
    public bool BakeOnlyBuildListScenes = Defaults.BAKE_ONLY_BUILD_LIST_SCENES;
    [Tooltip("When only baking build list scenes, whether or not to ignore disabled scenes in the list")]
    public bool BakeOnlyEnabledBuildListScenes = Defaults.BAKE_ONLY_ENABLED_BUILD_LIST_SCENES;

    [Tooltip("Whether or not to bake prefab instances within scenes\n" +
                "If override is false, only bakes nav mesh data to prefabs instances with missing navmesh data")]
    public bool BakePrefabInstancesInScenes = Defaults.BAKE_PREFAB_INSTANCES_IN_SCENES;
    [Tooltip("If baking prefab instances in scenes, whether or not to replace a bake if it already has one\n" +
                "Ignored if OnlyBakeMissingNavMeshes is true and the prefab instance has a NavMesh")]
    public bool OverrideExistingPrefabInstanceBakesInScenes = Defaults.OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES;

    [Tooltip("Makes prefab bakes behave like scenes, where baked NavMeshes are placed inside a folder next to the prefab\n" +
                "Ignored if the prefab's NavMeshes already have bakes")]
    public bool ForcePrefabBakeIntoFolder = Defaults.FORCE_PREFAB_BAKE_INTO_FOLDER;
    [Tooltip("Whether or not to ignore NavMeshSurfaces in prefab instances nested inside prefabs")]
    public bool IgnoreNavMeshSurfacesInNestedPrefabInstances = Defaults.IGNORE_NAV_MESH_SURFACES_IN_NESTED_PREFAB_INSTANCES;

    public static NavMeshBakerSettings GetOrCreateSettings()
    {
        NavMeshBakerSettings _settings = AssetDatabase.LoadAssetAtPath<NavMeshBakerSettings>(NAV_MESH_BAKER_SETTINGS_PATH);
        if(_settings == null)
        {
            _settings = CreateInstance<NavMeshBakerSettings>();
            AssetDatabase.CreateAsset(_settings, NAV_MESH_BAKER_SETTINGS_PATH);
            AssetDatabase.SaveAssets();
        }

        return _settings;
    }

    //If _writeAsset is true: Forcefully overwrites any existing settings object with a brand new default one to NAV_MESH_BAKER_SETTINGS_PATH
    public static NavMeshBakerSettings CreateSettings(bool _writeAsset = false)
    {
        NavMeshBakerSettings _settings = CreateInstance<NavMeshBakerSettings>();
        if (_writeAsset)
        {
            AssetDatabase.CreateAsset(_settings, NAV_MESH_BAKER_SETTINGS_PATH);
            AssetDatabase.SaveAssets();
        }

        return _settings;
    }

    public static SerializedObject GetOrCreateSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    //If _writeAsset is true: Forcefully overwrites any existing settings object with a brand new default one to NAV_MESH_BAKER_SETTINGS_PATH
    public static SerializedObject CreateNewSerializedSettings(bool _writeAsset = false)
    {
        return new SerializedObject(CreateSettings(_writeAsset));
    }

    //Resets the settings on the passed in instance of the settings
    public void ResetToDefaults()
    {
        //Reset all settings!
        //  Might be simpler to just make a new object,
        //  but we love options and it might hurt when changing existing saved assets
        BakeOnBuild = Defaults.BAKE_ON_BUILD;
        BakeScenesOnFullBake = Defaults.BAKE_SCENES_ON_FULL_BAKE;
        BakePrefabsOnFullBake = Defaults.BAKE_PREFABS_ON_FULL_BAKE;

        OnlyBakeMissingNavMeshes = Defaults.ONLY_BAKE_MISSING_NAV_MESHES;

        StopBakeOnError = Defaults.STOP_BAKE_ON_ERROR;

        BakeNavMeshSurfacesOnInactiveObjects = Defaults.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS;
        BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive = Defaults.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE;
        
        BakeInactiveNavMeshSurfaceComponents = Defaults.BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS;

        BakeOnlyBuildListScenes = Defaults.BAKE_ONLY_BUILD_LIST_SCENES;
        BakeOnlyEnabledBuildListScenes = Defaults.BAKE_ONLY_ENABLED_BUILD_LIST_SCENES;

        BakePrefabInstancesInScenes = Defaults.BAKE_PREFAB_INSTANCES_IN_SCENES;
        OverrideExistingPrefabInstanceBakesInScenes = Defaults.OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES;

        ForcePrefabBakeIntoFolder = Defaults.FORCE_PREFAB_BAKE_INTO_FOLDER;
        IgnoreNavMeshSurfacesInNestedPrefabInstances = Defaults.IGNORE_NAV_MESH_SURFACES_IN_NESTED_PREFAB_INSTANCES;
    }
}

public class NavMeshBakerSettingsProvider : SettingsProvider
{
    public SerializedObject navMeshBakerSettings;

    public NavMeshBakerSettingsProvider(string path, SettingsScope scope = SettingsScope.Project, IEnumerable<string> keywords = null) : base(path, scope, keywords) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        navMeshBakerSettings = NavMeshBakerSettings.GetOrCreateSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        float _oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 384;

        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.ONLY_BAKE_MISSING_NAV_MESHES));
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS));
        if (!navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS).boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS));
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_ON_BUILD));
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.STOP_BAKE_ON_ERROR));
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_SCENES_ON_FULL_BAKE));
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_PREFABS_ON_FULL_BAKE));
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_ONLY_BUILD_LIST_SCENES));
        if (navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_ONLY_BUILD_LIST_SCENES).boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_ONLY_ENABLED_BUILD_LIST_SCENES));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Prefab Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_PREFAB_INSTANCES_IN_SCENES));
        if (navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.BAKE_PREFAB_INSTANCES_IN_SCENES).boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.FORCE_PREFAB_BAKE_INTO_FOLDER));
        EditorGUILayout.PropertyField(navMeshBakerSettings.FindProperty(NavMeshBakerSettings.StringIdentifiers.IGNORE_NAV_MESH_SURFACES_IN_NESTED_PREFAB_INSTANCES));
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bake Nav Meshes", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Bake Scenes"))
        {
            NavMeshBaker.BakeAllScenes((NavMeshBakerSettings)navMeshBakerSettings.targetObject, out string _sceneError);
        }
        if (GUILayout.Button("Bake Prefabs"))
        {
            NavMeshBaker.BakePrefabs((NavMeshBakerSettings)navMeshBakerSettings.targetObject, out string _prefabError);
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Bake All"))
        {
            NavMeshBaker.FullBake((NavMeshBakerSettings)navMeshBakerSettings.targetObject, out string _allError);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        Color _oldColor = GUI.color;
        Color _oldBackgroundColor = GUI.backgroundColor;
        //Color picked straight from GitHub :)
        GUI.color = new Color(248 / 255f, 81 / 255f, 73 / 255f);
        EditorGUILayout.LabelField("DANGER ZONE", EditorStyles.boldLabel);
        GUI.color = _oldColor;

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Reset Settings"))
        {
            navMeshBakerSettings = NavMeshBakerSettings.CreateNewSerializedSettings(true);
        }
        GUI.backgroundColor = _oldBackgroundColor;

        EditorGUIUtility.labelWidth = _oldLabelWidth;

        navMeshBakerSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    [SettingsProvider]
    public static SettingsProvider CreateNavMeshBakerSettingsProvider()
    {
        return new NavMeshBakerSettingsProvider("Project/NavMeshBaker", SettingsScope.Project);
    }
}
