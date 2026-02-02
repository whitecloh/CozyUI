using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(menuName = "Settings/UI Settings", fileName = "UISettings")]
    public class UISettings : ScriptableObject
    {
        [SerializeField] private Language defaultLanguage = Language.Russian;
        [SerializeField] private Currency defaultCurrency = Currency.Usd;

        public Language DefaultLanguage => defaultLanguage;
        public Currency DefaultCurrency => defaultCurrency;
    }
}