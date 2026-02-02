using System;
using System.Collections.Generic;
using Game.Scripts.Editor_Utils;
using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(menuName = "UI/Premium/PremiumData", fileName = "PremiumData")]
    public class PremiumData : ScriptableObject
    {
        [SerializeField] private List<PremiumBonusItem> bonuses = new();
        [SerializeField] private List<PremiumOfferItem> offers = new();
        [SerializeField] private List<LinkType> links = new();

        public IReadOnlyList<PremiumBonusItem> Bonuses => bonuses;
        public IReadOnlyList<PremiumOfferItem> Offers => offers;
        public IReadOnlyList<LinkType> Links => links;
    }

    [Serializable]
    public struct PremiumBonusItem
    {
        [SerializeField] private Sprite image;

        [LocalizationKeyPopup]
        [SerializeField] private string textKey;

        public Sprite Image => image;
        public string TextKey => textKey;
    }

    [Serializable]
    public struct PremiumOfferItem
    {
        [OfferKeyPopup]
        [SerializeField] private string offerKey;

        [LocalizationKeyPopup]
        [SerializeField] private string duration;

        [SerializeField, Range(0f, 100f)]
        private float valuePercent;

        [LocalizationKeyPopup]
        [SerializeField] private string durationFormatKey;

        [LocalizationKeyPopup]
        [SerializeField] private string valueFormatKey;

        public string OfferKey => offerKey;
        public string Duration => duration;
        public float ValuePercent => valuePercent;
        public string DurationFormatKey => durationFormatKey;
        public string ValueFormatKey => valueFormatKey;
    }
}