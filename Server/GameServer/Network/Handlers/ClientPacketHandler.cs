using BG.GameServer.Actors;
using BG.GameServer.ServerGameContents;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Threading.Tasks;

namespace BG.GameServer.Network.Handlers
{
    internal class ClientPacketHandler : IProtocolHandler<object>
    {
        private HeartBeat _heartBeat;
        private Player _player;
        private ISession _session;
        public T DeserializeBody<T>(object body)
        {
            return (T)body;
        }
        public Task GetRoomList(GetRoomList getRoomList)
        {
            return Task.CompletedTask;
        }
        public Task StartGameRoom(ClientActor clientActor, StartGameRoom _)
        {
            return Task.CompletedTask;
        }
        public Task CreateRoom(ClientActor clientActor, CreateRoom createRoom)
        {
            return Task.CompletedTask;
        }
        public Task JoinRoom(ClientActor clientActor, JoinRoom joinRoom)
        {
            return Task.CompletedTask;
        }
        public Task LeaveRoom(ClientActor clientActor, LeaveRoom leaveRoom)
        {
            return Task.CompletedTask;
        }

        public Task Login(ClientActor clientActor, Login login)
        {
            return Task.CompletedTask;
        }
        public Task Pong(ClientActor clientActor, Pong pong)
        {
            return Task.CompletedTask;
        }
    }
}
