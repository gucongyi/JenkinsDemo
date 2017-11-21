using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GPCommon
{
    public class QuickBuildWindow : EditorWindow
    {
        private static QuickBuildWindow _instance;

        [MenuItem("QuickBuild/QuickBuild Window")]
        public static void Init()
        {
            _instance = GetWindow<QuickBuildWindow>("Quick Build");

            _instance.Setup();
            _instance.Show();
        }

        private string _buildComment;
        private string _curConfigPath;
        private BuildConfig _curConfig;

        private string DefaultConfigPath
        {
            get { return QuickBuild.ConfigFolder + "Development.asset"; }
        }

        private void Setup()
        {
            // Load default config
            if (!LoadConfig(DefaultConfigPath))
            {
                // Create default config
                QuickBuild.CreateConfig(DefaultConfigPath);

                LoadConfig(DefaultConfigPath);
            }
        }

        private bool LoadConfig(string configPath)
        {
            _curConfigPath = configPath;
            _curConfig = QuickBuild.LoadConfig(configPath);

            return _curConfig != null;
        }

        #region OnGUI

        private Vector2 _objectScrollPosition;
        private string _errorText;
        private bool _inputChanged;
        private bool _dataChanged;
        private bool _hasDirtyFlag;
        private Action _buildAction;

        private readonly List<BuildConfig.DefineSymbol> _pendingDeleteDefineList =
            new List<BuildConfig.DefineSymbol>();

        void OnGUI()
        {
            if (_curConfig == null)
                return;

            _buildAction = null;
            _errorText = null;

            GUI.enabled = !_curConfig.IsLock;

            EditorHelper.VecticalLayout(() =>
            {
                EditorGUI.BeginChangeCheck();

                DrawDefineList(_curConfig.DefineList, ref _objectScrollPosition);

                EditorHelper.SpaceLayout(3);

                DrawProductInfo();

                DrawBuildOptions();

                GUI.enabled = true;

                DrawLocker();

                CheckEditorChange();

                if (_hasDirtyFlag)
                {
                    DrawSaveButton();
                }
                else
                {
                    DrawOutputFolder();

                    DrawConfigManage();

                    DrawBuildButton();
                }


                EditorHelper.SpaceLayout(1);

                GUIHelper.DrawButton("Apply Define Symbol", () => { QuickBuild.ApplyDefineSymbol(_curConfig); },
                    Color.white, -1, 50);

                DrawErrorInfo();
            });

            // Execute build action at last progress due to avoid some OnGUI error
            if (_buildAction != null)
                _buildAction();
        }

        void OnDestroy()
        {
            _curConfig = null;

            if (_hasDirtyFlag)
                Save();
        }

        private void DrawDefineList(List<BuildConfig.DefineSymbol> list, ref Vector2 scrollPos)
        {
            EditorGUILayout.LabelField("Define Symbol List");

            EditorGUILayout.BeginVertical();

            // Display list
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));

            for (int i = 0; i < list.Count; i++)
            {
                DrawItem(list[i]);
            }

            EditorGUILayout.EndScrollView();

            GUIHelper.DrawButton("New Define", () =>
            {
                BuildConfig.DefineSymbol newSymbol = new BuildConfig.DefineSymbol();
                AddDefineSymbolWizard.DisplayWizard(newSymbol, () =>
                {
                    // Check duplicate
                    if (_curConfig.DefineList.Find(i => i.Name == newSymbol.Name) != null) return;

                    _curConfig.DefineList.Add(newSymbol);
                    _dataChanged = true;
                });
            }, Color.white);

            GUIHelper.DrawButton("Sync Define Symbols", () =>
            {
                if (QuickBuild.SyncAllDefineSymbols())
                    _dataChanged = true;
            }, Color.white);

            EditorGUILayout.EndVertical();

            DeletePendingDefineItem();
        }

        private void DrawItem(BuildConfig.DefineSymbol item)
        {
            EditorHelper.HorizontalLayout(() =>
            {
                item.Enable = EditorGUILayout.Toggle(item.Enable);

                EditorGUILayout.LabelField(item.Name);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(string.Format("/*{0}*/", item.Desc ?? ""));

                GUIHelper.DrawButton("Edit",
                    () => { AddDefineSymbolWizard.DisplayWizard(item, () => { _dataChanged = true; }); },
                    Color.green);

                GUIHelper.DrawButton("Delete", () => { _pendingDeleteDefineList.Add(item); }, Color.red);
            });
        }

        private void DeletePendingDefineItem()
        {
            if (_pendingDeleteDefineList == null) return;

            // Delete pending items
            foreach (var item in _pendingDeleteDefineList)
            {
                if (_curConfig.DefineList.Contains(item))
                {
                    _curConfig.DefineList.Remove(item);
                    _dataChanged = true;
                }
            }

            _pendingDeleteDefineList.Clear();
        }

        private void DrawProductInfo()
        {
            EditorGUILayout.LabelField(string.Format("CompanyName: {0}", QuickBuild.CompanyName));

            _curConfig.BundleIdentifier =
                EditorHelper.DrawStringField("BundleIdentifier", _curConfig.BundleIdentifier, 100);
            if (string.IsNullOrEmpty(_curConfig.BundleIdentifier)) _errorText = "BundleIdentifier invalid";

            _curConfig.ProductName = EditorHelper.DrawStringField("ProductName", _curConfig.ProductName, 85);
            if (string.IsNullOrEmpty(_curConfig.ProductName)) _errorText = "ProductName invalid";

            _curConfig.BundleVersion = EditorHelper.DrawStringField("BundleVersion", _curConfig.BundleVersion, 90);

            _curConfig.BundleVersionCode =
                EditorHelper.DrawIntField("BundleVersionCode", _curConfig.BundleVersionCode, 120);
        }

        private void DrawBuildOptions()
        {
            EditorHelper.HorizontalLayout(() =>
            {
                var currentOptions = new List<BuildOptions>();

                var arrays = Enum.GetValues(typeof(BuildOptions));
                for (int i = 0; i < arrays.LongLength; i++)
                {
                    var buildOption = (BuildOptions) arrays.GetValue(i);

                    if (_curConfig.HasBuildOption(buildOption))
                        currentOptions.Add(buildOption);
                }

                EditorGUILayout.LabelField("BuildOptions: ", GUILayout.Width(80f));

                currentOptions.ForEach((x) =>
                {
                    GUIHelper.DrawButton(x.ToString(),
                        () =>
                        {
                            _curConfig.DeleteBuildOption(x);
                            _dataChanged = true;
                        },
                        Color.white);
                });

                GUIHelper.DrawButton("Add",
                    () =>
                    {
                        NewBuildOptionWizard.DisplayWizard((buildOption) =>
                        {
                            if (_curConfig.HasBuildOption(buildOption)) return;
                            _curConfig.AddBuildOption(buildOption);
                            _dataChanged = true;
                        });
                    },
                    Color.green);
            });
        }

        private void DrawOutputFolder()
        {
            EditorHelper.SpaceLayout(2);

            EditorHelper.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("OutputFolder:", GUILayout.Width(95));
                GUIHelper.DrawButton(QuickBuild.OutputFolder, QuickBuild.ChangeOutputFolderPath, Color.white);
                if (string.IsNullOrEmpty(QuickBuild.OutputFolder)) _errorText = "OutputFolder invalid";
            });
        }

        private void DrawLocker()
        {
            EditorHelper.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("IsLock: ", GUILayout.Width(60));
                _curConfig.IsLock = EditorGUILayout.Toggle(_curConfig.IsLock, GUILayout.Width(15));
            });
        }

        private void DrawConfigManage()
        {
            // Set path text front
            var boldtext = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            EditorGUILayout.LabelField(string.Format("Current config path : {0}", _curConfigPath), boldtext,
                GUILayout.Height(25));

            // Create new config button
            GUIHelper.DrawButton("New Config", () =>
            {
                NewConfigWizard.DisplayWizard((configName) =>
                {
                    var newConfigPath = string.Format("{0}{1}.asset", QuickBuild.ConfigFolder, configName);
                    QuickBuild.CreateConfig(newConfigPath);
                    LoadConfig(newConfigPath);
                });
            }, Color.blue);

            // Save current config as new
            GUIHelper.DrawButton("Save as New..", () =>
            {
                NewConfigWizard.DisplayWizard((configName) =>
                {
                    string newConfigPath = string.Format("{0}{1}.asset", QuickBuild.ConfigFolder, configName);
                    QuickBuild.CreateConfig(newConfigPath, _curConfig);
                    LoadConfig(newConfigPath);
                });
            }, Color.blue);

            DrawConfigList();
        }

        private void DrawConfigList()
        {
            foreach (var path in QuickBuild.GetAllConfigPathList())
            {
                if (path != _curConfigPath)
                {
                    GUIHelper.DrawButton(string.Format("Load '{0}'", path), () => { LoadConfig(path); }, Color.white);
                }
            }
        }

        private void CheckEditorChange()
        {
            _inputChanged = EditorGUI.EndChangeCheck();
            if (_inputChanged || _dataChanged)
            {
                EditorUtility.SetDirty(_curConfig);

                _dataChanged = false;
                _hasDirtyFlag = true;
            }
        }

        private void DrawSaveButton()
        {
            GUIHelper.DrawButton("Save", Save, Color.green, -1, 60);
        }

        private void DrawBuildButton()
        {
            EditorHelper.HorizontalLayout(() =>
            {
                GUIHelper.DrawButton("BuildWin64",
                    () => { _buildAction = () => { QuickBuild.BuildWin(_curConfig, _buildComment); }; }, Color.green);

                GUIHelper.DrawButton("BuildAndroid",
                    () => { _buildAction = () => { QuickBuild.BuildAndroid(_curConfig, _buildComment); }; },
                    Color.green);

                GUIHelper.DrawButton("BuildIOS",
                    () => { _buildAction = () => { QuickBuild.BuildIOS(_curConfig, _buildComment); }; }, Color.green);
            });

            EditorHelper.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("ExportIPA: ", GUILayout.Width(70));
                QuickBuild.ExportIPA = EditorGUILayout.Toggle(QuickBuild.ExportIPA, GUILayout.Width(30));

                _buildComment = EditorHelper.DrawStringField("BuildComment", _buildComment, 120);
            });
        }

        private void DrawErrorInfo()
        {
            if (_errorText != null)
            {
                EditorGUILayout.HelpBox(_errorText, MessageType.Error);
            }
        }

        private void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _hasDirtyFlag = false;
        }

        #endregion
    }
}