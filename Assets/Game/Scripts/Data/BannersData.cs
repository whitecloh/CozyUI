using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.Data
{
    public enum BannerType
    {
        SimpleArt = 0,
        Audience = 1,
        Play = 2
    }

    [CreateAssetMenu(menuName = "UI/Banners/BannersData", fileName = "BannersData")]
    public class BannersData : ScriptableObject
    {
        [SerializeField] private List<BannerItem> items = new();

        public IReadOnlyList<BannerItem> Items => items;

        public bool TryGet(BannerType type, out BannerItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Type != type) continue;
                item = items[i];
                return true;
            }
            item = default;
            return false;
        }

        [Serializable]
        public struct BannerItem
        {
            [SerializeField] private BannerType type;
            [SerializeField] private BannerPrefab prefab;

            public BannerType Type => type;
            public BannerPrefab Prefab => prefab;
        }
    }
}