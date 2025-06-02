using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class PreBuildNavMeshBaker : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport _report)
    {
        NavMeshBakerSettings _editorSettings = (NavMeshBakerSettings)NavMeshBakerSettings.GetOrCreateSerializedSettings().targetObject;
        
        //Exit early if we don't want to bake on build
        if (!_editorSettings.BakeOnBuild)
        {
            Debug.Log("PreBuildNavMeshBaker.OnPreprocessBuild skipped: BakeOnBuild is false.");
            return;
        }

        Debug.Log("PreBuildNavMeshBaker.OnPreprocessBuild for target " + _report.summary.platform + " at path " + _report.summary.outputPath);


        if(!NavMeshBaker.FullBake(_editorSettings, out string _errorMsg))
        {
            throw new BuildFailedException(_errorMsg);
        }
    }
}
