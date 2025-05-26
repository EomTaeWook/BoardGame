using Assets.Scripts.GameContents.WallGo;
using System.Collections.Generic;

namespace Assets.Scripts.GameContents.Share
{
    public interface IBoardGame
    {
        void StartGame();
        void SetPlayers(ICollection<IPlayer> players);
        void EndGame();
    }
}
