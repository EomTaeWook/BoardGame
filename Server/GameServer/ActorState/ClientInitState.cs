using BG.GameServer.Actors;
using BG.GameServer.Internals;
using BG.GameServer.Messages;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
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

            clientActor.SetPlayer(player);

            await clientActor.ChangeStateAsync(new ClientLoggedInState(player, serviceProvider));

            player.Send(Packet.MakePacket(GSCProtocol.LoginResponse,
                new LoginResponse()
                {
                    LoginReason = LoginReason.Success
                }));
        }

        public void OnEnter()
        {

        }

        public void OnExit()
        {
        }
    }
}
