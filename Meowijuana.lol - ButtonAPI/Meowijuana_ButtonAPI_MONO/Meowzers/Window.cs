// Window.cs
using System;
using UnityEngine;

namespace Meowijuana_ButtonAPI_MONO.Meowzers
{
    public class Window
    {
        public Rect CurrentRect { get; private set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public int ID { get; private set; }
        public GUIStyle Style { get; set; } // Instance-specific style override

        // --- Static Default Styles (Initialized once) ---
        public static GUIStyle DefaultWindowStyle => GetEnsuredStyle(ref _sDefaultWindowStyle);
        public static GUIStyle DefaultSectionStyle => GetEnsuredStyle(ref _sDefaultSectionStyle);
        public static GUIStyle DefaultTitleStyle => GetEnsuredStyle(ref _sDefaultTitleStyle);
        public static GUIStyle DefaultLabelStyle => GetEnsuredStyle(ref _sDefaultLabelStyle);
        public static GUIStyle DefaultButtonStyle => GetEnsuredStyle(ref _sDefaultButtonStyle);
        public static GUIStyle DefaultToggleStyle => GetEnsuredStyle(ref _sDefaultToggleStyle);
        public static GUIStyle DefaultTextFieldStyle => GetEnsuredStyle(ref _sDefaultTextFieldStyle);
        public static GUIStyle DefaultTextAreaStyle => GetEnsuredStyle(ref _sDefaultTextAreaStyle);
        public static GUIStyle DefaultHorizontalSliderStyle => GetEnsuredStyle(ref _sDefaultHorizontalSliderStyle);
        public static GUIStyle DefaultHorizontalSliderThumbStyle => GetEnsuredStyle(ref _sDefaultHorizontalSliderThumbStyle);

        private static GUIStyle _sDefaultWindowStyle;
        private static GUIStyle _sDefaultSectionStyle;
        private static GUIStyle _sDefaultTitleStyle;
        private static GUIStyle _sDefaultLabelStyle;
        private static GUIStyle _sDefaultButtonStyle;
        private static GUIStyle _sDefaultToggleStyle;
        private static GUIStyle _sDefaultTextFieldStyle;
        private static GUIStyle _sDefaultTextAreaStyle;
        private static GUIStyle _sDefaultHorizontalSliderStyle;
        private static GUIStyle _sDefaultHorizontalSliderThumbStyle;

        private static bool _sStylesInitialized = false;

        // Statically managed textures for default styles
        private static Texture2D _sTexWindowBg, _sTexSectionBg, _sTexButtonNormal, _sTexButtonHover,
                                 _sTexButtonActive, _sTexToggleNormal, _sTexToggleOnNormal, _sTexTextFieldBg;

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
        /// Initializes all default GUIStyles. Call this once during your mod's startup,
        /// or it will be called automatically on first access to a default style.
        /// </summary>
        public static void EnsureStylesInitialized() // Was InitiateStyle
        {
            if (_sStylesInitialized) return;

            // --- Create Textures (using colors from original InitiateStyle) ---
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

            // --- Window Style ---
            _sDefaultWindowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = _sTexWindowBg, textColor = Color.white },
                active = { background = _sTexWindowBg, textColor = Color.white },
                focused = { background = _sTexWindowBg, textColor = Color.white },
                hover = { background = _sTexWindowBg, textColor = Color.white },
                onNormal = { background = _sTexWindowBg, textColor = Color.white },
                onActive = { background = _sTexWindowBg, textColor = Color.white },
                onFocused = { background = _sTexWindowBg, textColor = Color.white },
                onHover = { background = _sTexWindowBg, textColor = Color.white },
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(10, 10, 25, 10) // Top padding for title
            };
            _sDefaultWindowStyle.name = "MONO.Meowzers.DefaultWindow";

            // --- Section Style ---
            _sDefaultSectionStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = _sTexSectionBg },
                padding = new RectOffset(5, 5, 5, 5),
                border = new RectOffset(3, 3, 3, 3)
            };
            _sDefaultSectionStyle.name = "MONO.Meowzers.DefaultSection";

            // --- Label Style ---
            _sDefaultLabelStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.white },
                padding = new RectOffset(2, 2, 3, 3)
            };
            _sDefaultLabelStyle.name = "MONO.Meowzers.DefaultLabel";

            // --- Title Style (for sections) ---
            _sDefaultTitleStyle = new GUIStyle(_sDefaultLabelStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            _sDefaultTitleStyle.name = "MONO.Meowzers.DefaultTitle";

            // --- Button Style ---
            _sDefaultButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = _sTexButtonNormal, textColor = Color.white },
                hover = { background = _sTexButtonHover, textColor = Color.white },
                active = { background = _sTexButtonActive, textColor = Color.black }, // White BG, Black text on active
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(8, 8, 6, 6)
            };
            _sDefaultButtonStyle.name = "MONO.Meowzers.DefaultButton";

            // --- Toggle Style ---
            _sDefaultToggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                normal = { background = _sTexToggleNormal, textColor = Color.white },       // Black BG
                onNormal = { background = _sTexToggleOnNormal, textColor = Color.black },   // 0.5 White BG, Black text
                hover = { background = _sTexButtonHover, textColor = Color.white },        // Use button's hover
                onHover = { background = MakeTex(2, 2, new Color(0.9f, 0.9f, 0.9f, 0.7f)), textColor = Color.black }, // Brighter for onHover
                active = { background = _sTexButtonActive, textColor = Color.black },      // Use button's active
                onActive = { background = _sTexToggleOnNormal, textColor = Color.black },  // Keep onNormal for active 'on' state
                padding = new RectOffset(20, 4, 4, 4) // Space for toggle graphic
            };
            _sDefaultToggleStyle.name = "MONO.Meowzers.DefaultToggle";

            // --- Text Field Styles ---
            _sDefaultTextFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                normal = { background = _sTexTextFieldBg, textColor = Color.white },
                hover = { background = MakeTex(2, 2, new Color(0.12f, 0.12f, 0.12f, 0.95f)), textColor = Color.white },
                focused = { background = MakeTex(2, 2, new Color(0.08f, 0.08f, 0.08f, 0.95f)), textColor = Color.white },
                padding = new RectOffset(5, 5, 5, 5)
            };
            _sDefaultTextFieldStyle.name = "MONO.Meowzers.DefaultTextField";
            _sDefaultTextAreaStyle = new GUIStyle(_sDefaultTextFieldStyle) { wordWrap = true };
            _sDefaultTextAreaStyle.name = "MONO.Meowzers.DefaultTextArea";

            // --- Slider Styles (can be basic or use custom textures) ---
            _sDefaultHorizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            _sDefaultHorizontalSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            // Example customization:
            // _sDefaultHorizontalSliderThumbStyle.normal.background = MakeTex(10, 16, new Color(0.4f, 0.4f, 0.45f, 1f));
            _sDefaultHorizontalSliderStyle.name = "MONO.Meowzers.DefaultHSlider";
            _sDefaultHorizontalSliderThumbStyle.name = "MONO.Meowzers.DefaultHSliderThumb";

            _sStylesInitialized = true;
        }

        public Action<int> DrawWindowContent { get; set; }
        public bool IsDraggable { get; set; } = true;
        public Rect DraggableArea { get; set; }
        public bool IsResizable { get; set; } = true;
        public float ResizeBorderThickness { get; set; } = 8f;
        public float MinWindowWidth { get; set; } = 150f; // Adjusted from 200
        public float MinWindowHeight { get; set; } = 100f; // Adjusted from 150

        private enum ResizeDirection { None, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }
        private bool _isCurrentlyResizing;
        private ResizeDirection _currentResizeDirection = ResizeDirection.None;
        private Vector2 _resizeDragStartMousePosition;
        private Rect _resizeDragStartWindowRect;

        public Window(int id, string title, Rect initialRect, Action<int> drawContentDelegate, bool initialVisibility = false)
        {
            EnsureStylesInitialized(); // Ensure defaults are ready
            ID = id;
            Title = title;
            CurrentRect = initialRect;
            DrawWindowContent = drawContentDelegate;
            IsVisible = initialVisibility;
            // Default DraggableArea uses top padding of the window style for height
            DraggableArea = new Rect(0, 0, float.MaxValue, DefaultWindowStyle?.padding.top ?? 20f);
            // Instance 'Style' is null by default, GUI.Window will use DefaultWindowStyle in Render if this.Style is null
        }

        public void Render()
        {
            if (!IsVisible) return;
            GUIStyle windowStyleToUse = Style ?? DefaultWindowStyle ?? GUI.skin.window; // Fallback chain
            CurrentRect = GUI.Window(ID, CurrentRect, InternalWindowFunction, Title, windowStyleToUse);
        }

        private void InternalWindowFunction(int windowId)
        {
            if (IsResizable) HandleResizeInput();
            DrawWindowContent?.Invoke(windowId);
            if (IsDraggable && !_isCurrentlyResizing)
            {
                Rect actualDraggableArea = DraggableArea;
                if (actualDraggableArea.width == float.MaxValue)
                    actualDraggableArea.width = CurrentRect.width - actualDraggableArea.x; // Adjust width
                GUI.DragWindow(actualDraggableArea);
            }
        }
        // --- HandleResizeInput, Show, Hide, ToggleVisibility, SetRect methods ---
        // (These are identical to the robust version provided in the previous large refactor,
        //  so they are omitted here for brevity but should be included)
        private void HandleResizeInput()
        {
            Event e = Event.current;
            Vector2 mousePosInWindow = e.mousePosition;

            float width = CurrentRect.width;
            float height = CurrentRect.height;

            Rect topEdge = new Rect(ResizeBorderThickness, 0, width - 2 * ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomEdge = new Rect(ResizeBorderThickness, height - ResizeBorderThickness, width - 2 * ResizeBorderThickness, ResizeBorderThickness);
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
                    _resizeDragStartMousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
                    _resizeDragStartWindowRect = CurrentRect;
                    e.Use(); // Consume the event
                }
            }
            else if (e.type == EventType.MouseUp && e.button == 0 && _isCurrentlyResizing)
            {
                _isCurrentlyResizing = false;
                _currentResizeDirection = ResizeDirection.None;
                e.Use(); // Consume the event
            }
            else if (e.type == EventType.MouseDrag && _isCurrentlyResizing && e.button == 0)
            {
                Vector2 currentScreenMousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                Vector2 delta = currentScreenMousePos - _resizeDragStartMousePosition;
                Rect newRect = _resizeDragStartWindowRect;

                if (_currentResizeDirection == ResizeDirection.Left || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.BottomLeft) newRect.xMin = _resizeDragStartWindowRect.xMin + delta.x;
                if (_currentResizeDirection == ResizeDirection.Right || _currentResizeDirection == ResizeDirection.TopRight || _currentResizeDirection == ResizeDirection.BottomRight) newRect.xMax = _resizeDragStartWindowRect.xMax + delta.x;
                if (_currentResizeDirection == ResizeDirection.Top || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.TopRight) newRect.yMin = _resizeDragStartWindowRect.yMin + delta.y;
                if (_currentResizeDirection == ResizeDirection.Bottom || _currentResizeDirection == ResizeDirection.BottomLeft || _currentResizeDirection == ResizeDirection.BottomRight) newRect.yMax = _resizeDragStartWindowRect.yMax + delta.y;

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
                e.Use(); // Consume the event
            }
        }
        public void Show() => IsVisible = true;
        public void Hide() => IsVisible = false;
        public void ToggleVisibility() => IsVisible = !IsVisible;
        public void SetRect(Rect newRect) { if (!_isCurrentlyResizing) CurrentRect = newRect; }
    }
}