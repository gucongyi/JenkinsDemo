using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GPCommon
{
    public class ResItemCodeMakerWindow : EditorWindow
    {
        private static ResItemCodeMakerWindow _instance;

        [MenuItem("GPCommon/ResItemCodeMaker &G")]
        public static void Init()
        {
            _instance = GetWindow<ResItemCodeMakerWindow>("ResCode");
            _instance.Setup();
            _instance.Show();
        }

        [MenuItem("Assets/GPCommon-UpdateResItemCode")]
        public static void ExecuteItemCodeCreator()
        {
            _instance = GetWindow<ResItemCodeMakerWindow>("ResCode");
            _instance.Execute();
            _instance.Close();
        }

        private readonly string _configPath = "Assets/GPCommon/ResItemCodeMaker/Editor/Config.asset";

        #region Config Manage

        private ResItemCodeMakerConfig _curConfig;

        private void Setup()
        {
            GetCurrentConfig();
        }

        private void GetCurrentConfig()
        {
            // Load default config
            _curConfig = LoadConfig(_configPath);

            // Create default config
            if (_curConfig == null)
            {
                CreateConfig(_configPath);
                LoadConfig(_configPath);
            }
        }

        private void CreateConfig(string desPath)
        {
            // Prepare directory
            EditorHelper.PreparePathDirectory(desPath);

            // Replace with new
            if (File.Exists(desPath))
            {
                AssetDatabase.DeleteAsset(desPath);
            }

            // Create new instance
            ResItemCodeMakerConfig asset = CreateInstance<ResItemCodeMakerConfig>();

            // Create asset
            AssetDatabase.CreateAsset(asset, desPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private ResItemCodeMakerConfig LoadConfig(string path)
        {
            _curConfig = AssetDatabase.LoadAssetAtPath(path, typeof(ResItemCodeMakerConfig)) as ResItemCodeMakerConfig;
            return _curConfig;
        }

        #endregion

        #region OnGUI

        private bool _isDirty;

        private readonly List<ResItemCodeMakerConfig.CreatorTask> _pendingDeleteList =
            new List<ResItemCodeMakerConfig.CreatorTask>();

        void OnGUI()
        {
            if (_curConfig == null)
                return;

            EditorGUI.BeginChangeCheck();

            EditorHelper.VecticalLayout(() =>
            {
                DrawCreatorTaskList(_curConfig.taskList);

                DeletePendingItem();

                EditorHelper.SpaceLayout(3);
            });

            _isDirty = EditorGUI.EndChangeCheck();

            if (_isDirty)
            {
                EditorUtility.SetDirty(_curConfig);
                _isDirty = false;
            }

            GUIHelper.DrawButton("Add Task", () =>
            {
                _curConfig.taskList.Add(new ResItemCodeMakerConfig.CreatorTask());
                _isDirty = true;
            }, Color.blue);

            GUIHelper.DrawButton("Execute", Execute, Color.green);
        }

        private void DrawCreatorTaskList(List<ResItemCodeMakerConfig.CreatorTask> list)
        {
            EditorHelper.VecticalLayout(() =>
            {
                // Draw Items
                foreach (ResItemCodeMakerConfig.CreatorTask item in list)
                {

                    EditorHelper.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(string.Format("Item Folder Path:{0}", item.FolderFilePath));

                        GUIHelper.DrawButton("Select",
                            () =>
                            {
                                item.FolderFilePath =
                                    EditorUtility.OpenFolderPanel("Select Item Folder", item.FolderFilePath, "");
                            }, Color.red);
                    });

                    EditorHelper.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(string.Format("Source Code Path:{0}", item.CodeFilePath));

                        GUIHelper.DrawButton("Select",
                            () =>
                            {
                                item.CodeFilePath = EditorUtility.OpenFilePanelWithFilters("Select Code Path",
                                    item.CodeFilePath, new[] {"CSharp", "cs"});
                            }, Color.red);
                    });

                    GUIHelper.DrawButton("Delete", () => { _pendingDeleteList.Add(item); }, Color.red);
                }
            });
        }

        private void DeletePendingItem()
        {
            if (_pendingDeleteList != null)
            {
                // delete pending items
                foreach (var item in _pendingDeleteList)
                {
                    if (_curConfig.taskList.Contains(item))
                    {
                        _curConfig.taskList.Remove(item);
                        _isDirty = true;
                    }
                }

                _pendingDeleteList.Clear();
            }
        }

        #endregion


        public void Execute()
        {
            Debug.Log("GPCommon-UpdateItemCode start");

            GetCurrentConfig();

            if (_curConfig.taskList.Count == 0)
                return;

            foreach (ResItemCodeMakerConfig.CreatorTask task in _curConfig.taskList)
            {
                ResItemCodeMaker codeBuilder = new ResItemCodeMaker(task);

                // Write file
                File.WriteAllText(task.CodeFilePath, codeBuilder.ToString(), Encoding.UTF8);

                Debug.LogFormat("from {0} make {1} updated", task.FolderFilePath, task.CodeFilePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("GPCommon-UpdateItemCode complete");
        }
    }
}