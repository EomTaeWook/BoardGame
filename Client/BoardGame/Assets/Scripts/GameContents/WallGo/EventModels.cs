using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo.EventHandlers;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.GameContents.WallGo
{
    public class StartGame : IWallGoEvent
    {
    }
    public class EndGame : IWallGoEvent
    {
        public List<ScoreModel> ScoreModels { get; set; } = new List<ScoreModel>();
    }
    public class StartTurn : IWallGoEvent
    {
        public string AccountId { get; set; }
    }
    public class ChangeState : IWallGoEvent
    {
        public string AccountId { get; set; }

        public StateType UpdateStateType { get; set; }
    }
    public class SpawnPiece : IWallGoEvent
    {
        public string AccountId { get; set; }
        public int PieceId { get; set; }
        public Point SpawnedPoint { get; set; }
    }
    public class MovePiece : IWallGoEvent
    {
        public string AccountId { get; set; }
        public int PieceId { get; set; }
        public Point Dest { get; set; }
    }

    public class PlaceWall : IWallGoEvent
    {
        public string AccountId { get; set; }

        public Point Point { get; set; }
        public Direction Direction { get; set; }
    }

    public class RemoveWall : IWallGoEvent
    {
        public string AccountId { get; set; }

        public Point Point { get; set; }
        public Direction Direction { get; set; }
    }
}
