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
        RemoveGameRoom,

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
