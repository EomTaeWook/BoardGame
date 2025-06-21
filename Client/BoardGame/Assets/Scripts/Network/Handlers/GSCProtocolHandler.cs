using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Dignus.Unity;
using Dignus.Unity.DependencyInjection;
using Newtonsoft.Json;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;

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

        public void Ping(Ping ping)
        {
            _session.TrySend(Packet.MakePacket(CGSProtocol.Pong, new Pong()));
        }

        public void LoginResponse(LoginResponse loginResponse)
        {
            UnityMainThread.Add(() =>
            {
                if (loginResponse.LoginReason == LoginReason.AlreadyLogin)
                {
                    UIManager.Instance.ShowAlert(StringHelper.GetString(1001), StringHelper.GetString(1000));
                    DignusUnitySceneManager.Instance.LoadScene<TitleScene>(SceneType.TitleScene);
                    return;
                }
                DignusUnitySceneManager.Instance.LoadScene<LobbyScene>(SceneType.LobbyScene);
            });
        }
        public void LeaveRoomResponse(LeaveRoomResponse leaveRoomResponse)
        {
            UnityMainThread.Add(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();
                controller.LeaveRoom(leaveRoomResponse);
            });
        }
        public void GetRoomListResponse(GetRoomListResponse getRoomListResponse)
        {
            UnityMainThread.Add(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();
                controller.RoomList(getRoomListResponse);
            });
        }
        public void CreateRoomResponse(CreateRoomResponse createRoomResponse)
        {
            UnityMainThread.Add(() =>
            {
                if (createRoomResponse.Ok == false)
                {
                    UIManager.Instance.ShowAlert(StringHelper.GetString(1001), StringHelper.GetString(1002));
                    return;
                }


                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();

                controller.CreateRoom(createRoomResponse);
            });
        }
        public void JoinRoomResponse(JoinRoomResponse joinRoomResponse)
        {
            UnityMainThread.Add(() =>
            {
                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();

                if (joinRoomResponse.FailedJoinRoomReason == JoinRoomReason.NotFound)
                {
                    UIManager.Instance.ShowAlert(StringHelper.GetString(1001), StringHelper.GetString(1003));
                    return;
                }
                else if (joinRoomResponse.FailedJoinRoomReason == JoinRoomReason.IsFull)
                {
                    UIManager.Instance.ShowAlert(StringHelper.GetString(1001), StringHelper.GetString(1004));
                    return;
                }
                controller.JoinRoom(joinRoomResponse);
            });
        }
        public void StartGameRoomResponse(StartGameRoomResponse startGameRoomResponse)
        {
            UnityMainThread.Add(() =>
            {
                if (startGameRoomResponse.StartGameRoomReason == StartGameRoomReason.NotEnoughUser)
                {
                    UIManager.Instance.ShowAlert(StringHelper.GetString(1001), StringHelper.GetString(1005));
                    return;
                }

                var controller = DignusUnityServiceContainer.Resolve<LobbySceneController>();
                controller.StartGameRoom(startGameRoomResponse);
            });
        }
    }
}
