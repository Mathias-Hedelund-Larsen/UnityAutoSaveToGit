using UnityEditor;

namespace HephaestusForge.AutoGit
{
    [InitializeOnLoad]
    public static class AutoGitStartTimer 
    {
        static AutoGitStartTimer()
        {
            var guids = AssetDatabase.FindAssets("t:AutoGitTimerAndHistory");

            if (guids.Length == 1)
            {
                var autoGitTimer = AssetDatabase.LoadAssetAtPath<AutoGitTimerAndHistory>(AssetDatabase.GUIDToAssetPath(guids[0]));
                EditorApplication.delayCall += () => autoGitTimer.EditorInit();                
            }
        }
    }
}