using UnityEngine;
using System; // For Action
using MelonLoader;
using Meowijuana_SARS.API.Meowzers; // Required for MelonLoader mods

namespace Meowijuana_SARS.API.Menu;

public class Manager
{
    public Window _mainWindow;
    public Watermark _watermark;

    // State for UI elements
    private bool _feature1Enabled = false;
    private float _speedValue = 10f;
    private int _itemCount = 5;
    private string _playerName = "Hero";
    private Vector2 _scrollPosition = Vector2.zero;

    public void _initialize()
    {
        int mainWindowId = "MyMainWindowMelon".GetHashCode();

        _mainWindow = new Window(
            id: mainWindowId,
            title: "Title",
            initialRect: new Rect((Screen.width - 1005) / 2, // x position (centered horizontally)
                (Screen.height - (689 + (1 * 40))) / 2, // y position (centered vertically)
                1005, // width
                689 + (1 * 40)),
            drawContentDelegate: DrawMainWindowContent,
            initialVisibility: false
        );

        _mainWindow.IsResizable = true;
        _mainWindow.IsDraggable = true;

        _watermark = new Watermark()
        {
            CheatName = "Title",
            Version = "Version",
            UserName = "Username", // Or fetch dynamically if possible
            Alignment = TextAnchor.UpperRight,
            BackgroundColor = new Color(0, 0, 0, 1f)
        };
    }
    public void _OnGUIUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            _mainWindow.ToggleVisibility();
            _watermark.IsVisible = !_watermark.IsVisible;
        }
    }
    public void Update()
    {
        // Update UI elements here
        _watermark.Render();

        // Render the main window (if it's visible)
        _mainWindow.Render();
    }
    void DrawMainWindowContent(int windowId)
    {
        // Logic.AddLabel("Welcome to the Mod Menu! (MelonLoader)");
        Logic.BeginSubSection("Player Cheats", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
        if (Logic.AddToggle("Enable God Mode", ref _feature1Enabled, Window.DefaultToggleStyle))
        {
            MelonLogger.Msg("God Mode Toggled: " + _feature1Enabled);
            // Add your god mode logic here
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
            Window.InitiateStyle();
        }
        Logic.EndSubSection();

        GUI.DragWindow(); // Typically handled by PLWindow's title bar
    }
}

public static class Caller
{
    private static Manager _uiManager; // Instance of your UI Manager
    public static void LoadMenu()
    {
        _uiManager = new Manager();
        _uiManager._initialize();
    }

    public static void OnUpdate() => _uiManager?._OnGUIUpdate();
    public static void OnGUI() => _uiManager?.Update();
}