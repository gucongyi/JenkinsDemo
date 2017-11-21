using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GPCommon
{
    public class CallbackQueue
    {
        public Queue<Action> queue = new Queue<Action>();

        public void Enqueue(Action callback)
        {
            queue.Enqueue(callback);
        }

        public void Trigger()
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue()();
        }
    }
}