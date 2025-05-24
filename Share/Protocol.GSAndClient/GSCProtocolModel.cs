using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Protocol.GSAndClient
{
    public class Ping
    {
    }

    public class LoginResponse
    {
        public bool Ok { get; set; }
    }

    public class LeaveRoomResponse
    {
        public string AccountId { get; set; }
    }
    public class JoinRoomResponse
    {
        public bool Ok { get; set; }
        public string AccountId { get; set; }
    }
    public class CreateRoomResponse
    {
        public bool Ok { get; set; }

        public long RoomNumber { get; set; }
    }

    public class LobbyRoomListResponse
    {
        public List<RoomInfo> RoomInfos { get; set; }
    }
}
