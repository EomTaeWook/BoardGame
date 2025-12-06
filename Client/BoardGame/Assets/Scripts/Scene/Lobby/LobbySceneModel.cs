using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Lobby.UI;
using Dignus.Collections;
using Dignus.Unity.Binding;
using Dignus.Unity.Framework;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Scene.Title
{
    public class LobbySceneModel : ISceneModel
    {
        public Dictionary<int, RoomListItemUI> LobbyRoomInfos { get; set; } = new Dictionary<int, RoomListItemUI>();

        public int PageIndex { get; set; }
        public GamePlayer CurrentPlayer { get; set; }
        public List<PlayerModel> RoomMembers { get; set; }

        public int JoinRoomNumber { get; set; }

        public BindableProperty<int> RemoveGameRoom { get; set; } = new BindableProperty<int>(-1);
    }
}
