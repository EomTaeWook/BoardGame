using BG.GameServer.Actors;
using BG.GameServer.Messages;
using BG.GameServer.Network.Handlers;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Codec;
using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BG.GameServer.Network.Codecs
{
    internal class PacketFramer : IActorMessageDecoder
    {
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);
        protected const int CategorySize = sizeof(ushort);
        private const int MaxBodySize = 65536;
        protected const int TotalHeaderSize = CategorySize + ProtocolSize;

        private readonly ClientPacketHandler _clientPacketHandler = new();
        private readonly WallGoCommandActorHandler _wallGoCommandActorHandler = new();

        public IActorMessage Deserialize(ReadOnlySpan<byte> packet)
        {
            int category = BitConverter.ToInt16(packet);

            var packetCategory = (PacketCategory)category;

            var bytes = packet.ToArray();

            int protocol = BitConverter.ToInt16(bytes, CategorySize);

            var bodyString = Encoding.UTF8.GetString(bytes, TotalHeaderSize, bytes.Length - TotalHeaderSize);

            if (packetCategory == PacketCategory.Lobby)
            {
                if (ProtocolStateHandlerMapper.ValidateProtocol<ClientPacketHandler, ClientActor>(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return null;
                }

                var bodyType = ProtocolStateHandlerMapper.GetBodyType<ClientPacketHandler, ClientActor>(protocol);

                var bodyPacketObject = JsonSerializer.Deserialize(bodyString, bodyType);

                async Task lambdaMessage(ClientActor actor)
                {
                    await ProtocolStateHandlerMapper.InvokeHandlerAsync(_clientPacketHandler, protocol, actor, bodyPacketObject);
                }
                return new InBoundLambda(lambdaMessage);
            }
            else if (packetCategory == PacketCategory.WallGo)
            {
                if (ProtocolStateHandlerMapper.ValidateProtocol<WallGoCommandHandler, ClientActor>(protocol) == false)
                {
                    LogHelper.Error($"not found protocol : {protocol}");
                    return null;
                }

                var bodyType = ProtocolStateHandlerMapper.GetBodyType<WallGoCommandActorHandler, ClientActor>(protocol);

                var bodyPacketObject = JsonSerializer.Deserialize(bodyString, bodyType);

                async Task lambdaMessage(ClientActor actor)
                {
                    await ProtocolStateHandlerMapper.InvokeHandlerAsync(_wallGoCommandActorHandler, protocol, actor, bodyPacketObject);
                }
                return new InBoundLambda(lambdaMessage);
            }
            else
            {
                LogHelper.Error($"not found category : {packetCategory}");
                return new KickUserMessage()
                {
                    Reason = ErrorCode.InvalidRequest
                };
            }
        }

        public bool TryFrame(ISession session, ArrayQueue<byte> buffer, out ArraySegment<byte> packet, out int consumedBytes)
        {
            packet = default;
            consumedBytes = 0;

            if (!buffer.TrySlice(out var packetSizeBytes, SizeToInt))
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
