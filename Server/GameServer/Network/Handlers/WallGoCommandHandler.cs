using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.ServerContents;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using System.Text.Json;

namespace BG.GameServer.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class WallGoCommandHandler : IProtocolHandler<string>, ISessionComponent, IPlayerComponent
    {
        private Player _player;
        private ISession _session;
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }

        void ISessionComponent.Dispose()
        {
            _player = null;
        }

        public void PlaceWall(PlaceWall placeWall)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }

            if (_player.Room is not WallGoRoom wallGoRoom)
            {
                _session.Dispose();
                return;
            }
            wallGoRoom.PlacWallReqeust(_player, placeWall);
        }
        public void MovePiece(MovePiece movePiece)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }
            if (_player.Room is not WallGoRoom wallGoRoom)
            {
                _session.Dispose();
                return;
            }
            wallGoRoom.MovePieceReqeust(_player, movePiece);
        }
        public void SpawnPiece(SpawnPiece spawnPiece)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }

            if (_player.Room is not WallGoRoom wallGoRoom)
            {
                _session.Dispose();
                return;
            }

            wallGoRoom.SpawnPieceReqeust(_player, spawnPiece);
        }
    }
}
