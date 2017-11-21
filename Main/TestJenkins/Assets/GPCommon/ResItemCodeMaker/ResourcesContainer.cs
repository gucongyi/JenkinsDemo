using System.Collections.Generic;
using UnityEngine;

namespace GPCommon
{
    /// <summary>
    /// if T is GameObject，then managed by 'GameObjectPool'
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResourcesContainer<T> where T : Object
    {
        private GameObjectPool _pool;
        private readonly Dictionary<string, T> _resDic;
        private readonly IResCode _resGetter;
        private readonly bool _isGameObjectContainer;
        private readonly string _defaultResName;

        public ResourcesContainer(IResCode resGetter, string defaultResName = null, Transform trans = null,
            GameObjectPool.ICacheProcessor cacheProcessor = null)
        {
            _resGetter = resGetter;
            _resDic = new Dictionary<string, T>();
            _isGameObjectContainer = typeof(T) == typeof(GameObject);
            _defaultResName = defaultResName;

            InitGameObjectPool(trans, cacheProcessor);
        }

        public T Get(string resName, Transform trans = null)
        {
            // Get resource path by resource name or default resource name
            var resPath = _resGetter.GetResPath(resName);

            if (resPath == null)
            {
                Watchdog.LogWarning("Get", resName + " not found");
                resPath = _resGetter.GetResPath(_defaultResName);
            }

            if (resPath == null)
                return default(T);

            if (!_resDic.ContainsKey(resPath))
            {
                _resDic.Add(resPath, Resources.Load<T>(resPath));
            }

            var loaded = _resDic[resPath];

            if (_isGameObjectContainer)
            {
                return _pool.Get(loaded as GameObject, trans) as T;
            }

            return loaded;
        }

        public void Release(GameObject instance)
        {
            if (_pool != null)
                _pool.Release(instance);
        }

        public void Clear()
        {
            if (_pool != null)
                _pool.Clear();

            foreach (var o in _resDic.Values)
            {
                Resources.UnloadAsset(o);
            }

            _resDic.Clear();
        }

        private void InitGameObjectPool(Transform trans, GameObjectPool.ICacheProcessor cacheProcessor)
        {
            if (_isGameObjectContainer)
            {
                if (cacheProcessor == null)
                {
                    _pool = GameObjectPool.CreateSetActivePool(_resGetter.ResRoot, trans);
                }
                else
                {
                    _pool = GameObjectPool.CreateRaw(_resGetter.ResRoot, trans);
                    _pool.CacheProcessor = cacheProcessor;
                }
            }
        }
    }
}