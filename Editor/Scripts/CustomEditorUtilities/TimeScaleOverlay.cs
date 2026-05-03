using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.CustomEditorUtilities
{
    internal abstract class MainToolbarSliderExample
    {
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 10f;

        private const string TimeScaleToolbarPath = "TimeScale";
        private const string TimeScaleDisplayName = "Time Scale";

        private const string OverrideButtonTooltip = "If true overrides any timeScale change to specified in slider";
        private const string ResetTooltip = "Reset Time Scale to 1";

        private const string ResetIconName = "Refresh";
        private const string OverrideIconName = "LockIcon-On";

        private static float _currentTimeScale;
        private static bool _isOverride;

        private static MainToolbarSlider _mainToolbarElement;

        [MainToolbarElement(TimeScaleToolbarPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        internal static IEnumerable<MainToolbarElement> TimeSlider()
        {
            var sliderContent = new MainToolbarContent(TimeScaleDisplayName);

            yield return new MainToolbarSlider(
                sliderContent,
                Time.timeScale,
                MinTimeScale,
                MaxTimeScale,
                SetTimeScale);

            var resetIcon = EditorGUIUtility.IconContent(ResetIconName).image as Texture2D;
            var resetContent = new MainToolbarContent(resetIcon, ResetTooltip);
            yield return new MainToolbarButton(resetContent, ResetTimeScale);

            var overrideIcon = EditorGUIUtility.IconContent(OverrideIconName).image as Texture2D;
            var buttonContent = new MainToolbarContent(overrideIcon, OverrideButtonTooltip);
            yield return new MainToolbarToggle(buttonContent, false, ToggleOverride);
        }

        private static void ToggleOverride(bool isActive)
        {
            if (isActive)
            {
                EditorApplication.update += OverrideTimeScale;
                return;
            }

            EditorApplication.update -= OverrideTimeScale;
        }

        private static void OverrideTimeScale()
        {
            Time.timeScale = _currentTimeScale;
        }

        private static void ResetTimeScale()
        {
            SetTimeScale(1f);
            MainToolbar.Refresh(TimeScaleToolbarPath);
        }

        private static void SetTimeScale(float newValue)
        {
            Time.timeScale = newValue;
            _currentTimeScale = newValue;
        }
    }
}