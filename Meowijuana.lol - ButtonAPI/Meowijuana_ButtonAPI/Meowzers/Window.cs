// Window.cs
using System;
using UnityEngine;

namespace Meowijuana_ButtonAPI.API.Meowzers
{
    public class Window
    {
        public Rect CurrentRect { get; private set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public int ID { get; private set; }
        public GUIStyle Style { get; set; } // Instance-specific style override

        // --- Static Default Styles (Initialized once) ---
        private static GUIStyle _sDefaultWindowStyle;
        private static GUIStyle _sDefaultSectionStyle;
        private static GUIStyle _sDefaultTitleStyle; // For section titles
        private static GUIStyle _sDefaultLabelStyle;
        private static GUIStyle _sDefaultButtonStyle;
        private static GUIStyle _sDefaultToggleStyle;
        private static GUIStyle _sDefaultTextFieldStyle;
        private static GUIStyle _sDefaultTextAreaStyle;
        private static GUIStyle _sDefaultHorizontalSliderStyle;
        private static GUIStyle _sDefaultHorizontalSliderThumbStyle;


        private static bool _sStylesInitialized = false;

        // Textures for default styles - managed statically and created once.
        // Add HideFlags.HideAndDontSave to prevent them from being saved with scenes
        // and to ensure they are cleaned up when the AppDomain unloads.
        private static Texture2D _sTexWindowBg;
        private static Texture2D _sTexSectionBg;
        private static Texture2D _sTexButtonNormal;
        private static Texture2D _sTexButtonHover;
        private static Texture2D _sTexButtonActive;
        private static Texture2D _sTexToggleNormalOff;
        private static Texture2D _sTexToggleHoverOff;
        private static Texture2D _sTexToggleNormalOn;
        private static Texture2D _sTexToggleHoverOn;
        private static Texture2D _sTexTextFieldBg;


        // Public accessors for default styles
        public static GUIStyle DefaultWindowStyle => GetEnsuredStyle(ref _sDefaultWindowStyle);
        public static GUIStyle DefaultSectionStyle => GetEnsuredStyle(ref _sDefaultSectionStyle);
        public static GUIStyle DefaultTitleStyle => GetEnsuredStyle(ref _sDefaultTitleStyle);
        public static GUIStyle DefaultLabelStyle => GetEnsuredStyle(ref _sDefaultLabelStyle);
        public static GUIStyle DefaultButtonStyle => GetEnsuredStyle(ref _sDefaultButtonStyle);
        public static GUIStyle DefaultToggleStyle => GetEnsuredStyle(ref _sDefaultToggleStyle);
        public static GUIStyle DefaultTextFieldStyle => GetEnsuredStyle(ref _sDefaultTextFieldStyle);
        public static GUIStyle DefaultTextAreaStyle => GetEnsuredStyle(ref _sDefaultTextAreaStyle); // Often same as TextField
        public static GUIStyle DefaultHorizontalSliderStyle => GetEnsuredStyle(ref _sDefaultHorizontalSliderStyle);
        public static GUIStyle DefaultHorizontalSliderThumbStyle => GetEnsuredStyle(ref _sDefaultHorizontalSliderThumbStyle);


        private static GUIStyle GetEnsuredStyle(ref GUIStyle styleField)
        {
            EnsureStylesInitialized();
            return styleField;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) pix[i] = col;
            Texture2D result = new Texture2D(width, height) { hideFlags = HideFlags.HideAndDontSave };
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Initializes all default GUIStyles. Call this once during your mod's startup.
        /// </summary>
        public static void EnsureStylesInitialized()
        {
            if (_sStylesInitialized) return;

            // --- Create Textures ---
            Color grayDeepPlumBlack = new Color(0.0425f, 0.0425f, 0.0425f, 0.92f);

            // Original: darkRosewood = new Color(0.15f, 0.05f, 0.1f, 1f);
            Color grayDarkRosewood = new Color(0.0856f, 0.0856f, 0.0856f, 1f);

            // Original: charcoalPinkHint = new Color(0.2f, 0.1f, 0.15f, 1f);
            Color grayCharcoalPinkHint = new Color(0.1356f, 0.1356f, 0.1356f, 1f);

            // Original: vibrantFuchsia = new Color(1f, 0.15f, 0.6f, 1f);
            Color grayVibrantFuchsia = new Color(0.45545f, 0.45545f, 0.45545f, 1f); // Medium Gray

            // Original: hotPinkHover = new Color(1f, 0.35f, 0.7f, 1f);
            Color grayHotPinkHover = new Color(0.58425f, 0.58425f, 0.58425f, 1f); // Lighter Medium Gray

            // Original: electricPinkActive = new Color(1f, 0.05f, 0.5f, 1f);
            Color grayElectricPinkActive = new Color(0.38535f, 0.38535f, 0.38535f, 1f); // Darker Medium Gray

            _sTexWindowBg = MakeTex(2, 2, grayDeepPlumBlack);
            _sTexSectionBg = MakeTex(2, 2, grayDarkRosewood);

            _sTexButtonNormal = MakeTex(2, 2, grayCharcoalPinkHint);
            _sTexButtonOnNormal = MakeTex(2, 2, grayVibrantFuchsia);    // Was the main "ON" pink, now a medium gray
            _sTexButtonHover = MakeTex(2, 2, grayHotPinkHover);         // Was brighter pink hover, now a lighter medium gray
            _sTexButtonActive = MakeTex(2, 2, grayElectricPinkActive);  // Was intense pink, now a darker medium gray

            _sTexToggleNormal = MakeTex(2, 2, grayCharcoalPinkHint);
            _sTexToggleOnNormal = MakeTex(2, 2, grayVibrantFuchsia);    // "ON" state will be medium gray
            _sTexToggleHover = MakeTex(2, 2, grayHotPinkHover);         // Hover will be lighter medium gray
            _sTexToggleActive = MakeTex(2, 2, grayElectricPinkActive);  // Active/pressed will be darker medium gray

            _sTexTextFieldBg = MakeTex(2, 2, grayDarkRosewood);         // Consistent with section backgrounds


            // --- Create Styles ---
            _sDefaultWindowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = _sTexWindowBg, textColor = Color.white },
                active = { background = _sTexWindowBg, textColor = Color.white }, // Keep consistent
                focused = { background = _sTexWindowBg, textColor = Color.white },
                hover = { background = _sTexWindowBg, textColor = Color.white },
                onNormal = { background = _sTexWindowBg, textColor = Color.white },
                onActive = { background = _sTexWindowBg, textColor = Color.white },
                onFocused = { background = _sTexWindowBg, textColor = Color.white },
                onHover = { background = _sTexWindowBg, textColor = Color.white },
                border = new RectOffset(6, 6, 6, 6),
                padding = new RectOffset(8, 8, 22, 8) // Top padding for title
            };
            _sDefaultWindowStyle.name = "PLWare.DefaultWindow";

            _sDefaultSectionStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = _sTexSectionBg },
                border = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(5, 5, 5, 5)
            };
            _sDefaultSectionStyle.name = "PLWare.DefaultSection";

            _sDefaultLabelStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) }, // Slightly off-white
                padding = new RectOffset(2, 2, 3, 3) // More balanced padding
            };
            _sDefaultLabelStyle.name = "PLWare.DefaultLabel";

            _sDefaultTitleStyle = new GUIStyle(_sDefaultLabelStyle) // Inherit from label
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                padding = new RectOffset(5, 5, 5, 5)
            };
            _sDefaultTitleStyle.name = "PLWare.DefaultTitle";


            _sDefaultButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = _sTexButtonNormal, textColor = Color.white },
                hover = { background = _sTexButtonHover, textColor = Color.white },
                active = { background = _sTexButtonActive, textColor = new Color(0.8f, 1f, 1f, 1f) }, // Cyanish text on active
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(8, 8, 6, 6),
                alignment = TextAnchor.MiddleCenter
            };
            _sDefaultButtonStyle.name = "PLWare.DefaultButton";

            _sDefaultToggleStyle = new GUIStyle(GUI.skin.toggle) // Base on skin.toggle for checkbox area
            {
                normal = { background = _sTexToggleNormalOff, textColor = Color.white },
                hover = { background = _sTexToggleHoverOff, textColor = Color.white },
                active = { background = _sTexButtonActive, textColor = Color.cyan }, // When clicking
                onNormal = { background = _sTexToggleNormalOn, textColor = Color.white }, // ON state
                onHover = { background = _sTexToggleHoverOn, textColor = Color.white },   // ON state + hover
                onActive = { background = _sTexToggleNormalOn, textColor = Color.cyan },  // ON state + click
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(20, 4, 4, 4) // Left padding for toggle box
            };
            _sDefaultToggleStyle.name = "PLWare.DefaultToggle";

            _sDefaultTextFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                normal = { background = _sTexTextFieldBg, textColor = Color.white },
                hover = { background = _sTexTextFieldBg, textColor = Color.white }, // Keep consistent or slightly change bg
                active = { background = _sTexTextFieldBg, textColor = Color.white },
                focused = { background = _sTexTextFieldBg, textColor = Color.white }, // Often highlighted with border by engine
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(5, 5, 5, 5)
            };
            _sDefaultTextFieldStyle.name = "PLWare.DefaultTextField";
            _sDefaultTextAreaStyle = new GUIStyle(_sDefaultTextFieldStyle) { wordWrap = true }; // Inherit and set wordwrap
            _sDefaultTextAreaStyle.name = "PLWare.DefaultTextArea";

            // Sliders are often fine with GUI.skin defaults or need more complex textures
            _sDefaultHorizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            _sDefaultHorizontalSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            _sDefaultHorizontalSliderStyle.name = "PLWare.DefaultHSlider";
            _sDefaultHorizontalSliderThumbStyle.name = "PLWare.DefaultHSliderThumb";

            _sStylesInitialized = true;
        }

        // --- Instance Members ---
        public Action<int> DrawWindowContent { get; set; }
        public bool IsDraggable { get; set; } = true;
        public Rect DraggableArea { get; set; }

        public bool IsResizable { get; set; } = true;
        public float ResizeBorderThickness { get; set; } = 8f;
        public float MinWindowWidth { get; set; } = 100f; // Adjusted min width
        public float MinWindowHeight { get; set; } = 80f; // Adjusted min height

        private enum ResizeDirection { None, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }
        private bool _isCurrentlyResizing;
        private ResizeDirection _currentResizeDirection = ResizeDirection.None;
        private Vector2 _resizeDragStartMousePosition;
        private Rect _resizeDragStartWindowRect;

        public Window(int id, string title, Rect initialRect, Action<int> drawContentDelegate, bool initialVisibility = false)
        {
            EnsureStylesInitialized(); // Ensure defaults are ready when a window is created
            ID = id;
            Title = title;
            CurrentRect = initialRect;
            DrawWindowContent = drawContentDelegate;
            IsVisible = initialVisibility;
            // Default DraggableArea: full width, 20px height.
            // If CurrentRect isn't set yet, or can change, this might need to be dynamic or set later.
            // For now, GUI.DragWindow can take a Rect that's effectively the title bar area.
            DraggableArea = new Rect(0, 0, float.MaxValue, DefaultWindowStyle?.padding.top ?? 22f);
        }

        public void Render()
        {
            if (!IsVisible) return;
            GUIStyle windowStyleToUse = Style ?? DefaultWindowStyle ?? GUI.skin.window;
            CurrentRect = GUI.Window(ID, CurrentRect, InternalWindowFunction, Title, windowStyleToUse);
        }

        private void InternalWindowFunction(int windowId)
        {
            if (IsResizable) HandleResizeInput();
            DrawWindowContent?.Invoke(windowId);
            if (IsDraggable && !_isCurrentlyResizing)
            {
                // Adjust DraggableArea width if it's float.MaxValue
                Rect actualDraggableArea = DraggableArea;
                if (actualDraggableArea.width == float.MaxValue)
                    actualDraggableArea.width = CurrentRect.width - actualDraggableArea.x;

                GUI.DragWindow(actualDraggableArea);
            }
        }

        private void HandleResizeInput() // (Logic largely unchanged, ensure it uses CurrentRect)
        {
            Event e = Event.current;
            Vector2 mousePosInWindow = e.mousePosition;

            float width = CurrentRect.width; // Use current width/height for zones
            float height = CurrentRect.height;

            Rect topEdge = new Rect(ResizeBorderThickness, 0, width - 2 * ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomEdge = new Rect(ResizeBorderThickness, height - ResizeBorderThickness, width - 2 * ResizeBorderThickness, ResizeBorderThickness);
            // ... (rest of the zone definitions are the same) ...
            Rect leftEdge = new Rect(0, ResizeBorderThickness, ResizeBorderThickness, height - 2 * ResizeBorderThickness);
            Rect rightEdge = new Rect(width - ResizeBorderThickness, ResizeBorderThickness, ResizeBorderThickness, height - 2 * ResizeBorderThickness);
            Rect topLeftCorner = new Rect(0, 0, ResizeBorderThickness, ResizeBorderThickness);
            Rect topRightCorner = new Rect(width - ResizeBorderThickness, 0, ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomLeftCorner = new Rect(0, height - ResizeBorderThickness, ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomRightCorner = new Rect(width - ResizeBorderThickness, height - ResizeBorderThickness, ResizeBorderThickness, ResizeBorderThickness);


            if (e.type == EventType.MouseDown && e.button == 0 && !_isCurrentlyResizing)
            {
                if (topLeftCorner.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.TopLeft;
                else if (topRightCorner.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.TopRight;
                else if (bottomLeftCorner.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.BottomLeft;
                else if (bottomRightCorner.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.BottomRight;
                else if (topEdge.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.Top;
                else if (bottomEdge.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.Bottom;
                else if (leftEdge.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.Left;
                else if (rightEdge.Contains(mousePosInWindow)) _currentResizeDirection = ResizeDirection.Right;
                else _currentResizeDirection = ResizeDirection.None;

                if (_currentResizeDirection != ResizeDirection.None)
                {
                    _isCurrentlyResizing = true;
                    _resizeDragStartMousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition); // Use original event mouse pos
                    _resizeDragStartWindowRect = CurrentRect;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp && e.button == 0 && _isCurrentlyResizing) // only if resizing
            {
                _isCurrentlyResizing = false;
                _currentResizeDirection = ResizeDirection.None;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && _isCurrentlyResizing && e.button == 0)
            {
                Vector2 currentScreenMousePos = GUIUtility.GUIToScreenPoint(e.mousePosition); // Use original event mouse pos
                Vector2 delta = currentScreenMousePos - _resizeDragStartMousePosition;
                Rect newRect = _resizeDragStartWindowRect;

                // Apply deltas (same logic as before)
                if (_currentResizeDirection == ResizeDirection.Left || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.BottomLeft) newRect.xMin = _resizeDragStartWindowRect.xMin + delta.x;
                if (_currentResizeDirection == ResizeDirection.Right || _currentResizeDirection == ResizeDirection.TopRight || _currentResizeDirection == ResizeDirection.BottomRight) newRect.xMax = _resizeDragStartWindowRect.xMax + delta.x;
                if (_currentResizeDirection == ResizeDirection.Top || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.TopRight) newRect.yMin = _resizeDragStartWindowRect.yMin + delta.y;
                if (_currentResizeDirection == ResizeDirection.Bottom || _currentResizeDirection == ResizeDirection.BottomLeft || _currentResizeDirection == ResizeDirection.BottomRight) newRect.yMax = _resizeDragStartWindowRect.yMax + delta.y;

                // Enforce min width/height (same logic)
                if (newRect.width < MinWindowWidth)
                {
                    if (_currentResizeDirection == ResizeDirection.Left || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.BottomLeft) newRect.x = newRect.xMax - MinWindowWidth;
                    else newRect.width = MinWindowWidth;
                }
                if (newRect.height < MinWindowHeight)
                {
                    if (_currentResizeDirection == ResizeDirection.Top || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.TopRight) newRect.y = newRect.yMax - MinWindowHeight;
                    else newRect.height = MinWindowHeight;
                }
                CurrentRect = newRect;
                e.Use();
            }
        }
        public void Show() => IsVisible = true;
        public void Hide() => IsVisible = false;
        public void ToggleVisibility() => IsVisible = !IsVisible;
        public void SetRect(Rect newRect) { if (!_isCurrentlyResizing) CurrentRect = newRect; }
    }
}