using Dignus.Log;
using Dignus.Unity;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    internal static class GameObjectExtensions
    {
        public static T InstantiateWithPool<T>(this MonoBehaviour caller) where T : Component
        {
            var prefab = DignusUnityResourceManager.Instance.LoadAsset<T>();
            if (prefab == null)
            {
                LogHelper.Error($"no prefab path : {typeof(T).Name}");
                return null;
            }
            T item;
            item = DignusUnityObjectPool.Instance.Pop<T>(prefab);

            item.gameObject.transform.SetParent(caller.transform, false);
            item.gameObject.SetActive(true);
            return item;
        }
        public static T InstantiateWithPool<T>(this MonoBehaviour caller, string path) where T : Component
        {
            var prefab = DignusUnityResourceManager.Instance.LoadAsset<T>(path);
            if (prefab == null)
            {
                LogHelper.Error($"no prefab path : {path}");
                return null;
            }
            T item;
            item = DignusUnityObjectPool.Instance.Pop<T>(prefab);
            item.transform.SetParent(caller.transform, false);
            item.gameObject.SetActive(true);
            return item;
        }

        public static T InstantiateWithPool<T>(this MonoBehaviour caller, T prefab) where T : Component
        {
            T item = DignusUnityObjectPool.Instance.Pop<T>(prefab);
            item.transform.SetParent(caller.transform, false);
            item.gameObject.SetActive(true);
            return item;
        }
        public static T InstantiateWithPool<T>(this MonoBehaviour caller, GameObject prefab) where T : Component
        {
            T item = DignusUnityObjectPool.Instance.Pop<T>(prefab);
            item.transform.SetParent(caller.transform, false);
            item.gameObject.SetActive(true);
            return item;
        }
    }
}
