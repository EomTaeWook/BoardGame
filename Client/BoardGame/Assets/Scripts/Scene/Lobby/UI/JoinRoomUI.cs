using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Unity.Attributes;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class JoinRoomUI : UiItem
    {
        [SerializeField]
        private TMP_InputField _inputField;

        LobbySceneController _lobbySceneController;
        Action _onCloseCallback;

        public void Init(LobbySceneController lobbySceneController, Action onCloseCallback)
        {
            _lobbySceneController = lobbySceneController;
            _onCloseCallback = onCloseCallback;
        }
        public void OnJoinButtonClick()
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                return;
            }
            var roomNumber = int.Parse(_inputField.text);

            _lobbySceneController.JoinRoomRequest(roomNumber);
        }
        public void OnCloseButtionClick()
        {
            _onCloseCallback?.Invoke();
        }
    }
}
