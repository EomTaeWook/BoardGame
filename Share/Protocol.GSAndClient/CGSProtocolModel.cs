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

    public class LeaveRoom
    {
    }
    public class CreateRoom
    {
        public int GameType { get; set; }
    }
    public class JoinRoom
    {
        public long RoomNumber { get; set; }
    }
    public class LobbyRoomList
    {
        public int PageIndex { get; set; }
    }
}
