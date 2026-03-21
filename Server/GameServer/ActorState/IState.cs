using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal interface IState
    {
        void OnEnter();
        void OnExit();
    }

    internal interface IClientState : IState
    {
        ValueTask HandlePacket(object packet);
    }
}
