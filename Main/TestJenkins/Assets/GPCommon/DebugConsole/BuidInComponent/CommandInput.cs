using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GPCommon
{
    public class ConsoleConmand
    {
        public string Id;
        public Action<List<string>> callback;
    }

    public class CommandInput : ConsoleComponent
    {
        private readonly Dictionary<string, ConsoleConmand> _commandDic; // The name to command
        private string _commandInput;

        public CommandInput()
        {
            _commandInput = "";
            _commandDic = new Dictionary<string, ConsoleConmand>();
        }

        public override void OnDrawing()
        {
            // Set textField control name to focus it 
            const string controlName = "commandInput";
            GUI.SetNextControlName(controlName);

            // Draw command textField
            _commandInput = GUI.TextField(GUIHelper.RatioRect(0.18f, 0.85f, 0.65f, DebugConsole.BtnRHeight), _commandInput);

            // Focus textField
            if (GUI.GetNameOfFocusedControl() == string.Empty) GUI.FocusControl(controlName);

            // Draw run button
            if (GUI.Button(GUIHelper.RatioRect(0.84f, 0.85f, DebugConsole.BtnRWidth, DebugConsole.BtnRHeight),
                    "Run") || (Event.current.type == EventType.Used && Event.current.keyCode == KeyCode.Return))
            {
                if (!string.IsNullOrEmpty(_commandInput))
                {
                    ParseConsoleCommandStr(_commandInput);
                    _commandInput = "";
                }
            }
        }

        public void AddConsoleCommand(string id, Action<List<string>> callback)
        {
            if (_commandDic.ContainsKey(id))
                throw new Exception("Has exist command");
            else
                _commandDic.Add(id, new ConsoleConmand() { Id = id, callback = callback });
        }

        public void AddConsoleCommand(ConsoleConmand command)
        {
            if (_commandDic.ContainsKey(command.Id))
                throw new Exception("Has exist command: " + command.Id);
            else
                _commandDic.Add(command.Id, command);
        }

        public void ParseConsoleCommandStr(string input)
        {
            string[] cmdPeer = input.Split(' ');
            if (cmdPeer.Length != 0)
            {
                string commandName = cmdPeer[0];

                // Find command
                if (_commandDic.ContainsKey(commandName))
                {
                    if (cmdPeer.Length > 1)
                    {
                        // Invoke callback with arguments
                        List<string> args = new List<string>(cmdPeer);
                        args.RemoveAt(0);

                        _commandDic[commandName].callback(args);
                    }
                    else
                    {
                        // Or invoke callback with null
                        _commandDic[commandName].callback(null);
                    }
                }
                else
                {
                    Watchdog.Log(ComponentName(), "Command '" + commandName + "' not found");
                }
            }
        }

        public T GetCommand<T>(string id) where T : ConsoleConmand
        {
            if (!_commandDic.ContainsKey(id))
            {
                Watchdog.LogError(ComponentName(), "GetCommand '" + id + "' not found");
                return null;
            }
            else
            {
                return _commandDic[id] as T;
            }
        }
    }
}