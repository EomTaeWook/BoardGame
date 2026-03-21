using BG.GameServer.Actors;
using BG.GameServer.Internals;
using BG.GameServer.Messages;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using Protocol.GSAndClient;
using System;
using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal class ClientInitState(ClientActor clientActor,
        IServiceProvider serviceProvider) : IClientState
    {
        public ValueTask HandlePacket(object packet)
        {
            return packet switch
            {
                Login request => HandleLogin(request, clientActor.Self),
                _ => ValueTask.CompletedTask
            };
        }
        private async ValueTask HandleLogin(Login request, IActorRef _)
        {
            if (string.IsNullOrEmpty(request.AccountId))
            {
                LogHelper.Error($"account Id is empty");
                clientActor.Self.Post(new KickUserMessage(ErrorCode.InvalidRequest), clientActor.Self);
                return;
            }

            if (string.IsNullOrEmpty(request.Nickname))
            {
                LogHelper.Error($"nickname Id is empty");
                clientActor.Self.Post(new KickUserMessage(ErrorCode.InvalidRequest), clientActor.Self);
                return;
            }

            var player = new Player(request.AccountId,
                request.Nickname,
                clientActor.Session);

            var sessionManager = serviceProvider.GetService<SessionManager>();

            if(sessionManager.TryAddPlayer(player) == false)
            {
                LogHelper.Error($"already login user. accountId: {request.AccountId}");
                clientActor.Self.Post(new KickUserMessage(ErrorCode.AlreadyLogin), clientActor.Self);
                return;
            }

            var actorSystem = serviceProvider.GetService<ActorSystem>();
            if(actorSystem.TryGetActorRef(typeof(RoomManagerActor).Name, out var actorRef))
            {
                await clientActor.ChangeStateAsync(new ClientLoggedInState(player, actorRef));
            }
            else
            {
                clientActor.Self.Post(new KickUserMessage(ErrorCode.InternalServerError), clientActor.Self);
            }
        }

        public void OnEnter()
        {

        }

        public void OnExit()
        {
        }
    }
}
