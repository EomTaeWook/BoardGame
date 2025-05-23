using BG.GameServer.Network;
using Dignus.DependencyInjection.Extensions;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace BG.GameServer.Internals
{
    internal class GameServerNode
    {
        private GameServerMoudle _gameServerMoudle;
        private IServiceProvider _serviceProvider;
        public GameServerNode(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _gameServerMoudle = new GameServerMoudle(new SessionConfiguration(MakeSerializersFunc));
        }

        public void Start(int port)
        {
            _gameServerMoudle.Start(port);
        }

        private Tuple<IPacketSerializer, ISessionPacketProcessor, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            PacketProcessor packetProcessor = _serviceProvider.GetService<PacketProcessor>();

            return Tuple.Create<IPacketSerializer, ISessionPacketProcessor, ICollection<ISessionComponent>>(
                packetProcessor,
                packetProcessor,
                [packetProcessor, packetProcessor.CGProtocolHandler]);
        }
    }

    internal class GameServerMoudle : ServerBase
    {
        public GameServerMoudle(SessionConfiguration sessionConfiguration) : base(sessionConfiguration)
        {
        }

        protected override void OnAccepted(ISession session)
        {

        }

        protected override void OnDisconnected(ISession session)
        {

        }
    }
}
