using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GPCommon
{
    public class BuildConfig : ScriptableObject
    {
        [System.Serializable]
        public class DefineSymbol
        {
            public string Name;
            public string Desc;
            public bool Enable;

            public DefineSymbol CloneNewSymbol()
            {
                return new DefineSymbol()
                {
                    Name = Name,
                    Desc = Desc,
                    Enable = false,
                };
            }
        }

        public const string RuntimeResConfigPath = "QuickBuild/BuildConfig";

        public static string RuntimeAssetConfigPath
        {
            get { return string.Format("Assets/GPCommon/QuickBuild/Resources/{0}.asset", RuntimeResConfigPath); }
        }

        public static BuildConfig LoadRuntimeBuildConfig()
        {
            var buildConfig = Resources.Load(RuntimeResConfigPath) as BuildConfig;
            return buildConfig;
        }

        public List<DefineSymbol> DefineList = new List<DefineSymbol>();
        public string BundleIdentifier;
        public string ProductName;
        public int BundleVersionCode;
        public string BundleVersion;
        public int BuildOptionInt;
        public bool IsLock;
        public string PackageName;

        public string GetPackageDescript()
        {
            return string.Format("{0}({1})", PackageName, GetDefineStr());
        }

        public string GetDefineStr()
        {
            return string.Join("; ", DefineList.Where(x => x.Enable).Select(x => x.Name).ToArray());
        }
    }
}