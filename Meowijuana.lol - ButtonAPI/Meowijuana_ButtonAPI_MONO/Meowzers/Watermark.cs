// Watermark.cs
using System;
using System.Globalization;
using UnityEngine;

namespace Meowijuana_ButtonAPI_MONO.Meowzers
{
    public class Watermark : IDisposable
    {
        public bool IsVisible { get; set; } = true;
        public string CheatName { get; set; } = "Cheatname";
        public string Version { get; set; } = "[Freemium]";
        public string UserName { get; set; }

        // Style properties with backing fields to detect changes
        private Color _textColor = Color.white;
        public Color TextColor { get => _textColor; set { if (_textColor != value) { _textColor = value; _stylesNeedUpdate = true; } } }

        private Color _backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        public Color BackgroundColor { get => _backgroundColor; set { if (_backgroundColor != value) { _backgroundColor = value; _stylesNeedUpdate = true; } } }

        private int _fontSize = 12;
        public int FontSize { get => _fontSize; set { if (_fontSize != value) { _fontSize = value; _stylesNeedUpdate = true; } } }

        private FontStyle _fontStyle = FontStyle.Normal;
        // Renamed to avoid conflict with UnityEngine.FontStyle type itself
        public FontStyle WatermarkFontStyle { get => _fontStyle; set { if (_fontStyle != value) { _fontStyle = value; _stylesNeedUpdate = true; } } }

        public TextAnchor Alignment { get; set; } = TextAnchor.UpperRight;
        public float Padding { get; set; } = 5f; // Original padding was 5f

        // For GIF integration (if GIFLOADER_INTEGRATION is defined)
        public float SpaceBetweenGifAndText { get; set; } = 5f;

        private GUIStyle _watermarkStyle;
        private GUIStyle _backgroundStyle;
        private Texture2D _backgroundTexture; // Instance-specific texture

        private bool _stylesNeedUpdate = true;
        private bool _disposed = false;

        private float _lastFpsUpdateTime;
        private int _frameCount;
        private float _currentFps;
        private const float FpsUpdateInterval = 0.5f;

        public Watermark()
        {
            UserName = "Player"; // Placeholder
        }

        private void InitializeOrUpdateStyles()
        {
            if (!_stylesNeedUpdate && _watermarkStyle != null && _backgroundStyle != null && _backgroundTexture != null)
                return;

            // Watermark Text Style
            if (_watermarkStyle == null)
                _watermarkStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };

            _watermarkStyle.fontSize = FontSize;
            _watermarkStyle.fontStyle = WatermarkFontStyle; // Use the renamed property
            _watermarkStyle.normal.textColor = TextColor;

            // Background Box Style
            if (_backgroundStyle == null)
                _backgroundStyle = new GUIStyle(GUI.skin.box) { border = new RectOffset(0, 0, 0, 0) }; // Ensure no default border

            // Efficiently update or create the background texture
            bool recreateTexture = _backgroundTexture == null;
            if (!recreateTexture)
            {
                try
                {
                    if (_backgroundTexture.GetPixel(0, 0) != BackgroundColor) recreateTexture = true;
                }
                catch { recreateTexture = true; } // Texture might not be readable
            }

            if (recreateTexture)
            {
                if (_backgroundTexture != null) UnityEngine.Object.Destroy(_backgroundTexture);
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
            if (Time.unscaledTime > _lastFpsUpdateTime + FpsUpdateInterval)
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

            string timeString = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string fpsString = $"FPS: {_currentFps:F0}";
            string watermarkTextString = $"{CheatName} {Version}";
            if (!string.IsNullOrEmpty(UserName)) watermarkTextString += $" | {UserName}";
            watermarkTextString += $" | {timeString} | {fpsString}";

            GUIContent watermarkTextContent = new GUIContent(watermarkTextString);
            Vector2 textSize = _watermarkStyle.CalcSize(watermarkTextContent);

            float totalContentWidth = textSize.x;
            float totalContentHeight = textSize.y;

#if GIFLOADER_INTEGRATION
            Texture2D currentGifFrame = null;
            float gifWidth = 0f, gifHeight = 0f;
            bool gifIsPresent = false, gifIsActuallyLoading = false;
            GUIContent loadingTextContent = null; Vector2 loadingTextSize = Vector2.zero;

            try {
                gifIsActuallyLoading = GifLoader.IsLoading;
                if (GifLoader.IsLoaded && GifLoader.IsPlaying) {
                    currentGifFrame = GifLoader.GetCurrentFrame();
                    if (currentGifFrame != null) {
                        gifWidth = currentGifFrame.width; gifHeight = currentGifFrame.height;
                        gifIsPresent = true;
                    }
                }
            } catch (Exception ex) { Debug.LogWarning($"[Watermark.MONO] GifLoader access error: {ex.Message}"); }

            if (gifIsActuallyLoading && !gifIsPresent) {
                loadingTextContent = new GUIContent("[GIF]");
                loadingTextSize = _watermarkStyle.CalcSize(loadingTextContent);
            }

            if (gifIsPresent) {
                totalContentWidth = gifWidth + SpaceBetweenGifAndText + textSize.x;
                totalContentHeight = Mathf.Max(gifHeight, textSize.y);
            } else if (gifIsActuallyLoading) {
                totalContentWidth = loadingTextSize.x + SpaceBetweenGifAndText + textSize.x;
                totalContentHeight = Mathf.Max(loadingTextSize.y, textSize.y);
            }
#endif

            float boxWidth = totalContentWidth + Padding * 2;
            float boxHeight = totalContentHeight + Padding * 2;
            Rect mainRect = new Rect(0, 0, boxWidth, boxHeight);

            // Alignment logic (same as original)
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

            GUI.Box(mainRect, GUIContent.none, _backgroundStyle);

            float currentX = mainRect.x + Padding;
            float contentAreaY = mainRect.y + Padding;

#if GIFLOADER_INTEGRATION
            if (gifIsPresent) {
                Rect gifRect = new Rect(currentX, contentAreaY + (totalContentHeight - gifHeight) / 2f, gifWidth, gifHeight);
                GUI.DrawTexture(gifRect, currentGifFrame);
                currentX += gifWidth + SpaceBetweenGifAndText;
            } else if (gifIsActuallyLoading) {
                Rect loadingRect = new Rect(currentX, contentAreaY + (totalContentHeight - loadingTextSize.y) / 2f, loadingTextSize.x, loadingTextSize.y);
                GUI.Label(loadingRect, loadingTextContent, _watermarkStyle);
                currentX += loadingTextSize.x + SpaceBetweenGifAndText;
            }
#endif

            // Vertically center the main text content within the available height
            Rect textRect = new Rect(currentX, contentAreaY + (totalContentHeight - textSize.y) / 2f, textSize.x, textSize.y);
            GUI.Label(textRect, watermarkTextContent, _watermarkStyle);
        }

        public void SetCurrentUsername(string username) => UserName = username;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (_backgroundTexture != null)
                {
                    UnityEngine.Object.Destroy(_backgroundTexture);
                    _backgroundTexture = null;
                }
            }
            _disposed = true;
        }
        ~Watermark() { Dispose(false); }
    }
}