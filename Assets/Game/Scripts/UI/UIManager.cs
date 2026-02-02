using Cysharp.Threading.Tasks;
using Game.Scripts.Services;
using Game.Scripts.UI.UIArtViewerWindow;
using Game.Scripts.UI.UIMenuWindow;
using Game.Scripts.UI.UIPremiumWindow;
using Game.Scripts.UI.UISplashScreenWindow;
using UnityEngine;

namespace Game.Scripts.UI
{
    public sealed class UIManager: MonoBehaviour
    {
        [SerializeField] private UIMenuWindowPopup menuWindowPopup;
        [SerializeField] private UIPremiumWindowPopup premiumWindowPopup;
        [SerializeField] private UIArtViewerWindowPopup artViewerWindowPopup;
        [SerializeField] private UISplashScreenPopupWindow splashScreenPopupWindow;

        public RemoteTextureLoaderService TextureLoader { get; private set;}

        public void Setup(BannersService bannersService, ArtService artService, PremiumService premiumService, RemoteTextureLoaderService textureLoader)
        {
            TextureLoader = textureLoader;
            
            menuWindowPopup.Setup(bannersService, artService, this);
            premiumWindowPopup.Setup(premiumService);
            
            menuWindowPopup.Hide();
            premiumWindowPopup.Hide();
            artViewerWindowPopup.Hide();
        }

        public void ShowMenu()
        {
            menuWindowPopup.Show();
        }

        public void ShowPremium()
        {
            premiumWindowPopup.Show();
        }

        public void ShowSplash()
        {
            splashScreenPopupWindow.ShowAndAutoHide();
        }
        
        public void ShowArtViewer(string url)
        {
            artViewerWindowPopup.ShowUrl(url);

            LoadViewerTextureAsync(url).Forget();
        }
        
        private async UniTaskVoid LoadViewerTextureAsync(string url)
        {
            var texture = await TextureLoader.LoadAsync(url, this.GetCancellationTokenOnDestroy());

            if (artViewerWindowPopup.CurrentUrl != url)
                return;

            if (texture == null)
                artViewerWindowPopup.SetLoading(false);
            else
                artViewerWindowPopup.SetTexture(texture);
        }
    }
}