using Game.Scripts.Editor_Utils;
using UnityEngine;

namespace Game.Scripts.Data
{
    public enum LinkType
    {
        PrivacyPolicy = 0,
        TermsOfService = 1,
        Support = 2,
        RestorePurchases = 3,
        About = 4
    }

    [CreateAssetMenu(menuName = "UI/Links/LinksData", fileName = "LinksData")]
    public class LinksData : ScriptableObject
    {
        [SerializeField] private LinkEntry[] links;

        public LinkEntry[] Links => links;
    }

    [System.Serializable]
    public class LinkEntry
    {
        [SerializeField] private LinkType type;
        [LocalizationKeyPopup]
        [SerializeField] private string textKey;
        [SerializeField] private string url;

        public LinkType Type => type;
        public string TextKey => textKey;
        public string Url => url;
    }
}