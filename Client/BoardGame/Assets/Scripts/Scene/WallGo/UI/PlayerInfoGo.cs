using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Dignus.Coroutine;
using Dignus.Coroutine.Interfaces;
using Dignus.Unity.Attributes;
using System;
using System.Collections;
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
        [SerializeField]
        private TextMeshProUGUI _turnTimeoutText;

        private WallGoSceneController _wallGoSceneController;

        public WallGoPlayer WallGoPlayer { get; private set; }

        public Color PlayerColor { get; private set; }

        private CoroutineHandler _coroutineHandler = new();

        public bool IsRemoveWallMode { get; private set; }
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
        public void UseRemoveWallState(bool isRemoveWallMode)
        {
            IsRemoveWallMode = isRemoveWallMode;
        }
        public void SetTurnActive(bool isActive)
        {
            if (isActive)
            {
                _imageTurn.gameObject.SetActive(true);
                _coroutineHandler.Start(RefreshTurnTime());
            }
            else
            {
                _imageTurn.gameObject.SetActive(false);
                _coroutineHandler.StopAll();
            }
        }

        private IEnumerator RefreshTurnTime()
        {
            while(true)
            {
                _turnTimeoutText.text = $"{GetRemainTurnTime()}";
                yield return null;
            }
        }

        private void Update()
        {
            _coroutineHandler.UpdateCoroutines(Time.deltaTime);
            
        }

        public void RefreshUI()
        {
            ChangeState(WallGoPlayer.State);
        }
        private int GetRemainTurnTime()
        {
            return 90 - (DateTime.UtcNow - WallGoPlayer.TurnStartTime).Seconds;
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
                _stateText.text = $"move count : {2 - WallGoPlayer.MovePieceCount}";
            }
            else if (stateType == StateType.PlaceWall)
            {
                _stateText.text = "place wall";
            }
        }
    }
}
