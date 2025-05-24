namespace Protocol.GSAndClient
{
    public enum CGSProtocol : ushort
    {
        Login,
        LeaveRoom,
        JoinRoom,

        UserGameRequest,

        Max
    }
    public enum WallGoCommandProtocol : ushort
    {
        PlaceWall,
        MovePiece,
        SpawnPiece,

        Max
    }
}
