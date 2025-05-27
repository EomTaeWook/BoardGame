using Assets.Scripts.Extensions;
using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Assets.Scripts.Network;
using Assets.Scripts.Scene.WallGo.EventHandler;
using Assets.Scripts.Scene.WallGo.UI;
using Assets.Scripts.Service;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Unity.Extensions;
using Dignus.Unity.Framework;
using Protocol.GSAndClient;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.WallGo
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class WallGoSceneController : SceneControllerBase<WallGoScene, WallGoSceneModel>
    {
        private readonly WallGoEventHandler _wallGoEventHandler;
        private readonly BoardGo _boardGo;
        private readonly HorizontalLayoutGroup _horizontalLayoutGroup;

        private PlayerInfoGo _previousPlayer;
        private PlayerInfoGo _currentrTurnPlayer;

        private readonly Dictionary<string, PlayerInfoGo> _playerInfos = new();

        private Camera _mainCamera;

        private readonly Color[] _colors = new Color[]
{
            new Color32(43, 218, 207, 255),// 민트
            new Color32(245, 166, 35, 255),// 주황
            new Color32(155, 89, 182, 255),// 보라
            new Color32(255, 229, 143, 255)// 노랑
        };

        private readonly GameClientService _gameClientService;

        private readonly List<DraggablePieceGo> _unspawnedPieces = new();
        public WallGoSceneController(WallGoEventHandler wallGoEventHandler,
            GameClientService gameClientService,
            BoardGo boardGo,
            HorizontalLayoutGroup horizontalLayoutGroup)
        {
            _gameClientService = gameClientService;
            _boardGo = boardGo;
            _wallGoEventHandler = wallGoEventHandler;
            _horizontalLayoutGroup = horizontalLayoutGroup;
        }
        public void OnAwake()
        {
            _mainCamera = Camera.main;
            var index = 0;
            foreach (var player in Model.PlayersToMap.Values)
            {
                var playerInfo = _horizontalLayoutGroup.InstantiateWithPool<PlayerInfoGo>();
                var color = _colors[index];
                playerInfo.Init(this, color, player);
                _playerInfos.Add(player.AccountId, playerInfo);

                if (player.AccountId == Model.CurrentPlayer.AccountId)
                {
                    for (int i = 0; i < player.PlayerPieces.Count; ++i)
                    {
                        var pieceGo = _boardGo.InstantiateWithPool<DraggablePieceGo>();
                        pieceGo.Init(this, player.PlayerPieces[i], color);
                        pieceGo.transform.position = new Vector2(i, -1);
                        _unspawnedPieces.Add(pieceGo);
                    }
                }
                index++;
            }
        }

        private void ProcessGameEvent()
        {
            while (_wallGoEventHandler.WallGoEvents.Count > 0)
            {
                var evt = _wallGoEventHandler.WallGoEvents.Read();

                if (evt is StartGame startGame)
                {
                    UIManager.Instance.ShowToastAlert("게임 시작", 1.5F);
                }
                else if (evt is EndGame endGame)
                {
                    StringBuilder sb = new();
                    int rank = 1;
                    foreach (var item in endGame.ScoreModels)
                    {
                        var nickname = Model.PlayersToMap[item.AccountId].Nickname;

                        sb.AppendLine($"rank {rank} : {nickname} score : {item.Score}");
                        rank++;
                    }

                    UIManager.Instance.ShowAlert("게임 종료", $"Score : {sb}");
                }
                else if (evt is StartTurn startTurn)
                {
                    if (startTurn.AccountId.Equals(Model.CurrentPlayer.AccountId))
                    {
                        UIManager.Instance.ShowToastAlert("턴", 1.5F);
                    }
                    _previousPlayer = _currentrTurnPlayer;
                    _currentrTurnPlayer = _playerInfos[startTurn.AccountId];

                    if (_previousPlayer != null)
                    {
                        _previousPlayer.WallGoPlayer.EndTurn();
                        _previousPlayer.SetTurnActive(false);
                    }
                    _currentrTurnPlayer.WallGoPlayer.StartTurn();
                    _currentrTurnPlayer.SetTurnActive(true);
                }
                else if (evt is ChangeState changeState)
                {
                    var playerInfo = _playerInfos[changeState.AccountId];
                    playerInfo.WallGoPlayer.ChangeState(changeState.UpdateStateType);
                    playerInfo.RefreshUI();
                }
                else if (evt is SpawnPiece spawnPiece)
                {
                    var playerInfo = _playerInfos[spawnPiece.AccountId];

                    var wallGoPlayer = playerInfo.WallGoPlayer;

                    var isPlayerPiece = wallGoPlayer.AccountId == Model.CurrentPlayer.AccountId;

                    var piece = wallGoPlayer.PlayerPieces[spawnPiece.PieceId];

                    piece.SetSpawn(spawnPiece.SpawnedPoint);

                    _boardGo.SpawnPiece(wallGoPlayer.AccountId,
                        playerInfo.PlayerColor,
                        piece,
                        isPlayerPiece);


                    if (isPlayerPiece)
                    {
                        foreach (var item in _unspawnedPieces)
                        {
                            if (item.Piece.Id == spawnPiece.PieceId)
                            {
                                item.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else if (evt is MovePiece movePiece)
                {
                    var playerInfo = _playerInfos[movePiece.AccountId];

                    if (IsPlayer(movePiece.AccountId))
                    {
                        ShowPlaceableWalls(false);
                    }

                    _boardGo.MovePiece(movePiece);

                    foreach (var tileGo in Model.MoveAvailableTiles)
                    {
                        tileGo.SetMoveAvailable(false);
                    }
                    Model.MoveAvailableTiles.Clear();

                    playerInfo.WallGoPlayer.MovePieceCount++;

                    playerInfo.RefreshUI();


                    if (IsPlayer(movePiece.AccountId))
                    {
                        if (playerInfo.WallGoPlayer.MovePieceCount < 2)
                        {
                            ShowPlaceableWalls(true);
                            ShowMoveAvailableTiles();
                        }
                        else
                        {
                            foreach (var tileGo in Model.MoveAvailableTiles)
                            {
                                tileGo.SetMoveAvailable(false);
                            }
                            Model.MoveAvailableTiles.Clear();

                            ShowPlaceableWalls(true);
                        }
                    }
                }
                else if (evt is PlaceWall placeWall)
                {
                    var playerInfo = _playerInfos[placeWall.AccountId];
                    _boardGo.PlaceWall(playerInfo.PlayerColor, placeWall.Point, placeWall.Direction);

                    if (IsPlayer(placeWall.AccountId))
                    {
                        foreach (var tileGo in Model.MoveAvailableTiles)
                        {
                            tileGo.SetMoveAvailable(false);
                        }
                        Model.MoveAvailableTiles.Clear();
                        Model.SelectedPiece.SetActive(false);
                        Model.SelectedPiece = null;
                    }
                    playerInfo.WallGoPlayer.EndTurn();
                    playerInfo.RefreshUI();
                }
                else if (evt is RemoveWall removeWall)
                {
                    if (IsPlayer(removeWall.AccountId))
                    {
                        Scene.InactiveRemoveWall();
                    }
                    _boardGo.RemoveWall(removeWall.Point, removeWall.Direction);
                }
                else
                {
                    LogHelper.Error($"unknown event: {evt.GetType().Name}");
                }
            }
        }

        public bool IsPlayerTurn()
        {
            return _currentrTurnPlayer.WallGoPlayer.AccountId == Model.CurrentPlayer.AccountId;
        }

        public bool IsPlayer(string accountId)
        {
            return Model.CurrentPlayer.AccountId == accountId;
        }

        private void ShowPlaceableWalls(bool value)
        {
            var pos = Model.SelectedPiece.GetPiece().GridPosition;

            var tileGo = _boardGo.GetTileObject(pos);

            if (tileGo.Tile.WallBottom == false)
            {
                tileGo.SetWallAvailable(value, Direction.Down);
            }
            if (tileGo.Tile.WallLeft == false)
            {
                tileGo.SetWallAvailable(value, Direction.Left);
            }
            if (tileGo.Tile.WallRight == false)
            {
                tileGo.SetWallAvailable(value, Direction.Right);
            }
            if (tileGo.Tile.WallTop == false)
            {
                tileGo.SetWallAvailable(value, Direction.Up);
            }
        }
        private void ProcessPieceClick()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                if (EventSystem.current.IsPointerOverGameObject(Mouse.current.deviceId))
                {
                    return;
                }

                if (IsPlayer(_currentrTurnPlayer.WallGoPlayer.AccountId) == false)
                {
                    return;
                }

                var playerInfo = _playerInfos[Model.CurrentPlayer.AccountId];

                if (playerInfo.WallGoPlayer.State == StateType.RemoveWall)
                {
                    return;
                }
                else if (playerInfo.WallGoPlayer.State == StateType.SpawnPiece)
                {
                    return;
                }

                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));

                int layerMask = LayerMask.GetMask("PieceLayer");

                var hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, layerMask);
                if (hit.collider != null)
                {
                    var selectPieceGo = hit.collider.GetComponent<PieceGo>();

                    if (IsPlayer(selectPieceGo.GetPiece().Owner.AccountId) == false)
                    {
                        return;
                    }

                    if (Model.SelectedPiece != null)
                    {
                        var previousPieceId = Model.SelectedPiece.GetPiece().Id;
                        if (selectPieceGo.GetPiece().Id != previousPieceId && _currentrTurnPlayer.WallGoPlayer.MovePieceCount > 0)
                        {
                            return;
                        }

                        Model.SelectedPiece.SetActive(false);
                    }

                    Model.SelectedPiece = selectPieceGo;
                    Model.SelectedPiece.SetActive(true);
                    ShowMoveAvailableTiles();
                }
            }
        }
        private void ProcessTileClick()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                if (EventSystem.current.IsPointerOverGameObject(Mouse.current.deviceId))
                {
                    return;
                }

                if (IsPlayer(_currentrTurnPlayer.WallGoPlayer.AccountId) == false)
                {
                    return;
                }

                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));

                int layerMask = LayerMask.GetMask("TileLayer");

                var hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, layerMask);

                if (hit.collider != null)
                {
                    var selectTileGos = hit.collider.GetComponentsInParent<TileGo>();

                    if (_currentrTurnPlayer.WallGoPlayer.State == StateType.RemoveWall)
                    {
                        ProcessSelectRemoveWall(selectTileGos, hit.collider);
                        return;
                    }

                    if (Model.SelectedPiece == null)
                    {
                        return;
                    }

                    foreach (var selectTileGo in selectTileGos)
                    {
                        if (selectTileGo.IsAvailable())
                        {
                            if (selectTileGo.TryGetWallDirection(hit.collider, out var direction) == true)
                            {
                                PlaceWallRequest(selectTileGo.Tile.GridPosition, direction);
                                ShowPlaceableWalls(false);
                            }
                            else
                            {
                                MovePieceRequest(selectTileGo.Tile.GridPosition, Model.SelectedPiece.GetPiece());
                            }
                        }
                        else if (selectTileGo.Tile.GridPosition == Model.SelectedPiece.GetPiece().GridPosition)
                        {
                            if (selectTileGo.TryGetWallDirection(hit.collider, out var direction) == true)
                            {
                                PlaceWallRequest(selectTileGo.Tile.GridPosition, direction);
                                ShowPlaceableWalls(false);
                            }
                        }
                    }
                }
            }
        }

        public void ProcessSelectRemoveWall(TileGo[] tileGos, Collider2D collider)
        {
            foreach (var selectTileGo in tileGos)
            {
                if (selectTileGo.TryGetWallDirection(collider, out var direction) == true)
                {
                    if (selectTileGo.Tile.HasWall(direction) == false)
                    {
                        continue;
                    }
                    var playerInfo = _playerInfos[Model.CurrentPlayer.AccountId];
                    playerInfo.WallGoPlayer.HasUsedBreakWall = true;
                    var packet = Packet.MakePacket(WallGoCommandProtocol.RemoveWall, new RemoveWallReqeust()
                    {
                        TilePointX = selectTileGo.Tile.GridPosition.X,
                        TilePointY = selectTileGo.Tile.GridPosition.Y,
                        Direction = (int)direction
                    });
                    _gameClientService.Send(packet);
                    return;
                }
            }
        }

        public PlayerInfoGo GetCurrentPlayer()
        {
            return _playerInfos[Model.CurrentPlayer.AccountId];
        }

        private void PlaceWallRequest(Point point, Direction direction)
        {
            var packet = Packet.MakePacket(WallGoCommandProtocol.PlaceWall, new PlaceWallReqeust()
            {
                TilePointX = point.X,
                TilePointY = point.Y,
                Direction = (int)direction
            });
            _gameClientService.Send(packet);
        }

        private void MovePieceRequest(Point point, Piece piece)
        {
            var packet = Packet.MakePacket(WallGoCommandProtocol.MovePiece, new MovePieceReqeust()
            {
                PieceId = piece.Id,
                MovePointX = point.X,
                MovePointY = point.Y
            });
            _gameClientService.Send(packet);
        }
        private void ShowMoveAvailableTiles()
        {
            foreach (var tileGo in Model.MoveAvailableTiles)
            {
                tileGo.SetMoveAvailable(false);
            }
            Model.MoveAvailableTiles.Clear();

            var pos = Model.SelectedPiece.GetPiece().GridPosition;

            var up = pos + Point.Up;
            var left = pos + Point.Left;
            var right = pos + Point.Right;
            var down = pos + Point.Down;

            var upTile = _boardGo.GetTileObject(up);
            var leftTile = _boardGo.GetTileObject(left);
            var rightTile = _boardGo.GetTileObject(right);
            var downTile = _boardGo.GetTileObject(down);


            if (upTile != null && upTile.Tile.WallBottom == false)
            {
                Model.MoveAvailableTiles.Add(upTile);
            }

            if (leftTile != null && leftTile.Tile.WallRight == false)
            {
                Model.MoveAvailableTiles.Add(leftTile);
            }

            if (rightTile != null && rightTile.Tile.WallLeft == false)
            {
                Model.MoveAvailableTiles.Add(rightTile);
            }

            if (downTile != null && downTile.Tile.WallTop == false)
            {
                Model.MoveAvailableTiles.Add(downTile);
            }

            List<PieceGo> pieces = _boardGo.GetPieces();

            for (int i = 0; i < Model.MoveAvailableTiles.Count; ++i)
            {
                var tile = Model.MoveAvailableTiles[i];

                var isMoveAvailable = true;
                foreach (var piece in pieces)
                {
                    if (tile.Tile.GridPosition == piece.GetPiece().GridPosition)
                    {
                        isMoveAvailable = false;
                        break;
                    }
                }
                Model.MoveAvailableTiles[i].SetMoveAvailable(isMoveAvailable);
            }
        }
        public void Update()
        {
            ProcessPieceClick();
            ProcessTileClick();
            ProcessGameEvent();
        }
        public override void Dispose()
        {
            foreach (var item in _unspawnedPieces)
            {
                item.Recycle();
            }
            _unspawnedPieces.Clear();

            _boardGo.Dispose();
            foreach (var kv in _playerInfos)
            {
                kv.Value.Recycle();
            }
            _playerInfos.Clear();
        }

        public void SpawnPieceRequest(DraggablePieceGo piece, Point tilePoint)
        {
            var packet = Packet.MakePacket(WallGoCommandProtocol.SpawnPiece, new SpawnPieceReqeust()
            {
                PieceId = piece.Piece.Id,
                SpawnedPointX = tilePoint.X,
                SpawnedPointY = tilePoint.Y,
            });
            _gameClientService.Send(packet);

            piece.gameObject.SetActive(false);
        }
    }
}

