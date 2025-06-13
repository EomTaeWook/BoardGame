namespace Protocol.GSAndClient
{
    public enum CGSProtocol : ushort
    {
        Login,
        LeaveRoom,
        JoinRoom,
        CreateRoom,
        StartGameRoom,
        GetRoomList,

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
