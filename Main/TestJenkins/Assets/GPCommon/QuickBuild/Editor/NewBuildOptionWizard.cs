using UnityEditor;
using System;

namespace GPCommon
{
    public class NewBuildOptionWizard : ScriptableWizard
    {
        public BuildOptions BuildOption;
        public Action<BuildOptions> OnCreate;

        public static void DisplayWizard(Action<BuildOptions> onCreate)
        {
            // Display wizard
            NewBuildOptionWizard wizard =
                ScriptableWizard.DisplayWizard<NewBuildOptionWizard>("New Build Option wizard", "Create");

            // Setup
            wizard.BuildOption = BuildOptions.None;
            wizard.OnCreate = onCreate;
        }

        void OnWizardCreate()
        {
            if (OnCreate != null && BuildOption != BuildOptions.None)
                OnCreate(BuildOption);
        }
    }
}