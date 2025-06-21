using BG.GameServer.Network;
using BG.GameServer.Network.Handlers;
using BG.GameServer.ServerContents;
using Dignus.DependencyInjection.Attributes;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System;
using System.Collections.Generic;

namespace BG.GameServer.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class GameServerNode
    {
        private readonly GameServerMoudle _gameServerMoudle;
        private readonly IServiceProvider _serviceProvider;
        public GameServerNode(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            ProtocolHandlerMapper<CGProtocolHandler, string>.BindProtocol<CGSProtocol>();
            ProtocolHandlerMapper<WallGoCommandHandler, string>.BindProtocol<WallGoCommandProtocol>();

            _gameServerMoudle = new GameServerMoudle(new SessionConfiguration(MakeSerializersFunc));
        }

        public void Start(int port)
        {
            _gameServerMoudle.Start(port);
        }

        private Tuple<IPacketSerializer, ISessionPacketProcessor, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            PacketProcessor packetProcessor = _serviceProvider.GetService<PacketProcessor>();
            RobbyManager robbyManager = _serviceProvider.GetService<RobbyManager>();
            HeartBeat heartBeat = _serviceProvider.GetService<HeartBeat>();

            return Tuple.Create<IPacketSerializer, ISessionPacketProcessor, ICollection<ISessionComponent>>(
                packetProcessor,
                packetProcessor,
                [
                    heartBeat,
                    packetProcessor,
                    packetProcessor.CGProtocolHandler,
                    packetProcessor.WallGoCommandHandler,
                    robbyManager,
                ]);
        }
    }

    internal class GameServerMoudle(SessionConfiguration sessionConfiguration) : ServerBase(sessionConfiguration)
    {
        protected override void OnAccepted(ISession session)
        {
            LogHelper.Debug($"accepted session : {session.Id}");
        }

        protected override void OnDisconnected(ISession session)
        {
            LogHelper.Debug($"disconnect session : {session.Id}");
        }
    }
}
