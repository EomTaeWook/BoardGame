using Assets.Scripts.Internals;
using Assets.Scripts.Scene.WallGo.UI;
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

        protected override void OnDestroyScene()
        {
            this.SceneController.Dispose();
        }

        public void OnRemoveWallButtonClick()
        {
            var playerInfo = SceneController.GetCurrentPlayer();
            if(playerInfo.WallGoPlayer.HasUsedBreakWall == true)
            {
                return;
            }

            if(!SceneController.IsPlayerTurn())
            {
                UIManager.Instance.ShowToastAlert($"현재 턴이 아닙니다.", 1.5F);
                return;
            }

            if(playerInfo.WallGoPlayer.MovePieceCount != 0)
            {
                UIManager.Instance.ShowToastAlert($"이동을 하였기에 아이템을 사용하지 못합니다.", 1.5F);
                return;
            }

            if (playerInfo.WallGoPlayer.State != GameContents.Share.StateType.MovePeice)
            {
                UIManager.Instance.ShowToastAlert($"이동 전에만 사용 가능합니다.", 1.5F);
                return;
            }
            playerInfo.WallGoPlayer.ChangeState(GameContents.Share.StateType.RemoveWall);
            UIManager.Instance.ShowToastAlert($"부술 벽을 선택하세요", 1.5F);
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
