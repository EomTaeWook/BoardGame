using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Network;
using Assets.Scripts.Service;
using Dignus.DependencyInjection.Attributes;
using Dignus.Unity.Framework;
using Protocol.GSAndClient;

namespace Assets.Scripts.Scene.Title
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class TitleSceneController : SceneControllerBase<TitleScene>
    {
        private readonly UserService _userService;
        private readonly GameClientService _gameClientService;
        public TitleSceneController(UserService userService,
            GameClientService gameClientService)
        {
            _userService = userService;
            _gameClientService = gameClientService;
        }

        public void Init()
        {
            ApplicationManager.Instance.OnSceneLoadCompleted(SceneType.TitleScene.ToString());

            if (_userService.Load() == false)
            {
                Scene.ShowCreateAccountUI();
                return;
            }

            ProcessLogin();
        }

        public void ProcessLogin()
        {
            if (Scene.GetBuildTargetType() == Internals.BuildTaretType.Dev)
            {
                _gameClientService.SetIpStringAndPort("20.196.65.193", 40000);
            }
            else if (Scene.GetBuildTargetType() == BuildTaretType.Live)
            {
                _gameClientService.SetIpStringAndPort("20.196.65.193", 40000);
            }
            else if (Scene.GetBuildTargetType() == BuildTaretType.Local)
            {
                _gameClientService.SetIpStringAndPort("127.0.0.1", 20000);
            }

            _gameClientService.Connect();
            if (_gameClientService.IsConnect() == false)
            {
                UIManager.Instance.ShowAlert(StringHelper.GetString(1001),
                    StringHelper.GetString(1006));
                return;
            }

            var userModel = _userService.GetUserModel();

            _gameClientService.Send(Packet.MakePacket(Protocol.GSAndClient.CGSProtocol.Login, new Login()
            {
                AccountId = userModel.AccountId,
                Nickname = userModel.Nickname
            }));
        }
        public void CreateAccount(string nickname)
        {
            var accountId = _userService.CreateAccountId();

            _userService.SetUserModel(new UserModel()
            {
                AccountId = accountId,
                Nickname = nickname
            });
            _userService.SaveData();
        }

        public override void Dispose()
        {
        }
    }
}
