using Dignus.Sockets.Interfaces;

namespace BG.GameServer.Network
{
    internal class Player
    {
        private ISession _session;

        public Player(ISession session)
        {
            _session = session;
        }

        public void Send(IPacket packet)
        {
            _session.TrySend(packet);
        }
    }
}
