using Game.Scripts.Services;
using Game.Scripts.UI_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowPopup : AnimatedWindow
    {
        [Header("Panels")]
        [SerializeField] private UIPremiumWindowBonusPanel bonusPanel;
        [SerializeField] private UIPremiumWindowPricePanel pricePanel;
        [SerializeField] private UIPremiumWindowLinksPanel linksPanel;
        [SerializeField] private Button backButton;
        
        public PremiumService PremiumService{get; private set;}
        
        protected override void Awake()
        {
            base.Awake();
            
            backButton.onClick.AddListener(UI_Back);
        }

        private void OnDestroy()
        {
            backButton.onClick.RemoveListener(UI_Back);
        }

        public void Setup(PremiumService premiumService)
        { 
            PremiumService = premiumService;
        }

        private void UI_Back()
        {
            Hide();
        }
    }
}