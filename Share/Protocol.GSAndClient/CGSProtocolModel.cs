using Protocol.GSAndClient.Models;

namespace Protocol.GSAndClient
{
    public class Pong
    {
    }

    public class Login
    {
        public string AccountId { get; set; }
        public string Nickname { get; set; }
    }
    public class GetRoomList
    {
        public int Page { get; set; }

        public int ItemSize { get; set; }
    }

    public class LeaveRoom
    {
        public RoomMode RoomMode { get; set; }
    }
    public class CreateRoom
    {
        public int GameType { get; set; }

        public RoomMode RoomMode { get; set; }
    }
    public class JoinRoom
    {
        public int RoomNumber { get; set; }

        public RoomMode RoomMode { get; set; }
    }
    public class StartGameRoom
    {
    }

    public class SpawnPieceReqeust
    {
        public int PieceId { get; set; }
        public int SpawnedPointX { get; set; }
        public int SpawnedPointY { get; set; }
    }
    public class MovePieceReqeust
    {
        public int PieceId { get; set; }
        public int MovePointX { get; set; }
        public int MovePointY { get; set; }
    }
    public class PlaceWallReqeust
    {
        public int TilePointX { get; set; }
        public int TilePointY { get; set; }
        public int Direction { get; set; }
    }

    public class RemoveWallReqeust
    {
        public int TilePointX { get; set; }
        public int TilePointY { get; set; }
        public int Direction { get; set; }
    }
}
