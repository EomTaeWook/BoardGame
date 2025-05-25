using Assets.Scripts.Internals;
using Dignus.Unity.Framework;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Scene.Title
{
    public class LobbySceneModel : ISceneModel
    {
        public GamePlayer CurrentPlayer { get; set; }
        public List<PlayerModel> RoomMembers { get; set; }
    }
}
