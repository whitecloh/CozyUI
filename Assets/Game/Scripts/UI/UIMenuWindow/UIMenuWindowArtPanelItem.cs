using System;
using Game.Scripts.UI_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowArtPanelItem : UIWindowPanelItem<UIMenuWindowArtPanel>
    {
        [SerializeField] private Button button;
        [SerializeField] private RawImage previewRawImage;
        [SerializeField] private GameObject premiumBadge;

        public event Action<string, bool> Clicked;

        public string Url { get; private set; }
        private bool IsPremium { get; set; }

        private void Awake()
        {
            button.onClick.AddListener(UI_Click);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(UI_Click);
        }
        
        public void Fill(string url, bool isPremium)
        {
            Url = url;
            IsPremium = isPremium;
            
            premiumBadge.SetActive(isPremium);
        }
        
        public void SetTexture(Texture2D texture)
        {
            if (texture == null)
                return;
            
            previewRawImage.texture = texture;
        }

        private void UI_Click()
        {
            Clicked?.Invoke(Url, IsPremium);
        }
    }
}