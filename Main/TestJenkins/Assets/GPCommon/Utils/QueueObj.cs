using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GPCommon
{


    public class QueueValue<T>
    {
        private readonly List<T> _objList = new List<T>();

        public event Action OnChanged;

        public void Enqueue(T obj)
        {
            _objList.Add(obj);

            if (OnChanged != null)
                OnChanged();
        }

        public T Dequeue()
        {
            if (IsEmpty()) return default(T);

            var o = _objList[0];

            _objList.RemoveAt(0);

            if (OnChanged != null)
                OnChanged();

            return o;
        }

        public bool Remove(T obj)
        {
            if (!_objList.Contains(obj)) return false;

            _objList.Remove(obj);

            if (OnChanged != null)
                OnChanged();

            return true;
        }

        public bool IsEmpty()
        {
            return _objList.Count == 0;
        }

        public T Peek()
        {
            return !IsEmpty() ? _objList[0] : default(T);
        }
    }
}