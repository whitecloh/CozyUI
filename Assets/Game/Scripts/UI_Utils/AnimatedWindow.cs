using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.UI_Utils
{

    [RequireComponent(typeof(Animator))]
    public abstract class AnimatedWindow : UIWindow
    {
        [Header("Optional UI blocker (recommended)")] [SerializeField]
        private RectTransform clicksBlocker;

        [Header("Animator States")] [SerializeField]
        private string openStateName = "Open";
        
        [SerializeField] private string closeStateName = "Close";

        [Header("Timings")] [SerializeField, Min(0f)]
        private float openTime = 0.3f;

        [SerializeField, Min(0f)] private float closeTime = 0.5f;
        [SerializeField] private bool useUnscaledTime = true;

        private Animator _animator;
        private Coroutine _routine;

        private bool IsOpening { get; set; }
        private bool IsClosing { get; set; }

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            SetClicksBlocked(false);
        }

        public void Show()
        {
            if (IsOpen || IsOpening || IsClosing) return;

            IsOpening = true;

            gameObject.SetActive(true);
            SetClicksBlocked(true);

            OnBeforeShow();

            PlayState(openStateName);
            StartRoutine(OpenWait(openTime));
        }

        public void Hide(Action onComplete = null)
        {
            if (!IsOpen || IsOpening || IsClosing) return;

            IsClosing = true;
            SetClicksBlocked(true);

            OnBeforeHide();

            PlayState(closeStateName);
            StartRoutine(CloseWait(closeTime, onComplete));
        }

        private void StartRoutine(IEnumerator routine)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(routine);
        }

        private IEnumerator OpenWait(float time)
        {
            if (time > 0f)
            {
                yield return Wait(time);
            }

            IsOpening = false;
            
            MarkOpen();
            SetClicksBlocked(false);
            
            OpenPanels();

            OnAfterShow();
        }

        private IEnumerator CloseWait(float time, Action onComplete)
        {
            if (time > 0f)
            {
                yield return Wait(time);
            }

            IsClosing = false;
            
            ClosePanels();
            MarkClosed();

            SetClicksBlocked(false);
            OnAfterHide();

            gameObject.SetActive(false);

            onComplete?.Invoke();
        }

        private IEnumerator Wait(float time)
        {
            float elapsed = 0f;

            while (elapsed < time)
            {
                float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                elapsed += dt;
                yield return null;
            }
        }

        private void PlayState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) return;
            _animator.Play(stateName, 0, 0f);
        }

        private void SetClicksBlocked(bool isBlocked)
        {
            if (clicksBlocker == null) return;
            clicksBlocker.gameObject.SetActive(isBlocked);
        }

        protected virtual void OnBeforeShow()
        {
        }

        protected virtual void OnAfterShow()
        {
        }

        protected virtual void OnBeforeHide()
        {
        }

        protected virtual void OnAfterHide()
        {
        }
    }
}