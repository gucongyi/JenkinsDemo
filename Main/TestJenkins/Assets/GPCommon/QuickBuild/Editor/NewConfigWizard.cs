using UnityEditor;
using System;

namespace GPCommon
{
    public class NewConfigWizard : ScriptableWizard
    {
        public string ConfigName;
        public Action<string> OnCreate;

        public static void DisplayWizard(Action<string> onCreate)
        {
            // Display wizard
            NewConfigWizard wizard = ScriptableWizard.DisplayWizard<NewConfigWizard>("New Config wizard", "Create");

            // Setup
            wizard.ConfigName = "";
            wizard.OnCreate = onCreate;
        }

        void OnWizardCreate()
        {
            if (OnCreate != null && !string.IsNullOrEmpty(ConfigName))
                OnCreate(ConfigName);
        }
    }
}