using Assets.Scripts.Internals;
using Dignus.Unity.Attributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scene.Title.UI
{
    [PrefabPath(Consts.Path.Title)]
    public class CreateAccountUI : UIItem
    {
        [SerializeField]
        private TMP_InputField _nicknameInput;

        [SerializeField]
        private Button _createButton;

        private Action<string> _onClickCallback;
        public void SetCreateAccountCallback(Action<string> action)
        {
            _onClickCallback = action;
        }
        public void OnClickCreateAccount()
        {
            var nickname = _nicknameInput.text;

            if (nickname.Length > 20)
            {
                UIManager.Instance.ShowAlert("Alert", "nickname cannot exceed 20 characters");
                return;
            }
            _onClickCallback?.Invoke(nickname);
            this.DisposeUI();
        }
    }
}
