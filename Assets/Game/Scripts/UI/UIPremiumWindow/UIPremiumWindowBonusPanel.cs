using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowBonusPanel : UIWindowPanel<UIPremiumWindowPopup>
    {
        [SerializeField] private UIPremiumWindowBonusPanelItem itemPrefab;
        [SerializeField] private RectTransform content;
        
        ItemsPool<UIPremiumWindowBonusPanelItem> _itemsPool;
        ItemsPool<UIPremiumWindowBonusPanelItem> ItemsPool => _itemsPool ??= new ItemsPool<UIPremiumWindowBonusPanelItem>(itemPrefab, content);
        

        protected override void OnOpen()
        {
            base.OnOpen();
            
            if (Owner == null || Owner.PremiumService == null)
                return;
            
            foreach (var (i, bonusItem, instance) in ItemsPool.Change(Owner.PremiumService.PremiumBonusItems))
            {
                var text = Owner.PremiumService.GetBonusText(bonusItem);
                
                instance.Bind(this);
                instance.Fill(bonusItem.Image, text);
            }
        }
        
        protected override void OnClose()
        {
            base.OnClose();

            ItemsPool.Clear();
        }
    }
}