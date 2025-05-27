using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo.EventHandler;
using Dignus.Collections;
using Dignus.Coroutine;
using Dignus.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Scripts.GameContents.WallGo
{
    public class WallGoBoard : IBoardGame
    {
        //0,0으로 유니티 상으론 좌측 하단
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
                        GridPosition = new Point(i, ii)
                    };
                }
            }
        }
        public IPlayer GetPlayer(string accountId)
        {
            foreach(var player in _players)
            {
                if(accountId == player.AccountId)
                {
                    return player;
                }
            }
            return null;
        }
        public void SetPlayers(ICollection<IPlayer> players)
        {
            _players.Clear();
            foreach (var item in players)
            {
                _players.Add(new WallGoPlayer(item.AccountId, item.Nickname));
            }
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
        private List<Point> FloodFill(Point start, bool[,] visited)
        {
            var result = new List<Point>();
            var queue = new Queue<Point>();
            queue.Enqueue(start);
            visited[start.X, start.Y] = true;

            var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                result.Add(curr);

                foreach (var dir in directions)
                {
                    var next = GetNeighborPoint(curr, dir);

                    if (!IsInsideBoard(next) || visited[next.X, next.Y])
                    {
                        continue;
                    }

                    if (IsBlocked(curr, next))
                    {
                        continue;
                    }

                    visited[next.X, next.Y] = true;
                    queue.Enqueue(next);
                }
            }
            return result;
        }
        public bool IsEndGame()
        {
            var visited = new bool[_tiles.GetLength(0), _tiles.GetLength(1)];

            var piecePositions = new List<Point>();
            foreach (var player in _players)
            {
                foreach (var piece in player.PlayerPieces)
                {
                    piecePositions.Add(piece.GridPosition);
                }
            }
            foreach (var point in piecePositions)
            {
                if (visited[point.X, point.Y])
                {
                    continue;
                }

                var connectedPoints = FloodFill(point, visited);

                var piecesInRegion = 0;

                foreach (var item in piecePositions)
                {
                    if(connectedPoints.Contains(item))
                    {
                        piecesInRegion++;
                    }
                }

                if (piecesInRegion != 1)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<KeyValuePair<IPlayer, int>> GetScore()
        {
            var visited = new bool[_tiles.GetLength(0), _tiles.GetLength(1)];

            var pieces = new List<Piece>();
            Dictionary<IPlayer, int> scores = new Dictionary<IPlayer, int>();
            foreach (var player in _players)
            {
                scores.Add(player, 0);
                foreach (var piece in player.PlayerPieces)
                {
                    pieces.Add(piece);
                }
            }
            
            foreach(var piece in pieces)
            {
                var connectedPoints = FloodFill(piece.GridPosition, visited);
                scores[piece.Owner] += connectedPoints.Count;
            }

            return scores.OrderByDescending(r=>r.Value);
        }
        public void Stop()
        {
            _isRunning = false;
        }
        public void EndGame()
        {
            _isRunning = false;
            _coroutineHandler.StopAll();
            var scores = GetScore();
            var endGame = new EndGame();
            foreach (var item in scores)
            {
                endGame.ScoreModels.Add(new Protocol.GSAndClient.Models.ScoreModel()
                {
                    AccountId = item.Key.AccountId,
                    Score = item.Value
                });
            }
            _wallGoEventHandler.Process(endGame);
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
                    if (_currentPlayer.State == StateType.SpawnPiece ||
                        _currentPlayer.State == StateType.SpawnPiece1)
                    {
                        TryForceSpawn();
                        var currentState = _currentPlayer.State;
                        currentState++;
                        ChangeState(_currentPlayer, currentState);

                        _ = _turnQueue.Read();
                        _currentPlayer = _turnQueue.Peek();
                        StartTurn(_currentPlayer);
                    }
                    else if (_currentPlayer.State == StateType.MovePeice ||
                        _currentPlayer.State == StateType.PlaceWall)
                    {
                        _currentPlayer.ChangeState(StateType.PlaceWall);
                        //EndTurn 까지 호출실패
                        if (TryForcePlaceWall() == false)
                        {
                            _currentPlayer.ChangeState(StateType.MovePeice);
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
        private void ChangeState(WallGoPlayer wallGoPlayer, StateType changeState)
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

            _currentPlayer.MovePieceCount = 1;

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
            if (_currentPlayer.State == StateType.MovePeice ||
                _currentPlayer.State == StateType.PlaceWall)
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

        public bool TryRemoveWall(IPlayer player, Point point, Direction direction)
        {
            if (_currentPlayer.AccountId != player.AccountId)
            {
                LogHelper.Error($"not player turn. expected: {_currentPlayer.AccountId}");
                return false;
            }

            if (_currentPlayer.State != StateType.MovePeice)
            {
                LogHelper.Error($"cannot remove wall. current state: {_currentPlayer.State}");
                return false;
            }

            if (_currentPlayer.HasUsedBreakWall == true)
            {
                LogHelper.Error($"already used break wall remove wall. account id: {_currentPlayer.AccountId}");
                return false;
            }

            Point neighbor = GetNeighborPoint(point, direction);

            Tile fromTile = IsInsideBoard(point) ? GetTile(point) : null;
            Tile toTile = IsInsideBoard(neighbor) ? GetTile(neighbor) : null;

            if (IsWallAlreadyExists(fromTile, toTile, direction) == false)
            {
                LogHelper.Error($"wall not exists. from: {point}, dir: {direction}");
                return false;
            }

            SetWallBetween(fromTile, toTile, direction, false);

            _currentPlayer.HasUsedBreakWall = true;

            _wallGoEventHandler.Process(new RemoveWall()
            {
                AccountId = player.AccountId,
                Point = point,
                Direction = direction,
            });

            return true;
        }
        public bool TryPlaceWall(IPlayer player, Direction direction)
        {
            if (_currentPlayer.AccountId != player.AccountId)
            {
                LogHelper.Error($"not player turn. expected: {_currentPlayer.AccountId}");
                return false;
            }

            if (_currentPlayer.MovePieceCount == 0 && _currentPlayer.State == StateType.MovePeice)
            {
                LogHelper.Error($"wall placement requires at least one move.");
                return false;
            }
            else if (_currentPlayer.State == StateType.SpawnPiece ||
                _currentPlayer.State == StateType.SpawnPiece1 ||
                _currentPlayer.State == StateType.Max)
            {
                LogHelper.Error("cannot place wall. current state: " + _currentPlayer.State);
                return false;
            }
            var lastMovePiece = _currentPlayer.LastMovePiece;

            var point = lastMovePiece.GridPosition;

            Point neighbor = GetNeighborPoint(point, direction);

            Tile fromTile = IsInsideBoard(point) ? GetTile(point) : null;
            Tile toTile = IsInsideBoard(neighbor) ? GetTile(neighbor) : null;

            if (IsWallAlreadyExists(fromTile, toTile, direction) == true)
            {
                LogHelper.Error($"wall already exists. from: {point.X}, {point.Y}, dir: {direction}");
                return false;
            }

            SetWallBetween(fromTile, toTile, direction, true);

            _wallGoEventHandler.Process(new PlaceWall()
            {
                AccountId = player.AccountId,
                Point = fromTile.GridPosition,
                Direction = direction,
            });
            
            ChangeState(_currentPlayer, StateType.MovePeice);

            if(IsEndGame() == true)
            {
                EndGame();
            }
            else
            {
                EndTurn();
            }

            return true;
        }

        private void SetWallBetween(Tile fromTile, Tile toTile, Direction direction, bool value)
        {
            if (direction == Direction.Up)
            {
                if (fromTile != null)
                {
                    fromTile.WallTop = value;
                }
                if (toTile != null)
                {
                    toTile.WallBottom = value;
                }
            }
            else if (direction == Direction.Down)
            {
                if (fromTile != null)
                {
                    fromTile.WallBottom = value;
                }
                if (toTile != null)
                {
                    toTile.WallTop = value;
                }
            }
            else if (direction == Direction.Left)
            {
                if (fromTile != null)
                {
                    fromTile.WallLeft = value;
                }
                if (toTile != null)
                {
                    toTile.WallRight = value;
                }
            }
            else if (direction == Direction.Right)
            {
                if (fromTile != null)
                {
                    fromTile.WallRight = value;
                }
                if (toTile != null)
                {
                    toTile.WallLeft = value;
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

            if (_currentPlayer.State != StateType.MovePeice)
            {
                LogHelper.Error($"current state is not MovePiece. currnet: {_currentPlayer.State}");
                return false;
            }

            if (_currentPlayer.LastMovePiece != null && _currentPlayer.LastMovePiece.Id != pieceId)
            {
                LogHelper.Error($"invalid piece selection. already moved pieceId:{_currentPlayer.LastMovePiece.Id}, tried to move pieceId:{pieceId}.");
                return false;
            }

            if (_currentPlayer.MovePieceCount > 2)
            {
                LogHelper.Error($"Invalid move count: {_currentPlayer.MovePieceCount}, pieceId: {pieceId}");
                return false;
            }

            var piece = _currentPlayer.PlayerPieces[pieceId];

            Point diff = piece.GridPosition - dest;

            int absX = Math.Abs(diff.X);
            int absY = Math.Abs(diff.Y);

            if (!((absX == 1 && absY == 0) || (absX == 0 && absY == 1)))
            {
                LogHelper.Error("only 1 tile movement in straight direction is allowed.");
                return false;
            }

            if (IsBlocked(piece.GridPosition, dest))
            {
                LogHelper.Error("wall is blocking the movement.");
                return false;
            }
            if (IsTileOccupied(dest))
            {
                LogHelper.Error("another piece is on the destination.");
                return false;
            }

            piece.GridPosition = dest;
            _currentPlayer.LastMovePiece = piece;

            _wallGoEventHandler.Process(new MovePiece()
            {
                AccountId = player.AccountId,
                PieceId = piece.Id,
                Dest = dest
            });

            _currentPlayer.MovePieceCount++;

            if (_currentPlayer.MovePieceCount == 2)
            {
                ChangeState(_currentPlayer, StateType.PlaceWall);
            }

            return true;
        }
        private bool IsTileOccupied(Point point)
        {
            foreach (var player in _players)
            {
                foreach (var piece in player.PlayerPieces)
                {
                    if (piece.Spawned && piece.GridPosition == point)
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

