using System.Collections.Generic;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Formatter;
using CustomUtils.Runtime.UI.Theme;
using CustomUtils.Runtime.UI.Theme.Databases;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.UI.Theme
{
    [CustomPropertyDrawer(typeof(ThemeColorNameAttribute))]
    internal sealed class ThemeColorNamePropertyDrawer : PropertyDrawer
    {
        private const int DefaultRowCount = 1;
        private const int ExpandedRowCount = 3;

        private EditorStateControls _editorStateControls;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _editorStateControls ??=
                new EditorStateControls(property.serializedObject.targetObject, property.serializedObject);

            var colorType = property.GetPropertyFromParent<ColorType>(nameof(ColorData.ColorType));

            var rowCount = colorType != ColorType.None && !string.IsNullOrEmpty(property.stringValue)
                ? ExpandedRowCount
                : DefaultRowCount;

            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * rowCount;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var colorType = property.GetPropertyFromParent<ColorType>(nameof(ColorData.ColorType));

            if (colorType == ColorType.None)
                return;

            if (!TryGetColorNamesForType(colorType, out var colorNames, out var colorGuids))
            {
                var message = StringFormatter.Format("No {0} colors found in database.", colorType);
                EditorVisualControls.WarningBox(position, message);
                return;
            }

            var (colorName, _) = _editorStateControls.Dropdown(property, colorNames, colorGuids, GetRowRect(position, 0));

            DrawColorPreview(colorName, colorType, GetRowRect(position, 1));
            DrawDatabasePingButton(colorType, GetRowRect(position, 2));
        }

        private void DrawColorPreview(string colorName, ColorType colorType, Rect rect)
        {
            switch (colorType)
            {
                case ColorType.Solid:
                    if (SolidColorDatabase.Instance.TryGetColorByName(colorName, out var previewColor))
                        EditorVisualControls.ColorField(rect, "Preview", previewColor);
                    break;

                case ColorType.VertexGraphicGradient
                    or ColorType.TextGradient
                    or ColorType.ShaderGraphicGradient:
                    if (GradientColorDatabase.Instance.TryGetColorByName(colorName, out var gradient))
                        EditorVisualControls.GradientField(rect, "Preview", gradient);
                    break;
            }
        }

        private void DrawDatabasePingButton(ColorType colorType, Rect rect)
        {
            if (!GUI.Button(rect, "Select Database"))
                return;

            Object database = colorType switch
            {
                ColorType.Solid => SolidColorDatabase.Instance,
                ColorType.VertexGraphicGradient
                    or ColorType.TextGradient
                    or ColorType.ShaderGraphicGradient => GradientColorDatabase.Instance,
                _ => null
            };

            if (!database)
                return;

            EditorGUIUtility.PingObject(database);
            Selection.activeObject = database;
        }

        private bool TryGetColorNamesForType(
            ColorType colorType,
            out List<string> colorNames,
            out List<string> colorGuids)
        {
            (colorNames, colorGuids) = colorType switch
            {
                ColorType.Solid => (
                    SolidColorDatabase.Instance.GetColorNames(),
                    SolidColorDatabase.Instance.GetColorGuids()),
                ColorType.VertexGraphicGradient
                    or ColorType.TextGradient
                    or ColorType.ShaderGraphicGradient => (
                        GradientColorDatabase.Instance.GetColorNames(),
                        GradientColorDatabase.Instance.GetColorGuids()),
                _ => (null, null)
            };

            return !colorNames.IsNullOrEmpty() && !colorGuids.IsNullOrEmpty();
        }

        private static Rect GetRowRect(Rect position, int rowIndex) => new(
            position.x,
            position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * rowIndex,
            position.width,
            EditorGUIUtility.singleLineHeight
        );
    }
}