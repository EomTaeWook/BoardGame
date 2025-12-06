using BG.GameServer.ServerGameContents;

namespace BG.GameServer.Network.Handlers
{
    internal interface IPlayerComponent
    {
        void SetPlayer(Player player);
    }
}
