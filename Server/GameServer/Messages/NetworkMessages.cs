using BG.GameServer.Actors;
using Dignus.Actor.Abstractions;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Messages;
using System;
using System.Threading.Tasks;

namespace BG.GameServer.Messages
{
    internal class InBoundLambda(Func<ClientActor, Task> action) : INetworkActorMessage
    {
        public async Task InvokeAsync(ClientActor actor)
        {
            await action.Invoke(actor);
        }
    }
    internal record struct OutBoundMessage(byte[] Bytes) : INetworkActorMessage;
}
