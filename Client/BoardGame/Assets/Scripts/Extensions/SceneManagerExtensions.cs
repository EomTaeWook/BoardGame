using Assets.Scripts.Internals;
using Dignus.Unity;
using Dignus.Unity.Framework;
using System;

namespace Assets.Scripts.Extensions
{
    public static class SceneManagerExtensions
    {
        public static void LoadScene<T>(this DignusUnitySceneManager sceneManager, SceneType sceneType,
            Action<T> completeCallback = null) where T : SceneBase
        {
            var newCompleteCallback = new Action<SceneBase>((sceneBase) =>
            {
                var scene = sceneBase as T;
                completeCallback?.Invoke(scene);
            });
            sceneManager.LoadScene(sceneType.ToString(), newCompleteCallback);
        }
        public static void LoadScene(this DignusUnitySceneManager sceneManager, SceneType sceneType,
            Action<SceneBase> completeCallback = null)
        {
            sceneManager.LoadScene(sceneType.ToString(), completeCallback);
        }
    }
}
