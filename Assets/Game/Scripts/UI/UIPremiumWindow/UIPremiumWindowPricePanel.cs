using Game.Scripts.UI_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowPricePanel : UIWindowPanel<UIPremiumWindowPopup>
    {
        [SerializeField] private UIPremiumWindowPricePanelItem itemPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button buyButton;

        ItemsPool<UIPremiumWindowPricePanelItem> _itemsPool;
        ItemsPool<UIPremiumWindowPricePanelItem> ItemsPool => _itemsPool ??= new ItemsPool<UIPremiumWindowPricePanelItem>(itemPrefab, content);
        
        private int _selectedIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            
            buyButton.onClick.AddListener(UI_Buy);
        }

        protected override void OnDestroy()
        { 
            base.OnDestroy();
            
            buyButton.onClick.RemoveListener(UI_Buy);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            
            if (Owner == null || Owner.PremiumService == null)
                return;
            
            _selectedIndex = 0;

            foreach (var (i, offerItem, instance) in ItemsPool.Change(Owner.PremiumService.OfferItems))
            {
                instance.Bind(this);
                
                if (!Owner.PremiumService.TryBuildOfferViewData(offerItem, out string price, out string value))
                    continue;
                
                instance.Fill(i, price, value, i == _selectedIndex);
            }
        }
        
        protected override void OnClose()
        {
            base.OnClose();

            ItemsPool.Clear();
        }

        public void Select(int index)
        {
            _selectedIndex = index;
            
            for (var i = 0; i < ItemsPool.Instances.Count; i++)
            {
                ItemsPool.Instances[i].SetSelected(i == _selectedIndex);
            }
        }

        private void UI_Buy()
        {
            Debug.Log("UI_Buy " + _selectedIndex);
        }
    }
}
