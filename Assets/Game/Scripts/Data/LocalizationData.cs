using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Data
{
    public enum Language
    {
        Russian = 0,
        English = 1
    }
    
    [CreateAssetMenu(menuName = "Localization/LocalizationData", fileName = "LocalizationData")]
    public class LocalizationData : ScriptableObject
    {
        [SerializeField] private List<LocalizationItem> items = new();

        public string Get(string key, Language language)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Key == key)
                    return items[i].Get(language);
            }

            return key;
        }

        public IReadOnlyList<string> GetKeys()
        {
            var set = new HashSet<string>();
            var list = (from item in items where !string.IsNullOrEmpty(item.Key) && set.Add(item.Key) select item.Key).ToList();
            if (list.Count == 0) list.Add(string.Empty);
            return list;
        }

        [Serializable]
        public struct LocalizationItem
        {
            [SerializeField] private string key;
            [TextArea] [SerializeField] private string russian;
            [TextArea] [SerializeField] private string english;

            public string Key => key;

            public string Get(Language language)
            {
                string text = language == Language.Russian ? russian : english;
                return string.IsNullOrEmpty(text) ? key : text;
            }
        }
    }
}