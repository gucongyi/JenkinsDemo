using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPCommon
{
    public class ListView : MonoBehaviour
    {
        protected class VirtualItem
        {
            private const string VirtualizeMessage = "Virtualize";

            public Vector3 LocalPosition;
            public BaseListViewItem ListViewItem;
            public int Index;
            public Transform Parent;
            public Action<BaseListViewItem, int> OnUpdate;
            public GameObjectPool Pool;
            public GameObject CurContent;
            public bool IsRealized;

            public Vector3 Position
            {
                get { return LocalPosition + Parent.transform.localPosition; }
            }

            public bool Realize()
            {
                if (IsRealized)
                    return false;

                // Create content
                CurContent = Pool.Get(ListViewItem.gameObject, Parent);
                CurContent.transform.localPosition = LocalPosition;

                Update();

                IsRealized = true;

                return true;
            }

            public bool Virtualize()
            {
                if (!IsRealized || CurContent == null)
                {
                    return false;
                }

                CurContent.SendMessage(VirtualizeMessage, SendMessageOptions.DontRequireReceiver);

                Pool.Release(CurContent);

                CurContent = null;
                IsRealized = false;

                return true;
            }

            public void Update()
            {
                if (OnUpdate != null)
                    OnUpdate(CurContent.GetComponent<BaseListViewItem>(), Index);
            }
        }

        public bool EnableDebug;
        public BaseListViewItem ListViewItem;

        private Transform _root;
        private GameObjectPool _pool;
        private ResizableList<VirtualItem> _virtualItems;
        private Action<BaseListViewItem, int> _internalItemUpdate;

        private bool _inited;

        public int ContentHeight
        {
            get
            {
                if (_virtualItems == null || ListViewItem == null)
                    return 0;

                int lineCount = Mathf.CeilToInt(_virtualItems.Count / ListViewItem.LineCount);
                return lineCount * ListViewItem.ItemHeight;
            }
        }

        public int DataCount
        {
            get { return _virtualItems.Count; }
        }

        [ContextMenu("SortChildren")]
        public void SortChildren()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.localPosition = GetItemLocalPosition(ListViewItem, i);
            }
        }

        public void UpdateList<T>(List<T> list)
        {
            Init();

            var t = list ?? new List<T>();

            _virtualItems.Resize(t.Count);

            _internalItemUpdate = (item, index) => { item.SetData(t[index]); };

            InitScrolling();
        }

        public void UpdateList<TItem, TItemView>(List<TItem> list, Action<TItem, TItemView> onItemUpdated)
            where TItemView : BaseListViewItem
        {
            Init();

            var t = list ?? new List<TItem>();

            _virtualItems.Resize(t.Count);

            _internalItemUpdate = (item, index) =>
            {
                var data = t[index];
                item.SetData(data);
                onItemUpdated(data, item as TItemView);
            };

            InitScrolling();
        }

        public void UpdateList()
        {
            if (!_inited)
                return;

            _virtualItems.ForEach(x =>
            {
                if (x.IsRealized)
                    x.Update();
            });
        }

        public void SetScrollPercent(float percent)
        {
            RootPosY = Mathf.Lerp(_minRootPosY, _maxRootPosY, percent);
            RefreshAllItems();
        }

        protected virtual void Init()
        {
            if (_inited)
                return;

            // Will cache exist item later
            var pendingToCahce = new List<GameObject>();
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                pendingToCahce.Add(child);
            }

            // Create root for scrolling
            _root = new GameObject("Root").transform;
            _root.SetParent(transform, false);

            // Create object pool in root
            _pool = GameObjectPool.CreateSetScalePool("Pool", _root);
            pendingToCahce.ForEach(x =>
            {
                _pool.ReleaseAndRegister(x, ListViewItem.gameObject);
            });

            // Init virtual item record
            _virtualItems = new ResizableList<VirtualItem>(() =>
            {
                var item = new VirtualItem();
                item.ListViewItem = ListViewItem;
                item.Index = _virtualItems.Count;
                item.Parent = _root;
                item.OnUpdate = OnItemUpdate;
                item.Pool = _pool;
                item.LocalPosition = GetItemLocalPosition(ListViewItem, item.Index);
                return item;
            }, VisualizeItem);

            _inited = true;
        }

        private void OnItemUpdate(BaseListViewItem item, int index)
        {
            if (_internalItemUpdate != null)
                _internalItemUpdate(item, index);
        }

        private Vector3 GetItemLocalPosition(BaseListViewItem listViewItem, int index)
        {
            var posX = (index % listViewItem.LineCount) * listViewItem.ItemWidth;
            var posY = -(index / listViewItem.LineCount) * listViewItem.ItemHeight;

            return new Vector3(posX, posY, 0);
        }

        #region Scrolling Input

        [Tooltip("Public for debugging")] public int RangeStart;

        [Tooltip("Public for debugging")] public int RangeEnd;

        private float _velocity;
        private float _maxRootPosY;
        private float _minRootPosY;

        private float RootPosY
        {
            get { return _root.transform.localPosition.y; }
            set
            {
                var localPositon = _root.transform.localPosition;
                localPositon.y = value;
                _root.transform.localPosition = localPositon;
            }
        }

        internal void OnScroll(float deltaY)
        {
            if (!_inited)
            {
                Debug.LogWarning("ListView havn't updated list yet");
                return;
            }

            _velocity = deltaY;
        }

        private void InitScrolling()
        {
            // Figure out max and min root postion
            _minRootPosY = 0f;

            if (_virtualItems.Count != 0)
            {
                var lastItem = _virtualItems.Last();
                var lastItemEndY = lastItem.LocalPosition.y - lastItem.ListViewItem.ItemHeight;
                _maxRootPosY = Mathf.Max(0, GetClipEndY() - lastItemEndY);
            }
            else
            {
                _maxRootPosY = 0f;
            }

            // Clamp root position between max and min position
            RootPosY = Mathf.Clamp(RootPosY, _minRootPosY, _maxRootPosY);

            // Test what item should display
            RefreshAllItems();
        }

        private void RefreshAllItems()
        {
            var realizingStarted = false;
            RangeStart = 0;
            RangeEnd = 0;

            // Go through all virtual items
            for (var i = 0; i < _virtualItems.Count; i++)
            {
                var item = _virtualItems[i];

                if (ShouldRealize(item))
                {
                    // Figure out range start and end index
                    if (!realizingStarted)
                    {
                        RangeStart = i;
                        realizingStarted = true;
                    }

                    RangeEnd = i;

                    if (item.IsRealized)
                        item.Update();
                    else
                        RealizeItem(item);
                }
                else
                {
                    VisualizeItem(item);
                }
            }
        }

        private void OnForwardScrolling()
        {
            RangeStartVisualizeTest();
            RangeEndRealizeTest();
        }

        private void RangeStartVisualizeTest()
        {
            if (RangeStart >= _virtualItems.Count - 1 || RangeStart >= RangeEnd) return;

            var item = _virtualItems[RangeStart];
            if (!ShouldRealize(item))
            {
                VisualizeItem(item);

                RangeStart += 1;
                RangeStartVisualizeTest();
            }
        }

        private void RangeEndRealizeTest()
        {
            if (RangeEnd >= _virtualItems.Count - 1) return;

            var item = _virtualItems[RangeEnd + 1];
            if (ShouldRealize(item))
            {
                RealizeItem(item);

                RangeEnd += 1;
                RangeEndRealizeTest();
            }
        }

        private void OnBackwardScrolling()
        {
            RangeEndVisualizeTest();
            RangeStartRealizeTest();
        }

        private void RangeEndVisualizeTest()
        {
            if (RangeEnd <= RangeStart) return;

            var item = _virtualItems[RangeEnd];
            if (!ShouldRealize(item))
            {
                VisualizeItem(item);

                RangeEnd -= 1;
                RangeEndVisualizeTest();
            }
        }

        private void RangeStartRealizeTest()
        {
            if (RangeStart <= 0) return;

            var item = _virtualItems[RangeStart - 1];
            if (ShouldRealize(item))
            {
                RealizeItem(item);

                RangeStart -= 1;
                RangeStartRealizeTest();
            }
        }

        private void RealizeItem(VirtualItem item)
        {
            if (item.IsRealized) return;

            item.Realize();
        }

        private void VisualizeItem(VirtualItem item)
        {
            if (!item.IsRealized) return;

            item.Virtualize();
        }

        private float _dirtyY;
        private bool _isDirty;

        void Update()
        {
            if (!_inited) return;

            _dirtyY = RootPosY;
            _isDirty = false;

            if (Math.Abs(_velocity) > 0.03f)
            {
                _dirtyY += _velocity;

                // Velocity attenuation
                _velocity *= 0.8f;

                _isDirty = true;
            }

            // Clamp root position between max and min smoothly
            if (_dirtyY < _minRootPosY)
            {
                _dirtyY = Mathf.Lerp(_dirtyY, _minRootPosY, 0.1f);

                if (_minRootPosY - _dirtyY < 0.03f)
                    _dirtyY = _minRootPosY;

                _isDirty = true;
            }
            else if (_dirtyY > _maxRootPosY)
            {
                _dirtyY = Mathf.Lerp(_maxRootPosY, _dirtyY, 0.9f);

                if (Math.Abs(_dirtyY - _maxRootPosY) < 0.03f)
                    _dirtyY = _maxRootPosY;

                _isDirty = true;
            }

            if (_isDirty)
            {
                // Detect scrolling direction
                var isForward = RootPosY - _dirtyY < 0;

                RootPosY = _dirtyY;

                if (isForward)
                    OnForwardScrolling();
                else
                    OnBackwardScrolling();
            }
        }

        private bool ShouldRealize(VirtualItem item)
        {
            var itemStartY = item.Position.y;
            var itemEndY = item.Position.y - item.ListViewItem.ItemHeight;
            var clipStart = GetClipStartY();
            var clipEndY = GetClipEndY();

            bool result;
            if (clipEndY > itemStartY || clipStart < itemEndY)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            if (EnableDebug)
            {
                Debug.Log(" ShouldRealize ->" +
                          " name: " + item.ListViewItem.gameObject.name +
                          " index: " + item.Index +
                          " itemStartY: " + itemStartY +
                          " itemEndY: " + itemEndY +
                          " clipStart: " + clipStart +
                          " clipEndY: " + clipEndY +
                          " result: " + result);
            }
            return result;
        }

        #endregion

        protected virtual float GetClipStartY()
        {
            throw new NotImplementedException();
        }

        protected virtual float GetClipEndY()
        {
            throw new NotImplementedException();
        }
    }
}