using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.GameContents.WallGo.EventHandler;
using Assets.Scripts.GameContents.WallGo.EventHandlers;
using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;

namespace Assets.Scripts.Scene.WallGo.EventHandler
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class WallGoEventHandler : IWallGoEventHandler
    {
        public ArrayQueue<IWallGoEvent> WallGoEvents => _wallGoEvents;
        private readonly ArrayQueue<IWallGoEvent> _wallGoEvents = new();

        public void Process(StartGame evt)
        {
            _wallGoEvents.Add(evt);
        }

        public void Process(EndGame evt)
        {
            _wallGoEvents.Add(evt);
        }

        public void Process(StartTurn evt)
        {
            _wallGoEvents.Add(evt);
        }

        public void Process(ChangeState evt)
        {
            _wallGoEvents.Add(evt);
        }

        public void Process(SpawnPiece evt)
        {
            _wallGoEvents.Add(evt);
        }

        public void Process(MovePiece evt)
        {
            _wallGoEvents.Add(evt);
        }

        public void Process(PlaceWall evt)
        {
            _wallGoEvents.Add(evt);
        }
    }
}
