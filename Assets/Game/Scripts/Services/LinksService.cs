using Game.Scripts.Data;

namespace Game.Scripts.Services
{
    public sealed class LinksService
    {
        private readonly LinksData _linksData;
        private readonly LocalizationService _localization;

        public LinksService(LinksData linksData, LocalizationService localization)
        {
            _linksData = linksData;
            _localization = localization;
        }

        public bool TryGet(LinkType type, out string title, out string url)
        {
            title = string.Empty;
            url = string.Empty;

            var links = _linksData.Links;
            if (links == null)
            {
                return false;
            }

            foreach (var entry in links)
            {
                if (entry.Type != type)
                {
                    continue;
                }

                title = _localization.Get(entry.TextKey);
                url = entry.Url;
                return true;
            }

            return false;
        }
    }
}