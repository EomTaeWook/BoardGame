using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title.UI;
using Dignus.Unity.DependencyInjection;
using Dignus.Unity.Framework;
using UnityEngine;

namespace Assets.Scripts.Scene.Title
{
    public class TitleScene : SceneBase
    {
        [SerializeField]
        private BuildTaretType _buildTaretType;

        private TitleSceneController _titleSceneController;
        protected override void OnAwakeScene()
        {
            ApplicationManager.Instance.Init(_buildTaretType);

            _titleSceneController = DignusUnityServiceContainer.Resolve<TitleSceneController>();
            _titleSceneController.BindScene(this);

            _titleSceneController.Init();
        }
        public BuildTaretType GetBuildTargetType()
        {
            return _buildTaretType;
        }
        public void ShowCreateAccountUI()
        {
            var popup = UIManager.Instance.AddPopupUI<CreateAccountUI>();

            popup.SetCreateAccountCallback((nickname) =>
            {
                _titleSceneController.CreateAccount(nickname);

                _titleSceneController.ProcessLogin();
            });
        }
        public override void OnDestroyScene()
        {
            _titleSceneController.Dispose();
        }
    }
}
