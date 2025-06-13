using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Dignus.Unity.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.WallGo
{
    public class WallGoScene : SceneBase<WallGoSceneController>
    {
        [SerializeField]
        private Canvas _uiCanvas;

        [SerializeField]
        private Button _removeWallButton;

        [SerializeField]
        private Button _movePieceButton;

        protected override void OnAwakeScene()
        {
            _uiCanvas.worldCamera = UIManager.Instance.UICamera;
        }

        public override void OnDestroyScene()
        {
            SceneController.Dispose();
        }

        public void OnRemoveWallButtonClick()
        {
            var playerInfo = SceneController.GetCurrentPlayer();
            if (playerInfo.WallGoPlayer.HasUsedBreakWall == true)
            {
                return;
            }

            if (!SceneController.IsPlayerTurn())
            {
                UIManager.Instance.ShowToastAlert(StringHelper.GetString(1010), 1.5F);
                return;
            }

            if (playerInfo.WallGoPlayer.MovePieceCount != 0)
            {
                UIManager.Instance.ShowToastAlert(StringHelper.GetString(1011), 1.5F);
                return;
            }

            if (playerInfo.WallGoPlayer.State != GameContents.Share.StateType.MovePeice)
            {
                UIManager.Instance.ShowToastAlert(StringHelper.GetString(1012), 1.5F);
                return;
            }
            playerInfo.WallGoPlayer.ChangeState(GameContents.Share.StateType.RemoveWall);
            UIManager.Instance.ShowToastAlert(StringHelper.GetString(1013), 1.5F);
            _removeWallButton.gameObject.SetActive(false);
            _movePieceButton.gameObject.SetActive(true);
        }

        public void InactiveRemoveWall()
        {
            _removeWallButton.interactable = false;
        }

        public void OnMovePieceButtonClick()
        {
            var playerInfo = SceneController.GetCurrentPlayer();

            playerInfo.WallGoPlayer.ChangeState(GameContents.Share.StateType.MovePeice);
            _movePieceButton.gameObject.SetActive(false);
            _removeWallButton.gameObject.SetActive(true);
        }
        private void FixedUpdate()
        {
            SceneController.Update();
        }
    }
}
