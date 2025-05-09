public class NavMeshBakerSettings
{
    //Constant string keys to access NavMeshBaker's settings in ProjectPreferences
    public static class StringIdentifiers
    {
        //Whether or not to bake NavMeshes on Build
        public const string BAKE_ON_BUILD = "BakeOnBuild";
        //Whether or not to bake NavMeshSurfaces in scenes on full build (and prebuild)
        public const string BAKE_SCENES_ON_FULL_BUILD = "BakeScenesOnFullBuild";
        //Whether or not to bake NavMeshSurfaces in prefabs on full build (and prebuild)
        public const string BAKE_PREFABS_ON_FULL_BUILD = "BakePrefabsOnFullBuild";

        //Whether or not to only bake for NavMeshSurfaces that don't have an associated NavMesh
        public const string ONLY_BAKE_MISSING_NAV_MESHES = "OnlyBakeMissingNavMeshes";

        //Stops baking operations as soon as any object fails to save
        public const string STOP_BAKE_ON_ASSET_SAVE_FAILURE = "StopBakeOnAssetSaveFailure";

        //Whether or not the bake nav mesh surfaces if they are disabled
        public const string BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS = "BakeNavMeshSurfacesOnInactiveObjects";
        //Override applied only if BakeNavMeshSurfacesInInactiveObjects is true to bake a surface if it is enabled and its parent is not
        public const string BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE = "BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive";
        
        //Whether or not to bake disabled NavMeshSurface components
        //  Independent of object whether or not the GameObject it is attached to is enabled
        public const string BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS = "BakeInactiveNavMeshSurfaceComponents";
        
        //Bake only scenes in the build list or every scene
        public const string BAKE_ONLY_BUILD_LIST_SCENES = "BakeOnlyBuildListScenes";
        //When only baking build list scenes, whether or not to ignore disabled scenes in the list
        public const string BAKE_ONLY_ENABLED_BUILD_LIST_SCENES = "BakeOnlyEnabledBuildListScenes";
        
        //Whether or not to bake prefab instances within scenes
        public const string BAKE_PREFAB_INSTANCES_IN_SCENES = "BakePrefabInstancesInScenes";
        //If baking prefab instances in scenes, whether or not to replace a bake if it already has one
        //  Ignored if OnlyBakeMissingNavMeshes is true
        public const string OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES = "OverrideExistingPrefabInstanceBakesInScenes";

        //Makes prefab bakes behave like scenes, where baked NavMeshes are placed inside a folder next to the prefab
        //  Ignored if the prefab's NavMeshes already have bakes
        public const string FORCE_PREFAB_BAKE_INTO_FOLDER = "ForcePrefabBakeIntoFolder";
        //Whether or not to ignore NavMeshSurfaces in prefab instances nested inside prefabs
        public const string IGNORE_NAV_MESH_SURFACES_IN_PREFAB_INSTANCES_IN_PREFABS = "IgnoreNavMeshSurfacesInPrefabInstancesInPrefabs";
    }

    //Constants that represent the default values for each setting
    public static class Defaults
    {
        //Whether or not to bake NavMeshes on Build
        public const bool BAKE_ON_BUILD = true;
        //Whether or not to bake NavMeshSurfaces in scenes on full build (and prebuild)
        public const bool BAKE_SCENES_ON_FULL_BUILD = true;
        //Whether or not to bake NavMeshSurfaces in prefabs on full build (and prebuild)
        public const bool BAKE_PREFABS_ON_FULL_BUILD = true;

        //Whether or not to only bake for NavMeshSurfaces that don't have an associated NavMesh
        public const bool ONLY_BAKE_MISSING_NAV_MESHES = false;

        //Stops baking operations as soon as any object fails to save
        public const bool STOP_BAKE_ON_ASSET_SAVE_FAILURE = true;

        //Whether or not the bake nav mesh surfaces if they are disabled
        public const bool BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS = false;
        //Override applied only if BakeNavMeshSurfacesInInactiveObjects is true to bake a surface if it is enabled and its parent is not
        public const bool BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE = true;

        //Whether or not to bake disabled NavMeshSurface components
        //  Independent of object whether or not the GameObject it is attached to is enabled
        public const bool BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS = false;

        //Bake only scenes in the build list or every scene
        public const bool BAKE_ONLY_BUILD_LIST_SCENES = true;
        //When only baking build list scenes, whether or not to ignore disabled scenes in the list
        public const bool BAKE_ONLY_ENABLED_BUILD_LIST_SCENES = true;

        //Whether or not to bake prefab instances within scenes
        public const bool BAKE_PREFAB_INSTANCES_IN_SCENES = false;
        //If baking prefab instances in scenes, whether or not to replace a bake if it already has one
        //  Ignored if OnlyBakeMissingNavMeshes is true
        public const bool OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES = true;

        //Makes prefab bakes behave like scenes, where baked NavMeshes are placed inside a folder next to the prefab
        //  Ignored if the prefab's NavMeshes already have bakes
        public const bool FORCE_PREFAB_BAKE_INTO_FOLDER = true;
        //Whether or not to ignore NavMeshSurfaces in prefab instances nested inside prefabs
        public const bool IGNORE_NAV_MESH_SURFACES_IN_PREFAB_INSTANCES_IN_PREFABS = true;
    }

    //Whether or not to bake NavMeshes on Build
    public bool BakeOnBuild = Defaults.BAKE_ON_BUILD;
    //Whether or not to bake NavMeshSurfaces in scenes on full build (and prebuild)
    public bool BakeScenesOnFullBuild = Defaults.BAKE_SCENES_ON_FULL_BUILD;
    //Whether or not to bake NavMeshSurfaces in prefabs on full build (and prebuild)
    public bool BakePrefabsOnFullBuild = Defaults.BAKE_PREFABS_ON_FULL_BUILD;

    //Whether or not to only bake for NavMeshSurfaces that don't have an associated NavMesh
    public bool OnlyBakeMissingNavMeshes = Defaults.ONLY_BAKE_MISSING_NAV_MESHES;

    //Stops baking operations as soon as any object fails to save
    public bool StopBakeOnAssetSaveFailure = Defaults.STOP_BAKE_ON_ASSET_SAVE_FAILURE;

    //Whether or not the bake nav mesh surfaces if they are disabled
    public bool BakeNavMeshSurfacesOnInactiveObjects = Defaults.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS;
    //Override applied only if BakeNavMeshSurfacesInInactiveObjects is true to bake a surface if it is enabled and its parent is not
    public bool BakeNavMeshSurfacesOnInactiveObjectsIfSelfIsActive = Defaults.BAKE_NAV_MESH_SURFACES_ON_INACTIVE_OBJECTS_IF_SELF_IS_ACTIVE;

    //Whether or not to bake disabled NavMeshSurface components
    //  Independent of object whether or not the GameObject it is attached to is enabled
    public bool BakeInactiveNavMeshSurfaceComponents = Defaults.BAKE_INACTIVE_NAV_MESH_SURFACE_COMPONENTS;

    //Bake only scenes in the build list or every scene
    public bool BakeOnlyBuildListScenes = Defaults.BAKE_ONLY_BUILD_LIST_SCENES;
    //When only baking build list scenes, whether or not to ignore disabled scenes in the list
    public bool BakeOnlyEnabledBuildListScenes = Defaults.BAKE_ONLY_ENABLED_BUILD_LIST_SCENES;

    //Whether or not to bake prefab instances within scenes
    public bool BakePrefabInstancesInScenes = Defaults.BAKE_PREFAB_INSTANCES_IN_SCENES;
    //If baking prefab instances in scenes, whether or not to replace a bake if it already has one
    //  Ignored if OnlyBakeMissingNavMeshes is true
    public bool OverrideExistingPrefabInstanceBakesInScenes = Defaults.OVERRIDE_EXISTING_PREFAB_INSTANCE_BAKES_IN_SCENES;

    //Makes prefab bakes behave like scenes, where baked NavMeshes are placed inside a folder next to the prefab
    //  Ignored if the prefab's NavMeshes already have bakes
    public bool ForcePrefabBakeIntoFolder = Defaults.FORCE_PREFAB_BAKE_INTO_FOLDER;
    //Whether or not to ignore NavMeshSurfaces in prefab instances nested inside prefabs
    public bool IgnoreNavMeshSurfacesInPrefabInstancesInPrefabs = Defaults.IGNORE_NAV_MESH_SURFACES_IN_PREFAB_INSTANCES_IN_PREFABS;

    public static NavMeshBakerSettings GetProjectSettings()
    {
        NavMeshBakerSettings _projectSettings = new NavMeshBakerSettings();

        return _projectSettings;
    }
}
