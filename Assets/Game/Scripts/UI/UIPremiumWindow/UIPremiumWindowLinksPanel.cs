using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowLinksPanel : UIWindowPanel<UIPremiumWindowPopup>
    {
        [SerializeField] private UIPremiumWindowLinksPanelItem itemPrefab;
        [SerializeField] private RectTransform content;

        ItemsPool<UIPremiumWindowLinksPanelItem> _itemsPool;
        ItemsPool<UIPremiumWindowLinksPanelItem> ItemsPool => _itemsPool ??= new ItemsPool<UIPremiumWindowLinksPanelItem>(itemPrefab, content);
        

        protected override void OnOpen()
        {
            base.OnOpen();
            
            if (Owner == null || Owner.PremiumService == null)
                return;

            foreach (var (_, link, instance) in ItemsPool.Change(Owner.PremiumService.LinkItems))
            {
                instance.Bind(this);
                
                if (!Owner.PremiumService.TryBuildLinkViewData(link, out var title, out var url))
                    continue;

                instance.Fill(title, url);
            }
        }
        
        protected override void OnClose()
        {
            base.OnClose();

            ItemsPool.Clear();
        }
    }
}