using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    public static class UIStyles
    {
        public const int BORDER_WIDTH = 30;
        public const int INSPECTOR_WIDTH = 300;
        public const int TILE_WIDTH = 150;
        public const int TILE_HEIGHT = 150;
        public const int BUTTON_WIDTH = 25;

        private const int entryFontSize = 11;
        private const int entryFixedHeight = entryFontSize + 7;
        private const int toggleFixedWidth = 10;

        public static readonly GUIStyle tile = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedWidth = TILE_WIDTH,
            fixedHeight = TILE_HEIGHT,
            fontSize = 10,
            imagePosition = ImagePosition.ImageAbove,
            wordWrap = true
        };

        public static readonly GUILayoutOption[] buttonStyles = {GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_WIDTH)};
        public static readonly GUIStyle background = new GUIStyle("CN EntryBackodd") {fixedHeight = entryFixedHeight};
        public static readonly GUIStyle oddRow = new GUIStyle("CN EntryBackodd") {fixedHeight = entryFixedHeight};
        public static readonly GUIStyle evenRow = new GUIStyle("CN EntryBackEven") {fixedHeight = entryFixedHeight};
        public static readonly GUIStyle entryStyle = new GUIStyle(EditorStyles.miniLabel) {fontSize = entryFontSize, fixedHeight = entryFixedHeight};
        public static readonly GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle) {fixedWidth = toggleFixedWidth, fixedHeight = entryFixedHeight};
        public static readonly GUIStyle whiteCenter = new GUIStyle {alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState {textColor = Color.white}};
        public static readonly GUIStyle centerLabel = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
    }
}