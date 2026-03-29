using BG.GameServer.Actors;
using BG.GameServer.Extensions;
using BG.GameServer.Internals;
using BG.GameServer.Messages;
using BG.GameServer.Network.Codecs;
using Dignus.Actor.Core;
using Dignus.Actor.Core.DeadLetter;
using Dignus.Actor.Network;
using Dignus.Actor.Network.Options;
using Dignus.DependencyInjection.Attributes;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using System;

namespace BG.GameServer.Network
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class GameServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GameServerHost _gameServerHost;

        public GameServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var systemProtocolMapper = _serviceProvider.GetService<SystemProtocolMapper>();

            var wallGoProtocolMapper = _serviceProvider.GetService<WallGoProtocolMapper>();

            systemProtocolMapper.RegisterServerProtocols();

            wallGoProtocolMapper.RegisterServerProtocols();

            var option = ServerOptions.Builder()
                .UseDecoder(new PacketFramer(serviceProvider))
                .UseSerializer(new MessageSerializer())
                .Build();

            var actorSystem = _serviceProvider.GetService<ActorSystem>();

            actorSystem.Spawn(() => 
            {
                return new RoomManagerActor(_serviceProvider);
            },typeof(RoomManagerActor).Name);

            _gameServerHost = new GameServerHost(_serviceProvider, actorSystem, option);
        }
        public void Start(int port)
        {
            _gameServerHost.Start(port);
        }
    }

    internal class GameServerHost(IServiceProvider serviceProvider,
        ActorSystem actorSystem,
        ServerOptions options) 
        : TcpServerBase<ClientActor>(actorSystem, options)
    {
        protected override ClientActor CreateSessionActor()
        {
            return new ClientActor(serviceProvider);
        }

        protected override void OnAccepted(INetworkSessionRef connectedActorRef)
        {
            connectedActorRef.Post(new AcceptedMessage());
        }
        protected override void OnDeadLetterMessage(DeadLetterMessage deadLetterMessage)
        {
            if(deadLetterMessage.Reason == DeadLetterReason.ExecutionException)
            {
                if(deadLetterMessage.Message is ActorExceptionMessage exceptionMessage)
                {
                    LogHelper.Fatal(exceptionMessage.Exception);
                }
            }
        }
        protected override void OnDisconnected(INetworkSessionRef disconnectedSessionRef)
        {
            
        }
    }
}
