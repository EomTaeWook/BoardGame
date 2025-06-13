using Assets.Scripts.Extensions;
using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using DataContainer.Generated;
using Dignus.Collections;
using Dignus.Log;
using Dignus.Unity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    public class BoardGo : MonoBehaviour
    {
        private readonly Dictionary<string, ArrayQueue<PieceGo>> _playerPiecesByAccountId = new();
        private readonly ArrayQueue<TileGo> _tiles = new();

        private readonly Dictionary<TileGo, ArrayQueue<TileWallGo>> _tileWallMap = new Dictionary<TileGo, ArrayQueue<TileWallGo>>();
        private void Awake()
        {
            var x = 0;
            var y = 0;
            foreach (var template in TemplateContainer<WallGoBoardTile7x7Template>.Values)
            {
                foreach (var yPoint in template.Y)
                {
                    var tileGo = this.InstantiateWithPool<TileGo>();
                    tileGo.Tile = new Tile()
                    {
                        GridPosition = new Point(x, y)
                    };
                    tileGo.transform.position = new Vector3(template.X, yPoint, 0);
                    _tiles.Add(tileGo);
                    y++;
                }
                x++;
                y = 0;
            }
            x = y = 0;

            var templateGroup = TemplateContainer<WallGoBoardTileWall7x7Template>.Values.GroupBy(r => r.TileWallDirectionType);

            foreach (var group in templateGroup)
            {
                var templates = group.OrderBy(r => r.Id);

                foreach (var template in templates)
                {
                    foreach (var yPoint in template.Y)
                    {
                        if (yPoint == -1)
                        {
                            continue;
                        }
                        var tileWallGo = this.InstantiateWithPool<TileWallGo>();
                        var tileGo = GetTileObject(x, y);
                        tileWallGo.Init(tileGo.Tile, template.TileWallDirectionType);
                        tileWallGo.transform.position = new Vector3(template.X, yPoint, 0);


                        if (_tileWallMap.TryGetValue(tileGo, out var tileWallGos) == false)
                        {
                            tileWallGos = new ArrayQueue<TileWallGo>();
                            _tileWallMap.Add(tileGo, tileWallGos);
                        }

                        tileWallGos.Add(tileWallGo);
                        y++;
                    }
                    x++;
                    y = 0;
                }

                x = y = 0;
            }
        }
        public List<PieceGo> GetPieces()
        {
            var list = new List<PieceGo>();

            foreach (var pieces in _playerPiecesByAccountId)
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

            var tileGo = GetTileObject(spawnPiece.GridPosition);

            pieceGo.transform.position = new Vector3(tileGo.transform.position.x, tileGo.transform.position.y, -1);
        }
        public void PlaceWall(Color playerColor,
            Point point,
            Direction direction
            )
        {
            var tileGo = GetTileObject(point);

            var tileWalls = GetTileWallObjects(tileGo);

            if (direction == Direction.Right)
            {
                tileGo.Tile.WallRight = true;

                var rightPos = GetNeighborPoint(point, Direction.Right);

                var rightTileGo = GetTileObject(rightPos);

                if (rightTileGo != null)
                {
                    rightTileGo.Tile.WallLeft = true;
                }
            }
            else if (direction == Direction.Up)
            {
                tileGo.Tile.WallTop = true;

                var upPos = GetNeighborPoint(point, Direction.Up);

                var upTileGo = GetTileObject(upPos);

                if (upTileGo != null)
                {
                    upTileGo.Tile.WallBottom = true;
                }
            }

            foreach (var tileWall in tileWalls)
            {
                if (tileWall.GetDirection() == direction)
                {
                    tileWall.PlaceWall(playerColor);
                    break;
                }
            }
        }

        public void RemoveWall(Point point,
            Direction direction
            )
        {
            var tileGo = GetTileObject(point);

            var tileWalls = GetTileWallObjects(tileGo);

            if (direction == Direction.Right)
            {
                tileGo.Tile.WallRight = false;

                var rightPos = GetNeighborPoint(point, Direction.Right);

                var rightTileGo = GetTileObject(rightPos);

                if (rightTileGo != null)
                {
                    rightTileGo.Tile.WallLeft = false;
                }
            }
            else if (direction == Direction.Up)
            {
                tileGo.Tile.WallTop = false;

                var upPos = GetNeighborPoint(point, Direction.Up);

                var upTileGo = GetTileObject(upPos);

                if (upTileGo != null)
                {
                    upTileGo.Tile.WallBottom = false;
                }
            }

            foreach (var tileWall in tileWalls)
            {
                if (tileWall.GetDirection() == direction)
                {
                    tileWall.RemoveWall();
                    break;
                }
            }

        }

        private bool IsInsideBoard(Point p)
        {
            return p.X >= 0 && p.X < 7 &&
                   p.Y >= 0 && p.Y < 7;
        }

        public Point GetNeighborPoint(Point point, Direction direction)
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

            var tileGo = GetTileObject(movePiece.Dest);

            pieceGo.transform.position = new Vector3(tileGo.transform.position.x, tileGo.transform.position.y, -1);
        }

        public TileGo GetTileObject(Point point)
        {
            return GetTileObject(point.X, point.Y);
        }
        public ArrayQueue<TileWallGo> GetTileWallObjects(TileGo tileGo)
        {
            _tileWallMap.TryGetValue(tileGo, out ArrayQueue<TileWallGo> result);
            return result;
        }
        public TileGo GetTileObject(int x, int y)
        {
            if (x < 0 || x >= 7 || y < 0 || y >= 7)
            {
                LogHelper.Error($"invalid point. x: {x} y : {y}");
                return null;
            }
            int index = x * 7 + y;
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

            foreach (var array in _tileWallMap.Values)
            {
                foreach (var item in array)
                {
                    item.Recycle();
                }
                array.Clear();
            }
            _tileWallMap.Clear();
        }
    }
}
