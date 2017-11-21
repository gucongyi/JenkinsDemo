using UnityEngine;
using System;
using System.Collections.Generic;

namespace GPCommon
{
    internal class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; set; }

        public static void Init()
        {
            if (Instance != null) return;

            var go = new GameObject("[TimerManager]");
            DontDestroyOnLoad(go);

            Instance = go.AddComponent<TimerManager>();
        }

        private readonly List<Timer> _currentList = new List<Timer>();
        private readonly List<Timer> _pendingDelete = new List<Timer>();

        public void AddTimer(Timer timer)
        {
            _currentList.Add(timer);
        }

        public void KillTimer(Timer timer)
        {
            _pendingDelete.Add(timer);
        }

        public void KillTimerByLayer(string layer)
        {
            _currentList.ForEach(x =>
            {
                if (x.Layer == layer)
                {
                    KillTimer(x);
                }
            });
        }

        void Update()
        {
            // Process timer update
            int c = _currentList.Count;
            for (int i = 0; i < c; i++)
            {
                _currentList[i].Update();
            }

            // Process timer delete
            if (_pendingDelete.Count != 0)
            {
                for (int i = 0; i < _pendingDelete.Count; i++)
                {
                    var t = _pendingDelete[i];
                    if (_currentList.Contains(t))
                        _currentList.Remove(t);
                }

                _pendingDelete.Clear();
            }
        }
    }

    public class Timer
    {
        #region User-Interface

        public static void Init()
        {
            TimerManager.Init();
        }

        public static bool CheckRunning(Timer timer)
        {
            return timer != null && timer.CurrentState == State.Running;
        }

        public static Timer DelayAction(Action action, float timeSpan, string layer = null)
        {
            var timer = new Timer
            {
                Layer = layer,
                IsEndless = false,
                TimeSpan = timeSpan
            };

            timer.OnStateChanged = (state, time) =>
            {
                if (state == State.Completed)
                {
                    action();
                    timer.Kill();
                }
            };

            timer.Run();

            return timer;
        }

        public static Timer EndlessRepeat(Action action, float timeRepeat, string layer = null)
        {
            var timer = new Timer
            {
                Layer = layer,
                IsEndless = true, // Endless timer
                TimeRepeat = timeRepeat,
                OnRepeatChanged = action
            };

            timer.Run();

            return timer;
        }

        public static Timer Repeat(Action repeatAction, float timeRepeat, Action onComplete, float timeSpan,
            string layer = null)
        {
            var timer = new Timer
            {
                Layer = layer,
                IsEndless = false,
                TimeSpan = timeSpan,
                TimeRepeat = timeRepeat
            };

            timer.OnStateChanged = (state, time) =>
            {
                if (state == State.Completed)
                {
                    onComplete();
                    timer.Kill();
                }
            };

            timer.OnRepeatChanged = repeatAction;

            timer.Run();

            return timer;
        }

        public static Timer UpdateInEveryFrame(Action action, string layer = null)
        {
            var timer = new Timer
            {
                Layer = layer,
                IsEndless = true, // Endless timer
                OnEveryUpdate = (t) => { action(); },
            };

            timer.Run();
            return timer;
        }

        public static Timer UpdateUntilReturnTrue(Func<bool> func, string layer = null)
        {
            var timer = new Timer
            {
                Layer = layer,
                IsEndless = true, // Endless timer
                OnEveryUpdate = (t) =>
                {
                    if (func())
                    {
                        t.Kill();
                    }
                },
            };

            timer.Run();
            return timer;
        }

        public static void KillTimerByLayer(string layer)
        {
            TimerManager.Instance.KillTimerByLayer(layer);
        }

        #endregion

        public enum State
        {
            Idle,
            Running,
            Paused,
            Canceled,
            Completed
        }

        private State _currentState = State.Idle;
        private float _timeElapsed;
        private int _repeatCount;

        public State CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState == value) return;

                _currentState = value;

                if (OnStateChanged != null)
                    OnStateChanged(value, this);
            }
        }

        /// <summary>
        /// The time span for timer countdown, unit: second.
        /// </summary>
        public float TimeSpan { get; set; }

        public string Layer { get; set; }

        public float TimeRepeat { set; get; }

        public bool IsEndless { get; set; }

        public Action<State, Timer> OnStateChanged { get; set; }

        public Action<Timer> OnEveryUpdate { get; set; }

        public Action OnRepeatChanged { get; set; }

        public float Progress
        {
            get { return Mathf.Clamp01(_timeElapsed / TimeSpan); }
            set
            {
                _timeElapsed = TimeSpan * value;

                if (OnEveryUpdate != null)
                    OnEveryUpdate(this);
            }
        }

        public float Timeleft
        {
            get { return TimeSpan - _timeElapsed; }
        }

        public Timer()
        {
            Reset();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            TimerManager.Instance.AddTimer(this);
        }

        public void Run()
        {
            CurrentState = State.Running;
        }

        public void Pause()
        {
            if (CurrentState == State.Running)
                CurrentState = State.Paused;
        }

        public void Resume()
        {
            if (CurrentState == State.Paused)
                CurrentState = State.Running;
        }

        public void Reset()
        {
            // Resetting values prepare for use
            _timeElapsed = 0f;
            _repeatCount = 0;
            CurrentState = State.Idle;
        }

        public void Restart()
        {
            Reset();
            Run();
        }

        public void Kill()
        {
            CurrentState = State.Canceled;
            TimerManager.Instance.KillTimer(this);
        }

        public bool IsRunComplete
        {
            get { return Timeleft <= 0f && !IsEndless; }
        }

        public void Update()
        {
            if (CurrentState != State.Running) return;

            if (IsRunComplete)
            {
                CurrentState = State.Completed;
            }
            else
            {
                _timeElapsed += Time.deltaTime;

                // Process repeat count action
                var newRepeatCount = (int) (_timeElapsed / TimeRepeat);
                if (_repeatCount != newRepeatCount)
                {
                    _repeatCount = newRepeatCount;

                    if (OnRepeatChanged != null)
                        OnRepeatChanged();
                }
            }

            if (OnEveryUpdate != null)
                OnEveryUpdate(this);
        }
    }
}