using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Protocol.GSAndClient
{
    public class Ping
    {
    }

    public class LoginResponse
    {
        public LoginReason LoginReason { get; set; }
    }
    public class GetRoomListResponse
    {
        public int Page { get; set; }
        public List<RoomInfo> RoomList { get; set; }
    }

    public class LeaveRoomResponse
    {
        public List<PlayerModel> Members { get; set; }
    }
    public class JoinRoomResponse
    {
        public JoinRoomReason FailedJoinRoomReason { get; set; }

        public List<PlayerModel> Members { get; set; }
    }
    public class CreateRoomResponse
    {
        public bool Ok { get; set; }

        public int RoomNumber { get; set; }
    }

    public class StartGameRoomResponse
    {
        public StartGameRoomReason StartGameRoomReason { get; set; }
        public GameType GameType { get; set; }
    }
}
