using Assets.Scripts.Network.Handlers;
using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Dignus.Sockets.Processing;
using Protocol.GSAndClient;
using System;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Network
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class PacketProcessor : PacketHandlerBase, IPacketSerializer, ISessionComponent
    {
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);
        protected const int CategorySize = sizeof(ushort);
        protected const int TotalHeaderSize = CategorySize + ProtocolSize;

        public GSCProtocolHandler GsCProtocolHandler { get; private set; }
        public WallGoCommandHandler WallGoCommandHandler { get; private set; }

        private const int MaxBodySize = 65536;
        private ISession _session;

        public PacketProcessor(GSCProtocolHandler gscProtocolHandler,
        WallGoCommandHandler wallGoCommandHandler)
        {
            GsCProtocolHandler = gscProtocolHandler;
            WallGoCommandHandler = wallGoCommandHandler;
        }
        public void Dispose()
        {
            _session = null;
        }

        public ArraySegment<byte> MakeSendBuffer(IPacket packet)
        {
            var sendPacket = packet as Packet;

            var packetSize = sendPacket.GetLength();

            var buffer = new ArrayQueue<byte>();

            buffer.AddRange(BitConverter.GetBytes(packetSize));
            buffer.AddRange(BitConverter.GetBytes(sendPacket.Category));
            buffer.AddRange(BitConverter.GetBytes(sendPacket.Protocol));
            buffer.AddRange(sendPacket.Body);

            return buffer.ToArray();
        }

        public override void ProcessPacket(in ArraySegment<byte> packet)
        {
            int category = BitConverter.ToInt16(packet);

            var packetCategory = (PacketCategory)category;

            if (packetCategory == PacketCategory.Lobby)
            {
                int protocol = BitConverter.ToInt16(packet.Array, packet.Offset + CategorySize);
                if (ProtocolHandlerMapper.ValidateProtocol<GSCProtocolHandler>(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return;
                }
                var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + TotalHeaderSize, packet.Count - TotalHeaderSize);
                ProtocolHandlerMapper.DispatchToHandler(GsCProtocolHandler, protocol, body);
            }
            else if (packetCategory == PacketCategory.WallGo)
            {
                int protocol = BitConverter.ToInt16(packet.Array, packet.Offset + CategorySize);

                if (ProtocolHandlerMapper.ValidateProtocol<WallGoCommandHandler>(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return;
                }

                var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + TotalHeaderSize, packet.Count - TotalHeaderSize);
                ProtocolHandlerMapper.DispatchToHandler(WallGoCommandHandler, protocol, body);
            }
            else
            {
                LogHelper.Error($"not found category : {packetCategory}");
                return;
            }
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
                LogHelper.Error($"Invalid packetSize: {packetSize}");
                _session.Dispose();
                return false;
            }

            buffer.Read(SizeToInt);
            consumedBytes = packetSize;
            return buffer.TrySlice(out packet, consumedBytes);
        }
    }
}
