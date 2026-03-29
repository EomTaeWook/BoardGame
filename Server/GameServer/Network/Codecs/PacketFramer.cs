using BG.GameServer.Internals;
using BG.GameServer.Messages;
using Dignus.Actor.Abstractions;
using Dignus.Actor.Network.Codec;
using Dignus.Collections;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System;
using System.Text;
using System.Text.Json;

namespace BG.GameServer.Network.Codecs
{
    internal class PacketFramer(IServiceProvider serviceProvider) : IActorMessageDecoder
    {
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);
        protected const int CategorySize = sizeof(ushort);
        private const int MaxBodySize = 65536;
        protected const int TotalHeaderSize = CategorySize + ProtocolSize;

        private readonly SystemProtocolMapper _systemProtocolMapper = serviceProvider.GetService<SystemProtocolMapper>();

        private readonly WallGoProtocolMapper _wallGoProtocolMapper = serviceProvider.GetService<WallGoProtocolMapper>();

        public IActorMessage Deserialize(ReadOnlySpan<byte> packet)
        {
            int category = BitConverter.ToInt16(packet);

            var packetCategory = (PacketCategory)category;

            var bytes = packet.ToArray();

            int protocol = BitConverter.ToInt16(bytes, CategorySize);

            var bodyString = Encoding.UTF8.GetString(bytes, TotalHeaderSize, bytes.Length - TotalHeaderSize);

            if (packetCategory == PacketCategory.Lobby)
            {
                if (_systemProtocolMapper.Contains(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return null;
                }

                var bodyType = _systemProtocolMapper.GetBodyType(protocol);

                var bodyPacketObject = JsonSerializer.Deserialize(bodyString, bodyType);

                return (IActorMessage)bodyPacketObject;
            }
            else if (packetCategory == PacketCategory.WallGo)
            {
                if (_wallGoProtocolMapper.Contains(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return null;
                }

                var bodyType = _wallGoProtocolMapper.GetBodyType(protocol);

                var bodyPacketObject = JsonSerializer.Deserialize(bodyString, bodyType);

                return (IActorMessage)bodyPacketObject;
            }
            else
            {
                LogHelper.Error($"not found category : {packetCategory}");
                return new KickUserMessage(ErrorCode.InvalidRequest);
            }
        }

        public bool TryFrame(ISession session, ArrayQueue<byte> buffer, out ArraySegment<byte> packet, out int consumedBytes)
        {
            packet = default;
            consumedBytes = 0;

            if (buffer.TrySlice(out var packetSizeBytes, SizeToInt) == false)
            {
                return false;
            }

            var packetSize = BitConverter.ToInt32(packetSizeBytes);
            if (buffer.Count < packetSize + SizeToInt)
            {
                return false;
            }

            if (packetSize >= MaxBodySize)
            {
                LogHelper.Error($"invalid packet size: {packetSize}");
                session.Dispose();
                return false;
            }
            buffer.Advance(SizeToInt);
            consumedBytes = packetSize;
            return buffer.TrySlice(out packet, consumedBytes);
        }
    }
}
