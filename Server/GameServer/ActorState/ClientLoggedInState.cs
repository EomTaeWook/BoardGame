using BG.GameServer.Actors;
using BG.GameServer.Messages;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core;
using Dignus.DependencyInjection.Extensions;
using Protocol.GSAndClient;
using System;
using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal class ClientLoggedInState(Player player,
        IServiceProvider serviceProvider) : IClientState
    {
        private IActorRef _lobbyManagerRef;
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
        private ValueTask HandleStartGameRoom(StartGameRoom message)
        {
            if(player.RoomActorRef == null)
            {
                player.SessionRef.Kill();
                return ValueTask.CompletedTask;
            }

            player.RoomActorRef.Post(new StartGameRoomMessage(player), player.SessionRef);

            return ValueTask.CompletedTask;
        }
        private ValueTask HandleCreateRoom(CreateRoom message)
        {
            _lobbyManagerRef.Post(new PlayerMessage<CreateRoom>(message, player), player.SessionRef);
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
            _lobbyManagerRef.Post(new PlayerMessage<GetRoomList>(message, player), player.SessionRef);
            return ValueTask.CompletedTask;
        }
        private ValueTask HandleJoinRoom(JoinRoom message)
        {
            _lobbyManagerRef.Post(new PlayerMessage<JoinRoom>(message, player), player.SessionRef);
            return ValueTask.CompletedTask;
        }

        public void OnEnter()
        {
            var actorSystem = serviceProvider.GetService<ActorSystem>();

            if(actorSystem.TryGetActorRef(typeof(RoomManagerActor).Name, out var actorRef) == false)
            {
                player.SessionRef.Post(new KickUserMessage(ErrorCode.InternalServerError), player.SessionRef);
                return;
            }
            _lobbyManagerRef = actorRef;
        }

        public void OnExit()
        {
        }
    }
}
