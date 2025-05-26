using Assets.Scripts.Extensions;
using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.WallGo.EventHandler;
using Assets.Scripts.Scene.WallGo.UI;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Unity.Extensions;
using Dignus.Unity.Framework;
using System.Collections.Generic;
using System.Text;
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

        private readonly List<PlayerInfoGo> _playerInfos = new List<PlayerInfoGo>();
        public WallGoSceneController(WallGoEventHandler wallGoEventHandler,
            BoardGo boardGo,
            HorizontalLayoutGroup horizontalLayoutGroup)
        {
            _boardGo = boardGo;
            _wallGoEventHandler = wallGoEventHandler;
            _horizontalLayoutGroup = horizontalLayoutGroup;
        }
        public void OnAwake()
        {
            var index = 0;
            foreach(var player in Model.PlayersToMap.Values)
            {
                var playerInfo = _horizontalLayoutGroup.InstantiateWithPool<PlayerInfoGo>();

                playerInfo.Init(this, index, player);
                _playerInfos.Add(playerInfo);

                index++;
            }
        }
        public void Update()
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
                    foreach(var item in endGame.ScoreModels)
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
                        UIManager.Instance.ShowToastAlert("you turn.", 1);
                    }
                    _prevPlayer = _currentPlayer;
                    _currentPlayer = Model.PlayersToMap[startTurn.AccountId];

                    if(_prevPlayer != null)
                    {
                        _prevPlayer.EndTurn();
                    }
                }
                else if (evt is ChangeState changeState)
                {

                }
                else if (evt is SpawnPiece spawnPiece)
                {

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
        public override void Dispose()
        {
            foreach(var player in _playerInfos)
            {
                player.Recycle();
            }
            _playerInfos.Clear();
        }
    }
}

