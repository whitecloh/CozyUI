using System;
using System.Collections.Generic;
using Game.Scripts.Editor_Utils;
using UnityEngine;

namespace Game.Scripts.Data
{
    public enum ArtTabFilter
    {
        All = 0,
        Odd = 1,
        Even = 2
    }
    
    [CreateAssetMenu(menuName = "Content/RemoteContentData", fileName = "RemoteContentData")]
    public class ArtData : ScriptableObject
    {
        [SerializeField] private string urlPattern;
        [SerializeField] private int startIndex;
        [SerializeField] private int count;
        
        [SerializeField] private List<TabItem> items = new();

        public IReadOnlyList<TabItem> Items => items;

        public string UrlPattern => urlPattern;
        public int StartIndex => startIndex;
        public int Count => count;
        
        [Serializable]
        public struct TabItem
        {
            [LocalizationKeyPopup]
            [SerializeField] private string titleKey;
            [SerializeField] private ArtTabFilter filter;

            public string TitleKey => titleKey;
            public ArtTabFilter Filter => filter;
        }

        public string GetUrl(int index)
        {
            return string.Format(urlPattern, index);
        }
    }
}