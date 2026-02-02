using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Editor_Utils;
using UnityEngine;

namespace Game.Scripts.Data
{
    public enum Currency
    {
        Usd = 0,
        Rub = 1
    }
    
    [CreateAssetMenu(menuName = "Economy/CurrencyData", fileName = "CurrencyData")]
    public class CurrencyData : ScriptableObject
    {
        [SerializeField] private List<CurrencyItem> items = new();

        public IReadOnlyList<CurrencyItem> Items => items;

        public bool TryGet(Currency key, out CurrencyItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Currency != key) continue;
                item = items[i];
                return true;
            }
            item = default;
            return false;
        }

        public IReadOnlyList<Currency> GetCurrencies()
        {
            var set = new HashSet<Currency>();
            var list = items.Select(t => t.Currency).Where(currency => set.Add(currency)).ToList();

            if (list.Count == 0)
            {
                list.Add(default);
            }

            return list;
        }

        [Serializable]
        public struct CurrencyItem
        {
            [SerializeField] private Currency currency;

            [LocalizationKeyPopup]
            [SerializeField] private string displayKey;

            [SerializeField] private float unitsPerUsd;

            public Currency Currency => currency;
            public string DisplayKey => displayKey;
            public float UnitsPerUsd => unitsPerUsd;
        }
    }
}