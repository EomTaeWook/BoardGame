using Assets.Scripts.Extensions;
using Assets.Scripts.Scene.WallGo.UI;
using Dignus.Unity.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.WallGo
{
    public class WallGoScene : SceneBase<WallGoSceneController>
    {
        protected override void OnAwakeScene()
        {
            this.SceneController.OnAwake();
        }

        protected override void OnDestroyScene()
        {
            this.SceneController.Dispose();
        }

        private void FixedUpdate()
        {
            SceneController.Update();
        }
    }
}
