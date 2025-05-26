using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Dignus.Unity.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.WallGo.UI
{
    [PrefabPath(Consts.Path.WallGoUI)]
    public class PlayerInfoGo : UiItem
    {
        [SerializeField]
        private Image _imageTurn;
        [SerializeField]
        private Image _profileBackground;
        [SerializeField]
        private TextMeshProUGUI _nicknameText;
        [SerializeField]
        private TextMeshProUGUI _stateText;

        private WallGoSceneController _wallGoSceneController;

        public WallGoPlayer WallGoPlayer { get; private set; }

        public Color PlayerColor { get; private set; }

        public void Init(WallGoSceneController wallGoSceneController,
            Color color,
            WallGoPlayer player)
        {
            PlayerColor = color;
            WallGoPlayer = player;
            _wallGoSceneController = wallGoSceneController;
            _profileBackground.color = color;

            _nicknameText.text = WallGoPlayer.Nickname;

            RefreshUI();
        }

        public void RefreshUI()
        {
            ChangeState(WallGoPlayer.State);
        }
        public void SetTurnIndicator(bool isActive)
        {
            if (isActive == true)
            {
                _imageTurn.color = Color.red;
                _stateText.gameObject.SetActive(true);
            }
            else
            {
                _imageTurn.color = Color.white;
                _stateText.gameObject.SetActive(false);
            }
        }
        public void ChangeState(StateType stateType)
        {
            if (stateType == StateType.SpawnPiece1 ||
                stateType == StateType.SpawnPiece)
            {
                _stateText.text = "spawn piece";
            }
            else if (stateType == StateType.MovePeice)
            {
                _stateText.text = $"move piece : {WallGoPlayer.MovePieceCount}";
            }
            else if (stateType == StateType.PlaceWall)
            {
                _stateText.text = "place wall";
            }
        }
    }
}
