using Assets.Scripts.Extensions;
using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Dignus.Collections;
using Dignus.Unity.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    public class BoardGo : MonoBehaviour
    {
        private Dictionary<string, ArrayQueue<PieceGo>> _playerPiecesByAccountId = new();
        private readonly ArrayQueue<TileGo> _tiles = new();
        private void Awake()
        {
            for (int i = 0; i < 7; ++i)
            {
                for (int ii = 0; ii < 7; ++ii)
                {
                    var tileGo = this.InstantiateWithPool<TileGo>();

                    tileGo.Tile = new Tile()
                    {
                        GridPosition = new Point(i, ii)
                    };
                    tileGo.transform.position = new Vector3(i, ii, 0);
                    _tiles.Add(tileGo);
                }
            }
        }
        public List<PieceGo> GetPieces()
        {
            var list = new List<PieceGo>();

            foreach(var pieces in _playerPiecesByAccountId)
            {
                list.AddRange(pieces.Value);
            }

            return list;
        }
        public void SpawnPiece(string accountId,
            Color color,
            Piece spawnPiece,
            bool isPlayerPiece
            )
        {
            if (_playerPiecesByAccountId.TryGetValue(accountId, out ArrayQueue<PieceGo> pieces) == false)
            {
                pieces = new ArrayQueue<PieceGo>(4);
                _playerPiecesByAccountId.Add(accountId, pieces);
            }

            var pieceGo = this.InstantiateWithPool<PieceGo>();
            pieceGo.Init(spawnPiece, color, isPlayerPiece);
            _playerPiecesByAccountId[accountId][spawnPiece.Id] = pieceGo;
            pieceGo.transform.position = new Vector3(spawnPiece.GridPosition.X, spawnPiece.GridPosition.Y, -1);
        }
        public void PlaceWall(Color playerColor,
            Point point,
            Direction direction
            )
        {
            var tileGo = GetTileObject(point);
            tileGo.PlaceWall(direction, playerColor);

            var neighborPoint = GetNeighborPoint(point, direction);

            if(IsInsideBoard(neighborPoint)==false)
            {
                return;
            }

            Direction neighborDirection = Direction.Up;

            if(direction == Direction.Up)
            {
                neighborDirection = Direction.Down;
            }
            else if (direction == Direction.Left)
            {
                neighborDirection = Direction.Right;
            }
            else if (direction == Direction.Right)
            {
                neighborDirection = Direction.Left;
            }
            else if (direction == Direction.Down)
            {
                neighborDirection = Direction.Up;
            }

            var neighborTileGo = GetTileObject(neighborPoint);
            neighborTileGo.PlaceWall(neighborDirection, playerColor);
        }

        public void RemoveWall(Point point,
            Direction direction
            )
        {
            var tileGo = GetTileObject(point);
            tileGo.RemoveWall(direction);

            var neighborPoint = GetNeighborPoint(point, direction);

            if (IsInsideBoard(neighborPoint) == false)
            {
                return;
            }

            Direction neighborDirection = Direction.Up;

            if (direction == Direction.Up)
            {
                neighborDirection = Direction.Down;
            }
            else if (direction == Direction.Left)
            {
                neighborDirection = Direction.Right;
            }
            else if (direction == Direction.Right)
            {
                neighborDirection = Direction.Left;
            }
            else if (direction == Direction.Down)
            {
                neighborDirection = Direction.Up;
            }

            var neighborTileGo = GetTileObject(neighborPoint);
            neighborTileGo.RemoveWall(neighborDirection);
        }

        private bool IsInsideBoard(Point p)
        {
            return p.X >= 0 && p.X < 7 &&
                   p.Y >= 0 && p.Y < 7;
        }

        private Point GetNeighborPoint(Point point, Direction direction)
        {
            if (direction == Direction.Up)
            {
                return point + Point.Up;
            }
            else if (direction == Direction.Left)
            {
                return point + Point.Left;
            }
            else if (direction == Direction.Right)
            {
                return point + Point.Right;
            }
            else if (direction == Direction.Down)
            {
                return point + Point.Down;
            }

            throw new ArgumentOutOfRangeException(nameof(direction));
        }
        public void MovePiece(MovePiece movePiece)
        {
            var playerPieces = _playerPiecesByAccountId[movePiece.AccountId];

            var pieceGo = playerPieces[movePiece.PieceId];

            pieceGo.GetPiece().GridPosition = movePiece.Dest;

            pieceGo.transform.position = new Vector3(movePiece.Dest.X, movePiece.Dest.Y, -1);
        }

        public TileGo GetTileObject(Point point)
        {
            if (point.X < 0 || point.X >= 7 || point.Y < 0 || point.Y >= 7)
            {
                return null;
            }
            int index = point.X * 7 + point.Y;
            return _tiles[index];
        }

        public void Dispose()
        {
            foreach (var kv in _playerPiecesByAccountId)
            {
                foreach (var value in kv.Value)
                {
                    value.Recycle();
                }
                kv.Value.Clear();
            }
            _playerPiecesByAccountId.Clear();

            foreach (var item in _tiles)
            {
                item.Recycle();
            }
            _tiles.Clear();
        }
    }
}
