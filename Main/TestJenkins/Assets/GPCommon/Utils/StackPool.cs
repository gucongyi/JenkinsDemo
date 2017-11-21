using System.Collections.Generic;
using System;

namespace GPCommon
{
    public class StackPool<T>
    {
        private readonly Stack<T> _stack = new Stack<T>();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onPush;

        public StackPool(Func<T> createFunc = null, Action<T> onPush = null)
        {
            _createFunc = createFunc;
            _onPush = onPush;
        }

        public T Get()
        {
            if (_stack.Count != 0)
            {
                return _stack.Pop();
            }
            else
            {
                if (_createFunc == null)
                    return Activator.CreateInstance<T>();
                else
                    return _createFunc();
            }
        }

        public void Release(T item)
        {
            if (_onPush != null)
                _onPush(item);

            _stack.Push(item);
        }

        public void Release(List<T> itemList)
        {
            var count = itemList.Count;
            for (var i = 0; i < count; i++)
            {
                Release(itemList[i]);
            }
        }

        public void Drain()
        {
            _stack.Clear();
        }
    }
}