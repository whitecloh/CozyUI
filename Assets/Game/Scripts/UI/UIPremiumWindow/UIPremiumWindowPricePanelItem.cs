using Game.Scripts.UI_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowPricePanelItem : UIWindowPanelItem<UIPremiumWindowPricePanel>
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private GameObject unselectedRoot;

        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text valueText;
        
        [SerializeField] TMPMaterialToggle priceToggle;
        [SerializeField] TMPMaterialToggle valueToggle;

        private int _index = 0;

        private void Awake()
        {
            button.onClick.AddListener(UI_Select);
        }

        private void OnDestroy()
        { 
            button.onClick.RemoveListener(UI_Select);
        }

        public void Fill(int index, string price, string value, bool isSelected)
        {
            _index = index;
            priceText.text = price;
            valueText.text = value;
            
            SetSelected(isSelected);
        }

        public void SetSelected(bool isSelected)
        {
            selectedRoot.SetActive(isSelected);
            unselectedRoot.SetActive(!isSelected);
            
            priceToggle.SetState(isSelected);
            valueToggle.SetState(isSelected);
        }

        private void UI_Select()
        {
            if (Owner == null)
                return;

            Owner.Select(_index);
        }
    }
}