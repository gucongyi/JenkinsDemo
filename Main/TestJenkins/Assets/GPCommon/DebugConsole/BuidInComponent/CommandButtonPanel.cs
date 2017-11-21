using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GPCommon
{
    public class CommandButtonPanel : ConsoleComponent
    {
        public override string ComponentName()
        {
            return "Command";
        }

        private readonly List<string> _commandStrList;

        public CommandButtonPanel()
        {
            _commandStrList = new List<string>();
        }

        public void AddPresetCommandStr(string s)
        {
            _commandStrList.Add(s);
        }

        public override void OnDrawing()
        {
            for (var i = 0; i < _commandStrList.Count; i++)
            {
                // Get layout rect
                var buttonRect = GUIHelper.LayoutIndexRect(i, 5, DebugConsole.BtnRWidth, DebugConsole.BtnRHeight, 0.1837112f, 0.06644553f);

                if (GUI.Button(buttonRect, _commandStrList[i]))
                {
                    // Execute command string through 'CommandInput' component
                    DebugConsole.GetInstance().GetCommondInput().ParseConsoleCommandStr(_commandStrList[i]);
                }
            }
        }
    }
}
