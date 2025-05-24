using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Newtonsoft.Json;

namespace Assets.Scripts.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class WallGoCommandHandler : IProtocolHandler<string>, ISessionComponent
    {
        private ISession _session;
        public T DeserializeBody<T>(string body)
        {
            return JsonConvert.DeserializeObject<T>(body);
        }

        public void Dispose()
        {
            _session = null;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }
    }
}
