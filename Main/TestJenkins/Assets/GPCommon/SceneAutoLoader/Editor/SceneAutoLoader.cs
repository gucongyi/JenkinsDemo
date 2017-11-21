using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene auto loader.
/// </summary>
/// <description>
/// This class adds a Window > Scene Auto Loader menu containing options to select
/// a "master scene" enable it to be auto-loaded when the user presses play
/// in the editor.
///
/// When enabled, the selected scene will be loaded on play; stopping play will however return you to the original editor scene(s).
///
/// The scene loading is triggered after play mode has begun. This results in all game objects in the scenes you had loaded in the editor
/// receiving the following callbacks:
///   Awake
///   OnEnable
///   OnDisable
///   OnDestroy
/// To be compatible with this script you should ensure that all scenes in your game support the above flow without any strange side effects.
///
/// Based on an idea on this thread:
/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
/// </description>
[InitializeOnLoad]
public static class SceneAutoLoader
{
    // Static constructor binds a playmode-changed callback.
    // [InitializeOnLoad] above makes sure this gets execusted.
    static SceneAutoLoader()
    {
        EditorApplication.playmodeStateChanged += OnPlayModeChanged;
    }

    /// <summary>
    /// Play mode change callback detects when user presses Play, and schedules a scene load request.
    /// </summary>
    private static void OnPlayModeChanged()
    {
        // User pressed Play, and the editor is about to enter Play mode

        if (SceneAutoLoaderProperties.LoadMasterOnPlay &&
            SceneAutoLoaderProperties.MasterScene != "" &&
            !EditorApplication.isPlaying &&
            EditorApplication.isPlayingOrWillChangePlaymode)
        {

            var currentScene = SceneManager.GetActiveScene().path;
            SceneAutoLoaderProperties.PreviousScene = currentScene;

            if (currentScene == SceneAutoLoaderProperties.MasterScene)
                return;

            // Schedule a load of the master scene, if SceneAutoLoader is active and the user has chosen a scene
            SceneAutoLoaderProperties.SceneLoadRequested = true;
        }
    }

    /// <summary>
    /// Load the desired scene, if requested.
    /// BeforeSceneLoad runtime callback is triggered after the editor has entered Play mode.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        if (SceneAutoLoaderProperties.SceneLoadRequested)
        {
            SceneAutoLoaderProperties.SceneLoadRequested = false;

            // This LoadScene will be executed before any GameObjects have received any initialization messages.
            // LoadScene is however not an instant operation. Therefore, all GameObjects in the scenes that were
            // loaded in the editor will receive Awake/OnEnable callbacks before the scene loading command
            // takes effect (which in turn results in the GameObjects in the scene receiving OnDisable/OnDestroy callbacks).
            // After this, the MasterScene will begin loading.
            SceneManager.LoadScene(SceneAutoLoaderProperties.MasterScene);
        }
    }
}