using System;
using UnityEngine;
using System.Globalization;

namespace Meowijuana_ButtonAPI.Meowzers
{
    public class Watermark
    {
        public bool IsVisible { get; set; } = true;
        public string CheatName { get; set; } = "Cheatname";
        public string Version { get; set; } = "[Freemium]";
        public string UserName { get; set; } // Set this from your game's player data
        public Color TextColor { get; set; } = Color.white;
        public Color BackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.6f); // Dark semi-transparent
        public int FontSize { get; set; } = 12;
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;
        public TextAnchor Alignment { get; set; } = TextAnchor.UpperRight;
        public float Padding { get; set; } = 5f;

        private GUIStyle _watermarkStyle;
        private GUIStyle _backgroundStyle;
        private float _lastFpsUpdateTime;
        private int _frameCount;
        private float _currentFps;
        private const float FpsUpdateInterval = 0.5f; // Update FPS display twice a second

        public Watermark()
        {
            // Initialize with default or placeholder username if needed
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
                    alignment = TextAnchor.MiddleLeft
                };
                _watermarkStyle.normal.textColor = TextColor;
            }
            else
            {
                _watermarkStyle.fontSize = FontSize;
                _watermarkStyle.fontStyle = FontStyle;
                _watermarkStyle.normal.textColor = TextColor;
            }

            if (_backgroundStyle == null)
            {
                _backgroundStyle = new GUIStyle(GUI.skin.box);
                Texture2D bgTex = new Texture2D(1, 1);
                bgTex.SetPixel(0, 0, BackgroundColor);
                bgTex.Apply();
                _backgroundStyle.normal.background = bgTex;
            }
            else
            {
                if (_backgroundStyle.normal.background != null && 
                    _backgroundStyle.normal.background.GetPixel(0,0) != BackgroundColor)
                {
                    UnityEngine.Object.Destroy(_backgroundStyle.normal.background);
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
            if (Time.unscaledTime > _lastFpsUpdateTime + FpsUpdateInterval)
            {
                _currentFps = _frameCount / (Time.unscaledTime - _lastFpsUpdateTime);
                _lastFpsUpdateTime = Time.unscaledTime;
                _frameCount = 0;
            }
        }

        public void Render()
        {
            if (!IsVisible) return;

            InitializeStyles();
            UpdateFPS();

            string timeString = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string fpsString = $"FPS: {_currentFps:F0}";
            
            string watermarkText = $"{CheatName} {Version}";
            if (!string.IsNullOrEmpty(UserName))
            {
                watermarkText += $" | {UserName}";
            }
            watermarkText += $" | {timeString} | {fpsString}";
            
            Vector2 contentSize = _watermarkStyle.CalcSize(new GUIContent(watermarkText));
            float boxWidth = contentSize.x + Padding * 2;
            float boxHeight = contentSize.y + Padding * 2;

            Rect watermarkRect = new Rect(0,0, boxWidth, boxHeight);

            switch (Alignment)
            {
                case TextAnchor.UpperLeft:
                    watermarkRect.x = Padding;
                    watermarkRect.y = Padding;
                    break;
                case TextAnchor.UpperCenter:
                    watermarkRect.x = (Screen.width - boxWidth) / 2f;
                    watermarkRect.y = Padding;
                    break;
                case TextAnchor.UpperRight:
                    watermarkRect.x = Screen.width - boxWidth - Padding;
                    watermarkRect.y = Padding;
                    break;
                case TextAnchor.MiddleLeft:
                    watermarkRect.x = Padding;
                    watermarkRect.y = (Screen.height - boxHeight) / 2f;
                    break;
                case TextAnchor.MiddleCenter:
                    watermarkRect.x = (Screen.width - boxWidth) / 2f;
                    watermarkRect.y = (Screen.height - boxHeight) / 2f;
                    break;
                case TextAnchor.MiddleRight:
                    watermarkRect.x = Screen.width - boxWidth - Padding;
                    watermarkRect.y = (Screen.height - boxHeight) / 2f;
                    break;
                case TextAnchor.LowerLeft:
                    watermarkRect.x = Padding;
                    watermarkRect.y = Screen.height - boxHeight - Padding;
                    break;
                case TextAnchor.LowerCenter:
                    watermarkRect.x = (Screen.width - boxWidth) / 2f;
                    watermarkRect.y = Screen.height - boxHeight - Padding;
                    break;
                case TextAnchor.LowerRight:
                    watermarkRect.x = Screen.width - boxWidth - Padding;
                    watermarkRect.y = Screen.height - boxHeight - Padding;
                    break;
            }
            
            GUI.Box(watermarkRect, GUIContent.none, _backgroundStyle);
            Rect textRect = new Rect(watermarkRect.x + Padding, watermarkRect.y + Padding, contentSize.x, contentSize.y);
            GUI.Label(textRect, watermarkText, _watermarkStyle);
        }
        
        public void SetCurrentUsername(string username)
        {
            UserName = username;
        }
    }
}