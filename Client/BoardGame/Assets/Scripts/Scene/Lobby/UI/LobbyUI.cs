using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Unity.Attributes;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class LobbyUI : UiItem
    {
        [SerializeField]
        private TextMeshProUGUI _nicknameText;

        private LobbySceneController _lobbySceneController;
        public void Init(LobbySceneController lobbySceneController)
        {
            _lobbySceneController = lobbySceneController;
            _nicknameText.text = _lobbySceneController.Model.CurrentPlayer.Nickname;
        }
        public void OnCreateRoomUIButtonClick()
        {
            _lobbySceneController.Scene.CreateCreateRoomUI();
        }

        public void OnJoinRoomUIButtionClick()
        {
            _lobbySceneController.Scene.CreateJoinRoomUI();
        }
    }
}
