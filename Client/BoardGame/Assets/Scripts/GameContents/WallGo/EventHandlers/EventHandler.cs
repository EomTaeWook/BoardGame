namespace Assets.Scripts.GameContents.WallGo.EventHandler
{
    public interface IGameEventHandler
    { }
    public interface IWallGoEventHandler : IGameEventHandler
    {
        public void Process(StartGame evt);
        public void Process(EndGame evt);
        public void Process(StartTurn evt);
        public void Process(ChangeState evt);
        public void Process(SpawnPiece evt);
        public void Process(MovePiece evt);
        public void Process(PlaceWall evt);
        public void Process(RemoveWall evt);
    }
}