using Dignus.Sockets.Interfaces;
using Newtonsoft.Json;
using Protocol.GSAndClient;
using System.Text;

namespace Assets.Scripts.Network
{
    public class Packet : IPacket
    {
        public ushort Category { get; set; }
        public ushort Protocol { get; set; }
        public byte[] Body { get; set; }


        private const int SizeofShort = sizeof(ushort);
        public Packet(ushort protocol, byte[] body)
        {
            Protocol = protocol;
            Body = body;
        }

        public Packet(ushort category, ushort protocol, string body)
        {
            Category = category;
            Protocol = protocol;
            Body = Encoding.UTF8.GetBytes(body);
        }

        public int GetLength()
        {
            return SizeofShort + SizeofShort + Body.Length;
        }

        public static Packet MakePacket<T>(ushort category, ushort protocol, T packetData)
        {
            var packet = new Packet(category, protocol, JsonConvert.SerializeObject(packetData));
            return packet;
        }
        public static Packet MakePacket<T>(CGSProtocol protocol, T packetData)
        {
            return MakePacket<T>((ushort)PacketCategory.Lobby, (ushort)protocol, packetData);
        }
        public static Packet MakePacket<T>(WallGoCommandProtocol protocol, T packetData)
        {
            return MakePacket<T>((ushort)PacketCategory.WallGo, (ushort)protocol, packetData);
        }
    }
}
