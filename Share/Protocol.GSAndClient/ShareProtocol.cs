namespace Protocol.GSAndClient
{
    public enum PacketCategory : ushort
    {
        Lobby,
        WallGo,

        Max,
    }
    public enum GameType
    {
        WallGo,

        Max,
    }
    public enum ErrorCode
    {
        Success,
        InvalidRequest,
        DbError,
        AlreadyLogin,
        PingPongTimeout,

        InternalServerError,
        Max
    }
}
