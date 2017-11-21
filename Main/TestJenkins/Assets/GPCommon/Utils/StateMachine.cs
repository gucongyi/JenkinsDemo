using System;
using System.Collections.Generic;

namespace GPCommon
{
    public interface IState
    {
        StateMachine FSM { get; set; }

        void OnEnter(object param);

        void OnExit();

        void Update();

        void FixedUpdate();

        string StateName { get; }
    }

    public class State<T> : IState
    {
        public StateMachine FSM { get; set; }

        protected T Owner { get; private set; }

        private readonly string _stateName;

        public State(T owner)
        {
            Owner = owner;

            _stateName = GetType().Name;
        }

        #region IState Members

        public virtual void OnEnter(object param)
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public string StateName
        {
            get { return _stateName; }
        }

        #endregion
    }

    public class StateMachine
    {
        public bool EnableDebug;
        public string Name { get; private set; }

        private Dictionary<string, IState> _stateDictionary;
        private List<IState> _stateList;

        private string _pendingStateChange;
        private object _pendingStateParam;

        public IState CurrentState { get; private set; }

        public IState PreviousState { get; private set; }

        public string CurrentStateName
        {
            get { return CurrentState == null ? "null" : CurrentState.StateName; }
        }

        public string PreviousStateName
        {
            get { return PreviousState.StateName; }
        }

        // Action<FromState, ToState>
        public event Action<IState, IState> OnStateChanged;

        #region Interface

        public void Init<T>(List<IState> states, object defaultStateParam = null)
            where T : IState
        {
            Init(states, typeof(T).Name, defaultStateParam);
        }

        public void Init(List<IState> states, string defaultState, object defaultStateParam = null)
        {
            // Init all states
            Init(states);

            // To default state
            DoChangeState(defaultState, defaultStateParam);
        }

        public void Init(List<IState> states)
        {
            _stateList = states;
            _stateDictionary = new Dictionary<string, IState>();

            // Init all states
            states.ForEach(s =>
            {
                s.FSM = this;
                _stateDictionary.Add(s.StateName, s);
            });
        }

        public bool CheckCurrentState<T>() where T : IState
        {
            if (CurrentState == null)
                return false;
            return CurrentState.GetType() == typeof(T);
        }

        public int GetCurrentStateIndex()
        {
            return _stateList.IndexOf(CurrentState);
        }

        public void ForceChangeStateByIndex(int index, object param = null)
        {
            var state = _stateList[index];
            DoChangeState(state.StateName, param);
        }

        public void ForceChangeState<T>(object param = null) where T : IState
        {
            ForceChangeState(typeof(T).Name, param);
        }

        public void ForceChangeState(string stateName, object param = null)
        {
            DoChangeState(stateName, param);

            // Reset flag
            _pendingStateChange = null;
        }

        public void PendingChangeState<T>(object param = null) where T : IState
        {
            PendingChangeState(typeof(T).Name, param);
        }

        public void PendingChangeState(string stateName, object param = null)
        {
            _pendingStateChange = stateName;
            _pendingStateParam = param;
        }

        public T GetState<T>() where T : IState
        {
            return (T)GetStateByName(typeof(T).Name);
        }

        public IState GetStateByName(string stateName)
        {
            return _stateDictionary.ContainsKey(stateName) ? _stateDictionary[stateName] : null;
        }

        public void Update()
        {
            // Process change state
            if (_pendingStateChange != null)
            {
                var t1 = _pendingStateChange;
                var t2 = _pendingStateParam;

                // Clean state
                _pendingStateChange = null;
                _pendingStateParam = null;

                DoChangeState(t1, t2);
            }

            // Update current state
            if (CurrentState != null)
                CurrentState.Update();
        }

        public void Destroy()
        {
            if (CurrentState != null)
                CurrentState.OnExit();
        }

        #endregion

        public StateMachine(bool enableDebug = false, string name = "default")
        {
            EnableDebug = enableDebug;
            Name = name;
        }

        private void DoChangeState(string stateName, object param)
        {
            if (CurrentState != null && CurrentState.StateName == stateName)
                return;

            // Get new state
            var newState = GetStateByName(stateName);
            if (newState == null)
                throw new Exception("fsm: Invalid state name " + stateName);

            if (CurrentState != null)
            {
                // Exit current state
                CurrentState.OnExit();
            }

            PreviousState = CurrentState;
            CurrentState = newState;

            if (EnableDebug)
            {
                Watchdog.Log(Name,
                    string.Format("Change state from {0} to {1}", PreviousState, CurrentState));
            }

            // Enter new state
            CurrentState.OnEnter(param);

            if (OnStateChanged != null) OnStateChanged(PreviousState, CurrentState);
        }
    }
}