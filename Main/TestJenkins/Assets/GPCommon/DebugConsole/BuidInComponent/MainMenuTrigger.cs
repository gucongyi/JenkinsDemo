using UnityEngine;
using System.Collections;

namespace GPCommon
{
    public class MainMenuTrigger : ConsoleComponent
    {
        public ConsoleMainMenu MainMenu;
        public bool ShowMenu;

        public MainMenuTrigger()
        {
            MainMenu = new ConsoleMainMenu();
        }

        public override void OnDrawing()
        {
            if (GUI.Button(
                GUIHelper.RatioRect(DebugConsole.RSpacing, .1f, DebugConsole.BtnRWidth / 2, DebugConsole.BtnRHeight / 2),
                "Console"))
                ShowMenu = !ShowMenu; // Toggle main menu

            if (ShowMenu)
                MainMenu.OnDrawing();
        }
    }
}