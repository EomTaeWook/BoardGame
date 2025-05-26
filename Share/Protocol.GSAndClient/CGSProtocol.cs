namespace Protocol.GSAndClient
{
    public enum CGSProtocol : ushort
    {
        Login,
        LeaveRoom,
        JoinRoom,
        CreateRoom,
        StartGameRoom,

        Pong,

        Max
    }
    public enum WallGoCommandProtocol : ushort
    {
        PlaceWall,
        MovePiece,
        SpawnPiece,
        RemoveWall,

        Max
    }
}
