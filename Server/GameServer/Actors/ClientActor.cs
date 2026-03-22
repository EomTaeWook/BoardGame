using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.ActorState;
using BG.GameServer.Internals;
using BG.GameServer.Messages;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Abstractions;
using Dignus.Actor.Core;
using Dignus.Actor.Network;
using Dignus.DependencyInjection.Extensions;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System;
using System.Threading.Tasks;

namespace BG.GameServer.Actors
{
    internal class ClientActor : SessionActorBase
    {
        public INetworkSessionRef Session => this.NetworkSession;

        private IClientState _currentState;
        private readonly HeartBeat _heartBeat;
        private readonly IServiceProvider _serviceProvider;
        private Player _player;
        public ClientActor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _currentState = new ClientInitState(this, serviceProvider);
            _heartBeat = serviceProvider.GetService<HeartBeat>();            
        }
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            return message switch
            {
                InBoundLambda request => HandleInBound(request),
                KickUserMessage request => HandleServerNotify(request),
                AcceptedMessage => HandleAccept(),
                PlayerMessage<StartGameRoomResponse> request => HandleStartGame(request),
                PlayerMessage<EndGame> request => HandleEndGame(request),
                _ => ValueTask.CompletedTask
            };
        }
        private ValueTask HandleAccept()
        {
            _heartBeat.SetSession(this.Session);
            return ValueTask.CompletedTask;
        }
        private async ValueTask HandleEndGame(PlayerMessage<EndGame> message)
        {
            await ChangeStateAsync(new ClientLoggedInState(message.Player, _serviceProvider));
        }
        private async ValueTask HandleStartGame(PlayerMessage<StartGameRoomResponse> message)
        {
            if (message.Value.StartGameRoomReason == StartGameRoomReason.Success)
            {
                await ChangeStateAsync(new ClientInWallGoRoomState(message.Player));
            }

            this.Session.SendAsync(Packet.MakePacket(GSCProtocol.StartGameRoomResponse,
                message.Value));
        }
        private async ValueTask HandleInBound(InBoundLambda message)
        {
            await message.InvokeAsync(this);
        }
        private ValueTask HandleServerNotify(KickUserMessage message)
        {
            NetworkSession.SendAsync(Packet.MakePacket(GSCProtocol.ServerNotify, new ServerNotify()
            {
                ErrorCode = message.Reason
            }));

            this.Self.Kill();

            return ValueTask.CompletedTask;
        }
        public async Task ProcessPacket(IActorMessage packet)
        {
            await ActorAwait.Join(this);
            if (packet is Pong)
            {
                _heartBeat.Pong();
                return;
            }
            await _currentState.HandlePacket(packet);
        }
        public async ValueTask ChangeStateAsync(IClientState newState)
        {
            await ActorAwait.Join(this);

            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = newState;

            _currentState.OnEnter();
        }
        public void SetPlayer(Player player)
        {
            _player = player;
        }
        public override void OnKill()
        {
            _heartBeat.Dispose();

            if (_player != null)
            {
                var sessionManager = _serviceProvider.GetService<SessionManager>();
                sessionManager.TryRemovePlayer(_player);
            }
            base.OnKill();
        }
    }
}
