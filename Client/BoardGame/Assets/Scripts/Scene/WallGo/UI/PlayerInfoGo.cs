using Assets.Scripts.GameContents.Share;
using Assets.Scripts.Internals;
using Dignus.Unity.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.WallGo.UI
{
    [PrefabPath(Consts.Path.WallGoUI)]
    public class PlayerInfoGo : MonoBehaviour
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

        private Color[] _colors = new Color[]
        {
            new Color32(43, 218, 207, 255),// ¹ÎÆ®
            new Color32(245, 166, 35, 255),// ÁÖÈ²
            new Color32(155, 89, 182, 255),// º¸¶ó
            new Color32(255, 229, 143, 255)// ³ë¶û
        };

        public void Init(WallGoSceneController wallGoSceneController, int index)
        {
            _wallGoSceneController = wallGoSceneController;

            _profileBackground.color = _colors[index];
        }
        public void SetTurnIndicator(bool isActive)
        {
            if (isActive == true)
            {
                _imageTurn.color = Color.red;
            }
            else
            {
                _imageTurn.color = Color.white;
            }
        }
        public void ChangeState(StateType stateType)
        {
            if (stateType == StateType.SpawnPiece1 ||
                stateType == StateType.SpawnPiece)
            {
                _stateText.text = "spawn piece";
                _stateText.gameObject.SetActive(true);
            }
            else if (stateType == StateType.MovePeice)
            {
                var count = _wallGoSceneController.Model.CurrentPlayer.MovePieceCount;
                _stateText.text = $"move piece : {count}";
                _stateText.gameObject.SetActive(true);
            }
            else if (stateType == StateType.PlaceWall)
            {
                _stateText.text = "place wall";
                _stateText.gameObject.SetActive(true);
            }
            else
            {
                _stateText.gameObject.SetActive(false);
            }
        }
    }
}
