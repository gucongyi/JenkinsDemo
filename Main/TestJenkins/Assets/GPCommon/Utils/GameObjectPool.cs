using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace GPCommon
{
    /// <summary>
    /// 针对GameObject设计的对象池
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        /// <summary>
        /// 使用SetActive式缓存
        /// </summary>
        public static GameObjectPool CreateSetActivePool(string displayName, Transform trans = null)
        {
            GameObjectPool pool = CreateRaw(displayName, trans);
            pool.CacheProcessor = new SetActiveProcessor();

            return pool;
        }

        /// <summary>
        /// 使用SetScale式缓存
        /// </summary>
        public static GameObjectPool CreateSetScalePool(string displayName, Transform trans = null)
        {
            GameObjectPool pool = CreateRaw(displayName, trans);
            pool.CacheProcessor = new SetScaleProcessor();

            return pool;
        }

        /// <summary>
        /// 不使用缓存策略
        /// </summary>
        public static GameObjectPool CreateDefaultPool(string displayName, Transform trans = null)
        {
            GameObjectPool pool = CreateRaw(displayName, trans);
            pool.CacheProcessor = new NullProcessor();

            return pool;
        }

        /// <summary>
        /// 创建一个不带缓存策略的对象池，可以外部自定义缓存策略
        /// </summary>
        public static GameObjectPool CreateRaw(string displayName, Transform trans = null)
        {
            GameObject go = new GameObject(string.Format("[{0}]", displayName));
            go.transform.SetParent(trans, false);

            // DontDestroyOnLoad only work for root GameObjects or components on root GameObjects.
            if (go.transform.parent == null)
                DontDestroyOnLoad(go);

            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            pool.DisplayName = displayName;

            return pool;
        }

        /// <summary>
        /// 对象池缓存策略
        /// </summary>
        public interface ICacheProcessor
        {
            void OnPop(GameObject instance);
            void OnCahced(GameObject instance, GameObjectPool pool);
        }

        /// <summary>
        /// gameobject.SetActive式的缓存
        /// </summary>
        public class SetActiveProcessor : ICacheProcessor
        {
            public void OnPop(GameObject instance)
            {
                instance.SetActive(true);
            }

            public void OnCahced(GameObject instance, GameObjectPool pool)
            {
                instance.SetActive(false);
            }
        }

        /// <summary>
        /// transform.SetScale式的缓存，速度上有优势，但是会影响item的scale使用
        /// </summary>
        public class SetScaleProcessor : ICacheProcessor
        {
            public void OnPop(GameObject instance)
            {
                instance.transform.localScale = Vector3.one;
            }

            public void OnCahced(GameObject instance, GameObjectPool pool)
            {
                instance.transform.localScale = Vector3.zero;
            }
        }

        /// <summary>
        /// 缓存时不做任何操作
        /// </summary>
        public class NullProcessor : ICacheProcessor
        {
            public void OnPop(GameObject instance)
            {
            }

            public void OnCahced(GameObject instance, GameObjectPool pool)
            {
            }
        }

        public ICacheProcessor CacheProcessor;
        public string DisplayName;

        // prefab -> instance list
        private Dictionary<GameObject, Stack<GameObject>> pool;

        // instance hash code-> prefab
        private Dictionary<int, GameObject> keepPrefab;

        /// <summary>
        /// 在某节点下，预加载指定数目实例并缓存
        /// </summary>
        /// <param name="prefab">以prefab为key</param>
        /// <param name="parent">目标节点</param>
        /// <param name="count">目标数目</param>
        /// <param name="onInstantiate">当有新的Item生成时调用</param>
        public void Prepare(GameObject prefab, Transform parent, int count, Action<GameObject> onInstantiate = null)
        {
            Constrain(prefab, parent, count, onInstantiate);
            Constrain(prefab, parent, 0);
        }

        /// <summary>
        /// 控制某节点下的实例数目
        /// </summary>
        /// <param name="prefab">以prefab为key</param>
        /// <param name="parent">目标节点</param>
        /// <param name="count">目标数目</param>
        /// <param name="onInstantiate">当有新的Item生成时调用</param>
        /// <returns></returns>
        public void Constrain(GameObject prefab, Transform parent, int count, Action<GameObject> onInstantiate = null)
        {
            int curCount = parent.childCount;

            while (curCount != count)
            {
                if (curCount > count)
                {
                    Release(parent);
                    curCount--;
                }
                else
                {
                    Get(prefab, parent, onInstantiate);
                    curCount++;
                }
            }
        }

        /// <summary>
        /// 控制某节点下的实例数目并返回GameObject列表
        /// </summary>
        /// <param name="prefab">以prefab为key</param>
        /// <param name="parent">目标节点</param>
        /// <param name="count">目标数目</param>
        /// <returns></returns>
        public List<GameObject> ConstrainAndGetList(GameObject prefab, Transform parent, int count)
        {
            Constrain(prefab, parent, count);

            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < parent.childCount; i++)
            {
                list.Add(parent.GetChild(i).gameObject);
            }
            return list;
        }

        /// <summary>
        /// 控制某节点下的实例数目并返回指定MonoBehaviour脚本列表
        /// </summary>
        /// <typeparam name="T">继承MonoBehaviour的脚本类</typeparam>
        /// <param name="prefab">以prefab为key</param>
        /// <param name="parent">目标节点</param>
        /// <param name="count">目标数目</param>
        /// <param name="update">生成后根据索引刷新</param>
        /// <returns></returns>
        public List<T> ConstrainAndGetList<T>(GameObject prefab, Transform parent, int count,
            Action<T, int> update = null) where T : Component
        {
            Constrain(prefab, parent, count);

            List<T> list = new List<T>();

            for (int i = 0; i < parent.childCount; i++)
            {
                T item = parent.GetChild(i).gameObject.GetComponent<T>();

                if (update != null)
                    update(item, i);

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 在指定某节点下生成指定数目的实例
        /// </summary>
        /// <typeparam name="T">继承MonoBehaviour的脚本类</typeparam>
        /// <param name="prefab">以prefab为key</param>
        /// <param name="parent">目标节点</param>
        /// <param name="count">目标数目</param>
        /// <param name="update">生成后根据索引刷新</param>
        /// <param name="onInstantiate">当有新的Item生成时调用</param>
        public void Get<T>(GameObject prefab, Transform parent, int count, Action<T, int> update = null,
            Action<GameObject> onInstantiate = null) where T : Component
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = Get(prefab, parent, onInstantiate);

                if (update != null)
                {
                    T item = go.GetComponent<T>();
                    update(item, i);
                }
            }
        }


        private const string OnPopMessage = "OnPopFromPool";

        /// <summary>
        /// 在指定某节点下生成一个实例
        /// </summary>
        /// <param name="prefab">以prefab为key</param>
        /// <param name="parent">目标节点</param>
        /// <param name="onInstantiate">Item生成时调用</param>
        /// <returns></returns>
        public GameObject Get(GameObject prefab, Transform parent, Action<GameObject> onInstantiate = null)
        {
            GameObject instance = null;

            if (!pool.ContainsKey(prefab))
            {
                pool.Add(prefab, new Stack<GameObject>());
            }

            if (pool[prefab].Count > 0)
            {
                instance = pool[prefab].Pop();
                instance.transform.parent = parent;

                CacheProcessor.OnPop(instance);
                instance.SendMessage(OnPopMessage, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Vector3 localScale = prefab.transform.localScale; // Keep prefab localScale
                instance = Instantiate(prefab, parent) as GameObject;
                instance.transform.localScale = localScale;

                if (onInstantiate != null) onInstantiate(instance);

                RegisterAsGot(instance, prefab);
            }

            return instance;
        }

        /// <summary>
        /// 回收一个实例并注册，把实例和Prefab绑定，以后回收的时候就可以不用指定Prefab了
        /// </summary>
        public void ReleaseAndRegister(GameObject instance, GameObject prefab)
        {
            RegisterAsGot(instance, prefab);
            Release(instance);
        }

        /// <summary>
        /// 指定节点下回收一个实例
        /// </summary>
        /// <param name="parentTrans">目标节点</param>
        public void Release(Transform parentTrans)
        {
            Transform targetTrans = parentTrans.GetChild(0);
            Release(targetTrans.gameObject);
        }

        /// <summary>
        /// 直接回收一个实例
        /// </summary>
        public void Release(GameObject instance)
        {
            if (instance == null)
                return; // just ignore

            // Check exception
            if (!keepPrefab.ContainsKey(instance.GetHashCode()) || keepPrefab[instance.GetHashCode()] == null)
                throw new Exception("no record found, are you sure this instance was created from 'Get' method ?");

            // Find prefab
            GameObject prefab = keepPrefab[instance.GetHashCode()];

            // Collect to pool
            pool[prefab].Push(instance);

            // Attach as child
            instance.transform.parent = transform;

            // Caching process
            CacheProcessor.OnCahced(instance, this);
        }

        private void RegisterAsGot(GameObject instance, GameObject prefab)
        {
            if (!pool.ContainsKey(prefab))
            {
                pool.Add(prefab, new Stack<GameObject>());
            }

            var hash = instance.GetHashCode();

            if (!keepPrefab.ContainsKey(hash))
            {
                keepPrefab.Add(hash, prefab);
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void Clear()
        {
            keepPrefab.Clear();
            pool.Clear();

            while (transform.childCount != 0)
            {
                Destroy(transform.GetChild(0));
            }
        }

        void OnDestroy()
        {
            keepPrefab.Clear();
            pool.Clear();

            Destroy(gameObject);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (GameObject prefabKey in pool.Keys)
            {
                int cached = pool[prefabKey] != null ? pool[prefabKey].Count : 0;

                int total = 0;
                foreach (GameObject prefab in keepPrefab.Values)
                {
                    if (prefab == prefabKey) total++;
                }

                sb.AppendLine(string.Format("{0}：{1}/{2}\n", prefabKey, cached, total));
            }

            return sb.ToString();
        }

        void Awake()
        {
            pool = new Dictionary<GameObject, Stack<GameObject>>();
            keepPrefab = new Dictionary<int, GameObject>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 打印缓存数目于GameObject的名字上
        /// </summary>
        void Update()
        {
            int sum = 0;

            foreach (GameObject prefabKey in pool.Keys)
            {
                if (pool[prefabKey] != null)
                    sum += pool[prefabKey].Count;
            }

            gameObject.name = sum > 0
                ? string.Format("[{0}, {1} cached]", DisplayName, sum)
                : string.Format("[{0}]", DisplayName);
        }
#endif
    }
}