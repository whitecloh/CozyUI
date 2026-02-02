using Game.Scripts.Data;

namespace Game.Scripts.Services
{
    public sealed class CurrencyService
    {
        private readonly CurrencyData _currencyData;

        public Currency CurrentCurrency { get; private set; }

        public CurrencyService(CurrencyData currencyData, Currency initialCurrency)
        {
            _currencyData = currencyData;
            CurrentCurrency = initialCurrency;
        }

        public bool TryGetCurrent(out CurrencyData.CurrencyItem currencyItem)
        {
            return _currencyData.TryGet(CurrentCurrency, out currencyItem);
        }
    }
}