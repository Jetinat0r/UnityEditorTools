public class NavMeshBakerSettings
{
    public static class StringIdentifiers
    {
        public const string BAKE_NAV_MESH_SURFACES_IN_INACTIVE_OBJECTS = "BakeNavMeshSurfacesInInactiveObjects";
    }

    public bool BakeNavMeshSurfacesInInactiveObjects = false;

    public static NavMeshBakerSettings GetProjectSettings()
    {
        NavMeshBakerSettings _projectSettings = new NavMeshBakerSettings();

        return _projectSettings;
    }
}
