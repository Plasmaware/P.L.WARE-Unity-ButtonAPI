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
        public GUIStyle Style { get; set; }

        public static GUIStyle DefaultWindowStyle { get; private set; }
        public static GUIStyle DefaultButtonStyle { get; private set; }
        public static GUIStyle DefaultToggleStyle { get; private set; }

        /// <summary>
        /// The delegate for the method that will draw the content inside the window.
        /// It receives the window ID as a parameter.
        /// </summary>
        public Action<int> DrawWindowContent { get; set; }

        // --- Dragging Configuration ---
        /// <summary>
        /// Gets or sets whether the window is draggable.
        /// </summary>
        public bool IsDraggable { get; set; } = true;

        /// <summary>
        /// Defines the area (in local window coordinates) that can be used to drag the window.
        /// Default is the top 20 pixels, spanning the full width.
        /// </summary>
        public Rect DraggableArea { get; set; } = new Rect(0, 0, float.MaxValue, 20);

        // --- Resizing Configuration & State ---
        public bool IsResizable { get; set; } = true;
        public float ResizeBorderThickness { get; set; } = 8f;
        public float MinWindowWidth { get; set; } = 200f;
        public float MinWindowHeight { get; set; } = 150f;

        private enum ResizeDirection { None, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }

        private bool _isCurrentlyResizing;
        private ResizeDirection _currentResizeDirection = ResizeDirection.None;
        private Vector2 _resizeDragStartMousePosition;
        private Rect _resizeDragStartWindowRect;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="id">A unique ID for the window.</param>
        /// <param name="title">The title displayed on the window.</param>
        /// <param name="initialRect">The initial position and size of the window.</param>
        /// <param name="drawContentDelegate">The method that will be called to draw the window's content.</param>
        /// <param name="initialVisibility">Whether the window is visible initially.</param>

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        internal static void InitiateStyle(Texture2D windowBackground = null) // Pass the image or load it here
        {
            #region Styles
            DefaultWindowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                active = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                focused = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                hover = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                onNormal = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                onActive = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                onFocused = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                onHover = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                border = new RectOffset(5, 5, 5, 5),
            };

            DefaultButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) },
                onNormal = { background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.5f)) },
                onHover = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1f)) },
                hover = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1f)) },
                active = { background = MakeTex(2, 2, new Color(1f, 1f, 1f, 1f)) },
                onActive = { background = MakeTex(2, 2, new Color(1f, 1f, 1f, 1f)) },
                // REMOVED: textColor = Color.white, // This was incorrect
            };
            // Set default text color for all button states
            DefaultButtonStyle.normal.textColor = Color.white;
            DefaultButtonStyle.onNormal.textColor = Color.white;
            DefaultButtonStyle.hover.textColor = Color.white;
            DefaultButtonStyle.onHover.textColor = Color.white;
            DefaultButtonStyle.active.textColor = Color.white;
            DefaultButtonStyle.onActive.textColor = Color.white;
            DefaultButtonStyle.focused.textColor = Color.white;
            DefaultButtonStyle.onFocused.textColor = Color.white;

            // Then, set specific text colors for different button states, overriding the default white
            DefaultButtonStyle.hover.textColor = Color.cyan;
            DefaultButtonStyle.active.textColor = Color.green;


            DefaultToggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                normal = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f)) }, // Off state background
                onNormal = { background = MakeTex(2, 2, new Color(0.2f, 0.8f, 0.2f, 1f)) }, // On state background
                hover = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1f)) }, // Off hover
                onHover = { background = MakeTex(2, 2, new Color(0.3f, 0.9f, 0.3f, 1f)) }, // On hover
                active = { background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 1f)) }, // Off active (pressed)
                onActive = { background = MakeTex(2, 2, new Color(0.1f, 0.7f, 0.1f, 1f)) }, // On active (pressed)
                // REMOVED: textColor = Color.white,
            };
            // Set default text colors for all toggle states
            DefaultToggleStyle.normal.textColor = Color.white;
            DefaultToggleStyle.onNormal.textColor = Color.white;
            DefaultToggleStyle.hover.textColor = Color.white;
            DefaultToggleStyle.onHover.textColor = Color.white;
            DefaultToggleStyle.active.textColor = Color.white;
            DefaultToggleStyle.onActive.textColor = Color.white;
            DefaultToggleStyle.focused.textColor = Color.white;
            DefaultToggleStyle.onFocused.textColor = Color.white;

            // Specific text colors for toggle states (these will override the defaults above)
            DefaultToggleStyle.onNormal.textColor = Color.white; // Text color when toggle is ON
            DefaultToggleStyle.normal.textColor = Color.gray;   // Text color when toggle is OFF

            #endregion

            #region TextColors for Window Style
            if (DefaultWindowStyle != null) // Check if it was initialized
            {
                DefaultWindowStyle.normal.textColor = Color.white;
                DefaultWindowStyle.active.textColor = Color.white;
                DefaultWindowStyle.focused.textColor = Color.white;
                DefaultWindowStyle.hover.textColor = Color.white;
                DefaultWindowStyle.onNormal.textColor = Color.white;
                DefaultWindowStyle.onHover.textColor = Color.white;
                DefaultWindowStyle.onActive.textColor = Color.white;
                DefaultWindowStyle.onFocused.textColor = Color.white;
            }
            #endregion
        }
        public Window(int id, string title, Rect initialRect, Action<int> drawContentDelegate, bool initialVisibility = false)
        {
            ID = id;
            Title = title;
            CurrentRect = initialRect;
            DrawWindowContent = drawContentDelegate;
            IsVisible = initialVisibility;
            Style = null; // Will default to GUI.skin.window if not set
        }

        /// <summary>
        /// Renders the window if it's visible. This should be called from an OnGUI method.
        /// </summary>
        public void Render()
        {
            if (!IsVisible) return;
            GUIStyle currentStyle = Style ?? GUI.skin.window;
            GUI.WindowFunction windowFunctionDelegate = windowID => { InternalWindowFunction(windowID); };
            // if the above still gives issues in some very specific Mono/Unity versions (less likely) comment out the above and uncomment the below:
            // GUI.WindowFunction windowFunctionDelegate = new GUI.WindowFunction(InternalWindowFunction);
            CurrentRect = GUI.Window(ID, CurrentRect, windowFunctionDelegate, Title, currentStyle);
        }

        /// <summary>
        /// The internal function passed to GUI.Window. Handles content drawing, resizing, and dragging.
        /// </summary>
        private void InternalWindowFunction(int windowId)
        {
            if (IsResizable)
            {
                HandleResizeInput();
            }

            DrawWindowContent?.Invoke(windowId);

            if (IsDraggable && !_isCurrentlyResizing)
            {
                GUI.DragWindow(DraggableArea);
            }
        }

        /// <summary>
        /// Handles mouse input for resizing the window. Called within InternalWindowFunction.
        /// </summary>
        private void HandleResizeInput()
        {
            Event e = Event.current;
            Vector2 mousePosInWindow = e.mousePosition;
            Rect localBounds = new Rect(0, 0, CurrentRect.width, CurrentRect.height);

            Rect topEdge = new Rect(ResizeBorderThickness, 0, localBounds.width - 2 * ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomEdge = new Rect(ResizeBorderThickness, localBounds.height - ResizeBorderThickness, localBounds.width - 2 * ResizeBorderThickness, ResizeBorderThickness);
            Rect leftEdge = new Rect(0, ResizeBorderThickness, ResizeBorderThickness, localBounds.height - 2 * ResizeBorderThickness);
            Rect rightEdge = new Rect(localBounds.width - ResizeBorderThickness, ResizeBorderThickness, ResizeBorderThickness, localBounds.height - 2 * ResizeBorderThickness);

            Rect topLeftCorner = new Rect(0, 0, ResizeBorderThickness, ResizeBorderThickness);
            Rect topRightCorner = new Rect(localBounds.width - ResizeBorderThickness, 0, ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomLeftCorner = new Rect(0, localBounds.height - ResizeBorderThickness, ResizeBorderThickness, ResizeBorderThickness);
            Rect bottomRightCorner = new Rect(localBounds.width - ResizeBorderThickness, localBounds.height - ResizeBorderThickness, ResizeBorderThickness, ResizeBorderThickness);

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
                    _resizeDragStartMousePosition = GUIUtility.GUIToScreenPoint(mousePosInWindow);
                    _resizeDragStartWindowRect = CurrentRect;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp && e.button == 0)
            {
                if (_isCurrentlyResizing)
                {
                    _isCurrentlyResizing = false;
                    _currentResizeDirection = ResizeDirection.None;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseDrag && _isCurrentlyResizing && e.button == 0)
            {
                Vector2 currentScreenMousePos = GUIUtility.GUIToScreenPoint(mousePosInWindow);
                Vector2 delta = currentScreenMousePos - _resizeDragStartMousePosition;
                Rect newRect = _resizeDragStartWindowRect;

                if (_currentResizeDirection == ResizeDirection.Left || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.BottomLeft)
                    newRect.xMin += delta.x;
                if (_currentResizeDirection == ResizeDirection.Right || _currentResizeDirection == ResizeDirection.TopRight || _currentResizeDirection == ResizeDirection.BottomRight)
                    newRect.xMax += delta.x;
                if (_currentResizeDirection == ResizeDirection.Top || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.TopRight)
                    newRect.yMin += delta.y;
                if (_currentResizeDirection == ResizeDirection.Bottom || _currentResizeDirection == ResizeDirection.BottomLeft || _currentResizeDirection == ResizeDirection.BottomRight)
                    newRect.yMax += delta.y;

                if (newRect.width < MinWindowWidth)
                {
                    if (_currentResizeDirection == ResizeDirection.Left || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.BottomLeft)
                        newRect.x = newRect.xMax - MinWindowWidth;
                    newRect.width = MinWindowWidth;
                }
                if (newRect.height < MinWindowHeight)
                {
                    if (_currentResizeDirection == ResizeDirection.Top || _currentResizeDirection == ResizeDirection.TopLeft || _currentResizeDirection == ResizeDirection.TopRight)
                        newRect.y = newRect.yMax - MinWindowHeight;
                    newRect.height = MinWindowHeight;
                }

                CurrentRect = newRect;
                e.Use();
            }
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        public void Show() => IsVisible = true;

        /// <summary>
        /// Hides the window.
        /// </summary>
        public void Hide() => IsVisible = false;

        /// <summary>
        /// Toggles the visibility of the window.
        /// </summary>
        public void ToggleVisibility() => IsVisible = !IsVisible;

        /// <summary>
        /// Allows external modification of the window's Rect (e.g., for centering).
        /// </summary>
        public void SetRect(Rect newRect)
        {
            if (!_isCurrentlyResizing)
            {
                CurrentRect = newRect;
            }
        }
    }
}