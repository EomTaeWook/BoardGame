using Assets.Scripts.Extensions;
using Assets.Scripts.Internals.UI;
using Dignus.Unity;
using Dignus.Unity.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Assets.Scripts.Internals
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        public Camera UICamera { get; private set; }

        private readonly List<UiItem> _uiContainer = new();
        private readonly List<UiItem> _popupContainer = new();

        private GameObject _uiCanvas;
        private GameObject _uiPopupCanvas;
        private GameObject _popupDim;

        protected override void OnAwake()
        {
            var transform = gameObject.AddComponent<RectTransform>();
            transform.sizeDelta = ApplicationManager.Instance.GetResolution();
            transform.position = new Vector3(0, 0, 100);

            UICamera = gameObject.AddComponent<Camera>();
            UICamera.depth = 5;
            UICamera.clearFlags = CameraClearFlags.Depth;
            UICamera.orthographic = true;
            UICamera.cullingMask = 1 << 5;
            UICamera.orthographicSize = ApplicationManager.Instance.TargetHeight * 0.5f;
            var additionalCamData = UICamera.GetUniversalAdditionalCameraData();
            additionalCamData.renderType = CameraRenderType.Overlay;
            UICamera.allowMSAA = false;

            _uiCanvas = new GameObject("UICanvas")
            {
                layer = LayerMask.NameToLayer("UI")
            };
            _uiCanvas.transform.SetParent(transform, false);
            var uiCanvasComp = _uiCanvas.AddComponent<Canvas>();
            uiCanvasComp.sortingOrder = 10;
            uiCanvasComp.renderMode = RenderMode.ScreenSpaceCamera;

            uiCanvasComp.worldCamera = UICamera;
            _uiCanvas.AddComponent<GraphicRaycaster>();
            var canvasScaler = _uiCanvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = ApplicationManager.Instance.GetTargetResolution();
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            _uiPopupCanvas = new GameObject("UIPopupCanvas")
            {
                layer = LayerMask.NameToLayer("UI")
            };
            _uiPopupCanvas.transform.SetParent(transform, false);
            var uiPopupCanvasComp = _uiPopupCanvas.AddComponent<Canvas>();
            uiPopupCanvasComp.sortingOrder = 11;
            uiPopupCanvasComp.renderMode = RenderMode.ScreenSpaceCamera;
            uiPopupCanvasComp.worldCamera = UICamera;
            _uiPopupCanvas.AddComponent<GraphicRaycaster>();

            var popupCanvasScaler = _uiPopupCanvas.AddComponent<CanvasScaler>();
            popupCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            popupCanvasScaler.referenceResolution = ApplicationManager.Instance.GetTargetResolution();
            popupCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

            var dimPrefab = DignusUnityResourceManager.Instance.LoadAsset<GameObject>(Path.Combine(Consts.Path.Common, "Dim"));

            var dimTransform = this.InstantiateWithPool<RectTransform>(dimPrefab);
            dimTransform.SetParent(_uiPopupCanvas.transform, false);
            _popupDim = dimTransform.gameObject;
            _popupDim.SetActive(false);

            var eventSystem = gameObject.AddComponent<EventSystem>();
            eventSystem.firstSelectedGameObject = _uiCanvas;
            gameObject.AddComponent<InputSystemUIInputModule>();

            ApplicationManager.Instance.AdjustCameraViewportToAspectRatio(UICamera);
        }
        public void ShowAlert(string title, string body, Action onConfirmCallback = null)
        {
            var alert = AddPopupUI<AlertPopup>();
            alert.Init(AlertPopupType.Alert, title, body, onConfirmCallback);
        }
        public void ShowToastAlert(string body, float duration)
        {
            var toastMessage = AddPopupUI<ToastMessage>();
            toastMessage.Init(body, duration);
            toastMessage.Show();
        }
        public T AddUI<T>() where T : UiItem
        {
            var prefab = DignusUnityResourceManager.Instance.LoadAsset<T>();
            var item = DignusUnityObjectPool.Instance.Pop<UiItem>(prefab);
            _uiContainer.Add(item);
            item.transform.SetParent(_uiCanvas.transform, false);
            item.gameObject.SetActive(true);
            return item.GetComponent<T>();
        }
        public UiItem AddUI(UiItem prefab)
        {
            var item = DignusUnityObjectPool.Instance.Pop<UiItem>(prefab);
            _uiContainer.Add(item);
            item.transform.SetParent(_uiCanvas.transform, false);
            item.gameObject.SetActive(true);
            return item;
        }
        public T AddPopupUI<T>() where T : UiItem
        {
            if (_popupContainer.Count > 0)
            {
                _popupContainer[^1].gameObject.SetActive(false);
            }
            _popupDim.transform.SetAsLastSibling();
            _popupDim.SetActive(true);
            var prefab = DignusUnityResourceManager.Instance.LoadAsset<T>();
            var item = DignusUnityObjectPool.Instance.Pop<UiItem>(prefab);
            _popupContainer.Add(item);
            item.transform.SetParent(this._uiPopupCanvas.transform, false);
            item.gameObject.SetActive(true);

            return item.GetComponent<T>();
        }
        public void RemoveUI(UiItem item)
        {
            var removed = new List<UiItem>();
            for (int i = 0; i < _uiContainer.Count; ++i)
            {
                if (_uiContainer[i] == item)
                {
                    removed.Add(item);
                }
            }

            var removedPopup = new List<UiItem>();
            for (int i = 0; i < _popupContainer.Count; ++i)
            {
                if (_popupContainer[i] == item)
                {
                    removedPopup.Add(item);
                }
            }

            foreach (var remove in removed)
            {
                remove.Recycle();
                _uiContainer.Remove(remove);
            }

            foreach (var remove in removedPopup)
            {
                remove.Recycle();
                _popupContainer.Remove(remove);
                _popupDim.SetActive(false);
            }
            if (_popupContainer.Count > 0)
            {
                _popupDim.transform.SetAsLastSibling();
                _popupDim.SetActive(true);
                _popupContainer[^1].transform.SetAsLastSibling();
                _popupContainer[^1].gameObject.SetActive(true);
            }
        }
    }
}
