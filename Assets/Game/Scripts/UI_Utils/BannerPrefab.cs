using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI_Utils
{
    public class BannerPrefab : MonoBehaviour
    {
        [SerializeField] private Button actionButton;

        public Button ActionButton => actionButton;
        public event Action Clicked;
        
        private void Awake()
        {
            if (actionButton == null)
                return;
            
            actionButton.onClick.AddListener(UI_Click);
        }

        private void OnDestroy()
        {
            if (actionButton == null)
                return;
            
            actionButton.onClick.RemoveListener(UI_Click);
        }

        private void UI_Click()
        {
            Clicked?.Invoke();
        }
    }
}