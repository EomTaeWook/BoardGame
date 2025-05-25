using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;
using System;
using System.Collections;

namespace Assets.Scripts.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class UnityMainThread
    {
        private readonly static SynchronizedArrayQueue<Action> _actions = new();

        public static void Run(Action action)
        {
            _actions.Add(action);
        }

        public static IEnumerator ExecutePending()
        {
            while (true)
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
