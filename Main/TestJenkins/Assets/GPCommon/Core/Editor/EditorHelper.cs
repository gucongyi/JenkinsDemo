using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using Debug = UnityEngine.Debug;
using Process = System.Diagnostics.Process;

namespace GPCommon
{
    public static class EditorHelper
    {
        public static void SpaceLayout(int count)
        {
            for (int i = 0; i < count; i++)
                EditorGUILayout.Space();
        }

        public static void VecticalLayout(Action action)
        {
            EditorGUILayout.BeginVertical();
            action();
            EditorGUILayout.EndVertical();
        }

        public static void HorizontalLayout(Action action)
        {
            EditorGUILayout.BeginHorizontal();
            action();
            EditorGUILayout.EndHorizontal();
        }

        public static void PreparePathDirectory(string assetOrFilePath)
        {
            string outputDirectory = Path.GetDirectoryName(assetOrFilePath);

            // Check if directory exist
            if (!Directory.Exists(outputDirectory))
            {
                // Create it 
                Directory.CreateDirectory(outputDirectory);
            }
        }

        public static bool ValidateFolder(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        public static bool ValidateFile(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public static string DrawStringField(string label, string content, int width = -1)
        {
            HorizontalLayout(() =>
            {
                if (width == -1)
                {
                    EditorGUILayout.LabelField(string.Format("{0}: ", label));
                }
                else
                {
                    EditorGUILayout.LabelField(string.Format("{0}: ", label), GUILayout.Width(width));
                }

                content = EditorGUILayout.TextField(content);
            });

            return content == null ? null : content.Trim();
        }

        public static int DrawIntField(string label, int content, int width = -1)
        {
            HorizontalLayout(() =>
            {
                if (width == -1)
                {
                    EditorGUILayout.LabelField(string.Format("{0}: ", label));
                }
                else
                {
                    EditorGUILayout.LabelField(string.Format("{0}: ", label), GUILayout.Width(width));
                }

                content = EditorGUILayout.IntField(content);
            });

            return content;
        }

        public static bool IsStartAsNumber(string content)
        {
            char s = content[0];
            bool isMatch = Regex.IsMatch(s.ToString(), @"[0-9]");
            return isMatch;
        }

        public static bool IsLegalVariableName(string content)
        {
            return !Regex.IsMatch(content, @"[^\w+$]") && !IsStartAsNumber(content);
        }

        public static void ExecuteBashScript(string command, out string output)
        {
            Debug.Log(command);

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "/bin/bash",
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            process.Start();

            output = process.StandardOutput.ReadToEnd();
            Debug.Log(output);

            process.WaitForExit();
        }

        public static void ProcessCmdCommand(string fileName, string argument)
        {
            var start = new ProcessStartInfo(fileName)
            {
                Arguments = argument,
                CreateNoWindow = false,
                ErrorDialog = true,
                UseShellExecute = true
            };

            if (start.UseShellExecute)
            {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            }
            else
            {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = Encoding.UTF8;
                start.StandardErrorEncoding = Encoding.UTF8;
            }

            var p = Process.Start(start);
            if (p == null) return;

            p.WaitForExit();
            p.Close();
        }
    }
}