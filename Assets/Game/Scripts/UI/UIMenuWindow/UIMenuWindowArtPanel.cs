using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Data;
using Game.Scripts.Services;
using Game.Scripts.UI_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.UIMenuWindow
{
    public sealed class UIMenuWindowArtPanel : UIWindowPanel<UIMenuWindowPopup>
    {
        [Header("Tabs")]
        [SerializeField] private UIMenuWindowArtPanelToggleItem tabPrefab;
        [SerializeField] private RectTransform tabsContent;

        [Header("Gallery")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform viewport;
        
        [Tooltip("Extra margin (in pixels) around viewport to preload nearby items.")]
        [SerializeField, Min(0f)] private float preloadMargin = 800f;

        [Tooltip("Debounce delay for scroll updates (ms).")]
        [SerializeField, Min(0)] private int scrollDebounceMs = 80;
        
        [Header("Items")]
        [SerializeField] private UIMenuWindowArtPanelItem itemPrefab;
        [SerializeField] private RectTransform itemsContent;

        ItemsPool<UIMenuWindowArtPanelToggleItem> _tabsPool;
        ItemsPool<UIMenuWindowArtPanelToggleItem> TabsPool => _tabsPool ??= new ItemsPool<UIMenuWindowArtPanelToggleItem>(tabPrefab, tabsContent);

        ItemsPool<UIMenuWindowArtPanelItem> _itemsPool;
        ItemsPool<UIMenuWindowArtPanelItem> ItemsPool => _itemsPool ??= new ItemsPool<UIMenuWindowArtPanelItem>(itemPrefab, itemsContent);

        private int _selectedTabIndex;
        
        private CancellationTokenSource _panelCts;
        private CancellationTokenSource _scrollDebounceCts;
        
        private RemoteTextureLoaderService _loader;

        private readonly Dictionary<UIMenuWindowArtPanelItem, string> _appliedUrl = new();
        private readonly Dictionary<UIMenuWindowArtPanelItem, string> _requestedUrl = new();

        protected override void OnOpen()
        {
            base.OnOpen();

            if (Owner == null || Owner.UIManager == null || Owner.ArtService == null)
                return;
            
            _panelCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            _loader = Owner.UIManager.TextureLoader;
            
            scrollRect.onValueChanged.AddListener(UI_OnScroll);
            viewport = scrollRect.viewport;

            _selectedTabIndex = 0;

            foreach (var (i, tabItem, instance) in TabsPool.Change(Owner.ArtService.TabItems))
            {
                instance.Bind(this);
                instance.Fill(i, i == _selectedTabIndex, i != Owner.ArtService.TabItems.Count - 1, Owner.ArtService.GetTitle(tabItem.TitleKey), tabItem.Filter);
            }

            int count = Owner.ArtService.Count;

            foreach (var (i, instance) in ItemsPool.Change(count))
            {
                instance.Bind(this);
                instance.Clicked -= UI_ItemClicked;
                instance.Clicked += UI_ItemClicked;

                int remoteIndex = Owner.ArtService.StartIndex + i;
                string url = Owner.ArtService.GetUrl(remoteIndex);
                bool isPremium = remoteIndex % 4 == 0;

                instance.Fill(url, isPremium);
                
                _appliedUrl[instance] = string.Empty;
                _requestedUrl[instance] = string.Empty;
            }

            ApplySelectedTab();
            ScheduleInitialVisibleRequest().Forget();
        }
        
        protected override void OnClose()
        {
            base.OnClose();

            if (scrollRect != null)
            {
                scrollRect.onValueChanged.RemoveListener(UI_OnScroll);
            }

            CancelDebounce();

            CancelPanelTasks();

            foreach (var item in ItemsPool.Instances)
            {
                if (item == null)
                    continue;

                item.Clicked -= UI_ItemClicked;
            }
        }

        public void SelectTab(int index)
        {
            _selectedTabIndex = index;
            ApplySelectedTab();
            RequestVisiblePreviews();
        }

        private void ApplySelectedTab()
        {
            for (int i = 0; i < TabsPool.Instances.Count; i++)
                TabsPool.Instances[i].SetSelected(i == _selectedTabIndex);

            var filter = TabsPool.Instances.InBounds(_selectedTabIndex)
                ? TabsPool.Instances[_selectedTabIndex].Filter
                : ArtTabFilter.All;

            ApplyFilter(filter);
        }

        private void ApplyFilter(ArtTabFilter filter)
        {
            for (int i = 0; i < ItemsPool.Instances.Count; i++)
            {
                var view = ItemsPool.Instances[i];
                if (view == null)
                    continue;

                int remoteIndex = Owner.ArtService.StartIndex + i;

                bool visible = filter switch
                {
                    ArtTabFilter.All => true,
                    ArtTabFilter.Odd => remoteIndex % 2 == 1,
                    ArtTabFilter.Even => remoteIndex % 2 == 0,
                    _ => true
                };

                view.gameObject.SetActive(visible);
            }
            
            ScheduleVisibleRequest().Forget();
        }

        private void OpenPremium()
        {
            if (Owner == null || Owner.UIManager == null)
                return;

            Owner.ShowPremium();
        }

        private void OpenArt(string url)
        {
            if (Owner == null || Owner.UIManager == null)
                return;

            Owner.UIManager.ShowArtViewer(url);
        }
        
        private void UI_ItemClicked(string url, bool isPremium)
        {
            if (isPremium)
            {
                OpenPremium();
                return;
            }

            OpenArt(url);
        }
        
        private void UI_OnScroll(Vector2 _)
        {
            DebounceVisibleRequest();
        }
        
        private void DebounceVisibleRequest()
        {
            if (!isActiveAndEnabled)
                return;

            CancelDebounce();

            _scrollDebounceCts = CancellationTokenSource.CreateLinkedTokenSource(_panelCts.Token);
            DebouncedRequestAsync(_scrollDebounceCts.Token).Forget();
        }
        
        private async UniTaskVoid ScheduleVisibleRequest()
        {
            CancellationToken ct = _panelCts.Token;
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, ct);
            if (ct.IsCancellationRequested) return;

            RequestVisiblePreviews();
        }
        
        private async UniTaskVoid DebouncedRequestAsync(CancellationToken ct)
        {
            if (scrollDebounceMs > 0)
                await UniTask.Delay(scrollDebounceMs, cancellationToken: ct);

            if (ct.IsCancellationRequested)
                return;

            RequestVisiblePreviews();
        }
        
        private void RequestVisiblePreviews()
        {
            if (_loader == null)
                return;

            if (viewport == null)
                return;

            if (_panelCts == null || _panelCts.IsCancellationRequested)
                return;

            Rect preloadRect = GetViewportRectExpanded(viewport, preloadMargin);

            foreach (var item in ItemsPool.Instances)
            {
                if (item == null)
                    continue;

                if (!item.gameObject.activeInHierarchy)
                    continue;

                if (!IsItemInPreloadRect(item.transform as RectTransform, viewport, preloadRect))
                    continue;

                string url = item.Url;
                if (string.IsNullOrEmpty(url))
                    continue;

                if (_appliedUrl.TryGetValue(item, out string applied) && applied == url)
                    continue;

                if (_requestedUrl.TryGetValue(item, out string requested) && requested == url)
                    continue;

                _requestedUrl[item] = url;

                LoadPreviewAsync(item, url, _loader, _panelCts.Token).Forget();
            }
        }
        
        private async UniTaskVoid LoadPreviewAsync(UIMenuWindowArtPanelItem item, string url, RemoteTextureLoaderService loader, CancellationToken ct)
        {
            if (item == null || loader == null || string.IsNullOrEmpty(url))
                return;
            
            if (loader.TryGetCached(url, out Texture2D cached) && cached != null)
            {
                if (item.Url == url)
                    item.SetTexture(cached);

                return;
            }
            
            Texture2D texture = await loader.LoadAsync(url, ct);

            if (ct.IsCancellationRequested)
                return;

            if (item.Url != url)
                return;

            item.SetTexture(texture);
        }
        
        private async UniTaskVoid ScheduleInitialVisibleRequest()
        {
            if (_panelCts == null)
                return;

            CancellationToken ct = _panelCts.Token;
            
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, ct);

            if (ct.IsCancellationRequested)
                return;
            
            Canvas.ForceUpdateCanvases();

            RequestVisiblePreviews();
        }
        
        private static Rect GetViewportRectExpanded(RectTransform viewportRect, float margin)
        {
            Rect rect = viewportRect.rect;
            rect.xMin -= margin;
            rect.xMax += margin;
            rect.yMin -= margin;
            rect.yMax += margin;
            return rect;
        }
        
        private static bool IsItemInPreloadRect(RectTransform itemRect, RectTransform viewportRect, Rect preloadRect)
        {
            if (itemRect == null || viewportRect == null)
                return false;

            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewportRect, itemRect);

            Rect itemBoundsRect = new Rect(
                bounds.min.x,
                bounds.min.y,
                bounds.size.x,
                bounds.size.y);

            return itemBoundsRect.Overlaps(preloadRect);
        }
        
        private void CancelDebounce()
        {
            if (_scrollDebounceCts == null)
                return;

            _scrollDebounceCts.Cancel();
            _scrollDebounceCts.Dispose();
            _scrollDebounceCts = null;
        }
        
        private void CancelPanelTasks()
        {
            if (_panelCts == null)
                return;

            _panelCts.Cancel();
            _panelCts.Dispose();
            _panelCts = null;
        }
    }
}
