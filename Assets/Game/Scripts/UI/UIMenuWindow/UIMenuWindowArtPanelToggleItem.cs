using Game.Scripts.Data;
using Game.Scripts.UI_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowArtPanelToggleItem : UIWindowPanelItem<UIMenuWindowArtPanel>
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text titleText;
        
        [SerializeField] TMPMaterialToggle titleToggle;

        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private GameObject unselectedRoot;
        [SerializeField] private GameObject separator;
        
        public ArtTabFilter Filter { get; private set; }
        
        private int _index;

        private void Awake()
        {
            button.onClick.AddListener(UI_Select);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(UI_Select);
        }

        public void Fill(int index, bool isSelected, bool isSeparatorNeeded, string titleKey, ArtTabFilter filter)
        {
            _index = index;
            Filter = filter;

            titleText.text = titleKey;
            
            separator.SetActive(isSeparatorNeeded);
            
            SetSelected(isSelected);
        }

        public void SetSelected(bool isSelected)
        {
            selectedRoot.SetActive(isSelected);
            unselectedRoot.SetActive(!isSelected);
            
            titleToggle.SetState(isSelected);
        }

        private void UI_Select()
        {
            if (Owner == null)
                return;

            Owner.SelectTab(_index);
        }
    }
}