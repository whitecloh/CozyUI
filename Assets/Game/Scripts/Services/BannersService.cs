using System.Collections.Generic;
using Game.Scripts.Data;
using Game.Scripts.UI_Utils;

namespace Game.Scripts.Services
{
    public sealed class BannersService
    {
        private readonly BannersData _bannersData;

        public IReadOnlyList<BannersData.BannerItem> Banners => _bannersData.Items; 

        public BannersService(BannersData bannersData)
        {
            _bannersData = bannersData;
        }
    }
}