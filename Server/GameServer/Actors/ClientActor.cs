using BG.GameServer.ActorState;
using BG.GameServer.Messages;
using BG.GameServer.Network;
using Dignus.Actor.Core;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network;
using Dignus.DependencyInjection.Extensions;
using Protocol.GSAndClient;
using System;
using System.Threading.Tasks;

namespace BG.GameServer.Actors
{
    internal class ClientActor : SessionActorBase
    {
        public INetworkSessionRef Session => this.NetworkSession;

        private IClientState _currentState;
        private readonly HeartBeat _heartBeat;
        public ClientActor(IServiceProvider serviceProvider)
        {
            _currentState = new ClientInitState(this, serviceProvider);
            _heartBeat = serviceProvider.GetService<HeartBeat>();
        }
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            return message switch
            {
                InBoundLambda requet => HandleInBound(requet),
                KickUserMessage request => HandleServerNotify(request),

                _ => ValueTask.CompletedTask
            };
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
        public async Task ProcessPacket(object packet)
        {
            await ActorAwait.Join(this);
            if(packet is Pong)
            {
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
        public override void OnKill()
        {
            _heartBeat.Dispose();
            base.OnKill();
        }
    }
}
