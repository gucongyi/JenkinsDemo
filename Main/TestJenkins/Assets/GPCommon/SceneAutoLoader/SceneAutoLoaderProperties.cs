#if UNITY_EDITOR

using UnityEditor;

public class SceneAutoLoaderProperties
{
    // Properties are remembered as editor preferences.
    private static string cEditorPrefLoadMasterOnPlay { get { return "SceneAutoLoader." + PlayerSettings.productName + ".LoadMasterOnPlay"; } }
    private static string cEditorPrefMasterScene { get { return "SceneAutoLoader." + PlayerSettings.productName + ".MasterScene"; } }
    private static string cEditorPrefSceneLoadRequested { get { return "SceneAutoLoader." + PlayerSettings.productName + ".SceneLoadRequested"; } }
    private static string cEditorPrefPreviousScene { get { return "SceneAutoLoader." + PlayerSettings.productName + ".PreviousScene"; } }

    public static bool LoadMasterOnPlay
    {
        get { return EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false); }
        set { EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value); }
    }

    public static string MasterScene
    {
        get { return EditorPrefs.GetString(cEditorPrefMasterScene, ""); }
        set { EditorPrefs.SetString(cEditorPrefMasterScene, value); }
    }

    public static bool SceneLoadRequested
    {
        get { return EditorPrefs.GetBool(cEditorPrefSceneLoadRequested, false); }
        set { EditorPrefs.SetBool(cEditorPrefSceneLoadRequested, value); }
    }

    public static string PreviousScene
    {
        get { return EditorPrefs.GetString(cEditorPrefPreviousScene, ""); }
        set { EditorPrefs.SetString(cEditorPrefPreviousScene, value); }
    }
}
#endif
