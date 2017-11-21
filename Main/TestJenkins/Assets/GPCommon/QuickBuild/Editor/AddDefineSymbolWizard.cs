using UnityEditor;
using System;

namespace GPCommon
{
    public class AddDefineSymbolWizard : ScriptableWizard
    {
        public BuildConfig.DefineSymbol Symbol;
        public Action OnCreate;

        public static void DisplayWizard(BuildConfig.DefineSymbol symbol, Action onCreate)
        {
            // Display wizzard
            AddDefineSymbolWizard wizzard =
                ScriptableWizard.DisplayWizard<AddDefineSymbolWizard>("Add Define Symbol", "Create");

            // Setup
            wizzard.Symbol = symbol;
            wizzard.OnCreate = onCreate;
        }

        void OnWizardCreate()
        {
            if (Symbol != null && OnCreate != null)
                OnCreate();
        }
    }
}