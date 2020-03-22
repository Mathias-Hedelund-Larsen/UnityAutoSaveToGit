using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

namespace HephaestusForge.AutoGit
{
    public sealed class AutoGitTimerAndHistory : ScriptableObject
    {
#pragma warning disable 0649

        [SerializeField]
        private double _countdown;

        [SerializeField]
        private double _targetTime;

        [SerializeField]
        private double _secondsToDelay = 30;

#pragma warning restore 0649

        [MenuItem("Assets/Create/HephaestusForge/LimitToOne/AutoGitTimerAndHistory", false, 0)]
        private static void CreateInstance()
        {
            if (AssetDatabase.FindAssets("t:AutoGitTimerAndHistory").Length == 0)
            {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (path.Length > 0)
                {
                    var obj = CreateInstance<AutoGitTimerAndHistory>();

                    if (Directory.Exists(path))
                    {
                        AssetDatabase.CreateAsset(obj, path + "/AutoGitTimerAndHistory.asset");

                        return;
                    }

                    var pathSplit = path.Split('/').ToList();
                    pathSplit.RemoveAt(pathSplit.Count - 1);
                    path = string.Join("/", pathSplit);

                    if (Directory.Exists(path))
                    {
                        AssetDatabase.CreateAsset(obj, path + "/AutoGitTimerAndHistory.asset");

                        return;
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("An instance of AutoGitTimerAndHistory already exists");
                }
            }
        }

        public void EditorInit()
        {
            if(_countdown <= 0 && _targetTime <= EditorApplication.timeSinceStartup)
            {
                _countdown = _secondsToDelay;
                _targetTime = _secondsToDelay + EditorApplication.timeSinceStartup;
                UnityEngine.Debug.Log($"Starting timer, waiting for: {_secondsToDelay} seconds");
            }

            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }

        public void EditorUpdate()
        {
            _countdown = _targetTime - EditorApplication.timeSinceStartup;

            if(_countdown <= 0 && _targetTime <= EditorApplication.timeSinceStartup)
            {
                RunGitCommand(@"add .");
                RunGitCommand($"commit -m \"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}\"");
                RunGitCommand("push");
            }
        }

        private void RunGitCommand(string gitCommand)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("git", gitCommand)
            {
                CreateNoWindow = true,          // We want no visible pop-ups
                UseShellExecute = false,        // Allows us to redirect input, output and error streams
                RedirectStandardOutput = true,  // Allows us to read the output stream
                RedirectStandardError = true    // Allows us to read the error stream
            };

            Process process = new Process() { StartInfo = processInfo};

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }
    }
}