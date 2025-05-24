using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo.EventHandler;
using Dignus.Collections;
using Dignus.Coroutine;
using Dignus.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Scripts.GameContents.WallGo
{
    public class WallGoBoard : IBoardGame
    {
        //0,0���� ����Ƽ ������ ���� �ϴ�
        private readonly Tile[,] _tiles = new Tile[7, 7];

        private readonly CompactableArrayQueue<WallGoPlayer> _turnQueue = new();

        private readonly ArrayQueue<WallGoPlayer> _players = new();

        private WallGoPlayer _currentPlayer;

        private readonly IWallGoEventHandler _wallGoEventHandler;

        private readonly TimeSpan TurnTimeout = TimeSpan.FromSeconds(90);

        private readonly CoroutineHandler _coroutineHandler = new();

        private bool _isRunning = false;
        public WallGoBoard(IWallGoEventHandler wallGoEventHandler)
        {
            _wallGoEventHandler = wallGoEventHandler;
            for (int i = 0; i < _tiles.GetLength(0); ++i)
            {
                for (int ii = 0; ii < _tiles.GetLength(0); ++ii)
                {
                    _tiles[i, ii] = new Tile()
                    {
                        GridPos = new Point(i, ii)
                    };
                }
            }
        }

        public void SetPlayers(ArrayQueue<WallGoPlayer> wallGoPlayers)
        {
            _players.AddRange(wallGoPlayers);
        }
        public void StartGame()
        {
            for (int i = 0; i < _players.Count; ++i)
            {
                _turnQueue.Add(_players[i]);
            }
            for (int i = _players.Count - 1; i >= 0; --i)
            {
                _turnQueue.Add(_players[i]);
            }
            for (int i = 0; i < _players.Count; ++i)
            {
                _turnQueue.Add(_players[i]);
            }

            _currentPlayer = _turnQueue.Peek();
            _wallGoEventHandler.Process(new StartGame());
            StartTurn(_currentPlayer);

            _coroutineHandler.Start(Tick());
            _isRunning = true;
            Task.Run(async () =>
            {
                var startTime = DateTime.UtcNow;
                while (_isRunning)
                {
                    await Task.Delay(33);
                    var elapsedSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
                    _coroutineHandler.UpdateCoroutines((float)elapsedSeconds);
                    startTime = DateTime.UtcNow;
                }
            });
        }
        private void StartTurn(WallGoPlayer wallGoPlayer)
        {
            wallGoPlayer.StartTurn();
            _wallGoEventHandler.Process(new StartTurn()
            {
                AccountId = wallGoPlayer.AccountId
            });
        }
        public bool IsEndGame()
        {
            return false;
        }
        public void EndGame()
        {
            _isRunning = false;
            _coroutineHandler.StopAll();
        }
        private bool TryForceSpawn()
        {
            foreach (var piece in _currentPlayer.PlayerPieces)
            {
                if (piece.Spawned)
                {
                    continue;
                }

                for (int x = 0; x < _tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < _tiles.GetLength(1); y++)
                    {
                        var point = new Point(x, y);
                        if (!IsTileOccupied(point))
                        {
                            piece.SetSpawn(point);
                            _wallGoEventHandler.Process(new SpawnPiece()
                            {
                                AccountId = _currentPlayer.AccountId,
                                PieceId = piece.Id,
                                SpawnedPoint = point
                            });

                            return true;
                        }
                    }
                }
            }
            LogHelper.Error($"force spawn failed. no available tile for player {_currentPlayer.AccountId}");
            return false;
        }
        private IEnumerator Tick()
        {
            while (true)
            {
                if (IsTurnTimeout(_currentPlayer) == true)
                {
                    if (_currentPlayer.State == WallGoPlayer.StateType.SpawnPiece ||
                        _currentPlayer.State == WallGoPlayer.StateType.SpawnPiece1)
                    {
                        TryForceSpawn();
                        var currentState = _currentPlayer.State;
                        currentState++;
                        ChangeState(_currentPlayer, currentState);

                        _ = _turnQueue.Read();
                        _currentPlayer = _turnQueue.Peek();
                        StartTurn(_currentPlayer);
                    }
                    else if (_currentPlayer.State == WallGoPlayer.StateType.MovePeice ||
                        _currentPlayer.State == WallGoPlayer.StateType.PlaceWall)
                    {
                        _currentPlayer.ChangeState(WallGoPlayer.StateType.PlaceWall);

                        //EndTurn ���� ȣ�����
                        if (TryForcePlaceWall() == false)
                        {
                            _currentPlayer.ChangeState(WallGoPlayer.StateType.MovePeice);
                            _wallGoEventHandler.Process(new ChangeState
                            {
                                AccountId = _currentPlayer.AccountId,
                                UpdateStateType = _currentPlayer.State
                            });

                            EndTurn();
                        }
                    }
                }

                yield return null;
            }
        }
        private void ChangeState(WallGoPlayer wallGoPlayer, WallGoPlayer.StateType changeState)
        {
            wallGoPlayer.ChangeState(changeState);
            _wallGoEventHandler.Process(new ChangeState
            {
                AccountId = wallGoPlayer.AccountId,
                UpdateStateType = wallGoPlayer.State
            });
        }
        private void EndTurn()
        {
            var finishedPlayer = _turnQueue.Read();
            finishedPlayer.EndTurn();
            _turnQueue.Add(finishedPlayer);

            _currentPlayer = _turnQueue.Peek();
            _currentPlayer.StartTurn();
            _wallGoEventHandler.Process(new StartTurn()
            {
                AccountId = finishedPlayer.AccountId,
            });
        }
        public void Shuffle<T>(List<T> values)
        {
            Random random = new();
            for (int i = values.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
            }
        }
        private bool IsInsideBoard(Point p)
        {
            return p.X >= 0 && p.X < _tiles.GetLength(0) &&
                   p.Y >= 0 && p.Y < _tiles.GetLength(1);
        }
        private bool TryForcePlaceWall()
        {
            var directions = new List<Direction>()
                    {
                        Direction.Left,
                        Direction.Right,
                        Direction.Up,
                        Direction.Down
                    };

            if (_currentPlayer.LastMovePiece == null)
            {
                foreach (var piece in _currentPlayer.PlayerPieces)
                {
                    if (!piece.Spawned)
                        continue;

                    Shuffle(directions);

                    _currentPlayer.LastMovePiece = piece;

                    foreach (var direction in directions)
                    {
                        if (TryPlaceWall(_currentPlayer, direction) == true)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                Shuffle(directions);
                foreach (var direction in directions)
                {
                    if (TryPlaceWall(_currentPlayer, direction) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsTurnTimeout(WallGoPlayer wallGoPlayer)
        {
            return DateTime.UtcNow - wallGoPlayer.TurnStartTime > TurnTimeout;
        }
        public bool TrySpawnPiece(IPlayer player, int pieceId, Point point)
        {
            if (_currentPlayer.AccountId != player.AccountId)
            {
                LogHelper.Error($"not player turn. expected: {_currentPlayer.AccountId}");
                return false;
            }
            if (_currentPlayer.State == WallGoPlayer.StateType.MovePeice ||
                _currentPlayer.State == WallGoPlayer.StateType.PlaceWall)
            {
                LogHelper.Error("cannot spawn piece. current state: " + _currentPlayer.State);
                return false;
            }

            if (_currentPlayer.PlayerPieces.Count < pieceId)
            {
                LogHelper.Error($"Invalid piece ID: pieceId: {pieceId}");
                return false;
            }

            var spawnPiece = _currentPlayer.PlayerPieces[pieceId];

            if (spawnPiece.Spawned == true)
            {
                LogHelper.Error($"already spawned. pieceId: {pieceId}");
                return false;
            }

            if (IsTileOccupied(point))
            {
                LogHelper.Error($"tile already occupied at: {point}");
                return false;
            }

            spawnPiece.SetSpawn(point);

            _wallGoEventHandler.Process(new SpawnPiece()
            {
                AccountId = player.AccountId,
                PieceId = spawnPiece.Id,
                SpawnedPoint = point,
            });


            var currentState = _currentPlayer.State;
            currentState++;
            ChangeState(_currentPlayer, currentState);

            _ = _turnQueue.Read();
            _currentPlayer = _turnQueue.Peek();
            StartTurn(_currentPlayer);

            return true;
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
        public bool TryPlaceWall(IPlayer player, Direction direction)
        {
            if (_currentPlayer.AccountId != player.AccountId)
            {
                LogHelper.Error($"not player turn. expected: {_currentPlayer.AccountId}");
                return false;
            }

            if (_currentPlayer.State != WallGoPlayer.StateType.PlaceWall)
            {
                LogHelper.Error("cannot place wall. current state: " + _currentPlayer.State);
                return false;
            }

            var lastMovePiece = _currentPlayer.LastMovePiece;

            var point = lastMovePiece.GridPos;

            Point neighbor = GetNeighborPoint(point, direction);

            Tile fromTile = IsInsideBoard(point) ? GetTile(point) : null;
            Tile toTile = IsInsideBoard(neighbor) ? GetTile(neighbor) : null;

            if (IsWallAlreadyExists(fromTile, toTile, direction) == true)
            {
                LogHelper.Error($"wall already exists. from: {point}, dir: {direction}");
                return false;
            }

            SetWallBetween(fromTile, toTile, direction);

            _wallGoEventHandler.Process(new PlaceWall()
            {
                AccountId = player.AccountId,
                Point = point,
                Direction = direction,
            });

            ChangeState(_currentPlayer, WallGoPlayer.StateType.MovePeice);

            EndTurn();

            return true;
        }
        private void SetWallBetween(Tile fromTile, Tile toTile, Direction direction)
        {
            if (direction == Direction.Up)
            {
                if (fromTile != null)
                {
                    fromTile.WallTop = true;
                }
                if (toTile != null)
                {
                    toTile.WallBottom = true;
                }
            }
            else if (direction == Direction.Down)
            {
                if (fromTile != null)
                {
                    fromTile.WallBottom = true;
                }
                if (toTile != null)
                {
                    toTile.WallTop = true;
                }
            }
            else if (direction == Direction.Left)
            {
                if (fromTile != null)
                {
                    fromTile.WallLeft = true;
                }
                if (toTile != null)
                {
                    toTile.WallRight = true;
                }
            }
            else if (direction == Direction.Right)
            {
                if (fromTile != null)
                {
                    fromTile.WallRight = true;
                }
                if (toTile != null)
                {
                    toTile.WallLeft = true;
                }
            }
        }
        private bool IsWallAlreadyExists(Tile fromTile, Tile toTile, Direction direction)
        {
            var hasWallAtFrom = false;
            var hasWallAtTo = false;
            if (direction == Direction.Up)
            {
                if (fromTile != null)
                {
                    hasWallAtFrom = fromTile.WallTop;
                }

                if (toTile != null)
                {
                    hasWallAtTo = toTile.WallBottom;
                }
            }
            else if (direction == Direction.Left)
            {
                if (fromTile != null)
                {
                    hasWallAtFrom = fromTile.WallLeft;
                }
                if (toTile != null)
                {
                    hasWallAtTo = toTile.WallRight;
                }
            }
            else if (direction == Direction.Right)
            {
                if (fromTile != null)
                {
                    hasWallAtFrom = fromTile.WallRight;
                }
                if (toTile != null)
                {
                    hasWallAtTo = toTile.WallLeft;
                }
            }
            else if (direction == Direction.Down)
            {
                if (fromTile != null)
                {
                    hasWallAtFrom = fromTile.WallBottom;
                }
                if (toTile != null)
                {
                    hasWallAtTo = toTile.WallTop;
                }
            }
            return hasWallAtFrom || hasWallAtTo;
        }

        public bool MovePiece(IPlayer player, int pieceId, Point dest)
        {
            if (_currentPlayer.AccountId != player.AccountId)
            {
                LogHelper.Error($"not player turn. expected: {_currentPlayer.AccountId}");
                return false;
            }
            if (_currentPlayer.PlayerPieces.Count < pieceId)
            {
                LogHelper.Error($"Invalid piece ID: pieceId: {pieceId}");
                return false;
            }

            if (_currentPlayer.State != WallGoPlayer.StateType.MovePeice)
            {
                LogHelper.Error($"current state is not MovePiece. currnet: {_currentPlayer.State}");
                return false;
            }

            if (_currentPlayer.LastMovePiece != null && _currentPlayer.LastMovePiece.Id != pieceId)
            {
                LogHelper.Error($"invalid piece selection. already moved pieceId:{_currentPlayer.LastMovePiece.Id}, tried to move pieceId:{pieceId}.");
                return false;
            }

            var piece = _currentPlayer.PlayerPieces[pieceId];

            Point diff = piece.GridPos - dest;

            int absX = Math.Abs(diff.X);
            int absY = Math.Abs(diff.Y);

            if (!((absX == 1 && absY == 0) || (absX == 0 && absY == 1)))
            {
                LogHelper.Error("only 1 tile movement in straight direction is allowed.");
                return false;
            }

            if (IsBlocked(piece.GridPos, dest))
            {
                LogHelper.Error("wall is blocking the movement.");
                return false;
            }
            if (IsTileOccupied(dest))
            {
                LogHelper.Error("another piece is on the destination.");
                return false;
            }

            piece.GridPos = dest;
            _currentPlayer.LastMovePiece = piece;

            _wallGoEventHandler.Process(new MovePiece()
            {
                AccountId = player.AccountId,
                PieceId = piece.Id,
                Dest = dest
            });

            _currentPlayer.MovePieceCount--;

            if (_currentPlayer.MovePieceCount == 0)
            {
                ChangeState(_currentPlayer, WallGoPlayer.StateType.PlaceWall);
            }

            return true;
        }
        private bool IsTileOccupied(Point point)
        {
            foreach (var player in _players)
            {
                foreach (var piece in player.PlayerPieces)
                {
                    if (piece.Spawned && piece.GridPos == point)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Tile GetTile(Point point)
        {
            return _tiles[point.X, point.Y];
        }
        private bool IsBlocked(Point from, Point to)
        {
            var fromTile = GetTile(from);
            var toTile = GetTile(to);
            var diff = to - from;

            if (diff == Point.Up)
            {
                return fromTile.WallTop || toTile.WallBottom;
            }

            if (diff == Point.Down)
            {
                return fromTile.WallBottom || toTile.WallTop;
            }

            if (diff == Point.Left)
            {
                return fromTile.WallLeft || toTile.WallRight;
            }

            if (diff == Point.Right)
            {
                return fromTile.WallRight || toTile.WallLeft;
            }
            return true;
        }
    }
}

