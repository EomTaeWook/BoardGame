using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Unity.Attributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class RoomUI : UIItem
    {
        [SerializeField]
        private TextMeshProUGUI _roomNumber;

        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _roomNumberText;
        [SerializeField]
        private TextMeshProUGUI _startButtonText;
        [SerializeField]
        private TextMeshProUGUI _closeButtonText;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private List<TextMeshProUGUI> _playerNicknames;

        private LobbySceneController _lobbySceneController;
        private Action _onCloseCallback;
        public void Init(LobbySceneController lobbySceneController, int roomNumber, Action onCloseCallback)
        {
            _lobbySceneController = lobbySceneController;
            _onCloseCallback = onCloseCallback;
            foreach (var item in _playerNicknames)
            {
                item.gameObject.SetActive(false);
            }
            _startButton.interactable = false;

            _roomNumber.text = roomNumber.ToString();

            _titleText.text = StringHelper.GetString(1021);

            _roomNumberText.text = StringHelper.GetString(1020);

            _startButtonText.text = StringHelper.GetString(1022);

            _closeButtonText.text = StringHelper.GetString(1015);
        }

        public void RefreshUI()
        {
            var roomMembers = _lobbySceneController.Model.RoomMembers;
            _startButton.interactable = false;
            for (int i = 0; i < roomMembers.Count; ++i)
            {
                var playerInfo = roomMembers[i];
                _playerNicknames[i].text = playerInfo.Nickname;
                _playerNicknames[i].gameObject.SetActive(true);

                if (playerInfo.IsHost == true)
                {
                    if (_lobbySceneController.Model.CurrentPlayer.AccountId == playerInfo.AccountId)
                    {
                        _startButton.interactable = true;
                    }
                }
            }
            for (int i = roomMembers.Count; i < _playerNicknames.Count; ++i)
            {
                _playerNicknames[i].gameObject.SetActive(false);
            }
        }
        public void OnStartGameButtonClick()
        {
            _lobbySceneController.StartGameRoomRequest();
        }
        public void OnCloseButtonClick()
        {
            _lobbySceneController.LeaveRoomReqeust();
            _onCloseCallback?.Invoke();
        }
    }
}
