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
        public void JoinRoomResponse(JoinRoomResponse joinRoom)
        {
            UnityMainThread.Run(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();


            });
        }
        public void LobbyRoomListResponse(LobbyRoomListResponse lobbyRoomListResponse)
        {
            UnityMainThread.Run(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();
            });
        }
    }
}
