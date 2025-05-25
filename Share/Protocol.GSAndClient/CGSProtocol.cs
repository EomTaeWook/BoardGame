namespace Protocol.GSAndClient
{
    public enum CGSProtocol : ushort
    {
        Login,
        LeaveRoom,
        JoinRoom,
        CreateRoom,
        StartGameRoom,

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
