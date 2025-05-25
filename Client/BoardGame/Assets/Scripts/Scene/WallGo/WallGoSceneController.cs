using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.WallGo.EventHandler;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Unity.Framework;

namespace Assets.Scripts.Scene.WallGo
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class WallGoSceneController : SceneControllerBase<WallGoScene, WallGoSceneModel>
    {
        private readonly WallGoEventHandler _wallGoEventHandler;
        private readonly BoardGo _boardGo;
        public WallGoSceneController(WallGoEventHandler wallGoEventHandler,
            BoardGo boardGo)
        {
            _boardGo = boardGo;
            _wallGoEventHandler = wallGoEventHandler;
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
                    foreach (var player in Model.Players)
                    {
                        if (player.AccountId == endGame.AccountId)
                        {
                            UIManager.Instance.ShowAlert("GameEnd", $"Winner : {player.Nickname}");
                            break;
                        }
                    }
                }
                else if (evt is StartTurn startTurn)
                {
                    if (startTurn.AccountId.Equals(Model.CurrentPlayer.AccountId))
                    {
                        UIManager.Instance.ShowToastAlert("you turn.", 1);
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
        }
    }
}

