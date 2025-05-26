using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.ServerContents;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
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
        public void RemoveWall(RemoveWall removeWall)
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

            wallGoRoom.RemoveWallReqeust(_player, removeWall);
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

            wallGoRoom.PlaceWallReqeust(_player, placeWall);
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
        public void SpawnPiece(SpawnPeiceReqeust reqeust)
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

            wallGoRoom.SpawnPieceReqeust(_player, new Assets.Scripts.GameContents.WallGo.SpawnPiece()
            {
                AccountId = _player.AccountId,
                PieceId = reqeust.PieceId,
                SpawnedPoint = new Point(reqeust.SpawnedPointX, reqeust.SpawnedPointY)
            });
        }
    }
}
