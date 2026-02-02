using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Scripts.UI_Utils
{
    [DisallowMultipleComponent]
    public sealed class BannerCarouselController : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IBeginDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private ScrollRect scrollRect;

        [Header("Settings")]
        [SerializeField, Min(0f)] private float autoScrollInterval = 5f;
        [SerializeField, Min(0.01f)] private float snapDuration = 0.25f;
        [SerializeField] private Ease snapEase = Ease.OutCubic;

        [Header("Swipe")]
        [Tooltip("How much normalized position user must move to change page (0..1).")]
        [SerializeField, Range(0.02f, 0.5f)] private float swipeThresholdNormalized = 0.12f;

        public event Action<int> PageChanged;

        public int PagesCount { get; set; }
        private int CurrentPage { get; set; }

        private bool _pointerDown;
        private bool _dragging;

        private float _dragStartNormalized;
        private int _dragStartPage;

        private Tween _snapTween;
        private CancellationTokenSource _autoCts;
        private float _nextAutoTime;

        private void OnEnable()
        {
            if (scrollRect != null)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
                scrollRect.inertia = false;
            }

            ResetAutoTimer();
            StartAuto();
            SyncFromPosition();
        }

        private void OnDisable()
        {
            StopAuto();
            KillSnap();
        }

        public void Setup(int count, int startPage = 0)
        {
            PagesCount = Mathf.Max(1, count);
            CurrentPage = Mathf.Clamp(startPage, 0, PagesCount - 1);

            SetPage(CurrentPage, instant: true);
            ResetAutoTimer();
            StartAuto();
        }

        private void SetPage(int page, bool instant)
        {
            page = Mathf.Clamp(page, 0, PagesCount - 1);

            if (CurrentPage != page)
            {
                CurrentPage = page;
                PageChanged?.Invoke(CurrentPage);
            }

            float target = PageToNormalized(page);

            if (instant)
            {
                KillSnap();
                if (scrollRect != null)
                    scrollRect.horizontalNormalizedPosition = target;
                return;
            }

            SnapTo(target);
            ResetAutoTimer();
        }

        private void Next()
        {
            int next = CurrentPage + 1;
            if (next >= PagesCount)
                next = 0;

            SetPage(next, instant: false);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDown = true;
            KillSnap();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pointerDown = false;
            ResetAutoTimer();
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            KillSnap();

            _dragStartNormalized = scrollRect != null ? scrollRect.horizontalNormalizedPosition : 0f;
            _dragStartPage = CurrentPage;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;

            if (scrollRect == null)
                return;

            float end = scrollRect.horizontalNormalizedPosition;
            float delta = end - _dragStartNormalized;
            int nearestPage;
            
            if (Mathf.Abs(delta) >= swipeThresholdNormalized)
            {
                if (delta > 0f)
                    nearestPage = _dragStartPage + 1;
                else
                    nearestPage = _dragStartPage - 1;
            }
            else
            {
                nearestPage = NormalizedToNearestPage(end);
            }

            nearestPage = Mathf.Clamp(nearestPage, 0, PagesCount - 1);

            SetPage(nearestPage, instant: false);
            ResetAutoTimer();
        }

        private void StartAuto()
        {
            StopAuto();

            if (autoScrollInterval <= 0f)
                return;

            _autoCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            AutoLoop(_autoCts.Token).Forget();
        }

        private void StopAuto()
        {
            if (_autoCts == null)
                return;

            _autoCts.Cancel();
            _autoCts.Dispose();
            _autoCts = null;
        }

        private async UniTaskVoid AutoLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                while (Time.unscaledTime < _nextAutoTime)
                {
                    if (ct.IsCancellationRequested)
                        return;

                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                if (ct.IsCancellationRequested)
                    return;
                
                if (_pointerDown || _dragging)
                {
                    ResetAutoTimer();
                    continue;
                }

                Next();
                ResetAutoTimer();
            }
        }

        private void ResetAutoTimer()
        {
            _nextAutoTime = Time.unscaledTime + autoScrollInterval;
        }

        private void SnapTo(float targetNormalized)
        {
            if (scrollRect == null)
                return;

            KillSnap();
            
            _snapTween = DOTween.To(
                    () => scrollRect.horizontalNormalizedPosition,
                    value => scrollRect.horizontalNormalizedPosition = value,
                    targetNormalized,
                    snapDuration)
                .SetEase(snapEase)
                .SetUpdate(true)
                .OnUpdate(SyncFromPosition)
                .OnComplete(SyncFromPosition);
        }

        private void KillSnap()
        {
            if (_snapTween == null)
                return;

            _snapTween.Kill();
            _snapTween = null;
        }

        private void SyncFromPosition()
        {
            if (scrollRect == null)
                return;

            int nearest = NormalizedToNearestPage(scrollRect.horizontalNormalizedPosition);

            if (nearest == CurrentPage)
                return;

            CurrentPage = nearest;
            PageChanged?.Invoke(CurrentPage);
        }

        private float PageToNormalized(int page)
        {
            if (PagesCount <= 1)
                return 0f;

            return (float)page / (PagesCount - 1);
        }

        private int NormalizedToNearestPage(float normalized)
        {
            if (PagesCount <= 1)
                return 0;

            float step = 1f / (PagesCount - 1);
            int index = Mathf.RoundToInt(normalized / step);
            return Mathf.Clamp(index, 0, PagesCount - 1);
        }
    }
}
