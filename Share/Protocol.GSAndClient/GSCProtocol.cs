namespace Protocol.GSAndClient
{
    public enum GSCProtocol : ushort
    {
        LoginResponse,
        LeaveRoomResponse,
        JoinRoomResponse,
        CreateRoomResponse,

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

        Max
    }
}
