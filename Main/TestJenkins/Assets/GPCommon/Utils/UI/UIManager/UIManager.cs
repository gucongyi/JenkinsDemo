using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPCommon
{
    public class UIManager
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIManager();
                }

                return _instance;
            }
        }

        private Dictionary<string, IUIElement> _uiElements;
        private IUIManagerConnector _connector;

        public void Init(IUIManagerConnector connector)
        {
            _connector = connector;
            _reserveList = new List<ReservedUI>();
            _uiElements = new Dictionary<string, IUIElement>();
        }

        public T Show<T>(params object[] args) where T : IUIElement
        {
            return (T)Show(typeof(T).Name, args);
        }

        public IUIElement Show(string className, params object[] args)
        {
            var ui = Get(className);
            return Show(ui, args);
        }

        public IUIElement Show(IUIElement ui, params object[] args)
        {
            ui.OnSetup(args);

            if (ui.State == UIState.Hidden)
            {
#if DEBUG_UI
                //Debug.Log("ShowUI: " + ui);
#endif

                ui.PlayShownSound();

                ui.OnWillShow();

                _connector.Show(ui);

                ui.State = UIState.Show;

                ui.OnShown();

                return ui;
            }

            return ui;
        }

        public void Hide<T>()
        {
            Hide(typeof(T).Name);
        }

        public IUIElement Hide(string className)
        {
            var ui = Get(className);
            return Hide(ui);
        }

        public IUIElement Hide(IUIElement ui)
        {
            if (ui.State == UIState.Show)
            {
#if DEBUG_UI
                //Debug.Log("HideUI: " + ui);
#endif

                ui.PlayHidenSound();

                ui.OnWillHiden();

                _connector.Hide(ui);

                ui.State = UIState.Hidden;

                ui.OnHidden();

                return ui;
            }

            return ui;
        }

        public void HideAll()
        {
            foreach (var className in _uiElements.Keys)
            {
                var ui = Get(className);
                if (ui.State == UIState.Show)
                    Hide(ui);
            }
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T).Name);
        }

        public UIState GetUIState<T>()
        {
            var className = typeof(T).Name;
            if (!_uiElements.ContainsKey(className))
            {
                return UIState.Hidden;
            }

            return _uiElements[className].State;
        }

        public IUIElement Get(string className)
        {
            if (!_uiElements.ContainsKey(className))
            {
                var ui = _connector.CreateUI(className);

                _uiElements.Add(className, ui);

                ui.OnLoadComplete();
            }

            return _uiElements[className];
        }

        #region UI Reserving

        private class ReservedUI
        {
            public IUIElement UI;
            public object[] Args;
        }

        private List<ReservedUI> _reserveList;

        public T Reserve<T>(params object[] args) where T : IUIElement
        {
            var ui = Get<T>();

            var old = _reserveList.Find(x => x.UI == (IUIElement)ui);
            if (old != null)
            {
                old.Args = args;
            }
            else
            {
                _reserveList.Add(new ReservedUI() { UI = ui, Args = args });
            }

            return ui;
        }

        public T ShowReservedUIOnly<T>(params object[] args) where T : IUIElement
        {
            var ui = Reserve<T>(args);
            ShowReservedUIOnly();
            return ui;
        }

        public void ShowReservedUIOnly()
        {
            var temp = _uiElements.Keys.ToList();
            foreach (var className in temp)
            {
                var ui = Get(className);
                var reserveInfo = _reserveList.Find(x => x.UI == ui);

                if (ui.State == UIState.Show && reserveInfo == null)
                {
                    Hide(className);
                }
                else if (ui.State == UIState.Hidden && reserveInfo != null)
                {
                    Show(className, reserveInfo.Args);
                }
            }

            _reserveList.Clear();
        }

        #endregion
    }
}