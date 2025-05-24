using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.GameContents.WallGo.EventHandler;

namespace BG.GameServer.ServerContents.EventHandler
{
    internal class WallGoEventHandler : IWallGoEventHandler
    {
        public event Action<StartGame> StartGame;
        public event Action<EndGame> EndGame;
        public event Action<StartTurn> StartTurn;
        public event Action<ChangeState> ChangeState;
        public event Action<SpawnPiece> SpawnPiece;
        public event Action<MovePiece> MovePiece;
        public event Action<PlaceWall> PlaceWall;
        public void Process(StartGame evt)
        {
            StartGame?.Invoke(evt);
        }
        public void Process(EndGame evt)
        {
            EndGame?.Invoke(evt);
        }
        public void Process(StartTurn evt)
        {
            StartTurn?.Invoke(evt);
        }

        public void Process(ChangeState evt)
        {
            ChangeState?.Invoke(evt);
        }
        public void Process(SpawnPiece evt)
        {
            SpawnPiece?.Invoke(evt);
        }
        public void Process(MovePiece evt)
        {
            MovePiece?.Invoke(evt);
        }
        public void Process(PlaceWall evt)
        {
            PlaceWall?.Invoke(evt);
        }
    }
}
