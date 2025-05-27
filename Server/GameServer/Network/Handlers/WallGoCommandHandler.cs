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
        public void RemoveWall(RemoveWallReqeust reqeust)
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

            wallGoRoom.RemoveWallReqeust(new RemoveWall() 
            {
                AccountId = _player.AccountId,
                Point = new Point(reqeust.TilePointX, reqeust.TilePointY),
                Direction = (Direction)reqeust.Direction
            });
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

            wallGoRoom.PlaceWallReqeust(placeWall);
        }
        public void MovePiece(MovePieceReqeust request)
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

            wallGoRoom.MovePieceReqeust(new MovePiece()
            {
                AccountId = _player.AccountId,
                Dest = new Point(request.MovePointX, request.MovePointY),
                PieceId = request.PieceId
            });
        }
        public void SpawnPiece(SpawnPieceReqeust request)
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

            wallGoRoom.SpawnPieceReqeust(new SpawnPiece()
            {
                AccountId = _player.AccountId,
                PieceId = request.PieceId,
                SpawnedPoint = new Point(request.SpawnedPointX, request.SpawnedPointY)
            });
        }
        public void PlaceWall(PlaceWallReqeust request)
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

            wallGoRoom.PlaceWallReqeust(new PlaceWall()
            {
                AccountId = _player.AccountId,
                Point = new Point(request.TilePointX, request.TilePointY),
                Direction = (Direction)request.Direction
            });
        }
    }
}
