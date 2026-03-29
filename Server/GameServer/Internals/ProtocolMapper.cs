using Dignus.Actor.Network;
using Dignus.DependencyInjection.Attributes;

namespace BG.GameServer.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class SystemProtocolMapper : ProtocolBodyTypeMapper
    {
    }
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class WallGoProtocolMapper : ProtocolBodyTypeMapper
    {
    }
}
