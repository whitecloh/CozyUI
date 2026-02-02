using Game.Scripts.Data;
using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowBannerPanelItem : UIWindowPanelItem<UIMenuWindowBannerPanel>
    {
        [SerializeField] private RectTransform backContainer;
        
        private BannerType _bannerType;
        private BannerPrefab _spawned;

        public void Fill(BannerType bannerType, BannerPrefab artPrefab)
        {
            if (_spawned != null)
            {
                Destroy(_spawned);
                _spawned = null;
            }
            _bannerType = bannerType;
            _spawned = Instantiate(artPrefab, backContainer);

            bool hasAction = bannerType != BannerType.SimpleArt && _spawned.ActionButton != null;

            if (hasAction)
            {
                _spawned.Clicked += UI_Click;
            }
        }
        
        private void UI_Click()
        {
            if (Owner == null)
                return;

            Owner.OnBannerActionClicked(_bannerType);
        }
    }
}