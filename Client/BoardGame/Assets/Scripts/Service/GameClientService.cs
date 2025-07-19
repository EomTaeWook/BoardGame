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
using System.Threading.Tasks;

namespace Assets.Scripts.Service
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class GameClientService
    {
        private bool _isConnected;
        private readonly ClientModule _clientModule;
        private readonly IServiceProvider _serviceProvider;
        private string _ipString;
        private int _port;
        public GameClientService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ProtocolHandlerMapper<GSCProtocolHandler, string>.BindProtocol<GSCProtocol>();
            ProtocolHandlerMapper<WallGoCommandHandler, string>.BindProtocol<WallGoServerEvent>();

            _clientModule = new ClientModule(new SessionConfiguration(MakeSerializersFunc));

            _clientModule.Disconnected += ClientModule_Disconnected;
        }

        public void SetIpStringAndPort(string ipString, int port)
        {
            _ipString = ipString;
            _port = port;
        }

        private void ClientModule_Disconnected(ISession session)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                _ = ReconnectAsync();
            });

        }

        private Task ReconnectAsync()
        {
            if (Connect() == false)
            {
                _ = ReconnectAsync();
            }

            return Task.CompletedTask;
        }

        private Tuple<IPacketSerializer, IPacketHandler, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            PacketProcessor packetProcessor = _serviceProvider.GetService<PacketProcessor>();

            var components = new List<ISessionComponent>();
            components.Add(packetProcessor);
            components.Add(packetProcessor.GsCProtocolHandler);
            components.Add(packetProcessor.WallGoCommandHandler);

            return Tuple.Create<IPacketSerializer, IPacketHandler, ICollection<ISessionComponent>>(
                packetProcessor,
                packetProcessor,
                components);
        }
        public bool Connect()
        {
            try
            {
                _clientModule.Connect(_ipString, _port);
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
        public void Dispose()
        {
            _clientModule.Disconnected -= ClientModule_Disconnected;
            _clientModule.Close();
            _isConnected = false;
        }
        public void Send(IPacket packet)
        {
            if (_clientModule == null)
            {
                return;
            }
            _clientModule.Send(packet);
        }
    }
}
