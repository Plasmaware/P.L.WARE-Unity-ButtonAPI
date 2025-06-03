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
        // and to ensure they are cleaned up when the AppDomain unloads. This is vital for IL2CPP.
        private static Texture2D _sTexWindowBg;
        private static Texture2D _sTexSectionBg;
        private static Texture2D _sTexButtonNormal;
        private static Texture2D _sTexButtonOnNormal; // Added declaration to match creation code
        private static Texture2D _sTexButtonHover;
        private static Texture2D _sTexButtonActive;
        private static Texture2D _sTexToggleNormalOff;
        private static Texture2D _sTexToggleHoverOff;
        private static Texture2D _sTexToggleNormalOn;
        private static Texture2D _sTexToggleHoverOn;
        private static Texture2D _sTexTextFieldBg;


        // Public accessors for default styles. IL2CPP: Static properties are fine.
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
            EnsureStylesInitialized(); // Ensures styles are ready.
            return styleField;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height]; // Small allocation, fine for one-time init.
            for (int i = 0; i < pix.Length; ++i) pix[i] = col;
            Texture2D result = new Texture2D(width, height) { hideFlags = HideFlags.HideAndDontSave }; // Crucial flag!
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Initializes all default GUIStyles. Call this once during your mod's startup.
        /// IL2CPP: Static constructors or explicit static init methods are fine.
        /// </summary>
        public static void EnsureStylesInitialized()
        {
            if (_sStylesInitialized) return;

            // --- Create Textures ---
            // Color definitions are fine.
            Color grayDeepPlumBlack = new Color(0.0425f, 0.0425f, 0.0425f, 0.92f);
            Color grayDarkRosewood = new Color(0.0856f, 0.0856f, 0.0856f, 1f);
            Color grayCharcoalPinkHint = new Color(0.1356f, 0.1356f, 0.1356f, 1f);
            Color grayVibrantFuchsia = new Color(0.45545f, 0.45545f, 0.45545f, 1f);
            Color grayHotPinkHover = new Color(0.58425f, 0.58425f, 0.58425f, 1f);
            Color grayElectricPinkActive = new Color(0.38535f, 0.38535f, 0.38535f, 1f);

            _sTexWindowBg = MakeTex(2, 2, grayDeepPlumBlack);
            _sTexSectionBg = MakeTex(2, 2, grayDarkRosewood);

            _sTexButtonNormal = MakeTex(2, 2, grayCharcoalPinkHint);
            _sTexButtonOnNormal = MakeTex(2, 2, grayVibrantFuchsia); // Field _sTexButtonOnNormal declared now.
            _sTexButtonHover = MakeTex(2, 2, grayHotPinkHover);
            _sTexButtonActive = MakeTex(2, 2, grayElectricPinkActive);

            // **CORRECTED TEXTURE ASSIGNMENTS FOR TOGGLES**
            // Ensure these assignments match the declared static fields used in _sDefaultToggleStyle.
            _sTexToggleNormalOff = MakeTex(2, 2, grayCharcoalPinkHint);
            _sTexToggleHoverOff = MakeTex(2, 2, grayHotPinkHover);
            _sTexToggleNormalOn = MakeTex(2, 2, grayVibrantFuchsia);
            _sTexToggleHoverOn = MakeTex(2, 2, grayHotPinkHover); // Or a different color if desired for "on hover"

            _sTexTextFieldBg = MakeTex(2, 2, grayDarkRosewood);


            // --- Create Styles ---
            // GUIStyle creation is standard. Setting .name is good for debugging.
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
                border = new RectOffset(6, 6, 6, 6),
                padding = new RectOffset(8, 8, 22, 8)
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
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) },
                padding = new RectOffset(2, 2, 3, 3)
            };
            _sDefaultLabelStyle.name = "PLWare.DefaultLabel";

            _sDefaultTitleStyle = new GUIStyle(_sDefaultLabelStyle)
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
                active = { background = _sTexButtonActive, textColor = new Color(0.8f, 1f, 1f, 1f) },
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(8, 8, 6, 6),
                alignment = TextAnchor.MiddleCenter
            };
            _sDefaultButtonStyle.name = "PLWare.DefaultButton";

            _sDefaultToggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                normal = { background = _sTexToggleNormalOff, textColor = Color.white },   // Uses corrected texture assignment
                hover = { background = _sTexToggleHoverOff, textColor = Color.white },    // Uses corrected texture assignment
                active = { background = _sTexButtonActive, textColor = Color.cyan }, // Active (while pressing) uses button's active texture
                onNormal = { background = _sTexToggleNormalOn, textColor = Color.white },  // Uses corrected texture assignment
                onHover = { background = _sTexToggleHoverOn, textColor = Color.white },    // Uses corrected texture assignment
                onActive = { background = _sTexToggleNormalOn, textColor = Color.cyan }, // Active (while ON and pressing)
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(20, 4, 4, 4)
            };
            _sDefaultToggleStyle.name = "PLWare.DefaultToggle";

            _sDefaultTextFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                normal = { background = _sTexTextFieldBg, textColor = Color.white },
                hover = { background = _sTexTextFieldBg, textColor = Color.white },
                active = { background = _sTexTextFieldBg, textColor = Color.white },
                focused = { background = _sTexTextFieldBg, textColor = Color.white },
                border = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(5, 5, 5, 5)
            };
            _sDefaultTextFieldStyle.name = "PLWare.DefaultTextField";
            _sDefaultTextAreaStyle = new GUIStyle(_sDefaultTextFieldStyle) { wordWrap = true };
            _sDefaultTextAreaStyle.name = "PLWare.DefaultTextArea";

            _sDefaultHorizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            _sDefaultHorizontalSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            _sDefaultHorizontalSliderStyle.name = "PLWare.DefaultHSlider";
            _sDefaultHorizontalSliderThumbStyle.name = "PLWare.DefaultHSliderThumb";

            _sStylesInitialized = true;
        }

        // --- Instance Members ---
        public Action<int> DrawWindowContent { get; set; } // Delegate, IL2CPP-friendly.
        public bool IsDraggable { get; set; } = true;
        public Rect DraggableArea { get; set; }

        public bool IsResizable { get; set; } = true;
        public float ResizeBorderThickness { get; set; } = 8f;
        public float MinWindowWidth { get; set; } = 100f;
        public float MinWindowHeight { get; set; } = 80f;

        private enum ResizeDirection { None, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }
        private bool _isCurrentlyResizing;
        private ResizeDirection _currentResizeDirection = ResizeDirection.None;
        private Vector2 _resizeDragStartMousePosition;
        private Rect _resizeDragStartWindowRect;

        public Window(int id, string title, Rect initialRect, Action<int> drawContentDelegate, bool initialVisibility = false)
        {
            EnsureStylesInitialized();
            ID = id;
            Title = title;
            CurrentRect = initialRect;
            DrawWindowContent = drawContentDelegate;
            IsVisible = initialVisibility;
            // Default DraggableArea calculation is fine.
            DraggableArea = new Rect(0, 0, float.MaxValue, DefaultWindowStyle?.padding.top ?? 22f);
        }

        public void Render()
        {
            if (!IsVisible) return;
            // GUI.skin.window is a safe fallback.
            GUIStyle windowStyleToUse = Style ?? DefaultWindowStyle ?? GUI.skin.window;
            // GUI.Window is the standard way to draw IMGUI windows.
            CurrentRect = UnityEngine.GUI.Window(ID, CurrentRect,
                (UnityEngine.GUI.WindowFunction)InternalWindowFunction, Title, windowStyleToUse);
        }

        private void InternalWindowFunction(int windowId)
        {
            // This is the callback for GUI.Window. Standard Unity pattern.
            if (IsResizable) HandleResizeInput();
            DrawWindowContent?.Invoke(windowId);
            if (IsDraggable && !_isCurrentlyResizing)
            {
                Rect actualDraggableArea = DraggableArea;
                if (actualDraggableArea.width == float.MaxValue)
                    actualDraggableArea.width = CurrentRect.width - actualDraggableArea.x;

                GUI.DragWindow(actualDraggableArea); // Standard Unity API.
            }
        }

        private void HandleResizeInput() // (Logic largely unchanged, ensure it uses CurrentRect)
        {
            Event e = Event.current; // Standard IMGUI event handling.
            Vector2 mousePosInWindow = e.mousePosition;

            float width = CurrentRect.width;
            float height = CurrentRect.height;

            // Rect calculations for resize zones are fine. Rect is a struct.
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
                // Contains checks are efficient for Rects.
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
                    // GUIUtility.GUIToScreenPoint is correct for converting coordinates.
                    _resizeDragStartMousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
                    _resizeDragStartWindowRect = CurrentRect;
                    e.Use(); // Consuming event is important.
                }
            }
            else if (e.type == EventType.MouseUp && e.button == 0 && _isCurrentlyResizing)
            {
                _isCurrentlyResizing = false;
                _currentResizeDirection = ResizeDirection.None;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && _isCurrentlyResizing && e.button == 0)
            {
                Vector2 currentScreenMousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                Vector2 delta = currentScreenMousePos - _resizeDragStartMousePosition; // Vector2 operations are efficient.
                Rect newRect = _resizeDragStartWindowRect;

                // Applying deltas.
                if (_currentResizeDirection == ResizeDirection.Left || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.BottomLeft) newRect.xMin = _resizeDragStartWindowRect.xMin + delta.x;
                if (_currentResizeDirection == ResizeDirection.Right || _currentResizeDirection == ResizeDirection.TopRight || _currentResizeDirection == ResizeDirection.BottomRight) newRect.xMax = _resizeDragStartWindowRect.xMax + delta.x;
                if (_currentResizeDirection == ResizeDirection.Top || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.TopRight) newRect.yMin = _resizeDragStartWindowRect.yMin + delta.y;
                if (_currentResizeDirection == ResizeDirection.Bottom || _currentResizeDirection == ResizeDirection.BottomLeft || _currentResizeDirection == ResizeDirection.BottomRight) newRect.yMax = _resizeDragStartWindowRect.yMax + delta.y;

                // Enforce min width/height.
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