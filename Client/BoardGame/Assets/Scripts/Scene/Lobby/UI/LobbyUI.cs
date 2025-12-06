using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Coroutine;
using Dignus.Unity.Attributes;
using Dignus.Unity.Coroutine;
using Dignus.Unity.Extensions;
using Protocol.GSAndClient.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.Lobby.UI
{
    [PrefabPath(Consts.Path.Lobby)]
    public class LobbyUI : UIItem
    {
        [SerializeField]
        private TextMeshProUGUI _nicknameText;

        [SerializeField]
        private TextMeshProUGUI _createRoomText;
        [SerializeField]
        private TextMeshProUGUI _joinRoomText;
        [SerializeField]
        private TextMeshProUGUI _refreshButtonText;
        [SerializeField]
        private TextMeshProUGUI _roomListText;

        [SerializeField]
        private VerticalLayoutGroup _verticalLayoutGroup;

        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private int _itemSize = 10;

        private LobbySceneController _lobbySceneController;

        public void Init(LobbySceneController lobbySceneController)
        {
            _lobbySceneController = lobbySceneController;
            _nicknameText.text = _lobbySceneController.Model.CurrentPlayer.Nickname;

            _createRoomText.text = StringHelper.GetString(1016);
            _joinRoomText.text = StringHelper.GetString(1017);
            _roomListText.text = StringHelper.GetString(1028);
            _refreshButtonText.text = StringHelper.GetString(1029);

            _lobbySceneController.Model.RemoveGameRoom.ValueChanged += RemoveGameRoom_ValueChanged;

            _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            DignusUnityCoroutineManager.Start(AutoRefreshLobbyRooms());
        }

        private IEnumerator AutoRefreshLobbyRooms()
        {
            while(true)
            {
                yield return new DelayInSeconds(5);

                int currentPageIndex = GetCurrentPageIndex();

                _lobbySceneController.RequestRoomList(currentPageIndex, _itemSize);
            }
        }

        private void OnScrollValueChanged(Vector2 vector2)
        {
            int currentPageIndex = GetCurrentPageIndex();

            int preloadOffset = 2;

            int loadedPageCount = _lobbySceneController.Model.LobbyRoomInfos.Count / _itemSize;

            if (currentPageIndex >= loadedPageCount - preloadOffset)
            {
                _lobbySceneController.RequestRoomList(currentPageIndex, _itemSize);
            }
        }
        public override void DisposeUI()
        {
            _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            base.DisposeUI();
        }
        private void RemoveGameRoom_ValueChanged(int value)
        {
            if(value == -1)
            {
                return;
            }

            if(_lobbySceneController.Model.LobbyRoomInfos.TryGetValue(value, out RoomListItemUI roomListItemUI))
            {
                roomListItemUI.Recycle();
            }
        }
        public void OnRefreshButtonClick()
        {
            foreach(var item in _lobbySceneController.Model.LobbyRoomInfos)
            {
                item.Value.Recycle();
            }
            _lobbySceneController.Model.LobbyRoomInfos.Clear();

            _lobbySceneController.RequestRoomList(0, _itemSize);
        }

        private int GetCurrentPageIndex()
        {
            //10°³ = 1065
            var pageIndex = _scrollRect.content.anchoredPosition.y / (_itemSize * 100F);
            return (int)pageIndex;
        }

        public void RefreshRoomUI(List<RoomInfo> roomInfos)
        {
            foreach (var item in roomInfos)
            {
                if(_lobbySceneController.Model.LobbyRoomInfos.ContainsKey(item.RoomId))
                {
                    continue;
                }

                var roomInfoGo = _verticalLayoutGroup.InstantiateWithPool<RoomListItemUI>();
                _lobbySceneController.Model.LobbyRoomInfos[item.RoomId] = roomInfoGo;
                roomInfoGo.Init(_lobbySceneController, item);
                roomInfoGo.RefreshUI();
            }
        }
        public void OnCreateRoomUIButtonClick()
        {
            _lobbySceneController.Scene.CreateCreateRoomUI();
        }

        public void OnJoinRoomUIButtionClick()
        {
            _lobbySceneController.Scene.CreateJoinRoomUI();
        }
    }
}
