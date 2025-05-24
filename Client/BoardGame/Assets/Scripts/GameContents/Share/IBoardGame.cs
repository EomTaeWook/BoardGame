using Assets.Scripts.GameContents.WallGo;
using Dignus.Collections;

namespace Assets.Scripts.GameContents.Share
{
    public interface IBoardGame
    {
        void StartGame();
        void SetPlayers(ArrayQueue<WallGoPlayer> wallGoPlayers);
        void EndGame();
    }
}
