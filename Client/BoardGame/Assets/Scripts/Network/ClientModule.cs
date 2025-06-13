using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using System;

namespace Assets.Scripts.Network
{
    public class ClientModule : ClientBase
    {
        public event Action<ISession> Disconnected;

        public ClientModule(SessionConfiguration sessionConfiguration) : base(sessionConfiguration)
        {
        }

        protected override void OnConnected(ISession session)
        {
            LogHelper.Info("connected server");
        }

        protected override void OnDisconnected(ISession session)
        {
            LogHelper.Info("disconnected server");
            Disconnected?.Invoke(session);
        }
    }
}
