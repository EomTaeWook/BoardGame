using UnityEngine;

namespace Assets.Scripts.Internals
{
    public class UIItem : MonoBehaviour
    {
        public virtual void DisposeUI()
        {
            UIManager.Instance.RemoveUI(this);
        }
    }
}
