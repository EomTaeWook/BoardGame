using Assets.Scripts.Network;
using Assets.Scripts.Network.Handlers;
using Dignus.DependencyInjection.Attributes;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Service
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class GameClientService
    {
        private bool _isConnected;
        private readonly ClientModule _clientModule;
        private readonly IServiceProvider _serviceProvider;
        public GameClientService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ProtocolHandlerMapper<GSCProtocolHandler, string>.BindProtocol<GSCProtocol>();
            ProtocolHandlerMapper<WallGoCommandHandler, string>.BindProtocol<WallGoServerEvent>();

            _clientModule = new ClientModule(new SessionConfiguration(MakeSerializersFunc));
        }
        private Tuple<IPacketSerializer, ISessionPacketProcessor, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            PacketProcessor packetProcessor = _serviceProvider.GetService<PacketProcessor>();

            var components = new List<ISessionComponent>();
            components.Add(packetProcessor);
            components.Add(packetProcessor.GsCProtocolHandler);
            components.Add(packetProcessor.WallGoCommandHandler);

            return Tuple.Create<IPacketSerializer, ISessionPacketProcessor, ICollection<ISessionComponent>>(
                packetProcessor,
                packetProcessor,
                components);
        }
        public bool Connect(string ipString, int port)
        {
            try
            {
                _clientModule.Connect(ipString, port);
                _isConnected = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
            return true;
        }
        public bool IsConnect()
        {
            return _isConnected;
        }

        public void Send(IPacket packet)
        {
            _clientModule.TrySend(packet);
        }
    }
}
