using BG.GameServer.Messages;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal class ClientLoggedInState(Player player,
        IActorRef lobbyManagerRef) : IClientState
    {
        public ValueTask HandlePacket(object packet)
        {
            return packet switch
            {
                GetRoomList request => HandleGetRoomList(request),
                JoinRoom request => HandleJoinRoom(request),
                CreateRoom request => HandleCreateRoom(request),
                LeaveRoom request => HandleLeaveRoom(request),
                StartGameRoom request => HandleStartGameRoom(request),
                _ => ValueTask.CompletedTask
            };
        }
        private ValueTask HandleStartGameRoom(StartGameRoom _)
        {
            if (player.RoomActorRef != null)
            {
                player.RoomActorRef.Post(new StartGameRoomMessage(player), player.SessionRef);
            }
            return ValueTask.CompletedTask;
        }
        private ValueTask HandleCreateRoom(CreateRoom message)
        {
            lobbyManagerRef.Post(new CreateRoomMessage(message, player), player.SessionRef);
            return ValueTask.CompletedTask;
        }
        private ValueTask HandleLeaveRoom(LeaveRoom _)
        {
            if(player.RoomActorRef != null)
            {
                player.RoomActorRef.Post(new LeaveMemberMessage(player), player.SessionRef);
            }
            return ValueTask.CompletedTask;
        }
        private ValueTask HandleGetRoomList(GetRoomList message)
        {
            lobbyManagerRef.Post(new GetRoomListRequestMessage(message), player.SessionRef);
            return ValueTask.CompletedTask;
        }
        private ValueTask HandleJoinRoom(JoinRoom message)
        {
            lobbyManagerRef.Post(new JoinRoomMessage(message, player), player.SessionRef);
            return ValueTask.CompletedTask;
        }

        public void OnEnter()
        {
            player.Send(Packet.MakePacket(GSCProtocol.LoginResponse,
                new LoginResponse()
                {
                    LoginReason = LoginReason.Success
                }));
        }

        public void OnExit()
        {
        }
    }
}
