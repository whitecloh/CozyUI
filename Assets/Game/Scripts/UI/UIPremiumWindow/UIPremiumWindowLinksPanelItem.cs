using Game.Scripts.UI_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIPremiumWindow
{
    public sealed class UIPremiumWindowLinksPanelItem : UIWindowPanelItem<UIPremiumWindowLinksPanel>
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;

        private string _url;

        private void Awake()
        {
            button.onClick.AddListener(UI_OpenUrl);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(UI_OpenUrl);
        }

        public void Fill(string title, string url)
        {
            text.text = title;
            _url = url;
        }

        private void UI_OpenUrl()
        {
            Debug.Log("UI_OpenUrl");
        }
    }
}