using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPCommon
{
    public class ResizableList<T> : List<T>
    {
        private readonly StackPool<T> _stackPool;

        public ResizableList(Func<T> createFunc, Action<T> onPush = null) : base()
        {
            _stackPool = new StackPool<T>(createFunc, onPush);
        }

        public void Resize(int newCount)
        {
            while (Count != newCount)
            {
                if (Count > newCount)
                {
                    var lastItem = this[Count - 1];
                    _stackPool.Release(lastItem);
                    Remove(lastItem);
                }
                else
                {
                    Add(_stackPool.Get());
                }
            }
        }
    }
}