using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;
using Dignus.Unity.Coroutine;
using System;
using System.Collections;

namespace Assets.Scripts.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class UnityMainThread
    {
        private readonly static SynchronizedArrayQueue<Action> _actions = new();
        private static bool _running = true;
        public static void Add(Action action)
        {
            _actions.Add(action);
        }
        public static void Stop()
        {
            _running = false;
        }
        public static void Start()
        {
            _running = true;
            DignusUnityCoroutineManager.Start(ExecutePending());
        }
        private static IEnumerator ExecutePending()
        {
            while (_running)
            {
                while (_actions.Count > 0)
                {
                    var action = _actions.Read();
                    action?.Invoke();
                }
                yield return null;
            }
        }
    }
}
