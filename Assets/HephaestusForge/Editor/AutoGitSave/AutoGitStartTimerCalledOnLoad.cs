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
<<<<<<< HEAD
        /// So lets see if there is a conflict
=======
        /// Testing for merge conflict I hope
>>>>>>> 3ef3cbca9fd31d5de6fb23a2ebb279f606e78931
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
