using UnityEngine;

namespace Game.Scripts.UI_Utils
{
    public abstract class UIWindowPanelItem<TPanel> : MonoBehaviour where TPanel : MonoBehaviour
    {
        protected TPanel Owner { get; private set; }

        public void Bind(TPanel owner)
        {
            Owner = owner;
            OnBind();
        }

        protected virtual void OnBind() { }
    }
}