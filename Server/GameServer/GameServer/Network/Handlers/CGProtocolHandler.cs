using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using System.Text.Json;

namespace BG.GameServer.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class CGProtocolHandler : IProtocolHandler<string>, ISessionComponent
    {
        private Player _player;
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }

        void ISessionComponent.Dispose()
        {

        }

        void ISessionComponent.SetSession(ISession session)
        {
            _player = new Player(session);
        }
    }
}
