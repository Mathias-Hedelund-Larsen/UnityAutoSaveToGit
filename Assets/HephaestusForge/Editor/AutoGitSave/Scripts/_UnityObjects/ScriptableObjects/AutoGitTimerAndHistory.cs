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
            if (_countdown > 0)
            {
                _countdown = _targetTime - EditorApplication.timeSinceStartup;

                if (_countdown <= 0 && _targetTime <= EditorApplication.timeSinceStartup)
                {
                    if (RunGitCommand(@"add -A"))
                    {
                        if (RunGitCommand($"commit -m \"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}\""))
                        {
                            if (RunGitCommand("pull"))
                            {
                                RunGitCommand("push");
                            }
                        }
                    }

                    EditorApplication.update -= EditorUpdate;
                }
            }
        }

        private bool RunGitCommand(string gitCommand)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("git", gitCommand)
            {
                CreateNoWindow = true,          
                UseShellExecute = false,        
                RedirectStandardOutput = true,  
                RedirectStandardError = true    
            };

            using (Process process = new Process() { StartInfo = processInfo })
            {
                try
                {
                    process.Start();
                    process.WaitForExit();

                    UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());

                    return true;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                    return false;
                }
            }
        }
    }
}