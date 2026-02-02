using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI_Utils
{
    public abstract class UIWindow : MonoBehaviour
    {
        private readonly List<IWindowPanel> _panels = new();

        public bool IsOpen { get; private set; }

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        protected void ShowInstant()
        {
            if (IsOpen) return;

            IsOpen = true;
            gameObject.SetActive(true);

            OnShownInstant();

            for (int i = 0; i < _panels.Count; i++)
            {
                _panels[i].Open();
            }
        }

        protected void HideInstant()
        {
            if (!IsOpen) return;

            IsOpen = false;

            for (int i = 0; i < _panels.Count; i++)
            {
                _panels[i].Close();
            }

            OnHiddenInstant();
            gameObject.SetActive(false);
        }
        
        protected void MarkOpen()
        {
            IsOpen = true;
        }

        protected void MarkClosed()
        {
            IsOpen = false;
        }

        protected void OpenPanels()
        {
            for (int i = 0; i < _panels.Count; i++)
            {
                _panels[i].Open();
            }
        }

        protected void ClosePanels()
        {
            for (int i = 0; i < _panels.Count; i++)
            {
                _panels[i].Close();
            }
        }

        internal void AddPanel(IWindowPanel panel)
        {
            if (_panels.Contains(panel)) return;

            _panels.Add(panel);

            if (IsOpen)
            {
                panel.Open();
            }
        }

        internal void RemovePanel(IWindowPanel panel)
        {
            if (!_panels.Remove(panel)) return;
            panel.Close();
        }

        protected virtual void OnShownInstant()
        {
        }

        protected virtual void OnHiddenInstant()
        {
        }
    }
}