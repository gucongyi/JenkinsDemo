using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GPCommon
{
    public static class QuickBuild
    {
        private class BuildPlatformSchema
        {
            public BuildTarget BuildTarget;
            public BuildTargetGroup BuildTargetGroup;
            public Func<string, string> ProcessPackagePath;
            public Action<BuildConfig> OnPreBuild;
        }

        public const string CompanyName = "Huanyouzhe";
        public const string ConfigFolder = "Assets/GPCommon/QuickBuild/Editor/Config/";

        private static readonly BuildPlatformSchema AndroidBuildSchema;
        private static readonly BuildPlatformSchema IOSBuildSchema;
        private static readonly BuildPlatformSchema WinBuildSchema;

        public static string ExportIpaShellPath
        {
            get { return Application.dataPath + "/GPCommon/QuickBuild/Editor/exportIPA.sh"; }
        }

        public static string ExportLogPath
        {
            get { return Application.dataPath + "/GPCommon/QuickBuild/Editor/exportIPALog.log"; }
        }

        public static string LastBuildName
        {
            get { return EditorPrefs.GetString("QuickBuild_LastBuildName"); }
            private set { EditorPrefs.SetString("QuickBuild_LastBuildName", value); }
        }

        public static string LastBuildPath
        {
            get { return EditorPrefs.GetString("QuickBuild_LastBuildPath"); }
            private set { EditorPrefs.SetString("QuickBuild_LastBuildPath", value); }
        }

        public static string OutputFolder
        {
            get { return EditorPrefs.GetString("QuickBuild_OutputFolder"); }
            // private
             set { EditorPrefs.SetString("QuickBuild_OutputFolder", value); }
        }

        public static bool ExportIPA
        {
            get { return EditorPrefs.GetBool("QuickBuild_ExportIPA"); }
            set { EditorPrefs.SetBool("QuickBuild_ExportIPA", value); }
        }

        [MenuItem("QuickBuild/Log LastBuild Info")]
        public static void LogLastBuildInfo()
        {
            //Debug.Log(LastBuildPath);
            //Debug.Log(LastBuildName);
        }

        [MenuItem("QuickBuild/Export LastBuild IPA")]
        public static void ExportLastBuildIpa()
        {
            Debug.Log("==================Exporting IPA....");
            var dateStr = System.DateTime.Now.ToString("_M_d_h_m");
            var fileName = CurrBuildData.文件名 + (CurrBuildData.文件名加日期 ? dateStr : "");
            string ipaName= fileName;
            Debug.Log("===================currIsCustomPackName:" + currIsCustomPackName);
            if (QuickBuild.currIsCustomPackName)
            {
                ipaName = QuickBuild.currCustomPackName;
                Debug.Log("===================currCustomPackName:"+ currCustomPackName);
            }
            Debug.Log("===================currEnableDevBuild:" + currEnableDevBuild);
            if (QuickBuild.currEnableDevBuild)
            {
                ipaName = ipaName + "_Debug";
            }
            Debug.Log("===================ipaName:" + ipaName);
            var exportCommand = string.Format("{0} {1} {2} {3}", ExportIpaShellPath,
                LastBuildPath, LastBuildName, ipaName);//第四个参数是ipa Name
            

            string output;
            EditorHelper.ExecuteBashScript(exportCommand, out output);
            Debug.Log("=========================Gen IPA Log:"+ output);
            File.WriteAllText(ExportLogPath, output);
        }

        [MenuItem("QuickBuild/Open LastBuild Folder")]
        public static void OpenLastBuildFolder()
        {
            EditorUtility.RevealInFinder(LastBuildPath);
        }

        [MenuItem("QuickBuild/Open ExportIPALog")]
        public static void OpenExportIpaLog()
        {
            var path = CommonUtils.GetUnityAssetsPath(ExportLogPath);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            AssetDatabase.OpenAsset(obj);
        }

        static QuickBuild()
        {
            AndroidBuildSchema = new BuildPlatformSchema
            {
                BuildTarget = BuildTarget.Android,
                BuildTargetGroup = BuildTargetGroup.Android,
                ProcessPackagePath = (fileName) => fileName + ".apk",
                OnPreBuild = (buildConfig) =>
                {
                    PlayerSettings.keystorePass = CompanyName;
                    PlayerSettings.keyaliasPass = CompanyName;
                    PlayerSettings.Android.bundleVersionCode = buildConfig.BundleVersionCode;
                },
            };

            IOSBuildSchema = new BuildPlatformSchema
            {
                BuildTarget = BuildTarget.iOS,
                BuildTargetGroup = BuildTargetGroup.iOS,
                ProcessPackagePath = (fileName) => fileName, // Folder
                OnPreBuild = null,
            };

            WinBuildSchema = new BuildPlatformSchema
            {
                BuildTarget = BuildTarget.StandaloneWindows64,
                BuildTargetGroup = BuildTargetGroup.Standalone,
                ProcessPackagePath = (fileName) => string.Format("{0}/{0}.exe", fileName),
                OnPreBuild = null,
            };
        }

        public static void ChangeOutputFolderPath()
        {
            var path = EditorUtility.OpenFolderPanel("Select Package Folder", "", "");

            if (EditorHelper.ValidateFolder(path))
            {
                OutputFolder = path;
            }
        }
        //从shell中拿到参数 by gcy
        public static string FindParam(string propName)
        {
            //在这里分析shell传入的参数， 还记得上面我们说的哪个 project-$1 这个参数吗？
            //这里遍历所有参数，找到 project开头的参数， 然后把-符号 后面的字符串返回
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith(propName))
                {
                    string propNameValue = arg.Split("-"[0])[1];
                    Debug.Log("================"+ propName+"String:" + propNameValue);
                    return propNameValue;
                }
            }
            Debug.Log(" ================ "+ propName+"No Received");
            return "";
        }
        //by gcy
        public static void SetOutputFolderPath()
        {
            OutputFolder = FindParam("outputFolderPath");
        }

        public static void GetShellParam()
        {
            currEnableDevBuild = EnableDevBuild();
            currIsCustomPackName = IsCustomPackName();
            currCustomPackName = GetCustomPackName();
//			currEnableDevBuild=true;
//			currIsCustomPackName = true;
//			currCustomPackName="JenTest";


        }

        //by gcy
        public static BuildOptions GetBuildOptions()
        {
            BuildOptions options;

			var enableDevBuild = EnableDevBuild();
            if (enableDevBuild)
            {
                options = BuildOptions.CompressWithLz4HC | BuildOptions.Development | BuildOptions.ConnectWithProfiler|BuildOptions.ConnectToHost;
            }
            else
            {
                options = BuildOptions.CompressWithLz4HC;
            }
            return options;
        }
        //by gcy
        private static bool currEnableDevBuild;
        public static bool EnableDevBuild()
        {
            bool enableDevBuild = Boolean.Parse(QuickBuild.FindParam("EnableDevelopBuild"));
            return enableDevBuild;
        }

        private static bool currIsCustomPackName;
        public static bool IsCustomPackName()
        {
            bool isCustomPackName = Boolean.Parse(QuickBuild.FindParam("IsCustomPackName"));
            return isCustomPackName;
        }

        private static string currCustomPackName;
        public static string GetCustomPackName()
        {
            string customPackName = QuickBuild.FindParam("CustomPackName");
            return customPackName;
        }

        private static BuildData CurrBuildData = null;
        public static void SetBuildData(BuildData data)
        {
            CurrBuildData = data;
        }

        //by gcy
		[MenuItem("QuickBuild/SetBuildConfig")]
        public static void SetBuildConfig()
        {
            //SetOutputFolderPath();
            //         BuildConfig config = new BuildConfig();
            //         config.BundleIdentifier = FindParam("bundleIdentifier");
            //         config.ProductName = FindParam("productName");
            //         config.BundleVersionCode = int.Parse(FindParam("bundleVersionCode"));
            //         config.BundleVersion = FindParam("bundleVersion");
            //         config.BuildOptionInt = 0;
            //         config.PackageName= FindParam("gameName");
            //         string comment = FindParam("buildComment");
            BuildData data=new BuildData();
            data.文件名 = "G01";
            data.BundleIdentifier = "com.huanyz.g01";
            data.productName = "TestJenkins";
            data.BundleVersion = "1.0";
            data.bundleVersionCode = 1;
            SetBuildData(data);
            BuildConfig config = new BuildConfig();
            config.BundleIdentifier = CurrBuildData.BundleIdentifier;
            config.ProductName = CurrBuildData.productName;
            config.BundleVersionCode = CurrBuildData.bundleVersionCode;
            config.BundleVersion = CurrBuildData.BundleVersion;
			OutputFolder = "/Users/hyz/Desktop";
            QuickBuild.ExportIPA = true;
            BuildIOS(config, "");
			OpenLastBuildFolder();
        }


        public static void BuildWin(BuildConfig config, string buildComment = null)
        {
            Build(WinBuildSchema, config, buildComment);
        }

        public static void BuildAndroid(BuildConfig config, string buildComment = null)
        {
            Build(AndroidBuildSchema, config, buildComment);
        }

        public static void BuildIOS(BuildConfig config, string buildComment = null)
        {
            Build(IOSBuildSchema, config, buildComment);
        }

        private static void Build(BuildPlatformSchema schema, BuildConfig config, string buildComment)
        {
            // Validate folder path
            if (!EditorHelper.ValidateFolder(OutputFolder))
            {
                ChangeOutputFolderPath();
                return;
            }

            string packageName;
            // Set package name
            if (!string.IsNullOrEmpty(buildComment))
                packageName = string.Format("{0}_{1}_{2}", config.ProductName,
                    DateTimeUtils.StandardTimeStr, buildComment);
            else
                packageName = string.Format("{0}_{1}", config.ProductName, DateTimeUtils.StandardTimeStr);

            // Update runtime config
            config.PackageName = packageName;
            CreateConfig(BuildConfig.RuntimeAssetConfigPath, config);

            // Set define symbols
            PlayerSettings.SetScriptingDefineSymbolsForGroup(schema.BuildTargetGroup, config.GetDefineStr());

            // Set basic product info
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.applicationIdentifier = config.BundleIdentifier;
            PlayerSettings.productName = config.ProductName;
            PlayerSettings.bundleVersion = config.BundleVersion;
            PlayerSettings.iOS.appleDeveloperTeamID = "WBHCY56HJ5";

            if (schema.OnPreBuild != null)
                schema.OnPreBuild(config);

            // Prepare output path
            var packagePath = string.Format("{0}/{1}/{2}", OutputFolder, schema.BuildTarget,
                schema.ProcessPackagePath(packageName));

            EditorHelper.PreparePathDirectory(packagePath);

            // Save as last configuration before building
            LastBuildName = packageName;
            LastBuildPath = packagePath;
            GetShellParam();
            // Building...
            var result = BuildPipeline.BuildPlayer(GetBuildScenes(), packagePath, schema.BuildTarget,GetBuildOptions());

            if (string.IsNullOrEmpty(result))
            {
                //Debug.Log("Build complete!");

                // RevealInFinder after build complete
                EditorUtility.RevealInFinder(packagePath);
            }
            else
            {
                //Debug.Log("Build error occur!");

                //Debug.LogError(result);
            }
        }

        private static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();

            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null) continue;
                if (e.enabled) names.Add(e.path);
            }

            return names.ToArray();
        }

        public static void ApplyDefineSymbol(BuildConfig config)
        {
            if (config == null) return;

            // Start to define symbols
            string defineStr = config.GetDefineStr();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(WinBuildSchema.BuildTargetGroup,
                defineStr);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(AndroidBuildSchema.BuildTargetGroup,
                defineStr);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(IOSBuildSchema.BuildTargetGroup,
                defineStr);

            //Debug.Log(string.Format("Current define symbol: {0}", defineStr));
        }

        public static bool SyncAllDefineSymbols()
        {
            var allDefineList = new List<BuildConfig.DefineSymbol>();

            // Collect all define symbols
            GetAllConfigPathList().ForEach(path =>
            {
                var config = LoadConfig(path);
                config.DefineList.ForEach(x =>
                {
                    if (allDefineList.Find(y => y.Name == x.Name) != null)
                        return;

                    allDefineList.Add(x);
                });
            });

            var isChanged = false;
            // Apply all define symbols
            GetAllConfigPathList().ForEach(path =>
            {
                var config = LoadConfig(path);
                allDefineList.ForEach(x =>
                {
                    if (config.DefineList.Find(y => y.Name == x.Name) != null)
                        return;

                    config.DefineList.Add(x.CloneNewSymbol());
                    EditorUtility.SetDirty(config);
                    isChanged = true;
                });
            });

            return isChanged;
        }

        public static List<string> GetAllConfigPathList()
        {
            var t = Directory.GetFiles(ConfigFolder).ToList();
            var result = t.Where(path => path.Split('.').Last() == "asset").ToList();
            return result;
        }

        public static void CreateConfig(string desPath, BuildConfig source = null)
        {
            // Prepare directory
            EditorHelper.PreparePathDirectory(desPath);

            // Replace with new
            if (File.Exists(desPath))
            {
                AssetDatabase.DeleteAsset(desPath);
            }

            // Create instance
            var asset = ScriptableObject.CreateInstance<BuildConfig>();
            if (source != null) EditorUtility.CopySerialized(source, asset);

            // Create asset
            AssetDatabase.CreateAsset(asset, desPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static BuildConfig LoadConfig(string path)
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(BuildConfig)) as BuildConfig;
        }
    }
}