using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ToggleTab))]
public class ToggleTabDrawer : PropertyDrawer
{
    /// <summary>
    /// Options to display in the popup to select constant or variable.
    /// </summary>
    private readonly string[] popupOptions =
        { "Use Text", "Use Image" };

    /// <summary> Cached style to use to draw the popup button. </summary>
    private GUIStyle popupStyle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        // Get properties
        SerializedProperty useText = property.FindPropertyRelative("useText");
        SerializedProperty baseText = property.FindPropertyRelative("baseText");
        SerializedProperty baseImage = property.FindPropertyRelative("baseImage");

        // Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        // Store old indent level and set it to 0, the PrefixLabel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int result = EditorGUI.Popup(buttonRect, useText.boolValue ? 0 : 1, popupOptions, popupStyle);

        useText.boolValue = result == 0;

        EditorGUI.PropertyField(position,
            useText.boolValue ? baseText : baseImage,
            GUIContent.none);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
