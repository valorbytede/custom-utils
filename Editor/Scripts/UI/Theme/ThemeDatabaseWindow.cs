using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.Theme.Databases;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.UI.Theme
{
    internal sealed class ThemeDatabaseWindow : EditorWindow
    {
        private static readonly string[] _tabNames = { "Solid", "Gradient" };

        private int _selectedTab;

        [MenuItem(MenuItemNames.ThemeDatabasesMenuName)]
        private static void OpenWindow()
        {
            GetWindow<ThemeDatabaseWindow>(nameof(ThemeDatabaseWindow).ToSpacedWords());
        }

        private void CreateGUI()
        {
            var toolbar = new Toolbar();

            for (var i = 0; i < _tabNames.Length; i++)
            {
                var index = i;
                var button = new ToolbarButton(() => ShowTab(index))
                {
                    text = _tabNames[index]
                };

                toolbar.Add(button);
            }

            rootVisualElement.Add(toolbar);
            rootVisualElement.Add(new ScrollView { name = "tab-content" });

            ShowTab(0);
        }

        private void ShowTab(int index)
        {
            _selectedTab = index;

            var scrollView = rootVisualElement.Q<ScrollView>("tab-content");
            scrollView.Clear();

            Object target = _selectedTab == 0
                ? SolidColorDatabase.Instance
                : GradientColorDatabase.Instance;

            scrollView.Add(new InspectorElement(target));
        }
    }
}