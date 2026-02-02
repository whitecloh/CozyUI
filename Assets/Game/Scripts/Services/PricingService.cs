using System.Globalization;
using Game.Scripts.Data;

namespace Game.Scripts.Services
{
    public sealed class PricingService
    {
        private readonly OffersData _offersData;
        private readonly LocalizationService _localization;
        private readonly CurrencyService _currencyService;

        public PricingService(
            OffersData offersData,
            LocalizationService localization,
            CurrencyService currencyService)
        {
            _offersData = offersData;
            _localization = localization;
            _currencyService = currencyService;
        }

        public bool TryGetPriceText(string offerKey, out string priceText)
        {
            priceText = string.Empty;

            if (!_offersData.TryGet(offerKey, out OffersData.PriceItem offer))
            {
                return false;
            }

            if (!_currencyService.TryGetCurrent(out CurrencyData.CurrencyItem currency))
            {
                return false;
            }

            float amount = offer.Usd * currency.UnitsPerUsd;

            string currencyDisplay = _localization.Get(currency.DisplayKey);
            string amountText = FormatAmount(amount, _currencyService.CurrentCurrency);

            priceText = amountText + " " + currencyDisplay;
            return true;
        }

        private static string FormatAmount(float amount, Currency currency)
        {
            int decimals = currency == Currency.Rub ? 0 : 2;
            string format = decimals == 0 ? "0" : "0.00";
            return amount.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}