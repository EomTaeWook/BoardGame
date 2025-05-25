using Dignus.Unity.Framework;

namespace Assets.Scripts.Scene.WallGo
{
    public class WallGoScene : SceneBase<WallGoSceneController>
    {
        protected override void OnAwakeScene()
        {
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
