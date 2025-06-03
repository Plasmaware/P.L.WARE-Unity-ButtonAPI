using System;
using System.Globalization;
using Meowijuana_ButtonAPI.Meowzers.Image_System;
using UnityEngine;
// For CultureInfo.InvariantCulture

namespace Meowijuana_ButtonAPI.API.Meowzers
{
    public class Watermark : IDisposable
    {
        public bool IsVisible { get; set; } = true;
        public string CheatName { get; set; } = "CheatName";
        public string Version { get; set; } = "[v1.0]";
        public string UserName { get; set; }
        // Style properties with backing fields to detect changes
        private Color _textColor = Color.white;
        public Color TextColor { get => _textColor; set { if (_textColor != value) { _textColor = value; _stylesNeedUpdate = true; } } }
        private Color _backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.7f);
        public Color BackgroundColor { get => _backgroundColor; set { if (_backgroundColor != value) { _backgroundColor = value; _stylesNeedUpdate = true; } } }
        private int _fontSize = 12;
        public int FontSize { get => _fontSize; set { if (_fontSize != value) { _fontSize = value; _stylesNeedUpdate = true; } } }
        private FontStyle _fontStyle = FontStyle.Normal;
        public FontStyle FontStyleMember { get => _fontStyle; set { if (_fontStyle != value) { _fontStyle = value; _stylesNeedUpdate = true; } } } // Renamed to avoid conflict with type name

        public TextAnchor Alignment { get; set; } = TextAnchor.UpperRight;
        public float Padding { get; set; } = 10f;
        public float SpaceBetweenGifAndText { get; set; } = 5f;

        private GUIStyle _watermarkStyle;
        private GUIStyle _backgroundStyle;
        private Texture2D _backgroundTexture; // UnityEngine.Object, needs careful handling

        private bool _stylesNeedUpdate = true;
        private bool _disposed = false;

        private float _lastFpsUpdateTime;
        private int _frameCount;
        private float _currentFps;
        private const float FPS_UPDATE_INTERVAL = 0.5f;

        public Watermark()
        {
            UserName = "Player"; // Default
        }

        private void InitializeOrUpdateStyles()
        {
            if (!_stylesNeedUpdate && _watermarkStyle != null && _backgroundStyle != null) return;

            if (_watermarkStyle == null)
                _watermarkStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft }; // Standard practice

            _watermarkStyle.fontSize = FontSize;
            _watermarkStyle.fontStyle = FontStyleMember; // Property renamed to FontStyleMember
            _watermarkStyle.normal.textColor = TextColor;

            if (_backgroundStyle == null)
                _backgroundStyle = new GUIStyle(GUI.skin.box) { border = new RectOffset(0, 0, 0, 0) };

            // Only recreate texture if color changed or texture doesn't exist
            // IL2CPP: Texture2D creation and modification is standard.
            // HideFlags.HideAndDontSave is crucial for runtime textures.
            if (_backgroundTexture == null || _backgroundTexture.GetPixel(0, 0) != BackgroundColor) // Crude check, assumes 1x1. For more complex scenarios, store last set color.
            {
                if (_backgroundTexture != null) UnityEngine.Object.Destroy(_backgroundTexture); // Correctly destroy old texture
                _backgroundTexture = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
                _backgroundTexture.SetPixel(0, 0, BackgroundColor);
                _backgroundTexture.Apply();
                _backgroundStyle.normal.background = _backgroundTexture;
            }
            _stylesNeedUpdate = false;
        }

        private void UpdateFPS()
        {
            _frameCount++;
            // Time.unscaledTime is appropriate for UI elements like FPS counters.
            if (Time.unscaledTime > _lastFpsUpdateTime + FPS_UPDATE_INTERVAL)
            {
                _currentFps = _frameCount / (Time.unscaledTime - _lastFpsUpdateTime);
                _lastFpsUpdateTime = Time.unscaledTime;
                _frameCount = 0;
            }
        }

        public void Render()
        {
            if (!IsVisible || _disposed) return;

            InitializeOrUpdateStyles();
            UpdateFPS();

            // DateTime.ToString can cause some allocation. For an element like a watermark, this is usually acceptable.
            // CultureInfo.InvariantCulture is good for consistent formatting.
            string timeString = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string fpsString = $"FPS: {_currentFps:F0}";
            string watermarkTextString = $"{CheatName} {Version}";
            if (!string.IsNullOrEmpty(UserName)) watermarkTextString += $" | {UserName}";
            watermarkTextString += $" | {timeString} | {fpsString}";
            // IL2CPP: String concatenations and interpolations are handled. For extreme performance, StringBuilder could be used, but likely overkill here.

            GUIContent watermarkTextContent = new GUIContent(watermarkTextString);
            Vector2 textSize = _watermarkStyle.CalcSize(watermarkTextContent); // Standard GUI method.

            Texture2D currentGifFrame = null;
            float gifWidth = 0f, gifHeight = 0f;
            bool gifIsPresent = false, gifIsActuallyLoading = false;

            try // Safe access to GifLoader. IL2CPP: try-catch has some overhead but is fine for non-critical paths or error handling.
            {
                // Assuming GifLoader is an external component. Its IL2CPP compatibility is separate.
                // This code interacts with it defensively.
                gifIsActuallyLoading = GifLoader.IsLoading;
                if (GifLoader.IsLoaded && GifLoader.IsPlaying)
                {
                    currentGifFrame = GifLoader.GetCurrentFrame();
                    if (currentGifFrame != null)
                    {
                        gifWidth = currentGifFrame.width; gifHeight = currentGifFrame.height;
                        gifIsPresent = true;
                    }
                }
            }
            // It's good practice to catch specific exceptions if possible, rather than generic System.Exception.
            catch (Exception ex) { Debug.LogWarning($"[Watermark] GifLoader access error: {ex.Message}"); }


            float totalContentWidth = textSize.x;
            float totalContentHeight = textSize.y;
            GUIContent loadingContent = null; Vector2 loadingSize = Vector2.zero;

            if (gifIsActuallyLoading && !gifIsPresent)
            {
                loadingContent = new GUIContent("[GIF]");
                loadingSize = _watermarkStyle.CalcSize(loadingContent);
            }

            if (gifIsPresent)
            {
                totalContentWidth = gifWidth + SpaceBetweenGifAndText + textSize.x;
                totalContentHeight = Mathf.Max(gifHeight, textSize.y); // Mathf.Max is IL2CPP-friendly.
            }
            else if (gifIsActuallyLoading)
            {
                totalContentWidth = loadingSize.x + SpaceBetweenGifAndText + textSize.x;
                totalContentHeight = Mathf.Max(loadingSize.y, textSize.y);
            }

            float boxWidth = totalContentWidth + Padding * 2;
            float boxHeight = totalContentHeight + Padding * 2;
            Rect mainRect = new Rect(0, 0, boxWidth, boxHeight); // Rect is a struct, efficient.

            // Alignment logic using Screen.width/height is standard.
            switch (Alignment)
            {
                case TextAnchor.UpperLeft: mainRect.x = Padding; mainRect.y = Padding; break;
                case TextAnchor.UpperCenter: mainRect.x = (Screen.width - boxWidth) / 2f; mainRect.y = Padding; break;
                case TextAnchor.UpperRight: mainRect.x = Screen.width - boxWidth - Padding; mainRect.y = Padding; break;
                case TextAnchor.MiddleLeft: mainRect.x = Padding; mainRect.y = (Screen.height - boxHeight) / 2f; break;
                case TextAnchor.MiddleCenter: mainRect.x = (Screen.width - boxWidth) / 2f; mainRect.y = (Screen.height - boxHeight) / 2f; break;
                case TextAnchor.MiddleRight: mainRect.x = Screen.width - boxWidth - Padding; mainRect.y = (Screen.height - boxHeight) / 2f; break;
                case TextAnchor.LowerLeft: mainRect.x = Padding; mainRect.y = Screen.height - boxHeight - Padding; break;
                case TextAnchor.LowerCenter: mainRect.x = (Screen.width - boxWidth) / 2f; mainRect.y = Screen.height - boxHeight - Padding; break;
                case TextAnchor.LowerRight: mainRect.x = Screen.width - boxWidth - Padding; mainRect.y = Screen.height - boxHeight - Padding; break;
            }

            GUI.Box(mainRect, GUIContent.none, _backgroundStyle); // Standard GUI calls.

            float currentX = mainRect.x + Padding;
            float contentAreaY = mainRect.y + Padding;

            if (gifIsPresent)
            {
                Rect gifRect = new Rect(currentX, contentAreaY + (totalContentHeight - gifHeight) / 2f, gifWidth, gifHeight);
                GUI.DrawTexture(gifRect, currentGifFrame);
                currentX += gifWidth + SpaceBetweenGifAndText;
            }
            else if (gifIsActuallyLoading)
            {
                Rect loadingRect = new Rect(currentX, contentAreaY + (totalContentHeight - loadingSize.y) / 2f, loadingSize.x, loadingSize.y);
                GUI.Label(loadingRect, loadingContent, _watermarkStyle);
                currentX += loadingSize.x + SpaceBetweenGifAndText;
            }

            Rect textRect = new Rect(currentX, contentAreaY + (totalContentHeight - textSize.y) / 2f, textSize.x, textSize.y);
            GUI.Label(textRect, watermarkTextContent, _watermarkStyle);
        }

        // Proper IDisposable implementation for cleaning up native resources (Texture2D)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevents finalizer from running if Dispose is called.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Dispose managed state (managed objects).
                // (No managed objects that need explicit disposal here besides _backgroundTexture which is a Unity Object)
            }

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            if (_backgroundTexture != null)
            {
                UnityEngine.Object.Destroy(_backgroundTexture); // Crucial for Unity objects
                _backgroundTexture = null;
            }
            _disposed = true;
        }

        // Finalizer: Fallback for cleanup if Dispose() isn't called.
        // IL2CPP: Finalizers work but have performance implications if frequently created/destroyed.
        // For a long-lived object like a watermark, this is fine.
        ~Watermark()
        {
            Dispose(false);
        }
    }
}