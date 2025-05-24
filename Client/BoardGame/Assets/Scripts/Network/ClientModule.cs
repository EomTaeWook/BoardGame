using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace Assets.Scripts.Network
{
    public class ClientModule : ClientBase
    {
        public ClientModule(SessionConfiguration sessionConfiguration) : base(sessionConfiguration)
        {
        }

        protected override void OnConnected(ISession session)
        {

        }

        protected override void OnDisconnected(ISession session)
        {

        }
    }
}
