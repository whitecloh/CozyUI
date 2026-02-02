using Game.Scripts.Services;
using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowPopup : AnimatedWindow
    {
        [Header("Panels")]
        [SerializeField] private UIMenuWindowBannerPanel bannerPanel;
        [SerializeField] private UIMenuWindowArtPanel artPanel;

        public BannersService BannersService { get; private set; }
        public ArtService ArtService { get; private set; }
        public UIManager UIManager { get; private set; }

        public void Setup(BannersService bannersService, ArtService artService, UIManager uiManager)
        {
            BannersService = bannersService;
            ArtService = artService;
            UIManager = uiManager;
        }

        public void ShowPremium()
        {
            if (UIManager == null)
                return;

            UIManager.ShowPremium();
        }
    }
}