namespace Protocol.GSAndClient
{
    public enum GSCProtocol : ushort
    {
        LoginResponse,
        LeaveRoomResponse,
        JoinRoomResponse,
        CreateRoomResponse,
        StartGameRoomResponse,
        GetRoomListResponse,

        Ping,

        Max
    }

    public enum WallGoServerEvent : ushort
    {
        PlaceWall,
        MovePiece,
        SpawnPiece,
        ChangeState,
        StartGame,
        EndGame,
        StartTurn,
        RemoveWall,

        Max
    }
}
