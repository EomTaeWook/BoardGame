using Assets.Scripts.Network;
using Assets.Scripts.Service;
using Dignus.DependencyInjection.Attributes;
using Dignus.Unity.Framework;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Scene.Title
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class LobbySceneController : SceneControllerBase<TitleScene>
    {
        private readonly GameClientService _gameClientService;

        public LobbySceneController(GameClientService gameClientService)
        {
            _gameClientService = gameClientService;
        }
        public void RoomInfoReqeust()
        {
            _gameClientService.Send(Packet.MakePacket(CGSProtocol.LobbyRoomList, new LobbyRoomList()
            {
                PageIndex = 0
            }));
        }
        public void SetRoomInfos(List<RoomInfo> roomInfos)
        {

        }
        public override void Dispose()
        {
        }
    }
}
