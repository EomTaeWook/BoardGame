using Assets.Scripts.Extensions;
using Assets.Scripts.Internals;
using Assets.Scripts.Scene.Title;
using Dignus.Collections;
using Dignus.Coroutine;
using Dignus.Unity.Attributes;
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

        private bool _isLoading = false;

        private LobbySceneController _lobbySceneController;

        private int _lastRequestedPageIndex = -1;


        private CoroutineHandler _coroutineHandler = new CoroutineHandler();

        public void Init(LobbySceneController lobbySceneController)
        {
            _lobbySceneController = lobbySceneController;
            _nicknameText.text = _lobbySceneController.Model.CurrentPlayer.Nickname;

            _createRoomText.text = StringHelper.GetString(1016);
            _joinRoomText.text = StringHelper.GetString(1017);
            _roomListText.text = StringHelper.GetString(1028);
            _refreshButtonText.text = StringHelper.GetString(1029);
        }

        public void OnRefreshButtonClick()
        {
            _lastRequestedPageIndex = -1;
        }

        private IEnumerator Refresh()
        {
            while(true)
            {
                yield return new DelayInSeconds(5);
                _lastRequestedPageIndex = -1;
            }
        }

        private void Update()
        {
            _coroutineHandler.UpdateCoroutines(Time.deltaTime);

            if (_isLoading)
            {
                return;
            }

            int currentPageIndex = GetCurrentPageIndex();

            if (currentPageIndex != _lastRequestedPageIndex)
            {
                _lastRequestedPageIndex = currentPageIndex;

                _isLoading = true;
                _lobbySceneController.RoomListRequest(currentPageIndex, _itemSize);
            }

        }
        private int GetCurrentPageIndex()
        {
            //10°³ = 1065
            var pageIndex = _scrollRect.content.anchoredPosition.y / (_itemSize * 100F);
            return (int)pageIndex;
        }

        public void RefreshRoomUI(int pageIndex, List<RoomInfo> roomInfos)
        {
            if (_lobbySceneController.Model.LobbyRoomInfos.TryGetValue(pageIndex, out var values) == false)
            {
                values = new ArrayQueue<RoomListItemUI>();
                _lobbySceneController.Model.LobbyRoomInfos.Add(pageIndex, values);
            }

            foreach (var item in values)
            {
                item.Recycle();
            }
            values.Clear();

            var index = 0;
            foreach (var item in roomInfos)
            {
                var roomInfoGo = _verticalLayoutGroup.InstantiateWithPool<RoomListItemUI>();
                roomInfoGo.Init(_lobbySceneController, item);
                roomInfoGo.RefreshUI();
                values.Add(roomInfoGo);
                roomInfoGo.transform.SetSiblingIndex(pageIndex * _itemSize + index++);
            }

            _isLoading = false;

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
