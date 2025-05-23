using BG.GameServer.Network.Handlers;
using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Dignus.Sockets.Processing;
using System.Text;

namespace BG.GameServer.Network
{
    internal class PacketProcessor(CGProtocolHandler cgProtocolHandler) : SessionPacketProcessorBase, IPacketSerializer, ISessionComponent
    {
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);

        public CGProtocolHandler CGProtocolHandler { get => cgProtocolHandler; }

        private const int MaxBodySize = 65536;
        private ISession _session;
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
            buffer.AddRange(BitConverter.GetBytes(sendPacket.Protocol));
            buffer.AddRange(sendPacket.Body);

            return buffer.ToArray();
        }

        public override void ProcessPacket(in ArraySegment<byte> packet)
        {
            int protocol = BitConverter.ToInt16(packet);

            if (ProtocolHandlerMapper.ValidateProtocol<CGProtocolHandler>(protocol) == false)
            {
                LogHelper.Error($"not found protocol : {protocol}");
                return;
            }
            var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + ProtocolSize, packet.Count - ProtocolSize);

            ProtocolHandlerMapper.DispatchToHandler(cgProtocolHandler, protocol, body);
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
