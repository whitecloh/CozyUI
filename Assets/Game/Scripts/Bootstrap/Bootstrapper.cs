using Game.Scripts.Data;
using Game.Scripts.Services;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Bootstrap
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private UISettings uiSettings;
        [SerializeField] private LocalizationData localizationData;
        [SerializeField] private CurrencyData currencyData;
        [SerializeField] private OffersData offersData;
        [SerializeField] private LinksData linksData;
        [SerializeField] private PremiumData premiumData;
        [SerializeField] private BannersData bannersData;
        [SerializeField] private ArtData artData;
        
        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private RemoteTextureLoaderService textureLoader;

        private void Awake()
        {
            var localizationService = new LocalizationService(localizationData, uiSettings.DefaultLanguage);

            var currencyService = new CurrencyService(currencyData, uiSettings.DefaultCurrency);
            var pricingService = new PricingService(offersData, localizationService, currencyService);

            var linksService = new LinksService(linksData, localizationService);

            var premiumService = new PremiumService(premiumData, localizationService, pricingService, linksService);

            var bannersService = new BannersService(bannersData);
            var remoteContentService = new ArtService(artData, localizationService);

            uiManager.Setup(bannersService, remoteContentService, premiumService, textureLoader);
        }

        private void Start()
        {
            uiManager.ShowMenu();
            uiManager.ShowSplash();
        }
    }
}
