using Dignus.Unity.Attributes;
using Dignus.Unity.Coroutine;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Internals.UI
{
    [PrefabPath(Consts.Path.Common)]
    internal class ToastMessage : UiItem
    {
        [SerializeField]
        private TextMeshProUGUI _bodyText;

        private float _elapsedTime = 0;
        private float _duration;
        public void Init(string body, float duration)
        {
            _bodyText.text = body;
            _duration = duration;
            _elapsedTime = 0;
        }
        public void Show()
        {
            DignusUnityCoroutineManager.Start(InternalShow(), this.DisposeUI);
        }
        private IEnumerator InternalShow()
        {
            while (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
