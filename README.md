Im not writing an ENTIRE fucking readme for this so i gpt'd it, it should tell you what to do if ur on bepin nor melon and how to work it, kk by bye.

# P.L.WARE Button API: Documentation

## Table of Contents

1.  [Introduction](#introduction)
2.  [Key Features](#key-features)
3.  [Getting Started & Setup](#getting-started--setup)
    *   [Project Setup (General)](#project-setup-general)
    *   [For IL2CPP Games](#for-il2cpp-games)
    *   [For Mono Games](#for-mono-games)
4.  [API Reference](#api-reference)
    *   [Namespace: `PLWARE_ButtonAPI.Controls`](#namespace-plware_buttonapicontrols)
    *   [Class: `PLButtonAPI`](#class-plbuttonapi)
        *   [Buttons](#buttons)
        *   [Toggles](#toggles)
        *   [Sliders](#sliders)
        *   [Text Fields / Text Areas](#text-fields--text-areas)
        *   [Labels](#labels)
        *   [Sub-Sections](#sub-sections)
    *   [Class: `PLWatermark`](#class-plwatermark)
        *   [Properties](#properties-plwatermark)
        *   [Constructor](#constructor-plwatermark)
        *   [Methods](#methods-plwatermark)
    *   [Class: `PLWindow`](#class-plwindow)
        *   [Properties](#properties-plwindow)
        *   [Constructor](#constructor-plwindow)
        *   [Methods](#methods-plwindow)
5.  [Example Usage]([#example-usage](https://github.com/Plasmaware/P.L.WARE-Unity-ButtonAPI/tree/main?tab=readme-ov-file#5-example-usage))
6.  [Tips & Best Practices](#tips--best-practices)
7.  [Troubleshooting](#troubleshooting)

---

## 1. Introduction

The P.L.WARE Button API is a lightweight, easy-to-use library for creating ImGUI (Immediate Mode GUI) elements within Unity game mods. It simplifies the process of adding common UI controls like buttons, toggles, sliders, and managed windows, allowing mod developers to quickly build user interfaces for their cheats or tools. This API supports both IL2CPP and Mono-backend Unity games.

This document provides a comprehensive guide on how to integrate and use the P.L.WARE Button API in your projects.

## 2. Key Features

*   **Simple API:** Intuitive methods for adding UI elements with minimal boilerplate.
*   **Common Controls:** Buttons, toggles, float/int sliders, text fields, text areas, and labels.
*   **Managed Windows:** Draggable and resizable windows to encapsulate your UI (`PLWindow`).
*   **Watermark:** Customizable on-screen watermark to display cheat name, version, username, time, and FPS (`PLWatermark`).
*   **Sub-Sections:** Helper methods to visually group UI elements within windows.
*   **Customizable Styles:** Supports custom `GUIStyle` and `GUILayoutOption` for all elements.
*   **IL2CPP & Mono Support:** Designed to work seamlessly with modding frameworks for both types of Unity games.

## 3. Getting Started & Setup

To use the P.L.WARE Button API, you'll typically be creating a DLL (Dynamic Link Library) that gets loaded into the target Unity game by a mod loader.

### Project Setup (General)

1.  **Create a Class Library Project:**
    *   In Visual Studio or your preferred IDE, create a new C# Class Library project.
    *   Target a .NET Framework version compatible with the Unity game you're modding (e.g., .NET Framework 4.7.2, .NET Standard 2.0 for broader compatibility, especially with BepInEx).

2.  **Add API Files:**
    *   Copy the `Controls.cs`, `PLWatermark.cs`, and `PLWindow.cs` files from the [P.L.WARE Button API GitHub repository](https://github.com/Plasmaware/P.L.WARE-Unity-ButtonAPI) into your project (typically into a subfolder like `ButtonAPI`).

3.  **Add Unity Engine References:**
    *   You'll need to reference Unity's assemblies. These are usually found in the game's `Managed` folder (e.g., `GAME_Data/Managed/`) or within your mod loader's directories (e.g., `MelonLoader/Managed/` or `BepInEx/core/`).
    *   Essential references include:
        *   `UnityEngine.dll`
        *   `UnityEngine.CoreModule.dll` (for `MonoBehaviour`, `Time`, `Screen`, etc.)
        *   `UnityEngine.IMGUIModule.dll` (for `GUI`, `GUILayout`, `GUIStyle`, etc.)
    *   In your project, right-click "References" (or "Dependencies") -> "Add Reference..." -> "Browse" and locate these DLLs. Set "Copy Local" to `False` for these references.

### For IL2CPP Games

IL2CPP games have their C# code converted to C++ and then compiled to native code. To run C# mods, you need a specialized mod loader.

1.  **Choose a Mod Loader:**
    *   **MelonLoader:** A popular choice for IL2CPP games.
    *   **BepInEx (IL2CPP variant):** BepInEx also supports IL2CPP games through specific builds or plugins like `BepInEx.IL2CPP`.

2.  **Project Structure:**
    *   Your main mod class will typically inherit from a base class provided by the mod loader (e.g., `MelonMod` for MelonLoader).
    *   The `OnGUI()` Unity message will be available within your mod class, or the mod loader might provide its own event for GUI rendering.

3.  **Deployment:**
    *   Compile your project to a DLL.
    *   Place the DLL into the appropriate folder for your chosen mod loader (e.g., `Mods` for MelonLoader, `BepInEx/plugins` for BepInEx).

### For Mono Games

Mono games use a .NET runtime (Mono) to execute C# code.

1.  **Choose a Mod Loader:**
    *   **BepInEx (Mono variant):** The most common mod loader for Mono-backend Unity games.
    *   **Unity Mod Manager (UMM):** Another option for some games.
    *   Older methods might involve custom injectors, but mod loaders are recommended.

2.  **Project Structure:**
    *   Your main mod class will typically inherit from `MonoBehaviour` (if using BepInEx and attaching to a GameObject) or a base class from the mod loader (e.g., `BaseUnityPlugin` for BepInEx).
    *   You'll implement the `OnGUI()` Unity message in your `MonoBehaviour` or a class that the mod loader calls.

3.  **Deployment:**
    *   Compile your project to a DLL.
    *   Place the DLL into the `BepInEx/plugins` folder (or the equivalent for other loaders).

## 4. API Reference

### Namespace: `PLWARE_ButtonAPI.Controls`

All classes and methods of this API are contained within this namespace. Remember to add `using PLWARE_ButtonAPI.Controls;` at the top of your C# files.

---

### Class: `PLButtonAPI`

A static class (defined in `Controls.cs`) providing helper methods to create standard IMGUI controls.

#### Buttons

*   `public static bool AddButton(string text, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)`
    *   Creates a button that executes an action when clicked.
    *   **Parameters:**
        *   `text`: The text displayed on the button.
        *   `onClick`: An `Action` delegate that is invoked when the button is clicked.
        *   `style` (optional): A `GUIStyle` to customize the button's appearance. Defaults to `GUI.skin.button`.
        *   `options` (optional): `GUILayoutOption`s to control layout (e.g., `GUILayout.Width(100)`).
    *   **Returns:** `true` if the button was clicked this frame, `false` otherwise.

*   `public static bool AddButton(string text, GUIStyle style = null, params GUILayoutOption[] options)`
    *   Creates a button without an immediate `Action`. You can check the return value to perform an action.
    *   **Parameters:** Same as above, excluding `onClick`.
    *   **Returns:** `true` if the button was clicked this frame, `false` otherwise.

#### Toggles

*   `public static bool AddToggle(string text, ref bool toggleState, GUIStyle style = null, params GUILayoutOption[] options)`
    *   Creates a toggle (checkbox).
    *   **Parameters:**
        *   `text`: The label displayed next to the toggle.
        *   `toggleState`: A `ref bool` variable that holds the current state of the toggle. It will be updated by this method.
        *   `style` (optional): A `GUIStyle` for the toggle. Defaults to `GUI.skin.toggle`.
        *   `options` (optional): `GUILayoutOption`s for layout.
    *   **Returns:** `true` if the toggle's state changed this frame, `false` otherwise.

#### Sliders

*   `public static bool AddSlider(string label, ref float sliderValue, float minValue, float maxValue, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)`
    *   Creates a horizontal slider for `float` values.
    *   **Parameters:**
        *   `label`: Text displayed next to the slider.
        *   `sliderValue`: A `ref float` variable holding the slider's current value.
        *   `minValue`: The minimum value of the slider.
        *   `maxValue`: The maximum value of the slider.
        *   `sliderStyle` (optional): `GUIStyle` for the slider's track. Defaults to `GUI.skin.horizontalSlider`.
        *   `thumbStyle` (optional): `GUIStyle` for the slider's thumb. Defaults to `GUI.skin.horizontalSliderThumb`.
        *   `showValue` (optional): If `true` (default), displays the current value next to the label (e.g., "Speed (10.50)").
        *   `options` (optional): `GUILayoutOption`s for the slider itself (not the surrounding horizontal group).
    *   **Returns:** `true` if the slider's value changed this frame, `false` otherwise.

*   `public static bool AddSlider(string label, ref int sliderValue, int minValue, int maxValue, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)`
    *   Creates a horizontal slider for `int` values.
    *   **Parameters:** Same as the float slider, but `sliderValue` is `ref int`.
    *   **Returns:** `true` if the slider's value changed this frame, `false` otherwise.

#### Text Fields / Text Areas

*   `public static bool AddTextField(string label, ref string textValue, GUIStyle style = null, params GUILayoutOption[] options)`
    *   Creates a single-line text input field.
    *   **Parameters:**
        *   `label`: Optional label displayed above the text field.
        *   `textValue`: A `ref string` variable holding the current text.
        *   `style` (optional): `GUIStyle` for the text field. Defaults to `GUI.skin.textField`.
        *   `options` (optional): `GUILayoutOption`s for layout.
    *   **Returns:** `true` if the text value changed this frame, `false` otherwise.

*   `public static bool AddTextArea(string label, ref string textValue, GUIStyle style = null, params GUILayoutOption[] options)`
    *   Creates a multi-line text input area.
    *   **Parameters:** Same as `AddTextField`. `GUIStyle` defaults to `GUI.skin.textArea`.
    *   **Returns:** `true` if the text value changed this frame, `false` otherwise.

#### Labels

*   `public static void AddLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)`
    *   Displays a text label.
    *   **Parameters:**
        *   `text`: The text to display.
        *   `style` (optional): `GUIStyle` for the label. Defaults to `GUI.skin.label`.
        *   `options` (optional): `GUILayoutOption`s for layout.

#### Sub-Sections

Sub-sections help organize UI elements within a visually distinct group (typically a box with an optional title).

*   `public static void InitializeDefaultSubSectionStyles(GUIStyle boxStyle, GUIStyle titleStyle)`
    *   Sets the default styles to be used by `BeginSubSection` if no specific styles are provided to it. Call this once, perhaps during your mod's initialization.
    *   **Parameters:**
        *   `boxStyle`: The `GUIStyle` for the surrounding box of the sub-section.
        *   `titleStyle`: The `GUIStyle` for the sub-section's title.

*   `public static void BeginSubSection(string title = null, GUIStyle boxStyle = null, GUIStyle titleStyle = null, params GUILayoutOption[] options)`
    *   Starts a new sub-section. All subsequent `GUILayout` elements will be drawn inside this section until `EndSubSection()` is called.
    *   **Parameters:**
        *   `title` (optional): Text to display as a title for the sub-section.
        *   `boxStyle` (optional): `GUIStyle` for the sub-section's background/border. Defaults to the initialized default or `GUI.skin.box`.
        *   `titleStyle` (optional): `GUIStyle` for the title. Defaults to the initialized default or a bold, centered white label.
        *   `options` (optional): `GUILayoutOption`s for the vertical group of the sub-section. Defaults to `GUILayout.ExpandWidth(true)`.

*   `public static void EndSubSection()`
    *   Ends the current sub-section. Must be called after a `BeginSubSection()`.

---

### Class: `PLWatermark`

(Defined in `PLWatermark.cs`) Displays an on-screen watermark with customizable information.

#### Properties (PLWatermark)

*   `public bool IsVisible { get; set; } = true;`
    *   Controls whether the watermark is rendered.
*   `public string CheatName { get; set; } = "Cheatname";`
    *   The name of your cheat/mod to display.
*   `public string Version { get; set; } = "[Freemium]";`
    *   The version of your cheat/mod.
*   `public string UserName { get; set; }`
    *   The current user's name. You need to set this, e.g., from game data.
*   `public Color TextColor { get; set; } = Color.white;`
    *   The color of the watermark text.
*   `public Color BackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.6f);`
    *   The background color of the watermark box (semi-transparent dark gray by default).
*   `public int FontSize { get; set; } = 12;`
    *   Font size of the watermark text.
*   `public FontStyle FontStyle { get; set; } = FontStyle.Normal;`
    *   Font style (Normal, Bold, Italic, BoldAndItalic).
*   `public TextAnchor Alignment { get; set; } = TextAnchor.UpperRight;`
    *   Screen alignment of the watermark (e.g., `TextAnchor.UpperLeft`, `TextAnchor.LowerRight`).
*   `public float Padding { get; set; } = 5f;`
    *   Padding inside the watermark box, around the text.

#### Constructor (PLWatermark)

*   `public PLWatermark()`
    *   Initializes a new `PLWatermark` instance. Sets `UserName` to "Player" by default.

#### Methods (PLWatermark)

*   `public void Render()`
    *   Draws the watermark on screen if `IsVisible` is true. Call this within your `OnGUI()` method. It automatically updates FPS and time.

*   `public void SetCurrentUsername(string username)`
    *   Convenience method to update the `UserName` property.

---

### Class: `PLWindow`

(Defined in `PLWindow.cs`) Represents a draggable and resizable IMGUI window.

#### Properties (PLWindow)

*   `public Rect CurrentRect { get; private set; }`
    *   The current position and size of the window on screen. Modifiable via `SetRect()` or internally by dragging/resizing.
*   `public string Title { get; set; }`
    *   The title displayed in the window's title bar.
*   `public bool IsVisible { get; set; }`
    *   Controls whether the window is rendered.
*   `public int ID { get; private set; }`
    *   A unique integer ID for this window. **Crucial:** Each `GUI.Window` on screen at the same time must have a unique ID.
*   `public GUIStyle Style { get; set; }`
    *   The `GUIStyle` for the window. If `null` (default), `GUI.skin.window` is used.
*   `public Action<int> DrawWindowContent { get; set; }`
    *   A delegate (`Action<int>`) that points to the method responsible for drawing the content *inside* this window. This method will receive the window's `ID` as a parameter.
*   `public bool IsDraggable { get; set; } = true;`
    *   If `true`, the window can be dragged by its title bar (or `DraggableArea`).
*   `public Rect DraggableArea { get; set; } = new Rect(0, 0, float.MaxValue, 20);`
    *   Defines the area (in local window coordinates) used for dragging. Defaults to the top 20 pixels. `float.MaxValue` for width means it spans the entire window width.
*   `public bool IsResizable { get; set; } = true;`
    *   If `true`, the window can be resized by dragging its borders/corners.
*   `public float ResizeBorderThickness { get; set; } = 8f;`
    *   The thickness of the invisible border area used for detecting resize drags.
*   `public float MinWindowWidth { get; set; } = 200f;`
    *   Minimum width the window can be resized to.
*   `public float MinWindowHeight { get; set; } = 150f;`
    *   Minimum height the window can be resized to.

#### Constructor (PLWindow)

*   `public PLWindow(int id, string title, Rect initialRect, Action<int> drawContentDelegate, bool initialVisibility = false)`
    *   Initializes a new `PLWindow`.
    *   **Parameters:**
        *   `id`: Unique integer ID for the window.
        *   `title`: Window title.
        *   `initialRect`: Initial position and size (`Rect`) of the window.
        *   `drawContentDelegate`: The method that will draw the UI elements inside the window.
        *   `initialVisibility` (optional): Set to `true` to make the window visible immediately. Defaults to `false`.

#### Methods (PLWindow)

*   `public void Render()`
    *   Renders the window and its content if `IsVisible` is true. Call this within your `OnGUI()` method.

*   `public void Show()`
    *   Sets `IsVisible` to `true`.
*   `public void Hide()`
    *   Sets `IsVisible` to `false`.
*   `public void ToggleVisibility()`
    *   Toggles the `IsVisible` state.
*   `public void SetRect(Rect newRect)`
    *   Allows you to programmatically set the window's position and size, e.g., to center it on screen. Avoid calling while the user might be resizing.

## 5. Example Usage

This example assumes you have a class (e.g., a `MonoBehaviour` for BepInEx, or `MelonMod` for MelonLoader) where you can implement `OnGUI()`.

```csharp
using UnityEngine;
using PLWARE_ButtonAPI.Controls; // Don't forget this!
using System; // For Action

// If using BepInEx:
// using BepInEx;
// [BepInPlugin("com.yourname.mymod", "My Mod", "1.0.0")]
// public class MyModClass : BaseUnityPlugin

// If using MelonLoader:
// using MelonLoader;
// public class MyModClass : MelonMod

// For a simple MonoBehaviour example:
public class ModGUIManager : MonoBehaviour
{
    private PLWindow _mainWindow;
    private PLWatermark _watermark;

    // State for UI elements
    private bool _feature1Enabled = false;
    private float _speedValue = 10f;
    private int _itemCount = 5;
    private string _playerName = "Hero";
    private Vector2 _scrollPosition = Vector2.zero; // For scrollable content if needed

    void Awake()
    {
        // It's good practice to generate a unique ID for each window.
        // You can use a simple counter or GetHashCode on a unique string.
        int mainWindowId = "MyMainWindow".GetHashCode();

        // Initialize the main window
        _mainWindow = new PLWindow(
            id: mainWindowId,
            title: "P.L.WARE Mod Menu",
            initialRect: new Rect(20, 20, 400, 500), // x, y, width, height
            drawContentDelegate: DrawMainWindowContent,
            initialVisibility: false // Start hidden
        );

        // Configure window properties (optional)
        _mainWindow.IsResizable = true;
        _mainWindow.IsDraggable = true;

        // Initialize the watermark
        _watermark = new PLWatermark
        {
            CheatName = "My Awesome Mod",
            Version = "v1.1",
            UserName = "Player123", // Or fetch dynamically
            Alignment = TextAnchor.UpperLeft,
            BackgroundColor = new Color(0,0,0,0.7f)
        };
        // _watermark.SetCurrentUsername("ActualGameUser"); // if fetched later

        // (Optional) Initialize default styles for sub-sections
        // GUIStyle customBox = new GUIStyle(GUI.skin.box);
        // customBox.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        // GUIStyle customTitle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, normal = { textColor = Color.cyan }};
        // PLButtonAPI.InitializeDefaultSubSectionStyles(customBox, customTitle);
    }

    void Update()
    {
        // Example: Toggle menu visibility with a key (e.g., Insert)
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            _mainWindow.ToggleVisibility();
            _watermark.IsVisible = !_watermark.IsVisible; // Also toggle watermark
        }
    }

    void OnGUI()
    {
        // Always good to set skin once if you use custom global styles
        // GUI.skin = MyCustomSkin;

        // Render the watermark
        _watermark.Render();

        // Render the main window (if it's visible)
        _mainWindow.Render();
    }

    // This method will be called by the PLWindow class to draw its content
    void DrawMainWindowContent(int windowId)
    {
        // You can use GUILayout for automatic layouting
        GUILayout.Label("Welcome to the P.L.WARE Mod Menu!");
        GUILayout.Space(10);

        // --- Section 1: Player Cheats ---
        PLButtonAPI.BeginSubSection("Player Cheats"); // Using default styles for subsection

        if (PLButtonAPI.AddToggle("Enable God Mode", ref _feature1Enabled))
        {
            Debug.Log("God Mode Toggled: " + _feature1Enabled);
            // Add your god mode logic here
        }

        PLButtonAPI.AddSlider("Movement Speed", ref _speedValue, 5f, 50f);
        // PLButtonAPI.AddSlider("Item Count", ref _itemCount, 1, 100, showValue: true); // ShowValue is true by default

        PLButtonAPI.AddTextField("Player Name:", ref _playerName);

        if (PLButtonAPI.AddButton("Reset Player Name", () => { _playerName = "DefaultPlayer"; }))
        {
            Debug.Log("Player name reset!");
        }

        PLButtonAPI.EndSubSection();


        // --- Section 2: Other Options ---
        // Example with custom sub-section styles (if not using InitializeDefaultSubSectionStyles)
        GUIStyle customBoxStyle = new GUIStyle(GUI.skin.box);
        // customBoxStyle.normal.background = MakeTex(1,1, new Color(0.1f, 0.1f, 0.3f, 0.7f)); // Helper needed for MakeTex
        GUIStyle customTitleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic, normal = { textColor = Color.yellow } };

        PLButtonAPI.BeginSubSection("Miscellaneous", customBoxStyle, customTitleStyle);

        PLButtonAPI.AddLabel("Some informational text here.");

        if (PLButtonAPI.AddButton("Perform Action X"))
        {
            Debug.Log("Action X Performed!");
        }
        PLButtonAPI.EndSubSection();


        // --- Scrollable Area Example (Optional) ---
        // PLButtonAPI.BeginSubSection("Long List of Items");
        // _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(100));
        // for (int i = 0; i < 20; i++)
        // {
        //     PLButtonAPI.AddLabel($"Item Number {i+1}");
        // }
        // GUILayout.EndScrollView();
        // PLButtonAPI.EndSubSection();


        // You can make the window draggable by its content if you wish,
        // but it's generally done via the title bar (default for GUI.Window)
        // GUI.DragWindow(); // If you want the whole window to be draggable by its content area
    }

    // Helper to create a Texture2D for GUIStyle backgrounds (often needed for custom box styles)
    // private Texture2D MakeTex(int width, int height, Color col)
    // {
    //     Color[] pix = new Color[width * height];
    //     for (int i = 0; i < pix.Length; ++i)
    //     {
    //         pix[i] = col;
    //     }
    //     Texture2D result = new Texture2D(width, height);
    //     result.SetPixels(pix);
    //     result.Apply();
    //     return result;
    // }
}
```

### Adding the ModGUIManager to the Scene (for BepInEx/MonoBehaviour approach)

If you're using BepInEx and your `MyModClass` inherits `BaseUnityPlugin`, you'd typically create a GameObject in your `Awake` or `Start` method and add your `ModGUIManager` component to it:

```csharp
// Inside your BepInEx Plugin class (e.g., MyModClass : BaseUnityPlugin)
void Awake()
{
    // ... other init
    GameObject modObject = new GameObject("MyModGUIRunner");
    modObject.AddComponent<ModGUIManager>();
    DontDestroyOnLoad(modObject); // Keep it alive across scene loads
    Logger.LogInfo("Mod GUI Manager initialized!");
}
```

For MelonLoader, `OnGUI` is usually implemented directly in your `MelonMod` class.

## 6. Tips & Best Practices

*   **Unique Window IDs:** Ensure every `PLWindow` instance has a unique `ID`. Using `string.GetHashCode()` on a descriptive name is a common practice.
*   **`OnGUI()` Performance:** `OnGUI()` is called multiple times per frame (layout and repaint events). Keep your GUI logic efficient. Avoid complex calculations or allocations within `OnGUI()` if possible.
*   **`GUILayoutOption`:** Use `GUILayoutOption` arrays to control the size and spacing of elements (e.g., `GUILayout.Width(150)`, `GUILayout.ExpandWidth(false)`).
*   **`GUIStyle`:** For advanced customization, create and assign `GUIStyle` objects to elements. You can modify font, colors, background images, alignment, padding, etc.
    *   Initialize `GUIStyle` objects once (e.g., in `Awake()` or `Start()`) rather than in `OnGUI()` for better performance.
*   **Error Handling:** The API methods are straightforward, but always check Unity's console for GUI-related errors, which often provide clues about layout issues.
*   **Visibility Toggling:** Provide a clear way for users to show/hide your UI (e.g., a hotkey).
*   **IL2CPP Considerations:** The API itself is AOT-friendly. When writing your mod logic, be mindful of IL2CPP restrictions if you delve into advanced C# features (though for basic UI, this is rarely an issue).
*   **Input Focus:** Be aware that IMGUI can steal input focus. If your mod UI is visible, it might interfere with game controls until it's hidden or focus is managed.

## 7. Troubleshooting

*   **UI Not Appearing:**
    *   Ensure `OnGUI()` is being called. (Add `Debug.Log("OnGUI called");`)
    *   Check if the `PLWindow`'s `IsVisible` property is `true`.
    *   Verify the `PLWindow`'s `initialRect` is within screen bounds and has a positive width/height.
    *   Ensure your mod loader is correctly loading your plugin/mod.
*   **Window ID Conflicts:** If windows behave erratically or one disappears when another appears, you might have duplicate window IDs.
*   **`GUILayout` Errors:** Console errors like "GUILayout: Mismatched BeginVertical/EndVertical" usually mean you have an unbalanced `PLButtonAPI.BeginSubSection()` and `PLButtonAPI.EndSubSection()` or other `GUILayout.Begin*`/`GUILayout.End*` calls.
*   **Style Issues:** If default styles look off, the game might have a very custom `GUI.skin`. You might need to define your own `GUIStyle`s more explicitly.
*   **Performance Drops:** If the UI causes lag, try to simplify the layout, reduce the number of elements, or optimize any logic within your `DrawWindowContent` method. Avoid creating new `GUIStyle` objects every frame in `OnGUI`.
