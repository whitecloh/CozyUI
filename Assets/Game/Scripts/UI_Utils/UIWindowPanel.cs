using UnityEngine;

namespace Game.Scripts.UI_Utils
{
    public abstract class UIWindowPanel<TWindow> : MonoBehaviour, IWindowPanel where TWindow : UIWindow
    {
        protected TWindow Owner { get; private set;}

        private bool IsOpen { get; set; }

        protected virtual void Awake()
        {
            Owner = GetComponentInParent<TWindow>(true);
            
            gameObject.SetActive(false);

            if (Owner != null)
            {
                Owner.AddPanel(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Owner != null)
            {
                Owner.RemovePanel(this);
            }
        }

        void IWindowPanel.Open()
        {
            if (IsOpen)
            {
                return;
            }

            IsOpen = true;
            gameObject.SetActive(true);
            OnOpen();
        }

        void IWindowPanel.Close()
        {
            if (!IsOpen)
            {
                return;
            }

            IsOpen = false;
            OnClose();
            gameObject.SetActive(false);
        }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
    }
}