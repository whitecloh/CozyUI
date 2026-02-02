using System.Collections.Generic;
using Game.Scripts.Data;

namespace Game.Scripts.Services
{
    public sealed class PremiumService
    {
        private readonly PremiumData _premiumData;
        private readonly LocalizationService _localization;
        private readonly PricingService _pricing;
        private readonly LinksService _links;

        public IReadOnlyList<PremiumBonusItem> PremiumBonusItems => _premiumData.Bonuses;
        public IReadOnlyList<PremiumOfferItem> OfferItems => _premiumData.Offers;
        public IReadOnlyList<LinkType> LinkItems => _premiumData.Links;

        public PremiumService(PremiumData premiumData, LocalizationService localization, PricingService pricing, LinksService links)
        {
            _premiumData = premiumData;
            _localization = localization;
            _pricing = pricing;
            _links = links;
        }

        public string GetBonusText(PremiumBonusItem item)
        {
            return _localization.Get(item.TextKey);
        }

        public bool TryBuildOfferViewData(PremiumOfferItem item, out string priceText, out string valueText)
        {
            priceText = string.Empty;
            valueText = string.Empty;

            var duration = _localization.Get(item.Duration);
            
            if (!_pricing.TryGetPriceText(item.OfferKey, out var price))
            {
                return false;
            }
            
            priceText = _localization.Format(item.DurationFormatKey, price, duration);
            valueText = _localization.Format(item.ValueFormatKey, item.ValuePercent);
            return true;
        }

        public bool TryBuildLinkViewData(LinkType type, out string title, out string url)
        {
            return _links.TryGet(type, out title, out url);
        }
    }
}