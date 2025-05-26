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

        private WallGoPlayer _prevPlayer;
        private WallGoPlayer _currentPlayer;

        private readonly Dictionary<string, PlayerInfoGo> _playerInfos = new();

        private Camera _mainCamera;

        private readonly Color[] _colors = new Color[]
{
            new Color32(43, 218, 207, 255),// ¹ÎÆ®
            new Color32(245, 166, 35, 255),// ÁÖÈ²
            new Color32(155, 89, 182, 255),// º¸¶ó
            new Color32(255, 229, 143, 255)// ³ë¶û
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
                    UIManager.Instance.ShowToastAlert("game start.", 1);
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

                    UIManager.Instance.ShowAlert("Game End", $"Score : {sb.ToString()}");
                }
                else if (evt is StartTurn startTurn)
                {
                    if (startTurn.AccountId.Equals(Model.CurrentPlayer.AccountId))
                    {
                        UIManager.Instance.ShowToastAlert("you turn.", 1.5F);
                    }
                    _prevPlayer = _currentPlayer;
                    _currentPlayer = Model.PlayersToMap[startTurn.AccountId];

                    if (_prevPlayer != null)
                    {
                        _prevPlayer.EndTurn();
                    }
                }
                else if (evt is ChangeState changeState)
                {

                }
                else if (evt is SpawnPiece spawnPiece)
                {
                    var playerInfo = _playerInfos[spawnPiece.AccountId];

                    var wallGoPlayer = playerInfo.WallGoPlayer;

                    var piece = wallGoPlayer.PlayerPieces[spawnPiece.PieceId];

                    piece.SetSpawn(spawnPiece.SpawnedPoint);

                    _boardGo.SpawnPiece(wallGoPlayer.AccountId,
                        playerInfo.PlayerColor,
                        piece,
                        wallGoPlayer.AccountId == Model.CurrentPlayer.AccountId);
                }
                else if (evt is MovePiece movePiece)
                {
                    Model.PlayersToMap[movePiece.AccountId].MovePieceCount--;
                }
                else if (evt is PlaceWall placeWall)
                {

                }
                else
                {
                    LogHelper.Error($"unknown event: {evt.GetType().Name}");
                }
            }
        }
        private void ProcessInput()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);

                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                var hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider != null)
                {

                }
            }
        }
        public void Update()
        {
            ProcessInput();
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
            var packet = Packet.MakePacket(WallGoCommandProtocol.SpawnPiece, new SpawnPeiceReqeust()
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

