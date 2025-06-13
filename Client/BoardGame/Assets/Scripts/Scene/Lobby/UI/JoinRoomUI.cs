using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Unity.Attributes;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class JoinRoomUI : UIItem
    {
        [SerializeField]
        private TMP_InputField _inputField;

        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _roomNumberText;

        [SerializeField]
        private TextMeshProUGUI _joinButtonText;
        [SerializeField]
        private TextMeshProUGUI _closeButtonText;

        LobbySceneController _lobbySceneController;
        Action _onCloseCallback;

        public void Init(LobbySceneController lobbySceneController, Action onCloseCallback)
        {
            _lobbySceneController = lobbySceneController;
            _onCloseCallback = onCloseCallback;

            _titleText.text = StringHelper.GetString(1017);
            _joinButtonText.text = StringHelper.GetString(1019);
            _closeButtonText.text = StringHelper.GetString(1015);
            _roomNumberText.text = StringHelper.GetString(1020);
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
