

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Scripts.Services
{
    [DisallowMultipleComponent]
    public sealed class RemoteTextureLoaderService : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxConcurrentDownloads = 6;
        [SerializeField, Min(1)] private int maxCacheSize = 128;

        private readonly Dictionary<string, Texture2D> _cache = new();
        private readonly Dictionary<string, TaskCompletionSource<Texture2D>> _inFlight = new();

        private SemaphoreSlim _semaphore;

        private void Awake()
        {
            _semaphore = new SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);
        }

        private void OnDestroy()
        {
            ClearCache();
            _semaphore?.Dispose();
            _semaphore = null;
        }

        public bool TryGetCached(string url, out Texture2D texture)
        {
            if (string.IsNullOrEmpty(url))
            {
                texture = null;
                return false;
            }

            return _cache.TryGetValue(url, out texture) && texture != null;
        }

        public UniTask<Texture2D> LoadAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                return UniTask.FromResult<Texture2D>(null);

            if (TryGetCached(url, out Texture2D cached))
                return UniTask.FromResult(cached);

            if (_inFlight.TryGetValue(url, out TaskCompletionSource<Texture2D> existing))
            {
                return existing.Task.AsUniTask();
            }

            var tcs = new TaskCompletionSource<Texture2D>(TaskCreationOptions.RunContinuationsAsynchronously);
            _inFlight[url] = tcs;

            DownloadAndCompleteAsync(url, tcs).Forget();

            return tcs.Task.AsUniTask();
        }
        
        public void ClearCache()
        {
            foreach (var pair in _cache)
            {
                if (pair.Value != null)
                    Destroy(pair.Value);
            }

            _cache.Clear();
        }

        private async UniTaskVoid DownloadAndCompleteAsync(string url, TaskCompletionSource<Texture2D> tcs)
        {
            try
            {
                if (_semaphore != null)
                    await _semaphore.WaitAsync(this.GetCancellationTokenOnDestroy());

                using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                await request.SendWebRequest().ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

#if UNITY_2020_2_OR_NEWER
                bool hasError = request.result != UnityWebRequest.Result.Success;
#else
                bool hasError = request.isNetworkError || request.isHttpError;
#endif
                if (hasError)
                {
                    Debug.LogWarning($"RemoteTextureLoaderUniTask: failed to load {url}. Error: {request.error}", this);
                    tcs.TrySetResult(null);
                    return;
                }

                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture == null)
                {
                    tcs.TrySetResult(null);
                    return;
                }

                texture.name = $"RemoteTexture: {url}";

                if (_cache.Count >= maxCacheSize)
                    ClearCache();

                _cache[url] = texture;

                tcs.TrySetResult(texture);
            }
            catch (OperationCanceledException)
            {
                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"RemoteTextureLoaderUniTask: exception while loading {url}. {ex}", this);
                tcs.TrySetResult(null);
            }
            finally
            {
                _inFlight.Remove(url);

                if (_semaphore != null)
                {
                    try { _semaphore.Release(); } catch { }
                }
            }
        }
    }
}
