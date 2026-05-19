using Dignus.Actor.Network;
using Dignus.DependencyInjection.Attributes;
using Protocol.GSAndClient;

namespace BG.GameServer.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class SystemProtocolMapper : ProtocolBodyTypeMapper
    {
        public void RegisterServerProtocols()
        {
            this.RegisterByProtocolName<CGSProtocol>(typeof(CGSProtocol).Assembly);
        }
    }
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class WallGoProtocolMapper : ProtocolBodyTypeMapper
    {
        public void RegisterServerProtocols() 
        {
            this.RegisterByProtocolName<WallGoCommandProtocol>(typeof(WallGoCommandProtocol).Assembly);
        }
    }
}
