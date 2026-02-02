using Game.Scripts.Data;
using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowBannerPanel : UIWindowPanel<UIMenuWindowPopup>
    {
        [Header("Banner Items")]
        [SerializeField] private UIMenuWindowBannerPanelItem itemPrefab;
        [SerializeField] private RectTransform itemsContent;

        [Header("Toggles")]
        [SerializeField] private UIMenuWindowBannerPanelToggleItem togglePrefab;
        [SerializeField] private RectTransform togglesContent;
        
        [SerializeField] private BannerCarouselController carousel;

        ItemsPool<UIMenuWindowBannerPanelItem> _itemsPool;
        ItemsPool<UIMenuWindowBannerPanelItem> ItemsPool =>
            _itemsPool ??= new ItemsPool<UIMenuWindowBannerPanelItem>(itemPrefab, itemsContent);

        ItemsPool<UIMenuWindowBannerPanelToggleItem> _togglesPool;
        ItemsPool<UIMenuWindowBannerPanelToggleItem> TogglesPool =>
            _togglesPool ??= new ItemsPool<UIMenuWindowBannerPanelToggleItem>(togglePrefab, togglesContent);

        private int _selectedIndex;

        protected override void OnOpen()
        {
            base.OnOpen();

            if (Owner == null || Owner.BannersService == null)
                return;

            _selectedIndex = 0;

            foreach (var (_,item, instance) in ItemsPool.Change(Owner.BannersService.Banners))
            {
                instance.Bind(this);
                instance.Fill(item.Type, item.Prefab);
            }

            foreach (var (i,_, instance) in TogglesPool.Change(Owner.BannersService.Banners))
            {
                instance.Bind(this);
                instance.Fill(i == _selectedIndex);
            }

            carousel.PagesCount = Owner.BannersService.Banners.Count;
            
            carousel.PageChanged -= UI_PageChanged;
            carousel.PageChanged += UI_PageChanged;

            carousel.Setup(Owner.BannersService.Banners.Count);
            
            ApplySelected();
        }

        protected override void OnClose()
        {
            base.OnClose();

            ItemsPool.Clear();
            TogglesPool.Clear();
            
            carousel.PageChanged -= UI_PageChanged;
        }

        private void ApplySelected()
        {
            for (var i = 0; i < TogglesPool.Instances.Count; i++)
            {
                TogglesPool.Instances[i].SetSelected(i == _selectedIndex);
            }
        }
        
        private void UI_PageChanged(int index)
        {
            _selectedIndex = index;
            ApplySelected();
        }
        
        public void OnBannerActionClicked(BannerType type)
        {
            switch (type)
            {
                case BannerType.SimpleArt:
                    break;

                case BannerType.Play:
                    Debug.Log("Play banner clicked");
                    break;

                case BannerType.Audience:
                    Debug.Log("Open url");
                    break;
            }
        }
    }
}
