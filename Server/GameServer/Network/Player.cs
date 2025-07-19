using Assets.Scripts.GameContents;
using BG.GameServer.ServerContents;
using Dignus.Sockets.Interfaces;

namespace BG.GameServer.Network
{
    internal class Player : IPlayer
    {
        public string AccountId { get; private set; }

        public string Nickname { get; private set; }

        private readonly ISession _session;

        public RoomBase Room { get; private set; }

        public Player(string accountId, string nickname, ISession session)
        {
            AccountId = accountId;
            Nickname = nickname;
            _session = session;
        }
        public void SetRoom(RoomBase room)
        {
            Room = room;
        }
        public void Send(IPacket packet)
        {
            _session.Send(packet);
        }
    }
}
