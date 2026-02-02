using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowBannerPanelToggleItem : UIWindowPanelItem<UIMenuWindowBannerPanel>
    {
        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private GameObject unselectedRoot;

        public void Fill(bool isSelected)
        {
            SetSelected(isSelected);
        }

        public void SetSelected(bool isSelected)
        {
            selectedRoot.SetActive(isSelected);
            unselectedRoot.SetActive(!isSelected);
        }
    }
}