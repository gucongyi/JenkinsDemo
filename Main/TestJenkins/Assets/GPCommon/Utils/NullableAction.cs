using UnityEngine;
using System.Collections;
using System;

namespace GPCommon
{
    public class NullableAction
    {
        private Action _action;

        public void Invoke()
        {
            if (_action != null)
                _action();
        }

        public void Add(Action other)
        {
            _action += other;
        }

        public void Remove(Action other)
        {
            // ReSharper disable once DelegateSubtraction
            if (other != null) _action -= other;
        }

        public static NullableAction operator +(NullableAction lhs, Action other)
        {
            lhs.Add(other);
            return lhs;
        }

        public static NullableAction operator -(NullableAction lhs, Action other)
        {
            lhs.Remove(other);
            return lhs;
        }
    }

    public class NullableAction<T1>
    {
        private Action<T1> _action;

        public void Invoke(T1 obj1)
        {
            if (_action != null)
                _action(obj1);
        }

        public void Add(Action<T1> other)
        {
            _action += other;
        }

        public void Remove(Action<T1> other)
        {
            // ReSharper disable once DelegateSubtraction
            if (other != null) _action -= other;
        }

        public static NullableAction<T1> operator +(NullableAction<T1> lhs, Action<T1> other)
        {
            lhs.Add(other);
            return lhs;
        }

        public static NullableAction<T1> operator -(NullableAction<T1> lhs, Action<T1> other)
        {
            lhs.Remove(other);
            return lhs;
        }
    }

    public class NullableAction<T1, T2>
    {
        private Action<T1, T2> _action;

        public void Invoke(T1 obj1, T2 obj2)
        {
            if (_action != null)
                _action(obj1, obj2);
        }

        public void Add(Action<T1, T2> other)
        {
            _action += other;
        }

        public void Remove(Action<T1, T2> other)
        {
            // ReSharper disable once DelegateSubtraction
            if (other != null) _action -= other;
        }

        public static NullableAction<T1, T2> operator +(NullableAction<T1, T2> lhs, Action<T1, T2> other)
        {
            lhs.Add(other);
            return lhs;
        }

        public static NullableAction<T1, T2> operator -(NullableAction<T1, T2> lhs, Action<T1, T2> other)
        {
            lhs.Remove(other);
            return lhs;
        }
    }
}