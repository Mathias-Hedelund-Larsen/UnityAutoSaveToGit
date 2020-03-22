using UnityEditor;

namespace HephaestusForge.AutoGit
{
    /// <summary>
    /// Called when the editor reloads the assets
    /// </summary>
    [InitializeOnLoad]
    public static class AutoGitStartTimerCalledOnLoad 
    {
        /// <summary>
        /// Called by unity because of the class attribute
        /// </summary>
        static AutoGitStartTimerCalledOnLoad()
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