using System.Collections.Generic;
using Game.Scripts.Data;

namespace Game.Scripts.Services
{
    public sealed class ArtService
    {
        private readonly LocalizationService _localization;
        private readonly ArtData _artData;

        public int StartIndex => _artData.StartIndex;
        public int Count => _artData.Count;

        public IReadOnlyList<ArtData.TabItem> TabItems => _artData.Items;

        public ArtService(ArtData artData, LocalizationService localizationService)
        {
            _artData = artData;
            _localization = localizationService;
        }

        public string GetUrl(int index)
        {
            return _artData.GetUrl(index);
        }

        public string GetTitle(string key)
        {
            return _localization.Get(key);
        }
    }
}