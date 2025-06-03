using UnityEngine;
using System; // For Action
using MelonLoader;
using Meowijuana_ButtonAPI.API.Menu;
// Use the namespace where your Window class (with static styles) and Logic class are defined
using Meowijuana_ButtonAPI.API.Meowzers;

namespace Meowijuana_ButtonAPI.API.Menu // This namespace is for the Manager class itself
{
    public class Manager
    {
        // Use the Window class from the correct namespace
        public Window _mainWindow;
        public Watermark _watermark; // Assuming Watermark is also in Meowijuana_ButtonAPI.API.Meowzers

        // State for UI elements
        private bool _feature1Enabled = false;
        private float _speedValue = 10f;
        private int _itemCount = 5; // Unused in current DrawMainWindowContent
        private string _playerName = "Hero";
        private Vector2 _scrollPosition = Vector2.zero; // Unused in current DrawMainWindowContent

        public void _initialize()
        {
            int mainWindowId = "MyMainWindowMelon".GetHashCode();

            _mainWindow = new Window( // Use fully qualified name or ensure correct 'using'
                id: mainWindowId,
                title: "Title",
                initialRect: new Rect((Screen.width - 1005) / 2,
                    (Screen.height - (689 + (1 * 40))) / 2,
                    1005,
                    689 + (1 * 40)),
                drawContentDelegate: DrawMainWindowContent,
                initialVisibility: false
            );

            _mainWindow.IsResizable = true;
            _mainWindow.IsDraggable = true;

            _watermark = new Watermark() // Use fully qualified name or ensure correct 'using'
            {
                CheatName = "Title",
                Version = "Version",
                UserName = "Username",
                Alignment = TextAnchor.UpperRight,
                BackgroundColor = new Color(0, 0, 0, 1f)
            };
        }

        // This method handles INPUT and other non-drawing updates, called by MelonLoader's OnUpdate
        public void HandleInputAndLogicUpdates()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _mainWindow.ToggleVisibility();
                _watermark.IsVisible = !_watermark.IsVisible;
            }

            // Example:
            // if (_feature1Enabled) { /* apply god mode effects to the game */ }
            // if (_speedValue != previousSpeed) { /* update player speed in the game */ }
        }

        // This method is for DRAWING GUI, called by MelonLoader's OnGUI
        public void RenderGUI()
        {
            // Ensure styles are initialized ONCE at the beginning of the actual OnGUI flow
            // This is critical. Check Event.current.type to avoid unnecessary calls.
            if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
            {
                Window.EnsureStylesInitialized();
            }

            // Render the watermark (if it's visible)
            // Watermark.Render() also contains GUI calls, so it must be here.
            if (_watermark != null)
            {
                _watermark.Render();
            }

            // Render the main window (if it's visible)
            // Window.Render() contains GUI.Window, which is a GUI call.
            if (_mainWindow != null)
            {
                _mainWindow.Render();
            }
        }

        void DrawMainWindowContent(int windowId)
        {
            // Use the Logic class from the correct namespace
            // And access static styles from the correct Window class (Meowijuana_ButtonAPI.API.Meowzers.Window)

            Logic.BeginSubSection("Player Cheats", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
            if (Logic.AddToggle("Enable God Mode", ref _feature1Enabled, Window.DefaultToggleStyle))
            {
                MelonLogger.Msg("God Mode Toggled: " + _feature1Enabled);
                // Add your god mode logic here (preferably in HandleInputAndLogicUpdates or a dedicated game logic method)
            }
            Logic.AddSlider("Movement Speed", ref _speedValue, 5f, 50f);
            Logic.AddTextField("Player Name:", ref _playerName);
            if (Logic.AddButtonOnClick("Reset Player Name", () => { _playerName = "DefaultPlayer"; }, Window.DefaultButtonStyle))
            {
                MelonLogger.Msg("Player name reset!");
            }
            Logic.EndSubSection();

            Logic.BeginSubSection("Miscellaneous", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
            Logic.AddLabel("Some informational text here.");
            if (Logic.AddButton("Update Style", Window.DefaultButtonStyle))
            {
                MelonLogger.Msg("Style Update Performed!");
                // To force re-initialization of styles:
                // Meowijuana_ButtonAPI.API.Meowzers.Window._sStylesInitialized = false; // Requires _sStylesInitialized to be public/internal or have a public static setter
                Window.EnsureStylesInitialized(); // This will re-initialize if _sStylesInitialized was set to false
            }
            Logic.EndSubSection();

            // GUI.DragWindow(); // REMOVED - Handled by your Window class
        }
    }

    public static class Caller
    {
        public static Manager _uiManager;

        public static void LoadMenu()
        {
            _uiManager = new Manager();
            _uiManager._initialize();
        }

        // Called by MelonLoader's OnUpdate for game logic and input
        public static void OnUpdate()
        {
            if (_uiManager != null)
            {
                _uiManager.HandleInputAndLogicUpdates();
            }
        }

        // Called by MelonLoader's OnGUI for drawing GUI
        public static void OnGUI()
        {
            if (_uiManager != null)
            {
                _uiManager.RenderGUI();
            }
        }
    }
}