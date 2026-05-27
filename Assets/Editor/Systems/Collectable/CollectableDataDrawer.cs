using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CollectableData))]
public class CollectableDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty requirementTypeProp = property.FindPropertyRelative("_collectableRequirement");
        SerializedProperty singleRefProp = property.FindPropertyRelative("_collectableReference");
        SerializedProperty listRefProp = property.FindPropertyRelative("_collectableListReference");
        SerializedProperty minGoalProp = property.FindPropertyRelative("_minimumGoalAmount");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float y = position.y;

        // Use the label passed in from the outer PropertyField call as the label for the type dropdown
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), requirementTypeProp, label);
        y += lineHeight + spacing;

        CollectableRequirementType reqType = (CollectableRequirementType)requirementTypeProp.enumValueIndex;

        switch (reqType)
        {
            case CollectableRequirementType.SingleCollectable:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), singleRefProp);
                    break;
                }
            case CollectableRequirementType.AllCollectables:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), listRefProp);
                    break;
                }
            case CollectableRequirementType.Custom:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), listRefProp);
                    y += lineHeight + spacing;
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), minGoalProp);
                    break;
                }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty requirementTypeProp = property.FindPropertyRelative("_collectableRequirement");
        CollectableRequirementType reqType = (CollectableRequirementType)requirementTypeProp.enumValueIndex;

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        switch (reqType)
        {
            case CollectableRequirementType.Custom:
                {
                    return (lineHeight + spacing) * 3;
                }
            default:
                {
                    return (lineHeight + spacing) * 2;
                }
        }
    }
}