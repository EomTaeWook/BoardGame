namespace Protocol.GSAndClient
{
    public enum CGSProtocol : ushort
    {
        Login,
        LeaveRoom,
        JoinRoom,
        LobbyRoomList,

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
