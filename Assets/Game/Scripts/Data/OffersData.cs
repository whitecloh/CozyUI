using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(menuName = "Economy/OffersData", fileName = "OffersData")]
    public class OffersData : ScriptableObject
    {
        [SerializeField] private List<PriceItem> items = new();

        public IReadOnlyList<PriceItem> Items => items;

        public bool TryGet(string key, out PriceItem item)
        {
            foreach (var t in items.Where(t => t.Key == key))
            {
                item = t;
                return true;
            }

            item = default;
            return false;
        }

        public IReadOnlyList<string> GetKeys()
        {
            var set = new HashSet<string>();
            var list = (from item in items where !string.IsNullOrEmpty(item.Key) && set.Add(item.Key) select item.Key).ToList();
            if (list.Count == 0) list.Add(string.Empty);
            return list;
        }

        [Serializable]
        public struct PriceItem
        {
            [SerializeField] private string key;
            [SerializeField] private float usd;

            public string Key => key;
            public float Usd => usd;
        }
    }
}