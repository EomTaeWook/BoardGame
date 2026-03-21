using BG.GameServer.Messages;
using Dignus.Actor.Network.Codec;
using Dignus.Actor.Network.Messages;
using Dignus.Collections;
using Dignus.Sockets.Interfaces;
using System;
using System.Linq;

namespace BG.GameServer.Network.Codecs
{
    internal class MessageSerializer : IActorMessageSerializer
    {
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

        public ArraySegment<byte> MakeSendBuffer(INetworkActorMessage message)
        {
            if (message is OutBoundMessage outBoundMessage)
            {
                return outBoundMessage.Bytes;
            }

            return null;
        }
    }
}
