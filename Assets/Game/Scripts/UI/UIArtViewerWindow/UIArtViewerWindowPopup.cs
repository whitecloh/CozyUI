using Game.Scripts.UI_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIArtViewerWindow
{
    public sealed class UIArtViewerWindowPopup : AnimatedWindow
    {
        [Header("UI")]
        [SerializeField] private RawImage image;
        [SerializeField] private Button closeButton;

        [Header("Optional")]
        [SerializeField] private GameObject loadingRoot;

        public string CurrentUrl { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            closeButton.onClick.AddListener(UI_Close);

            SetLoading(false);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(UI_Close);
        }

        public void ShowUrl(string url)
        {
            CurrentUrl = url;

            image.texture = null;

            SetLoading(true);
            Show();
        }

        public void SetTexture(Texture texture)
        {
            image.texture = texture;
            SetLoading(false);
        }

        private void UI_Close()
        {
            Hide();
        }

        public void SetLoading(bool isLoading)
        { 
            loadingRoot.SetActive(isLoading);
        }
    }
}