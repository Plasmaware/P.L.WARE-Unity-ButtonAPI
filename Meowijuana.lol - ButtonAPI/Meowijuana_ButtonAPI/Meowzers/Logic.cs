using System;
using UnityEngine;

namespace Meowijuana_ButtonAPI.API.Meowzers // Assuming this is the correct namespace from your last full code paste
{
    public static class Logic
    {
        private static GUIStyle GetEffectiveStyle(GUIStyle preferredStyle, Func<GUIStyle> defaultStyleProvider)
        {
            // Now this refers to Meowijuana_SARS.API.Meowzers.Window
            Window.EnsureStylesInitialized();
            return preferredStyle ?? defaultStyleProvider() ?? new GUIStyle();
        }

        // --- Button ---
        public static bool AddButtonOnClick(string text, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultButtonStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();

            if (GUILayout.Button(text, currentStyle, options))
            {
                onClick?.Invoke();
                return true;
            }
            return false;
        }

        public static bool AddButton(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultButtonStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            return GUILayout.Button(text, currentStyle, options);
        }

        // --- Toggle ---
        public static bool AddToggle(string text, ref bool toggleState, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultToggleStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            bool previousState = toggleState;
            toggleState = GUILayout.Toggle(toggleState, text, currentStyle, options);
            return toggleState != previousState;
        }

        // --- Slider ---
        public static bool AddSlider(string label, ref float sliderValue, float minValue, float maxValue, GUIStyle labelStyle = null, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)
        {
            float previousValue = sliderValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentSliderStyle = GetEffectiveStyle(sliderStyle, () => Window.DefaultHorizontalSliderStyle);
            GUIStyle currentThumbStyle = GetEffectiveStyle(thumbStyle, () => Window.DefaultHorizontalSliderThumbStyle);

            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>()); // Applied Fix
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue:F2})" : label;
                // Applied Fix: Wrapped GUILayout.Width in an array
                GUILayout.Label(labelText, currentLabelStyle, new GUILayoutOption[] { GUILayout.Width(150) });
            }
            if (options == null || options.Length == 0) options = new[] { GUILayout.ExpandWidth(true) };
            sliderValue = GUILayout.HorizontalSlider(sliderValue, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            GUILayout.EndHorizontal();
            return !Mathf.Approximately(sliderValue, previousValue);
        }

        public static bool AddHSlider(string label, ref int sliderValue, int minValue, int maxValue, GUIStyle labelStyle = null, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)
        {
            int previousValue = sliderValue;
            float tempFloat = sliderValue;

            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentSliderStyle = GetEffectiveStyle(sliderStyle, () => Window.DefaultHorizontalSliderStyle);
            GUIStyle currentThumbStyle = GetEffectiveStyle(thumbStyle, () => Window.DefaultHorizontalSliderThumbStyle);

            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>()); // Applied Fix (was already in your last paste)
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue})" : label;
                // Applied Fix: Wrapped GUILayout.Width in an array
                GUILayout.Label(labelText, currentLabelStyle, new GUILayoutOption[] { GUILayout.Width(150) });
            }
            if (options == null || options.Length == 0) options = new[] { GUILayout.ExpandWidth(true) };
            tempFloat = GUILayout.HorizontalSlider(tempFloat, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            sliderValue = Mathf.RoundToInt(tempFloat);
            GUILayout.EndHorizontal();
            return sliderValue != previousValue;
        }

        // --- Text Field / Text Area ---
        public static bool AddTextField(string label, ref string textValue, GUIStyle labelStyle = null, GUIStyle fieldStyle = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentFieldStyle = GetEffectiveStyle(fieldStyle, () => Window.DefaultTextFieldStyle);

            if (!string.IsNullOrEmpty(label))
            {
                // Applied Fix: Added Array.Empty<GUILayoutOption>()
                GUILayout.Label(label, currentLabelStyle, Array.Empty<GUILayoutOption>());
            }
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            textValue = GUILayout.TextField(textValue, currentFieldStyle, options);
            return textValue != previousValue;
        }

        public static bool AddTextArea(string label, ref string textValue, GUIStyle labelStyle = null, GUIStyle areaStyle = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentAreaStyle = GetEffectiveStyle(areaStyle, () => Window.DefaultTextAreaStyle);

            if (!string.IsNullOrEmpty(label))
            {
                // Applied Fix: Added Array.Empty<GUILayoutOption>()
                GUILayout.Label(label, currentAreaStyle, Array.Empty<GUILayoutOption>()); // Corrected to use currentAreaStyle if that was intended for label, or currentLabelStyle
            }
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            textValue = GUILayout.TextArea(textValue, currentAreaStyle, options);
            return textValue != previousValue;
        }

        // --- Label ---
        public static void AddLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultLabelStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            GUILayout.Label(text, currentStyle, options); // This was already correct
        }

        // --- SubSection Helpers ---
        // Inside Meowijuana_ButtonAPI.API.Meowzers.Logic
        public static void BeginSubSection(string title = null, GUIStyle boxStyle = null, GUIStyle titleStyle = null, params GUILayoutOption[] options)
        {
            // Ensure styles are initialized (this is good)
            Meowijuana_ButtonAPI.API.Meowzers.Window.EnsureStylesInitialized(); // Added this for robustness

            GUIStyle currentBoxStyle = GetEffectiveStyle(boxStyle, () => Meowijuana_ButtonAPI.API.Meowzers.Window.DefaultSectionStyle);
            GUIStyle currentTitleStyle = GetEffectiveStyle(titleStyle, () => Meowijuana_ButtonAPI.API.Meowzers.Window.DefaultTitleStyle);

            // This is where the GUILayoutOption array for BeginVertical is determined
            GUILayoutOption[] verticalOptions = (options != null && options.Length > 0) ? options : new[] { GUILayout.ExpandWidth(true) };

            // THE CALL CAUSING THE ERROR ACCORDING TO THE STACK TRACE
            GUILayout.BeginVertical(currentBoxStyle, verticalOptions);

            if (!string.IsNullOrEmpty(title))
            {
                // AddLabel itself contains GUILayout.Label
                AddLabel(title, currentTitleStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(5);
            }
        }
        public static void EndSubSection()
        {
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }
    }
}