using Assets.Scripts.Extensions;
using Dignus.Unity.Attributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Internals.UI
{
    public enum AlertPopupType
    {
        Alert,
        Confirm,
        Max,
    }

    [PrefabPath(Consts.Path.Common)]
    internal class AlertPopup : UIItem
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _bodyText;
        [SerializeField]
        private TextMeshProUGUI _confirmButtonText;
        [SerializeField]
        private TextMeshProUGUI _cancelButtonText;

        [SerializeField]
        private Button _confirmButton;
        [SerializeField]
        private Button _cancelButton;

        private Action _confrimCallback;
        private Action _cancelCallback;

        public void Init(AlertPopupType alertPopupType,
            string title,
            string body,
            Action onConfrimCallback = null,
            Action onCancelCallback = null)
        {
            _confirmButtonText.text = StringHelper.GetString(1014);
            _cancelButtonText.text = StringHelper.GetString(1015);
            InitPopup(alertPopupType, title, body, onConfrimCallback, onCancelCallback);
        }

        private void InitPopup(AlertPopupType alertPopupType, string title, string body,
            Action onConfrimCallback = null, Action onCancelCallback = null)
        {
            _titleText.text = title;
            _bodyText.text = body;

            if (alertPopupType == AlertPopupType.Alert)
            {
                _confirmButton.gameObject.SetActive(true);
                _cancelButton.gameObject.SetActive(false);
            }
            else
            {
                _confirmButton.gameObject.SetActive(true);
                _cancelButton.gameObject.SetActive(true);
            }
            _confrimCallback = onConfrimCallback;
            _cancelCallback = onCancelCallback;
        }

        public void OnConfirmButtonClick()
        {
            _confrimCallback?.Invoke();
            DisposeUI();
        }
        public void OnCancelButtonClick()
        {
            _cancelCallback?.Invoke();
            DisposeUI();
        }
    }
}
