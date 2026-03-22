using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo.EventHandlers;
using Dignus.Actor.Abstractions;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.GameContents.WallGo
{
    public class StartGame : IWallGoEvent, IActorMessage
    {
    }
    public class EndGame : IWallGoEvent, IActorMessage
    {
        public List<ScoreModel> ScoreModels { get; set; } = new List<ScoreModel>();
    }
    public class StartTurn : IWallGoEvent, IActorMessage
    {
        public string AccountId { get; set; }
    }
    public class ChangeState : IWallGoEvent, IActorMessage
    {
        public string AccountId { get; set; }
        public StateType UpdateStateType { get; set; }
    }
    public class SpawnPiece : IWallGoEvent, IActorMessage
    {
        public string AccountId { get; set; }
        public int PieceId { get; set; }
        public Point SpawnedPoint { get; set; }
    }
    public class MovePiece : IWallGoEvent, IActorMessage
    {
        public string AccountId { get; set; }
        public int PieceId { get; set; }
        public Point Dest { get; set; }
    }

    public class PlaceWall : IWallGoEvent, IActorMessage
    {
        public string AccountId { get; set; }
        public Point Point { get; set; }
        public Direction Direction { get; set; }
    }

    public class RemoveWall : IWallGoEvent, IActorMessage
    {
        public string AccountId { get; set; }
        public Point Point { get; set; }
        public Direction Direction { get; set; }
    }
}
