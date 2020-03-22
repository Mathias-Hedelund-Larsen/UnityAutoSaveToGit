using System.Linq;
using UnityEditor;

namespace HephaestusForge.AutoGit
{
    public class AutoGitStartTimerProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!importedAssets.Any(s => s.Contains("AutoGitTimerAndHistory")) && !deletedAssets.Any(s => s.Contains("AutoGitTimerAndHistory")) &&
                !movedAssets.Any(s => s.Contains("AutoGitTimerAndHistory")) && !movedFromAssetPaths.Any(s => s.Contains("AutoGitTimerAndHistory")))
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
}