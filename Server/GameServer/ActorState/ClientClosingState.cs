using BG.GameServer.Actors;
using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal class ClientClosingState(ClientActor clientActor) : IClientState
    {
        public ValueTask HandlePacket(object packet)
        {
            return ValueTask.CompletedTask;
        }
        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}
