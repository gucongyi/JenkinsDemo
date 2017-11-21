using UnityEngine;
using System.Collections.Generic;

namespace GPCommon
{
    public class WatchdogViewer : ConsoleComponent
    {
        public CommandInput CommandInput;

        private List<Watchdog.LogMessage.Type> _messageTypeFilter;

        public WatchdogViewer()
        {
            CommandInput = new CommandInput();
        }

        public override string ComponentName()
        {
            return "Watchdog";
        }

        private Vector2 _scrollPosition = new Vector2();

        public override void OnDrawing()
        {
            string watchdogString = Watchdog.ToString(null, _messageTypeFilter);
            GUIHelper.PrintScrollText(watchdogString, GUIHelper.RatioRect(0.16f, DebugConsole.RSpacing, 0.83f, 0.82f),
                ref _scrollPosition);

            // Directly draw command input area
            CommandInput.OnDrawing();

            // Draw show exception button
            if (GUI.Button(GUIHelper.RatioRect(0.01f, 0.85f, DebugConsole.BtnRWidth, DebugConsole.BtnRHeight),
                _messageTypeFilter == null ? "Exception" : "Resume"))
            {
                _messageTypeFilter = _messageTypeFilter == null
                    ? new List<Watchdog.LogMessage.Type>() {Watchdog.LogMessage.Type.Exception}
                    : null;
            }
        }
    }
}