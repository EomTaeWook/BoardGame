using Dignus.Actor.Core;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network;
using System.Threading.Tasks;

namespace BG.GameServer.Actors
{
    internal class ClientActor : SessionActorBase
    {

        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {

            return ValueTask.CompletedTask;
        }
    }
}
