using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPCommon
{
    public class AutoRemovableList<T> : List<T>
    {
        private class AutoRemovableItem
        {
            public T Item;
            public float Time;
            public Action Callback;
        }

        private readonly List<AutoRemovableItem> _itemList;

        public AutoRemovableList()
        {
            _itemList = new List<AutoRemovableItem>();
        }

        public void Update()
        {
            List<AutoRemovableItem> pendingDelete = null;
            for (int i = 0; i < _itemList.Count; i++)
            {
                var item = _itemList[i];

                item.Time -= Time.deltaTime;
                if (item.Time > 0) continue;

                if (pendingDelete == null) pendingDelete = new List<AutoRemovableItem>();
                pendingDelete.Add(item);
            }

            if (pendingDelete != null)
                pendingDelete.ForEach(RemoveItem);
        }

        public void AddItem(T item, float time, Action callback = null)
        {
            Add(item);

            _itemList.Add(new AutoRemovableItem()
            {
                Item = item,
                Time = time,
                Callback = callback
            });
        }

        private void RemoveItem(AutoRemovableItem item)
        {
            if (Contains(item.Item))
            {
                Remove(item.Item);

                if (item.Callback != null)
                    item.Callback();
            }

            if (_itemList.Contains(item))
                _itemList.Remove(item);
        }

        public void ClearItem()
        {
            Clear();
            _itemList.Clear();
        }
    }
}