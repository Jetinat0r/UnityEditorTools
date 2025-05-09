using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class PreBuildNavMeshBaker : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport _report)
    {
        Debug.Log("PreBuildNavMeshBaker.OnPreprocessBuild for target " + _report.summary.platform + " at path " + _report.summary.outputPath);

        NavMeshBakerSettings _editorSettings = new NavMeshBakerSettings();

        if(!NavMeshBaker.BakeAll(_editorSettings, out string _errorMsg))
        {
            throw new BuildFailedException(_errorMsg);
        }
    }

    
}
