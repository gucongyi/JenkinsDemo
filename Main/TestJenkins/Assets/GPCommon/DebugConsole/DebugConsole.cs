using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace GPCommon
{
    public abstract class ConsoleComponent
    {
        public virtual string ComponentName()
        {
            string name = GetType().Name;
            return name.Split('.').ToList().Last();
        }

        public abstract void OnDrawing();

        public virtual void OnUpdate()
        {
        }
    }

    public class DebugConsole : MonoBehaviour
    {
        public static float BtnRWidth = 0.15f;
        public static float BtnRHeight = 0.1f;
        public static float RSpacing = 0.01f;

        #region Singleton

        private static DebugConsole _instance;

        public static DebugConsole GetInstance()
        {
            if (_instance == null)
            {
                throw new Exception("Please initialize it before use, and make sure 'DEBUG_CONSOLE was defined");
            }

            return _instance;
        }

        public static bool Inited
        {
            get { return _instance != null; }
        }

        #endregion

        #region Setup Interfaces

        private List<ConsoleComponent> componentsList;

        public static void Init(Transform parent = null)
        {
#if DEBUG_CONSOLE

            Watchdog.Init();

            // Create instance
            var go = new GameObject("[DebugConsole]");
            _instance = go.AddComponent<DebugConsole>();
            _instance.transform.parent = parent;
            _instance._Init();
#endif
        }

        private void _Init()
        {
            // Add build-in components
            componentsList = new List<ConsoleComponent>();
            AddConsoleComponent(new FpsDetector());
            AddConsoleComponent(new MainMenuTrigger());

            AddComponentToMainMenu(new WatchdogViewer());
            AddComponentToMainMenu(new CommandButtonPanel());
            AddComponentToMainMenu(new TimerScaler());

            AddConsoleCommand(new FieldReflectCommand());

            GetMainMenu().SetCurrentComponent<CommandButtonPanel>();

            // Log success info
            Watchdog.Log("DebugConsole", "**Initialized**");
        }

        public void AddConsoleComponent(ConsoleComponent component)
        {
            componentsList.Add(component);
        }

        public void AddComponentToMainMenu(ConsoleComponent component)
        {
            GetMainMenu().AddConsoleComponent(component);
        }

        public void AddConsoleCommand(string id, Action<List<string>> callback, bool isPreset = false)
        {
            if (id.IndexOf(" ") != -1)
            {
                //Debug.LogError(string.Format("The command id '{0}' you defined in console should not contains space",  id));
                return;
            }

            GetCommondInput().AddConsoleCommand(id, callback);

            if (isPreset)
                AddPresetCommandStr(id);
        }

        public void AddConsoleCommand(ConsoleConmand command, bool isPreset = false)
        {
            if (command.Id.IndexOf(" ") != -1)
            {
                //Debug.LogError(string.Format("The command id '{0}' you defined in console should not contains space", command.Id));
                return;
            }

            GetCommondInput().AddConsoleCommand(command);

            if (isPreset)
                AddPresetCommandStr(command.Id);
        }

        public void AddPresetCommandStr(string commandId)
        {
            GetMainMenu().GetConsoleComponent<CommandButtonPanel>()
                .AddPresetCommandStr(commandId);
        }

        public void ExecuteCommand(string commandStr)
        {
            GetCommondInput().ParseConsoleCommandStr(commandStr);
        }

        public void SetFinderToRelectCommand(IGameGlobalObjectFinder finder)
        {
#if DEBUG_CONSOLE
            GetCommondInput().GetCommand<FieldReflectCommand>(FieldReflectCommand.ID).SetFinder(finder);
#endif
        }

        public T GetConsoleComponent<T>() where T : ConsoleComponent
        {
            return componentsList.OfType<T>().Select(c => c).FirstOrDefault();
        }

        public void ResetCurrentComponent()
        {
            GetConsoleComponent<MainMenuTrigger>().ShowMenu = false;
        }

        internal CommandInput GetCommondInput()
        {
            return GetMainMenu().GetConsoleComponent<WatchdogViewer>().CommandInput;
        }

        private ConsoleMainMenu GetMainMenu()
        {
            return GetConsoleComponent<MainMenuTrigger>().MainMenu;
        }

        #endregion

//#if DEBUG_CONSOLE

//    void OnApplicationPause(bool paused)
//    {
//        if (!paused)
//        {
//            globalToggle = !globalToggle;
//             Watchdog.Log("OnApplicationPause", "globalToggle = " + globalToggle);
//        }
//    }

//#endif

#if DEBUG_CONSOLE

        public bool GlobalToggle = true;

        void OnGUI()
        {
            if (!GlobalToggle) return;

            for (int i = 0; i < componentsList.Count; i++)
                componentsList[i].OnDrawing();
        }

        void Update()
        {
            for (int i = 0; i < componentsList.Count; i++)
                componentsList[i].OnUpdate();
        }

#endif
    }
}