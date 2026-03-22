using Dignus.Actor.Abstractions;
using Protocol.GSAndClient.Models;

namespace Protocol.GSAndClient
{
    public class Pong : IActorMessage
    {
    }
    public class Login : IActorMessage
    {
        public string AccountId { get; set; }
        public string Nickname { get; set; }
    }
    public class GetRoomList : IActorMessage
    {
        public int Page { get; set; }

        public int ItemSize { get; set; }
    }

    public class LeaveRoom : IActorMessage
    {
        public RoomMode RoomMode { get; set; }
    }
    public class CreateRoom : IActorMessage
    {
        public int GameType { get; set; }

        public RoomMode RoomMode { get; set; }
    }
    public class JoinRoom : IActorMessage
    {
        public int RoomNumber { get; set; }

        public RoomMode RoomMode { get; set; }
    }
    public class StartGameRoom : IActorMessage
    {
    }
    public class SpawnPieceReqeust : IActorMessage
    {
        public int PieceId { get; set; }
        public int SpawnedPointX { get; set; }
        public int SpawnedPointY { get; set; }
    }
    public class MovePieceReqeust : IActorMessage
    {
        public int PieceId { get; set; }
        public int MovePointX { get; set; }
        public int MovePointY { get; set; }
    }
    public class PlaceWallReqeust : IActorMessage
    {
        public int TilePointX { get; set; }
        public int TilePointY { get; set; }
        public int Direction { get; set; }
    }

    public class RemoveWallReqeust : IActorMessage
    {
        public int TilePointX { get; set; }
        public int TilePointY { get; set; }
        public int Direction { get; set; }
    }
    public class Logout : IActorMessage
    {
    }
}
