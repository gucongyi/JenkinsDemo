using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GPCommon
{
    public class ConsoleMainMenu : ConsoleComponent
    {
        readonly List<ConsoleComponent> _componentsList;

        private ConsoleComponent _currentComponent;

        public ConsoleMainMenu()
        {
            _componentsList = new List<ConsoleComponent>();
        }

        public void SetCurrentComponent<T>() where T : ConsoleComponent
        {
            T component = GetConsoleComponent<T>();
            SetCurrentComponent(component);
        }

        public void SetCurrentComponent(ConsoleComponent component)
        {
            _currentComponent = component;
        }

        public void AddConsoleComponent(ConsoleComponent component)
        {
            _componentsList.Add(component);
        }

        public T GetConsoleComponent<T>() where T : ConsoleComponent
        {
            foreach (ConsoleComponent c in _componentsList)
                if (c is T) return c as T;

            return null;
        }

        public override void OnDrawing()
        {
            for (int i = 0; i < _componentsList.Count; i++)
            {
                // Layout main menu buttons Vertically
                if (GUI.Button(GUIHelper.RatioRect(DebugConsole.RSpacing,
                    2 * DebugConsole.BtnRHeight + i * (DebugConsole.BtnRHeight + DebugConsole.RSpacing),
                    DebugConsole.BtnRWidth, DebugConsole.BtnRHeight), _componentsList[i].ComponentName()))
                {
                    // Select or toggle current component
                    if (_currentComponent == _componentsList[i])
                        _currentComponent = null;
                    else
                        _currentComponent = _componentsList[i];
                }
            }

            if (_currentComponent != null)
                _currentComponent.OnDrawing();
        }
    }
}