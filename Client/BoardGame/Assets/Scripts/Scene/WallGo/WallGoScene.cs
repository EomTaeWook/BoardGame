using Assets.Scripts.Internals;
using Dignus.Unity.Framework;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    public class WallGoScene : SceneBase<WallGoSceneController>
    {
        [SerializeField]
        private Canvas _uiCanvas;

        protected override void OnAwakeScene()
        {
            _uiCanvas.worldCamera = UIManager.Instance.UICamera;
        }

        protected override void OnDestroyScene()
        {
            this.SceneController.Dispose();
        }

        public void OnRemoveWallButtonClick()
        {
            UIManager.Instance.ShowToastAlert($"Select a wall to destroy", 1.5F);


        }

        private void FixedUpdate()
        {
            SceneController.Update();
        }
    }
}
