using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo.EventHandlers;

namespace Assets.Scripts.GameContents.WallGo
{
    public class StartGame : IWallGoEvent
    {
    }
    public class EndGame : IWallGoEvent
    {
        public string AccountId { get; set; }
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
}
