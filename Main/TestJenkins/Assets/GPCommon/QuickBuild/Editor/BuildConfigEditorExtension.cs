using UnityEditor;

namespace GPCommon
{
    public static class BuildConfigEditorExtension
    {
        public static BuildOptions GetBuildOption(this BuildConfig buildConfig)
        {
            return (BuildOptions) buildConfig.BuildOptionInt;
        }

        public static void SetBuildOption(this BuildConfig buildConfig, BuildOptions newValue)
        {
            buildConfig.BuildOptionInt = (int) newValue;
        }

        public static bool HasBuildOption(this BuildConfig buildConfig, BuildOptions buildOption)
        {
            return (buildConfig.GetBuildOption() & buildOption) != 0;
        }

        public static void DeleteBuildOption(this BuildConfig buildConfig, BuildOptions buildOption)
        {
            buildConfig.SetBuildOption(buildConfig.GetBuildOption() & ~buildOption);
        }

        public static void AddBuildOption(this BuildConfig buildConfig, BuildOptions buildOption)
        {
            buildConfig.SetBuildOption(buildConfig.GetBuildOption() | buildOption);
        }
    }
}
