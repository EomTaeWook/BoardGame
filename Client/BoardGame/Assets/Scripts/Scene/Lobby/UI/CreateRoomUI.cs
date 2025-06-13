using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Unity.Attributes;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class CreateRoomUI : UIItem
    {
        [SerializeField]
        private ToggleGroup _gameModeToggleGroup;

        [SerializeField]
        private ToggleGroup _roomModeToggleGroup;
        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _createButtonText;
        [SerializeField]
        private TextMeshProUGUI _closeButtonText;

        private RoomMode _roomMode;
        private LobbySceneController _lobbySceneController;
        private Action _onCloseCallback;

        public void Init(LobbySceneController lobbySceneController, Action onCloseCallback)
        {
            _lobbySceneController = lobbySceneController;
            _onCloseCallback = onCloseCallback;

            _titleText.text = StringHelper.GetString(1016);
            _createButtonText.text = StringHelper.GetString(1018);
            _closeButtonText.text = StringHelper.GetString(1015);
        }
        public void OnChangeRoomMode(bool isCheck)
        {
            if(isCheck == false)
            {
                return;
            }

            var activeToggle = _roomModeToggleGroup.GetFirstActiveToggle();

            if(activeToggle == null)
            {
                return;
            }

            var roomModeToggle = activeToggle.GetComponent<RoomModeToggle>();

            if(roomModeToggle == null)
            {
                return;
            }

            _roomMode = roomModeToggle.Mode;
        }
        public void OnCreateButtonClick()
        {
            var activeToggle = _gameModeToggleGroup.GetFirstActiveToggle();

            if (activeToggle == null)
            {
                return;
            }

            var gameModeToggle = activeToggle.GetComponent<GameModeToggle>();

            _lobbySceneController.CreateRoomReqeust(new CreateRoom()
            {
                GameType = (int)gameModeToggle.Mode,
                RoomMode = _roomMode
            });
        }

        public void OnCloseButtionClick()
        {
            _onCloseCallback?.Invoke();
        }
    }
}
