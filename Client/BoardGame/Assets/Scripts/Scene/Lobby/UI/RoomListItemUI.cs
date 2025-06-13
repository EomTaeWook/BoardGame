using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using DataContainer.Generated;
using Dignus.Unity.Attributes;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class RoomListItemUI : UIItem
    {
        [SerializeField]
        private TextMeshProUGUI _roomNumberText;
        [SerializeField]
        private TextMeshProUGUI _gameTypeText;
        [SerializeField]
        private TextMeshProUGUI _enterButtonText;
        [SerializeField]
        private TextMeshProUGUI _memberCountText;

        private RoomInfo _roomInfo;

        private LobbySceneController _lobbySceneController;

        public void Init(LobbySceneController lobbySceneController, RoomInfo roomInfo)
        {
            _lobbySceneController = lobbySceneController;
            _roomInfo = roomInfo;
        }
        public void RefreshUI()
        {
            _roomNumberText.text = _roomInfo.RoomId.ToString();
            _gameTypeText.text = GetGameTypeText();
            _enterButtonText.text = StringHelper.GetString(1027);
            _memberCountText.text = $"{_roomInfo.MemberCount}/{GetMaxRoomMemberCount()}";
        }
        private int GetMaxRoomMemberCount()
        {
            if (_roomInfo.GameType == GameType.WallGo)
            {
                var template = TemplateContainer<ConstantTemplate>.Find("MaxWallGoMember");

                return template.Value;
            }

            return 0;
        }
        private string GetGameTypeText()
        {
            if (_roomInfo.GameType == GameType.WallGo)
            {
                return StringHelper.GetString(1026);
            }

            return string.Empty;
        }

        public void OnJoinButtonClick()
        {
            _lobbySceneController.JoinRoomRequest(_roomInfo.RoomId);
        }
    }
}
