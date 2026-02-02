using Cysharp.Threading.Tasks;
using Game.Scripts.UI_Utils;
using UnityEngine;

namespace Game.Scripts.UI.UISplashScreenWindow
{
    public sealed class UISplashScreenPopupWindow : UIWindow
    {
        [SerializeField] private float lifeTime = 4f;

        public async void ShowAndAutoHide()
        {
            ShowInstant();
            
            await UniTask.WaitForSeconds(lifeTime);
            
            HideInstant();
        }
    }
}
