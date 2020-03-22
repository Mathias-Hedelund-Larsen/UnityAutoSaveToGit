using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using HephaestusForge.ReadOnly;
using System.Collections.Generic;

namespace HephaestusForge.GitWorks
{
    /// <summary>
    /// The timer for when to do the git commands
    /// </summary>
    public sealed class AutoGitTimerAndHistory : ScriptableObject
    {
#pragma warning disable 0649

        [SerializeField, ReadOnly]
        private double _countdownTimer;

        [SerializeField, ReadOnly]
        private double _targetTime;

        [SerializeField, ReadOnly]
        private double _warningTimer;

        [SerializeField, ReadOnly]
        private double _warningTargetTime;

        [SerializeField]
        private double _secondsToDelay = 30;

        [SerializeField, ReadOnly]
        private List<string> _history = new List<string>();

#pragma warning restore 0649

        /// <summary>
        /// Creating an instance of this scriptable object if none exists, this is called in the creation menu in the editor.
        /// </summary>
        [MenuItem("Assets/Create/HephaestusForge/Limited to one/AutoGitTimerAndHistory", false, 0)]
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

        /// <summary>
        /// Adding the timer to the editor application update
        /// </summary>
        public void EditorInit()
        {
            if(_countdownTimer <= 0 && _targetTime <= EditorApplication.timeSinceStartup)
            {
                _countdownTimer = _secondsToDelay;
                _targetTime = _secondsToDelay + EditorApplication.timeSinceStartup;
                UnityEngine.Debug.Log($"Starting timer, waiting for: {_secondsToDelay} seconds");
            }

            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }

        /// <summary>
        /// Added to the Editor application update
        /// </summary>
        private void EditorUpdate()
        {
            if (_countdownTimer > 0)
            {
                _countdownTimer = _targetTime - EditorApplication.timeSinceStartup;
                _warningTimer = _warningTargetTime - EditorApplication.timeSinceStartup;

                if(_countdownTimer < 10 && _warningTimer <= 0)
                {
                    UnityEngine.Debug.LogWarning($"Git update coming in: {(int)_countdownTimer + 1} seconds");
                    _warningTargetTime = EditorApplication.timeSinceStartup + 1;
                }

                if (_countdownTimer <= 0 && _targetTime <= EditorApplication.timeSinceStartup)
                {
                    Thread runGitCommands = new Thread(() =>
                    {
                        if (RunGitCommand(@"add -A"))
                        {
                            string time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                            if (RunGitCommand($"commit -m \"{time}\""))
                            {
                                if (RunGitCommand("pull"))
                                {
                                    if (RunGitCommand("push"))
                                    {
                                        _history.Add(time);
                                    }
                                }
                            }
                        }
                    });

                    runGitCommands.Start();

                    EditorApplication.update -= EditorUpdate;
                }
            }
        }

        /// <summary>
        /// Running the git command through a process
        /// </summary>
        /// <param name="gitCommand">The command of for either add, commit, pull, push</param>
        /// <returns>Whether the command was succesful</returns>
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
                    string error = process.StandardError.ReadToEnd();
                    string output = process.StandardOutput.ReadToEnd();

                    if (error.Length == 0)
                    {
                        UnityEngine.Debug.Log($"Git command succes msg: {output}");
                        return true;
                    }
                    else if(output.Length > 0)
                    {
                        UnityEngine.Debug.LogError($"Git error was met output was: {output} error was: {error}");
                        return false;
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Git command returned: {error}");
                        return true;
                    }
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
