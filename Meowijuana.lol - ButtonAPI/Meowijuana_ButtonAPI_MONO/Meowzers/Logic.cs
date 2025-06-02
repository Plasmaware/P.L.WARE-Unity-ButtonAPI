// Logic.cs
using System;
using UnityEngine;

namespace Meowijuana_ButtonAPI_MONO.Meowzers
{
    public static class Logic
    {
        // Helper to select the appropriate style
        private static GUIStyle GetEffectiveStyle(GUIStyle preferredStyle, Func<GUIStyle> defaultStyleProvider, Func<GUIStyle> skinDefaultProvider = null)
        {
            Window.EnsureStylesInitialized(); // Ensure Window's static styles are available
            if (preferredStyle != null) return preferredStyle;

            GUIStyle styleToUse = defaultStyleProvider?.Invoke();
            if (styleToUse != null) return styleToUse;

            styleToUse = skinDefaultProvider?.Invoke();
            return styleToUse ?? new GUIStyle(); // Absolute fallback to a new empty style
        }

        // --- Button ---
        // Renamed first AddButton to AddButtonOnClick for clarity if it's distinct.
        public static bool AddButtonOnClick(string text, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultButtonStyle, () => GUI.skin.button);
            options = options ?? Array.Empty<GUILayoutOption>();
            if (GUILayout.Button(text, currentStyle, options))
            {
                onClick?.Invoke();
                return true;
            }
            return false;
        }

        public static bool AddButton(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultButtonStyle, () => GUI.skin.button);
            options = options ?? Array.Empty<GUILayoutOption>();
            return GUILayout.Button(text, currentStyle, options);
        }

        // --- Toggle ---
        public static bool AddToggle(string text, ref bool toggleState, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultToggleStyle, () => GUI.skin.toggle);
            options = options ?? Array.Empty<GUILayoutOption>();
            bool previousState = toggleState;
            toggleState = GUILayout.Toggle(toggleState, text, currentStyle, options);
            return toggleState != previousState;
        }

        // --- Slider ---
        public static bool AddSlider(string label, ref float sliderValue, float minValue, float maxValue,
                                     GUIStyle labelStyle = null, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null,
                                     bool showValue = true, params GUILayoutOption[] options)
        {
            float previousValue = sliderValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle, () => GUI.skin.label);
            GUIStyle currentSliderStyle = GetEffectiveStyle(sliderStyle, () => Window.DefaultHorizontalSliderStyle, () => GUI.skin.horizontalSlider);
            GUIStyle currentThumbStyle = GetEffectiveStyle(thumbStyle, () => Window.DefaultHorizontalSliderThumbStyle, () => GUI.skin.horizontalSliderThumb);

            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue:F2})" : label;
                GUILayout.Label(labelText, currentLabelStyle, GUILayout.Width(150)); // Consider making width configurable or part of style
            }

            options = (options == null || options.Length == 0) ? new[] { GUILayout.ExpandWidth(true) } : options;
            sliderValue = GUILayout.HorizontalSlider(sliderValue, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            GUILayout.EndHorizontal();
            return !Mathf.Approximately(sliderValue, previousValue);
        }

        public static bool AddSlider(string label, ref int sliderValue, int minValue, int maxValue,
                                     GUIStyle labelStyle = null, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null,
                                     bool showValue = true, params GUILayoutOption[] options)
        {
            int previousValue = sliderValue;
            float tempFloat = sliderValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle, () => GUI.skin.label);
            GUIStyle currentSliderStyle = GetEffectiveStyle(sliderStyle, () => Window.DefaultHorizontalSliderStyle, () => GUI.skin.horizontalSlider);
            GUIStyle currentThumbStyle = GetEffectiveStyle(thumbStyle, () => Window.DefaultHorizontalSliderThumbStyle, () => GUI.skin.horizontalSliderThumb);

            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue})" : label;
                GUILayout.Label(labelText, currentLabelStyle, GUILayout.Width(150));
            }

            options = (options == null || options.Length == 0) ? new[] { GUILayout.ExpandWidth(true) } : options;
            tempFloat = GUILayout.HorizontalSlider(tempFloat, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            sliderValue = Mathf.RoundToInt(tempFloat);
            GUILayout.EndHorizontal();
            return sliderValue != previousValue;
        }

        // --- Text Field / Text Area ---
        public static bool AddTextField(string label, ref string textValue, GUIStyle labelStyle = null, GUIStyle fieldStyle = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle, () => GUI.skin.label);
            GUIStyle currentFieldStyle = GetEffectiveStyle(fieldStyle, () => Window.DefaultTextFieldStyle, () => GUI.skin.textField);

            options = options ?? Array.Empty<GUILayoutOption>();
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, currentLabelStyle); // Use AddLabel's style logic for label
            textValue = GUILayout.TextField(textValue, currentFieldStyle, options);
            return textValue != previousValue;
        }

        public static bool AddTextArea(string label, ref string textValue, GUIStyle labelStyle = null, GUIStyle areaStyle = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle, () => GUI.skin.label);
            GUIStyle currentAreaStyle = GetEffectiveStyle(areaStyle, () => Window.DefaultTextAreaStyle, () => GUI.skin.textArea);

            options = options ?? Array.Empty<GUILayoutOption>();
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, currentLabelStyle);
            textValue = GUILayout.TextArea(textValue, currentAreaStyle, options);
            return textValue != previousValue;
        }

        // --- Label ---
        public static void AddLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultLabelStyle, () => GUI.skin.label);
            options = options ?? Array.Empty<GUILayoutOption>();
            GUILayout.Label(text, currentStyle, options);
        }

        // --- SubSection Helpers ---
        // Removed InitializeDefaultSubSectionStyles as Window.EnsureStylesInitialized handles all defaults.
        public static void BeginSubSection(string title = null, GUIStyle boxStyle = null, GUIStyle titleStyle = null, params GUILayoutOption[] options)
        {
            GUIStyle currentBoxStyle = GetEffectiveStyle(boxStyle, () => Window.DefaultSectionStyle, () => GUI.skin.box);
            GUIStyle currentTitleStyle = GetEffectiveStyle(titleStyle, () => Window.DefaultTitleStyle, () => GUI.skin.label);

            GUILayoutOption[] verticalOptions = (options != null && options.Length > 0) ? options : new[] { GUILayout.ExpandWidth(true) };
            GUILayout.BeginVertical(currentBoxStyle, verticalOptions);
            if (!string.IsNullOrEmpty(title))
            {
                AddLabel(title, currentTitleStyle, GUILayout.ExpandWidth(true)); // Use AddLabel for title
                GUILayout.Space(5);
            }
        }

        public static void EndSubSection()
        {
            GUILayout.EndVertical();
            GUILayout.Space(10); // Consistent spacing
        }
    }
}