using Assets.Scripts.GameContents.WallGo;
using Dignus.Collections;
using Dignus.Unity.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.Scene.WallGo
{
    public class WallGoSceneModel : ISceneModel
    {
        public Dictionary<string, WallGoPlayer> PlayersToMap { get; set; }

        public WallGoPlayer CurrentPlayer { get; set; }

        public PieceGo SelectedPiece { get; set; }

        public ArrayQueue<TileGo> MoveAvailableTiles { get; set; } = new ArrayQueue<TileGo>();

    }
}
