using System;
using UnityEngine;


namespace Meowijuana_ButtonAPI.Meowzers
{
    public static class Logic
    {
        // --- Button ---
        public static bool AddButton(string text, Action onClick, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = style ?? GUI.skin.button;
            if (options == null) options = Array.Empty<GUILayoutOption>();
            if (GUILayout.Button(text, currentStyle, options))
            {
                onClick?.Invoke();
                return true;
            }
            return false;
        }

        public static bool AddButton(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = style ?? GUI.skin.button;
            if (options == null) options = Array.Empty<GUILayoutOption>();
            return GUILayout.Button(text, currentStyle, options);
        }

        // --- Toggle ---
        public static bool AddToggle(string text, ref bool toggleState, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = style ?? GUI.skin.toggle;
            if (options == null) options = Array.Empty<GUILayoutOption>();
            bool previousState = toggleState;
            toggleState = GUILayout.Toggle(toggleState, text, currentStyle, options);
            return toggleState != previousState;
        }

        // --- Slider ---
        // Horizontal Slider (float)
        public static bool AddSlider(string label, ref float sliderValue, float minValue, float maxValue, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)
        {
            float previousValue = sliderValue;
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue:F2})" : label;
                GUILayout.Label(labelText, new[] { GUILayout.Width(150) });
            }
            
            GUIStyle currentSliderStyle = sliderStyle ?? GUI.skin.horizontalSlider;
            GUIStyle currentThumbStyle = thumbStyle ?? GUI.skin.horizontalSliderThumb;
            if (options == null) options = Array.Empty<GUILayoutOption>();

            sliderValue = GUILayout.HorizontalSlider(sliderValue, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            GUILayout.EndHorizontal();
            return !Mathf.Approximately(sliderValue, previousValue);
        }

        // Horizontal Slider (int)
        public static bool AddSlider(string label, ref int sliderValue, int minValue, int maxValue, GUIStyle sliderStyle = null, GUIStyle thumbStyle = null, bool showValue = true, params GUILayoutOption[] options)
        {
            int previousValue = sliderValue;
            float tempFloat = sliderValue;

            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            if (!string.IsNullOrEmpty(label))
            {
                string labelText = showValue ? $"{label} ({sliderValue})" : label;
                GUILayout.Label(labelText, new[] { GUILayout.Width(150) });
            }

            GUIStyle currentSliderStyle = sliderStyle ?? GUI.skin.horizontalSlider;
            GUIStyle currentThumbStyle = thumbStyle ?? GUI.skin.horizontalSliderThumb;
            if (options == null) options = Array.Empty<GUILayoutOption>();

            tempFloat = GUILayout.HorizontalSlider(tempFloat, minValue, maxValue, currentSliderStyle, currentThumbStyle, options);
            sliderValue = Mathf.RoundToInt(tempFloat);
            GUILayout.EndHorizontal();

            return sliderValue != previousValue;
        }

        // --- Text Field / Text Area ---
        public static bool AddTextField(string label, ref string textValue, GUIStyle style = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            if (!string.IsNullOrEmpty(label))
                GUILayout.Label(label, Array.Empty<GUILayoutOption>());
            GUIStyle currentStyle = style ?? GUI.skin.textField;
            if (options == null) options = Array.Empty<GUILayoutOption>();
            textValue = GUILayout.TextField(textValue, currentStyle, options);
            return textValue != previousValue;
        }
        
        public static bool AddTextArea(string label, ref string textValue, GUIStyle style = null, params GUILayoutOption[] options)
        {
            string previousValue = textValue;
            if (!string.IsNullOrEmpty(label))
                GUILayout.Label(label, Array.Empty<GUILayoutOption>());
            GUIStyle currentStyle = style ?? GUI.skin.textArea;
            if (options == null) options = Array.Empty<GUILayoutOption>();
            textValue = GUILayout.TextArea(textValue, currentStyle, options);
            return textValue != previousValue;
        }

        // --- Label ---
        public static void AddLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUIStyle currentStyle = style ?? GUI.skin.label;
            if (options == null) options = Array.Empty<GUILayoutOption>();
            GUILayout.Label(text, currentStyle, options);
        }

        // --- SubSection Helpers ---
        private static GUIStyle _defaultBoxStyle;
        private static GUIStyle _defaultTitleStyle;

        public static void InitializeDefaultSubSectionStyles(GUIStyle boxStyle, GUIStyle titleStyle)
        {
            _defaultBoxStyle = boxStyle;
            _defaultTitleStyle = titleStyle;
        }
        
        public static void BeginSubSection(string title = null, GUIStyle boxStyle = null, GUIStyle titleStyle = null, params GUILayoutOption[] options)
        {
            GUIStyle currentBoxStyle = boxStyle ?? _defaultBoxStyle ?? GUI.skin.box;
            GUIStyle currentTitleStyle = titleStyle ?? _defaultTitleStyle ?? new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };

            GUILayoutOption[] verticalOptions = (options != null && options.Length > 0) ? options : new[] { GUILayout.ExpandWidth(true) };
            /*if (verticalOptions == null) verticalOptions = new GUILayoutOption[0];*/

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
            GUILayout.Space(10);
        }
    }
}