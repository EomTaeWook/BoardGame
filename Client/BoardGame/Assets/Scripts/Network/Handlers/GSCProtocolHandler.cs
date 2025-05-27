using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Dignus.Unity;
using Dignus.Unity.DependencyInjection;
using Newtonsoft.Json;
using Protocol.GSAndClient;

namespace Assets.Scripts.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class GSCProtocolHandler : IProtocolHandler<string>, ISessionComponent
    {
        private ISession _session;
        public T DeserializeBody<T>(string body)
        {
            return JsonConvert.DeserializeObject<T>(body);
        }

        public void Dispose()
        {
            _session = null;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }

        public void LoginResponse(LoginResponse loginResponse)
        {
            UnityMainThread.Run(() =>
            {
                if (loginResponse.Ok == false)
                {
                    UIManager.Instance.ShowAlert("Alert", "failed to login");
                    DignusUnitySceneManager.Instance.LoadScene<TitleScene>(SceneType.TitleScene);
                    return;
                }
                DignusUnitySceneManager.Instance.LoadScene<LobbyScene>(SceneType.LobbyScene);
            });
        }
        public void LeaveRoomResponse(LeaveRoomResponse leaveRoomResponse)
        {
            UnityMainThread.Run(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();
                controller.LeaveRoom(leaveRoomResponse);
            });
        }
        public void CreateRoomResponse(CreateRoomResponse createRoomResponse)
        {
            UnityMainThread.Run(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();

                controller.CreateRoom(createRoomResponse);
            });
        }
        public void JoinRoomResponse(JoinRoomResponse joinRoomResponse)
        {
            UnityMainThread.Run(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();

                controller.JoinRoom(joinRoomResponse);
            });
        }
        public void StartGameRoomResponse(StartGameRoomResponse startGameRoomResponse)
        {
            UnityMainThread.Run(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();
                controller.StartGameRoom(startGameRoomResponse);
            });
        }
    }
}
