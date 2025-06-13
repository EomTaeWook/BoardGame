using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Scene.WallGo.EventHandler;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Newtonsoft.Json;

namespace Assets.Scripts.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class WallGoCommandHandler : IProtocolHandler<string>, ISessionComponent
    {
        private ISession _session;
        private readonly WallGoEventHandler _eventHandler;
        public WallGoCommandHandler(WallGoEventHandler wallGoEventHandler)
        {
            _eventHandler = wallGoEventHandler;
        }
        public T DeserializeBody<T>(string body)
        {
            return JsonConvert.DeserializeObject<T>(body);
        }

        public void Dispose()
        {
            _session = null;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }

        public void StartGame(StartGame evt)
        {
            _eventHandler.Process(evt);
        }
        public void StartTurn(StartTurn evt)
        {
            _eventHandler.Process(evt);
        }
        public void EndGame(EndGame evt)
        {
            _eventHandler.Process(evt);
        }
        public void ChangeState(ChangeState evt)
        {
            _eventHandler.Process(evt);
        }
        public void SpawnPiece(SpawnPiece evt)
        {
            _eventHandler.Process(evt);
        }
        public void MovePiece(MovePiece evt)
        {
            _eventHandler.Process(evt);
        }

        public void PlaceWall(PlaceWall evt)
        {
            _eventHandler.Process(evt);
        }
        public void RemoveWall(RemoveWall evt)
        {
            _eventHandler.Process(evt);
        }
    }
}
