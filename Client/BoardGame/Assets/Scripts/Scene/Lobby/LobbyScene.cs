using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Lobby.UI;
using Dignus.Unity.Framework;

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

        public void CreateRoomUI()
        {
            _roomUI = UIManager.Instance.AddUI<RoomUI>();
            _roomUI.Init(SceneController, CloseRoomUI);
        }

        public void RoomUIRefresh()
        {
            if (_roomUI != null)
            {
                _roomUI.RefreshUI();
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
        protected override void OnDestroyScene()
        {
            if (_lobbyUI != null)
            {
                UIManager.Instance.RemoveUI(_lobbyUI);
                _lobbyUI = null;
            }

            CloseCreateRoomUI();
            CloseJoinRoomUI();
        }
    }
}
