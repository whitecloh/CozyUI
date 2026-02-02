using Game.Scripts.UI_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowBonusPanelItem : UIWindowPanelItem<UIPremiumWindowBonusPanel>
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text text;

        public void Fill(Sprite icon, string value)
        { 
            iconImage.sprite = icon; 
            iconImage.SetNativeSize();
            text.text = value;
        }
    }
}