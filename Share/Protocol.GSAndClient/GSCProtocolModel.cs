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
        public List<PlayerModel> Members { get; set; }
    }
    public class JoinRoomResponse
    {
        public bool Ok { get; set; }

        public List<PlayerModel> Members { get; set; }
    }
    public class CreateRoomResponse
    {
        public bool Ok { get; set; }

        public int RoomNumber { get; set; }
    }

    public class StartGameRoomResponse
    {
        public bool Ok { get; set; }
    }
}
