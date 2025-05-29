using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using UnityEngine.Networking; // For MelonCoroutines and MelonLogger

// Make sure you have the correct 'using' directive for UniGif
// e.g., using UniGif;

namespace Meowijuana_ButtonAPI.Meowzers.Image_System
{
    public static class GifLoader
    {
        private static Texture2D[] _gifFrames;
        private static float[] _frameDelays;
        private static int _currentFrameIndex;
        private static Coroutine _animationCoroutine;
        private static Coroutine _loadingCoroutine; // To manage the loading (download/decode) process

        public static bool IsLoaded { get; private set; }
        public static bool IsPlaying { get; private set; }
        public static bool IsLoading { get; private set; }

        /// <summary>
        /// Loads a GIF from the specified local file path and starts playing it.
        /// </summary>
        /// <param name="filePath">The full path to the GIF file.</param>
        /// <param name="onLoadComplete">Optional callback: Action invoked with true if load/decode was successful, false otherwise.</param>
        public static void LoadAndPlayGifFromFile(string filePath, System.Action<bool> onLoadComplete = null)
        {
            if (IsLoading)
            {
                MelonLogger.Warning("[P.L.GIF] Another GIF loading process is already in progress. Please wait or cancel.");
                onLoadComplete?.Invoke(false);
                return;
            }
            
            PrepareForNewLoad();

            if (!File.Exists(filePath))
            {
                MelonLogger.Error($"[P.L.GIF] GIF file not found: {filePath}");
                onLoadComplete?.Invoke(false);
                return;
            }

            try
            {
                byte[] gifData = File.ReadAllBytes(filePath);
                IsLoading = true;
                _loadingCoroutine = MelonCoroutines.Start(DecodeGifCoroutine(gifData, (success) => {
                    IsLoading = false;
                    onLoadComplete?.Invoke(success);
                })) as Coroutine;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[P.L.GIF] Error reading GIF file {filePath}: {ex.Message}");
                IsLoading = false;
                onLoadComplete?.Invoke(false);
            }
        }

        /// <summary>
        /// Loads a GIF from the specified URI (e.g., HTTP/HTTPS URL) and starts playing it.
        /// </summary>
        /// <param name="uri">The URI of the GIF file.</param>
        /// <param name="onLoadComplete">Optional callback: Action invoked with true if load/decode was successful, false otherwise.</param>
        public static void LoadAndPlayGifFromUri(string uri, System.Action<bool> onLoadComplete = null)
        {
            if (IsLoading)
            {
                MelonLogger.Warning("[P.L.GIF] Another GIF loading process is already in progress. Please wait or cancel.");
                onLoadComplete?.Invoke(false);
                return;
            }

            PrepareForNewLoad();

            if (string.IsNullOrEmpty(uri) || !System.Uri.IsWellFormedUriString(uri, System.UriKind.Absolute))
            {
                MelonLogger.Error($"[P.L.GIF] Invalid URI: {uri}");
                onLoadComplete?.Invoke(false);
                return;
            }
            
            IsLoading = true;
            _loadingCoroutine = MelonCoroutines.Start(DownloadAndDecodeGifCoroutine(uri, (success) => {
                IsLoading = false;
                onLoadComplete?.Invoke(success);
            })) as Coroutine;
        }

        private static void PrepareForNewLoad()
        {
            StopAnimation();
            UnloadGifResources(); // Clear any previous GIF data

            if (_loadingCoroutine != null) // Cancel any ongoing previous load
            {
                MelonCoroutines.Stop(_loadingCoroutine);
                _loadingCoroutine = null;
                IsLoading = false; // Should be reset by the coroutine itself, but good for safety
            }
        }

        private static IEnumerator DownloadAndDecodeGifCoroutine(string uri, System.Action<bool> onComplete)
        {
            MelonLogger.Msg($"[P.L.GIF] Starting download from URI: {uri}");
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                MelonLogger.Error($"[P.L.GIF] Error downloading GIF from {uri}: {webRequest.error}");
                onComplete?.Invoke(false);
                yield break;
            }
            MelonLogger.Msg($"[P.L.GIF] Download successful. Size: {webRequest.downloadHandler.data.Length} bytes. Decoding...");
            byte[] gifData = webRequest.downloadHandler.data;
            yield return MelonCoroutines.Start(DecodeGifCoroutine(gifData, onComplete));
        }


        private static IEnumerator DecodeGifCoroutine(byte[] gifData, System.Action<bool> onComplete)
        {
            List<UniGif.GifTexture> decodedGifTextures = null;
            bool success;

            // UniGif.GetTextureListCoroutine is itself a coroutine.
            // We yield it to wait for its completion.
            yield return UniGif.GetTextureListCoroutine(
                gifData,
                (textures, loopCount, width, height) => { decodedGifTextures = textures; },
                filterMode: FilterMode.Bilinear,
                wrapMode: TextureWrapMode.Clamp
            );

            if (decodedGifTextures != null && decodedGifTextures.Count > 0)
            {
                _gifFrames = decodedGifTextures.Select(f => f.m_texture2d).ToArray();
                _frameDelays = decodedGifTextures.Select(f => f.m_delaySec).ToArray();

                for (int i = 0; i < _frameDelays.Length; i++)
                {
                    if (_frameDelays[i] <= 0.01f)
                    {
                        _frameDelays[i] = 0.1f;
                    }
                }

                IsLoaded = true;
                success = true;
                MelonLogger.Msg($"[P.L.GIF] GIF decoded successfully. Frames: {_gifFrames.Length}");
                StartAnimationInternal();
            }
            else
            {
                MelonLogger.Error("[P.L.GIF] Failed to decode GIF or GIF contains no frames.");
                IsLoaded = false;
                success = false;
            }
            onComplete?.Invoke(success);
        }

        private static void StartAnimationInternal()
        {
            if (!IsLoaded || _gifFrames == null || _gifFrames.Length == 0)
            {
                MelonLogger.Warning("[P.L.GIF] Cannot start animation: GIF not loaded or no frames.");
                return;
            }
            if (IsPlaying) return;

            IsPlaying = true;
            _currentFrameIndex = 0;
            if (_animationCoroutine != null) MelonCoroutines.Stop(_animationCoroutine); // Stop previous if any
            _animationCoroutine = MelonCoroutines.Start(PlayGifAnimationLoop()) as Coroutine;
            MelonLogger.Msg("[P.L.GIF] GIF animation started.");
        }

        private static IEnumerator PlayGifAnimationLoop()
        {
            while (IsPlaying && IsLoaded && _gifFrames != null && _gifFrames.Length > 0)
            {
                yield return new WaitForSeconds(_frameDelays[_currentFrameIndex]);
                _currentFrameIndex = (_currentFrameIndex + 1) % _gifFrames.Length;
            }
            IsPlaying = false;
        }

        public static Texture2D GetCurrentFrame()
        {
            if (!IsLoaded || !IsPlaying || _gifFrames == null || _gifFrames.Length == 0)
            {
                return null;
            }
            int safeIndex = Mathf.Clamp(_currentFrameIndex, 0, _gifFrames.Length - 1);
            return _gifFrames[safeIndex];
        }

        public static void StopAnimation()
        {
            if (!IsPlaying && _animationCoroutine == null) return;

            IsPlaying = false;
            if (_animationCoroutine != null)
            {
                MelonCoroutines.Stop(_animationCoroutine);
                _animationCoroutine = null;
            }
            //MelonLogger.Msg("[P.L.GIF] GIF animation stopped."); // Can be a bit noisy if called often
        }

        public static void UnloadGifResources()
        {
            StopAnimation();

            if (_loadingCoroutine != null) // Stop any ongoing loading process too
            {
                MelonCoroutines.Stop(_loadingCoroutine);
                _loadingCoroutine = null;
                IsLoading = false;
            }

            if (_gifFrames != null)
            {
                foreach (Texture2D frame in _gifFrames)
                {
                    if (frame != null)
                    {
                        Object.Destroy(frame);
                    }
                }
                _gifFrames = null;
            }

            _frameDelays = null;
            _currentFrameIndex = 0;
            IsLoaded = false;
            // IsPlaying should already be false from StopAnimation()
            // MelonLogger.Msg("[P.L.GIF] GIF resources unloaded."); // Can be noisy
        }
    }
}