using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Unity.Attributes;
using Protocol.GSAndClient;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class CreateRoomUI : UiItem
    {
        [SerializeField]
        private ToggleGroup _toggleGroup;
        private LobbySceneController _lobbySceneController;
        private Action _onCloseCallback;
        public void Init(LobbySceneController lobbySceneController, Action onCloseCallback)
        {
            _lobbySceneController = lobbySceneController;
            _onCloseCallback = onCloseCallback;
        }
        public void OnCreateButtonClick()
        {
            var activeToggle = _toggleGroup.GetFirstActiveToggle();

            if (activeToggle == null)
            {
                return;
            }

            var gameModeToggle = activeToggle.GetComponent<GameModeToggle>();

            _lobbySceneController.CreateRoomReqeust(new CreateRoom()
            {
                GameType = (int)gameModeToggle.Mode
            });
        }

        public void OnCloseButtionClick()
        {
            _onCloseCallback?.Invoke();
        }
    }
}
