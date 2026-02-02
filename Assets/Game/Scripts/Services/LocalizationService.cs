using Game.Scripts.Data;

namespace Game.Scripts.Services
{
    public sealed class LocalizationService
    {
        private readonly LocalizationData _localizationData;

        public Language CurrentLanguage { get; private set; }

        public LocalizationService(LocalizationData localizationData, Language initialLanguage)
        {
            _localizationData = localizationData;
            CurrentLanguage = initialLanguage;
        }

        public string Get(string key)
        {
            return _localizationData.Get(key, CurrentLanguage);
        }

        public string Format(string formatKey, params object[] args)
        {
            string format = Get(formatKey);
            return string.Format(format, args);
        }
    }
}