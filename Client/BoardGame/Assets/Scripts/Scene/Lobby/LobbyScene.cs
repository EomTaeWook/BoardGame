using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Lobby.UI;
using Dignus.Unity.Framework;
using NUnit.Framework;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Scene.Title
{
    public class LobbyScene : SceneBase<LobbySceneController>
    {
        private LobbyUI _lobbyUI;

        private CreateRoomUI _createRoomUI;
        private JoinRoomUI _joinRoomUI;
        private RoomUI _roomUI;
        protected override void OnAwakeScene()
        {
            SceneController.OnAwake();

            _lobbyUI = UIManager.Instance.AddUI<LobbyUI>();
            _lobbyUI.Init(SceneController);
        }
        public void CreateCreateRoomUI()
        {
            _createRoomUI = UIManager.Instance.AddUI<CreateRoomUI>();
            _createRoomUI.Init(SceneController, CloseCreateRoomUI);
        }
        public void CreateJoinRoomUI()
        {
            _joinRoomUI = UIManager.Instance.AddUI<JoinRoomUI>();
            _joinRoomUI.Init(SceneController, CloseJoinRoomUI);
        }

        public void CreateRoomUI(int roomNumber)
        {
            if (_roomUI != null)
            {
                return;
            }
            _roomUI = UIManager.Instance.AddUI<RoomUI>();
            _roomUI.Init(SceneController, roomNumber, CloseRoomUI);
        }

        public void RoomUIRefresh()
        {
            if (_roomUI != null)
            {
                _roomUI.RefreshUI();
            }
        }
        public void LobbyGameRoomUIRefresh(int pageIndex, List<RoomInfo> roomInfos)
        {
            if(_lobbyUI != null)
            {
                _lobbyUI.RefreshRoomUI(pageIndex, roomInfos);
            }
        }
        public void CloseRoomUI()
        {
            if (_roomUI != null)
            {
                UIManager.Instance.RemoveUI(_roomUI);
                _roomUI = null;
            }
        }
        public void CloseJoinRoomUI()
        {
            if (_joinRoomUI != null)
            {
                UIManager.Instance.RemoveUI(_joinRoomUI);
                _joinRoomUI = null;
            }
        }
        public void CloseCreateRoomUI()
        {
            if (_createRoomUI != null)
            {
                UIManager.Instance.RemoveUI(_createRoomUI);
                _createRoomUI = null;
            }
        }
        public override void OnDestroyScene()
        {
            if (_lobbyUI != null)
            {
                UIManager.Instance.RemoveUI(_lobbyUI);
                _lobbyUI = null;
            }

            CloseCreateRoomUI();
            CloseJoinRoomUI();
            CloseRoomUI();

            SceneController.Dispose();
        }
    }
}
