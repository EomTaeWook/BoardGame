using BG.GameServer.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.GameServer.Messages
{
    public enum ErrorCode
    {
        Success,
        InvalidRequest,
        DbError,
        DuplicateLogin,

        InternalServerError,
        Max
    }

    internal class InBoundLambda(Func<ClientActor, Task> action) : INetworkActorMessage
    {
        public async Task InvokeAsync(ClientActor actor)
        {
            await action.Invoke(actor);
        }
    }

    internal class KickUserMessage : IActorMessage
    {
        public ErrorCode Reason { get; set; }
    }
}
