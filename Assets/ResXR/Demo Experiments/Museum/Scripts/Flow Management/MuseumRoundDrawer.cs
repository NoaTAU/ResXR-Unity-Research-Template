#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Museum_Round))]
public class MuseumRoundDrawer : PropertyDrawer
{
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // When collapsed: 1 line (foldout)
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        var roundTypeProp = property.FindPropertyRelative("roundType");
        bool showDuration = (Museum_RoundType)roundTypeProp.enumValueIndex == Museum_RoundType.FreeExploration;

        // Expanded: roundType line + (optional duration line) + foldout line
        int lines = 2 + (showDuration ? 1 : 0);
        return lines * EditorGUIUtility.singleLineHeight
               + (lines - 1) * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var lineH = EditorGUIUtility.singleLineHeight;
        var spacing = EditorGUIUtility.standardVerticalSpacing;

        // Foldout
        var r = new Rect(position.x, position.y, position.width, lineH);
        property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        // Round Type
        r.y += lineH + spacing;
        var roundTypeProp = property.FindPropertyRelative("roundType");
        EditorGUI.PropertyField(r, roundTypeProp);

        // Duration (only for FreeExploration)
        bool showDuration = (Museum_RoundType)roundTypeProp.enumValueIndex == Museum_RoundType.FreeExploration;
        if (showDuration)
        {
            r.y += lineH + spacing;
            var durationProp = property.FindPropertyRelative("durationInSeconds");
            EditorGUI.PropertyField(r, durationProp, new GUIContent("Duration In Seconds"));
        }

        EditorGUI.indentLevel--;
    }
}
#endif
