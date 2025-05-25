using Assets.Scripts.Internals;
using Assets.Scripts.Network;
using Assets.Scripts.Service;
using Dignus.DependencyInjection.Attributes;
using Dignus.Unity.Framework;
using Protocol.GSAndClient;

namespace Assets.Scripts.Scene.Title
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class LobbySceneController : SceneControllerBase<LobbyScene, LobbySceneModel>
    {
        private readonly GameClientService _gameClientService;
        private readonly UserService _userService;
        public LobbySceneController(GameClientService gameClientService,
            UserService userService)
        {
            _userService = userService;
            _gameClientService = gameClientService;
        }
        public void OnAwake()
        {
            Model.CurrentPlayer = new GamePlayer()
            {
                AccountId = _userService.GetUserModel().AccountId,
                Nickname = _userService.GetUserModel().Nickname
            };
        }
        public void CreateRoomReqeust(CreateRoom createRoom)
        {
            _gameClientService.Send(Packet.MakePacket(CGSProtocol.CreateRoom, createRoom));
        }
        public void JoinRoomRequest(long roomNumber)
        {
            _gameClientService.Send(Packet.MakePacket(CGSProtocol.JoinRoom, new JoinRoom()
            {
                RoomNumber = roomNumber
            }));
        }
        public void StartGameRoomRequest()
        {
            _gameClientService.Send(Packet.MakePacket(CGSProtocol.StartGameRoom, new StartGameRoom()));
        }

        public void JoinRoom(JoinRoomResponse joinRoomResponse)
        {
            if (joinRoomResponse.Ok == false)
            {
                UIManager.Instance.ShowAlert("alert", "failed to join room");
                return;
            }

            Model.RoomMembers = joinRoomResponse.Members;

            Scene.RoomUIRefresh();
        }
        public void LeaveRoom(LeaveRoomResponse leaveRoomResponse)
        {
            Model.RoomMembers = leaveRoomResponse.Members;

            Scene.RoomUIRefresh();
        }

        public override void Dispose()
        {
        }
    }
}
