using System;
using UnityEngine;

namespace Meowijuana_ButtonAPI.API.Meowzers
{
    public static class Logic
    {
        private static GUIStyle GetEffectiveStyle(GUIStyle preferredStyle, Func<GUIStyle> defaultStyleProvider)
        {
            // This call is crucial. Ensure Window.EnsureStylesInitialized() is called
            // at the beginning of your OnGUI method before any API calls.
            Window.EnsureStylesInitialized();

            // The preferredStyle ?? defaultStyleProvider() pattern is good.
            // The final '?? new GUIStyle()' is a safe fallback if a default style itself was somehow null,
            // preventing NullReferenceExceptions, though ideally defaultStyleProvider always returns a valid style.
            return preferredStyle ?? defaultStyleProvider() ?? new GUIStyle();
        }

        // --- Button ---
        public static bool AddButtonOnClick(string text, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultButtonStyle);
            // Array.Empty<T>() is generally fine with modern IL2CPP.
            // For extreme legacy compatibility, 'new GUILayoutOption[0]' could be used, but it's not usually necessary.
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();

            if (GUILayout.Button(text, currentStyle, options))
            {
                onClick?.Invoke(); // Safe invocation of the delegate.
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
            // GUILayout.Toggle is standard and IL2CPP-friendly.
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

            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                // String interpolation is fine with IL2CPP.
                string labelText = showValue ? $"{label} ({sliderValue:F2})" : label;
                GUILayout.Label(labelText, currentLabelStyle, GUILayout.Width(150)); // Consider making width configurable if needed
            }
            // Ensuring options are not null for GUILayout.HorizontalSlider is good.
            if (options == null || options.Length == 0) options = new[] { GUILayout.ExpandWidth(true) };
            sliderValue = GUILayout.HorizontalSlider(sliderValue, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            GUILayout.EndHorizontal();
            return !Mathf.Approximately(sliderValue, previousValue); // Mathf.Approximately is good for float comparisons.
        }

        public static bool AddHSlider(string label, ref int sliderValue, int minValue, int maxValue, GUIStyle labelStyle = null, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)
        {
            int previousValue = sliderValue;
            float tempFloat = sliderValue; // Using a temp float for GUILayout.HorizontalSlider is standard.

            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentSliderStyle = GetEffectiveStyle(sliderStyle, () => Window.DefaultHorizontalSliderStyle);
            GUIStyle currentThumbStyle = GetEffectiveStyle(thumbStyle, () => Window.DefaultHorizontalSliderThumbStyle);

            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue})" : label;
                GUILayout.Label(labelText, currentLabelStyle, GUILayout.Width(150));
            }
            if (options == null || options.Length == 0) options = new[] { GUILayout.ExpandWidth(true) };
            tempFloat = GUILayout.HorizontalSlider(tempFloat, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            sliderValue = Mathf.RoundToInt(tempFloat); // Mathf.RoundToInt is standard.
            GUILayout.EndHorizontal();
            return sliderValue != previousValue;
        }

        // --- Text Field / Text Area ---
        public static bool AddTextField(string label, ref string textValue, GUIStyle labelStyle = null, GUIStyle fieldStyle = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentFieldStyle = GetEffectiveStyle(fieldStyle, () => Window.DefaultTextFieldStyle);

            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, currentLabelStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            textValue = GUILayout.TextField(textValue, currentFieldStyle, options);
            return textValue != previousValue; // String comparison is fine.
        }

        public static bool AddTextArea(string label, ref string textValue, GUIStyle labelStyle = null, GUIStyle areaStyle = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            GUIStyle currentLabelStyle = GetEffectiveStyle(labelStyle, () => Window.DefaultLabelStyle);
            GUIStyle currentAreaStyle = GetEffectiveStyle(areaStyle, () => Window.DefaultTextAreaStyle);

            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, currentLabelStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            textValue = GUILayout.TextArea(textValue, currentAreaStyle, options);
            return textValue != previousValue;
        }

        // --- Label ---
        public static void AddLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = GetEffectiveStyle(style, () => Window.DefaultLabelStyle);
            if (options == null || options.Length == 0) options = Array.Empty<GUILayoutOption>();
            GUILayout.Label(text, currentStyle, options);
        }

        // --- SubSection Helpers ---
        public static void BeginSubSection(string title = null, GUIStyle boxStyle = null, GUIStyle titleStyle = null, params GUILayoutOption[] options)
        {
            GUIStyle currentBoxStyle = GetEffectiveStyle(boxStyle, () => Window.DefaultSectionStyle);
            GUIStyle currentTitleStyle = GetEffectiveStyle(titleStyle, () => Window.DefaultTitleStyle);

            // Using a new array if options is null/empty is correct.
            GUILayoutOption[] verticalOptions = (options != null && options.Length > 0) ? options : new[] { GUILayout.ExpandWidth(true) };
            GUILayout.BeginVertical(currentBoxStyle, verticalOptions);
            if (!string.IsNullOrEmpty(title))
            {
                AddLabel(title, currentTitleStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(5);
            }
        }
        public static void EndSubSection()
        {
            GUILayout.EndVertical();
            GUILayout.Space(10); // Standard GUILayout.Space.
        }
    }
}