using BG.GameServer.Network.Handlers;
using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Dignus.Sockets.Processing;
using Protocol.GSAndClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BG.GameServer.Network
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class PacketProcessor(CGProtocolHandler cgProtocolHandler,
        WallGoCommandHandler wallGoCommandHandler) : PacketHandlerBase, ISessionComponent
    {
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);
        protected const int CategorySize = sizeof(ushort);
        protected const int TotalHeaderSize = CategorySize + ProtocolSize;

        public CGProtocolHandler CGProtocolHandler { get => cgProtocolHandler; }
        public WallGoCommandHandler WallGoCommandHandler { get => wallGoCommandHandler; }

        private const int MaxBodySize = 65536;
        private ISession _session;
        public void Dispose()
        {
            _session = null;
        }

        public override Task ProcessPacketAsync(ArraySegment<byte> packet)
        {
            int category = BitConverter.ToInt16(packet);

            var packetCategory = (PacketCategory)category;

            if (packetCategory == PacketCategory.Lobby)
            {
                int protocol = BitConverter.ToInt16(packet.Array, packet.Offset + CategorySize);
                if (ProtocolHandlerMapper.ValidateProtocol<CGProtocolHandler>(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return Task.CompletedTask;
                }

                var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + TotalHeaderSize, packet.Count - TotalHeaderSize);
                try
                {
                    return ProtocolHandlerMapper.InvokeHandlerAsync(cgProtocolHandler, protocol, body);
                }
                catch (Exception ex)
                {
                    LogHelper.Fatal(ex);
                }

            }
            else if (packetCategory == PacketCategory.WallGo)
            {
                int protocol = BitConverter.ToInt16(packet.Array, packet.Offset + CategorySize);

                if (ProtocolHandlerMapper.ValidateProtocol<WallGoCommandHandler>(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return Task.CompletedTask;
                }

                var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + TotalHeaderSize, packet.Count - TotalHeaderSize);

                try
                {
                    return ProtocolHandlerMapper.InvokeHandlerAsync(wallGoCommandHandler, protocol, body);
                }
                catch (Exception ex)
                {
                    LogHelper.Fatal(ex);
                }
            }
            else
            {
                LogHelper.Error($"not found category : {packetCategory}");
                _session.Dispose();
            }
            return Task.CompletedTask;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }

        public override bool TakeReceivedPacket(ArrayQueue<byte> buffer, out ArraySegment<byte> packet, out int consumedBytes)
        {
            consumedBytes = 0;
            packet = null;

            if (buffer.Count < SizeToInt)
            {
                return false;
            }

            var packetSize = BitConverter.ToInt32(buffer.Peek(SizeToInt));
            if (buffer.Count < packetSize + SizeToInt)
            {
                return false;
            }

            if (packetSize >= MaxBodySize)
            {
                LogHelper.Error($"invalid packet size: {packetSize}");
                _session.Dispose();
                return false;
            }

            buffer.Read(SizeToInt);
            consumedBytes = packetSize;
            return buffer.TrySlice(out packet, consumedBytes);
        }
    }
}
