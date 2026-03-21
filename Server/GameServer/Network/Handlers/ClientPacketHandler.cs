using BG.GameServer.Actors;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Threading.Tasks;

namespace BG.GameServer.Network.Handlers
{
    internal class ClientPacketHandler : IProtocolHandler<object>
    {
        public T DeserializeBody<T>(object body)
        {
            return (T)body;
        }
        public static async Task GetRoomList(ClientActor clientActor, GetRoomList getRoomList)
        {
            await clientActor.ProcessPacket(getRoomList);
        }
        public static async Task StartGameRoom(ClientActor clientActor, StartGameRoom startGameRoom)
        {
            await clientActor.ProcessPacket(startGameRoom);
        }
        public static async Task CreateRoom(ClientActor clientActor, CreateRoom createRoom)
        {
            await clientActor.ProcessPacket(createRoom);
        }
        public static async Task JoinRoom(ClientActor clientActor, JoinRoom joinRoom)
        {
            await clientActor.ProcessPacket(joinRoom);
        }
        public static async Task LeaveRoom(ClientActor clientActor, LeaveRoom leaveRoom)
        {
            await clientActor.ProcessPacket(leaveRoom);
        }

        public static async Task Login(ClientActor clientActor, Login login)
        {
            await clientActor.ProcessPacket(login);
        }
        public static async Task Pong(ClientActor clientActor, Pong pong)
        {
            await clientActor.ProcessPacket(pong);
        }
    }
}
