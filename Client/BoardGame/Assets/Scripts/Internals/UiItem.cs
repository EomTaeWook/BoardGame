using UnityEngine;

namespace Assets.Scripts.Internals
{
    public class UiItem : MonoBehaviour
    {
        public virtual void DisposeUI()
        {
            UIManager.Instance.RemoveUI(this);
        }
    }
}
