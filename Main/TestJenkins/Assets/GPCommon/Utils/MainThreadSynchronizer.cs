using System;
using System.Collections;
using UnityEngine;

namespace GPCommon
{
    public class MainThreadSynchronizer : MonoBehaviour
    {
        private static MainThreadSynchronizer _instance;

        private readonly Queue _actionQueue = Queue.Synchronized(new Queue());

        public static void Init()
        {
            if (_instance != null) return;

            var go = new GameObject("[MainThreadSynchronizer]");
            DontDestroyOnLoad(go);

            _instance = go.AddComponent<MainThreadSynchronizer>();
        }

        public static void Dispatch(Action action)
        {
            if(action == null) return;
            _instance._actionQueue.Enqueue(action);
        }

        void Update()
        {
            while (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();
                ((Action) action).Invoke();
            }
        }
    }
}