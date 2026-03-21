using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;

namespace BG.GameServer.Models
{
    public class RoomSummary(int roomId,
        RoomMode roomMode,
        GameType gameType,
        int currentUserCount,
        int maxUserCount)
    {
        public int RoomId { get; set; } = roomId;

        public RoomMode RoomMode { get; set; } = roomMode;
        public GameType GameType { get; set; } = gameType;
        public int CurrentUserCount { get; set; } = currentUserCount;
        public int MaxUserCount { get; set; } = maxUserCount;
        public bool IsStarted { get; set; }
    }
}
