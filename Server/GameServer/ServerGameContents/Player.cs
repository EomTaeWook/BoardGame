using Assets.Scripts.GameContents;
using Dignus.Sockets.Interfaces;

namespace BG.GameServer.ServerGameContents
{
    internal class Player : IPlayer
    {
        public string AccountId { get; private set; }

        public string Nickname { get; private set; }

        private ISession _session;

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
            _session.SendAsync(packet);
        }
        public void Close()
        {
            var session = _session;
            if (session != null)
            {
                session.Dispose();
                _session = null;
            }
        }
    }
}
