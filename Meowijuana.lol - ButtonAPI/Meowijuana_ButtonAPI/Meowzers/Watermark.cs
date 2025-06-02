using System;
using System.Globalization;
using UnityEngine;
// IMPORTANT: Add the correct 'using' directive for your static GifLoader class here!
// For example, if your GifLoader is in Meowijuana_ButtonAPI_MONO.Meowzers.Image_System:
using Meowijuana_ButtonAPI_MONO.Meowzers.Image_System; // <--- !!! ADJUST THIS NAMESPACE !!!

namespace Meowijuana_SARS.API.Meowzers // Your namespace
{
    public class Watermark
    {
        public bool IsVisible { get; set; } = true;
        public string CheatName { get; set; } = "Cheatname";
        public string Version { get; set; } = "[Freemium]";
        public string UserName { get; set; } // Set this from your game's player data
        public Color TextColor { get; set; } = Color.white;
        public Color BackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.7f); // Dark semi-transparent
        public int FontSize { get; set; } = 12;
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;
        public TextAnchor Alignment { get; set; } = TextAnchor.UpperRight;
        public float Padding { get; set; } = 10f; // Padding around the content inside the box
        public float SpaceBetweenGifAndText { get; set; } = 5f; // Space between GIF and text

        private GUIStyle _watermarkStyle;
        private GUIStyle _backgroundStyle;
        private float _lastFpsUpdateTime;
        private int _frameCount;
        private float _currentFps;
        private const float FPS_UPDATE_INTERVAL = 0.5f; // Update FPS display twice a second

        public Watermark()
        {
            UserName = "Player"; // Placeholder
        }

        private void InitializeStyles()
        {
            if (_watermarkStyle == null)
            {
                _watermarkStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = FontSize,
                    fontStyle = FontStyle,
                    alignment = TextAnchor.MiddleLeft // Text itself aligns left within its own rect
                };
                _watermarkStyle.normal.textColor = TextColor;
            }
            else
            {
                // Update if properties changed
                if (_watermarkStyle.fontSize != FontSize) _watermarkStyle.fontSize = FontSize;
                if (_watermarkStyle.fontStyle != FontStyle) _watermarkStyle.fontStyle = FontStyle;
                if (_watermarkStyle.normal.textColor != TextColor) _watermarkStyle.normal.textColor = TextColor;
            }

            if (_backgroundStyle == null)
            {
                _backgroundStyle = new GUIStyle(GUI.skin.box);
                Texture2D bgTex = new Texture2D(1, 1);
                bgTex.SetPixel(0, 0, BackgroundColor);
                bgTex.Apply();
                _backgroundStyle.normal.background = bgTex;
                _backgroundStyle.border = new RectOffset(0, 0, 0, 0); // Ensure no default box border styling interferes
            }
            else
            {
                // Update if background color changed
                bool needsUpdate = false;
                if (_backgroundStyle.normal.background == null)
                {
                    needsUpdate = true;
                }
                else
                {
                    try
                    {
                        if (_backgroundStyle.normal.background.GetPixel(0, 0) != BackgroundColor)
                        {
                            needsUpdate = true;
                        }
                    }
                    catch (UnityException) // Catches "Texture 'Texture2D' is not readable"
                    {
                        needsUpdate = true; // Force update if we can't read it
                    }
                }

                if (needsUpdate)
                {
                    if (_backgroundStyle.normal.background != null)
                    {
                        UnityEngine.Object.Destroy(_backgroundStyle.normal.background);
                    }
                    Texture2D bgTex = new Texture2D(1, 1);
                    bgTex.SetPixel(0, 0, BackgroundColor);
                    bgTex.Apply();
                    _backgroundStyle.normal.background = bgTex;
                }
            }
        }

        private void UpdateFPS()
        {
            _frameCount++;
            if (Time.unscaledTime > _lastFpsUpdateTime + FPS_UPDATE_INTERVAL)
            {
                _currentFps = _frameCount / (Time.unscaledTime - _lastFpsUpdateTime);
                _lastFpsUpdateTime = Time.unscaledTime;
                _frameCount = 0;
            }
        }

        public void Render()
        {
            if (!IsVisible) return;

            InitializeStyles(); // Ensure styles are up-to-date if properties changed
            UpdateFPS();

            // --- Prepare Content ---
            string timeString = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string fpsString = $"FPS: {_currentFps:F0}";
            string watermarkTextString = $"{CheatName} {Version}";
            if (!string.IsNullOrEmpty(UserName))
            {
                watermarkTextString += $" | {UserName}";
            }
            watermarkTextString += $" | {timeString} | {fpsString}";

            GUIContent watermarkTextContent = new GUIContent(watermarkTextString);
            Vector2 textSize = _watermarkStyle.CalcSize(watermarkTextContent);

            Texture2D currentGifFrame = null;
            float gifWidth = 0f;
            float gifHeight = 0f;
            bool gifIsPresent = false;

            // Check GifLoader status - Make sure your GifLoader class is static and accessible
            if (GifLoader.IsLoaded && GifLoader.IsPlaying)
            {
                currentGifFrame = GifLoader.GetCurrentFrame();
                if (currentGifFrame != null)
                {
                    gifWidth = currentGifFrame.width;
                    gifHeight = currentGifFrame.height;
                    gifIsPresent = true;
                }
            }

            // --- Calculate Dimensions ---
            float totalContentWidth = textSize.x;
            float totalContentHeight = textSize.y;

            if (gifIsPresent)
            {
                totalContentWidth = gifWidth + SpaceBetweenGifAndText + textSize.x;
                totalContentHeight = Mathf.Max(gifHeight, textSize.y);
            }
            else if (GifLoader.IsLoading) // If GIF is loading, reserve some space for loading text
            {
                GUIContent loadingContent = new GUIContent("[GIF L]");
                Vector2 loadingSize = _watermarkStyle.CalcSize(loadingContent);
                totalContentWidth = loadingSize.x + SpaceBetweenGifAndText + textSize.x;
                totalContentHeight = Mathf.Max(loadingSize.y, textSize.y);
            }


            float boxWidth = totalContentWidth + Padding * 2;
            float boxHeight = totalContentHeight + Padding * 2;

            // --- Calculate Box Position (Alignment) ---
            Rect mainRect = new Rect(0, 0, boxWidth, boxHeight);

            switch (Alignment)
            {
                case TextAnchor.UpperLeft:
                    mainRect.x = Padding; // Screen edge padding
                    mainRect.y = Padding;
                    break;
                case TextAnchor.UpperCenter:
                    mainRect.x = (Screen.width - boxWidth) / 2f;
                    mainRect.y = Padding;
                    break;
                case TextAnchor.UpperRight:
                    mainRect.x = Screen.width - boxWidth - Padding;
                    mainRect.y = Padding;
                    break;
                case TextAnchor.MiddleLeft:
                    mainRect.x = Padding;
                    mainRect.y = (Screen.height - boxHeight) / 2f;
                    break;
                case TextAnchor.MiddleCenter:
                    mainRect.x = (Screen.width - boxWidth) / 2f;
                    mainRect.y = (Screen.height - boxHeight) / 2f;
                    break;
                case TextAnchor.MiddleRight:
                    mainRect.x = Screen.width - boxWidth - Padding;
                    mainRect.y = (Screen.height - boxHeight) / 2f;
                    break;
                case TextAnchor.LowerLeft:
                    mainRect.x = Padding;
                    mainRect.y = Screen.height - boxHeight - Padding;
                    break;
                case TextAnchor.LowerCenter:
                    mainRect.x = (Screen.width - boxWidth) / 2f;
                    mainRect.y = Screen.height - boxHeight - Padding;
                    break;
                case TextAnchor.LowerRight:
                    mainRect.x = Screen.width - boxWidth - Padding;
                    mainRect.y = Screen.height - boxHeight - Padding;
                    break;
            }

            // --- Draw Background ---
            GUI.Box(mainRect, GUIContent.none, _backgroundStyle);

            // --- Draw Content (GIF and Text) ---
            float currentX = mainRect.x + Padding;
            float contentAreaY = mainRect.y + Padding;

            // Draw GIF or Loading Text
            if (gifIsPresent)
            {
                float gifYOffset = (totalContentHeight - gifHeight) / 2f;
                Rect gifRect = new Rect(currentX, contentAreaY + gifYOffset, gifWidth, gifHeight);
                GUI.DrawTexture(gifRect, currentGifFrame);
                currentX += gifWidth + SpaceBetweenGifAndText;
            }
            else if (GifLoader.IsLoading)
            {
                GUIContent loadingContent = new GUIContent("[GIF]"); // Shorter text
                Vector2 loadingSize = _watermarkStyle.CalcSize(loadingContent);
                float loadingYOffset = (totalContentHeight - loadingSize.y) / 2f;
                Rect loadingRect = new Rect(currentX, contentAreaY + loadingYOffset, loadingSize.x, loadingSize.y);
                GUI.Label(loadingRect, loadingContent, _watermarkStyle);
                currentX += loadingSize.x + SpaceBetweenGifAndText;
            }

            // Draw Text
            float textYOffset = (totalContentHeight - textSize.y) / 2f;
            Rect textRect = new Rect(currentX, contentAreaY + textYOffset, textSize.x, textSize.y);
            GUI.Label(textRect, watermarkTextContent, _watermarkStyle);
        }

        // If you need to set username dynamically, you might add a method like this:
        public void SetCurrentUsername(string username)
        {
            UserName = username;
        }
    }
}