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
        // --- Main Layout: Tabs on Top, Content Below ---
        GUILayout.BeginHorizontal(); // For side tabs, or skip if tabs are above content area

        // --- Tab Navigation Area (Top in this example) ---
        DrawTabButtons();

        GUILayout.EndHorizontal(); // End tab bar horizontal layout

        GUILayout.Space(10); // Space between tab bar and content

        // --- Content Area ---
        // GUILayout.BeginVertical(Window.DefaultSectionStyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)); // Optional: wrap content in a styled box

        switch (_currentTab)
        {
            case MenuTab.Player:
                DrawPlayerTab();
                break;
            case MenuTab.Economy:
                DrawEconomyTab();
                break;
            case MenuTab.Visuals:
                DrawVisualsTab();
                break;
            // Add cases for other tabs
            default:
                break;
        }
        // GUILayout.EndVertical(); // End content area box

        GUI.DragWindow(_mainWindow.DraggableArea); // Use the window's defined draggable area
    }

    void DrawTabButtons()
    {
        // Example for top tabs using GUILayout.Toolbar
        // For more custom side tabs, you'd use GUILayout.BeginVertical for the tab bar
        // and then GUILayout.Button for each tab.

        string[] tabNames = Enum.GetNames(typeof(MenuTab));
        GUIStyle tabButtonStyle = new GUIStyle(Window.DefaultButtonStyle ?? GUI.skin.button); // Use your default or fallback
        tabButtonStyle.fixedHeight = 30; // Example fixed height for tabs
        tabButtonStyle.margin = new RectOffset(2, 2, 5, 5); // Add some margin

        // For a more "plvsmvvrw.lol" style (buttons across the top):
        GUILayout.BeginHorizontal();
        foreach (MenuTab tab in Enum.GetValues(typeof(MenuTab)))
        {
            // Highlight the active tab
            GUIStyle currentTabStyle = new GUIStyle(tabButtonStyle);
            if (_currentTab == tab)
            {
                // Modify style for active tab (e.g., different background or text color)
                currentTabStyle.normal.background = Window.DefaultToggleStyle?.onNormal?.background ?? MakeTex(2, 2, Color.gray); // Example active color
                currentTabStyle.normal.textColor = Color.cyan; // Example active text color
            }

            if (GUILayout.Button(tab.ToString(), currentTabStyle, GUILayout.ExpandWidth(true)))
            {
                _currentTab = tab;
            }
        }
        GUILayout.EndHorizontal();


        // --- Alternative: Side Tab Buttons ---
        /*
        GUILayout.BeginVertical(Window.DefaultSectionStyle, GUILayout.Width(150), GUILayout.ExpandHeight(true)); // Tab bar on the left

        foreach (MenuTab tab in Enum.GetValues(typeof(MenuTab)))
        {
            GUIStyle currentTabStyle = new GUIStyle(tabButtonStyle);
             if (_currentTab == tab)
            {
                currentTabStyle.normal.background = Window.DefaultToggleStyle?.onNormal?.background ?? MakeTex(2,2, Color.gray);
                currentTabStyle.normal.textColor = Color.cyan;
            }

            if (GUILayout.Button(tab.ToString(), currentTabStyle, GUILayout.Height(35))) // GUILayout.ExpandWidth(true) if vertical bar
            {
                _currentTab = tab;
            }
            GUILayout.Space(2); // Space between tab buttons
        }
        GUILayout.EndVertical();
        */
    }

    // --- Individual Tab Drawing Methods ---
    void DrawPlayerTab()
    {
        Logic.BeginSubSection("Player Cheats", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
        if (Logic.AddToggle("Enable God Mode", ref _godModeEnabled, Window.DefaultToggleStyle))
        {
            MelonLogger.Msg("God Mode Toggled: " + _godModeEnabled);
        }
        Logic.AddSlider("Movement Speed", ref _speedValue, 5f, 50f);
        Logic.AddTextField("Player Name:", ref _playerName);
        if (Logic.AddButton("Reset Player Name", () => { _playerName = "DefaultPlayer"; }, Window.DefaultButtonStyle))
        {
            MelonLogger.Msg("Player name reset!");
        }
        Logic.EndSubSection();

        Logic.BeginSubSection("Miscellaneous", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
        Logic.AddLabel("Some informational text here.");
        if (Logic.AddButton("Perform Action X", Window.DefaultButtonStyle))
        {
            MelonLogger.Msg("Action X Performed!");
        }
        Logic.EndSubSection();
    }

    void DrawEconomyTab()
    {
        Logic.BeginSubSection("Self & Economy", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
        Logic.AddToggle("Force Revive", ref _forceRevive, Window.DefaultToggleStyle);
        Logic.AddToggle("Infinite Money", ref _infMoney, Window.DefaultToggleStyle);
        // ... more economy options
        Logic.EndSubSection();
    }

    void DrawVisualsTab()
    {
        Logic.BeginSubSection("Visual Options", Window.DefaultSectionStyle, null, GUILayout.ExpandWidth(true));
        Logic.AddLabel("Visual settings will go here.");
        // ... visual options
        Logic.EndSubSection();
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