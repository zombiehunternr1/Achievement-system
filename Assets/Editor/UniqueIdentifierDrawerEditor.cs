using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawerEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string assetPath = AssetDatabase.GetAssetPath(property.serializedObject.targetObject.GetInstanceID());
        string uniqueID = AssetDatabase.AssetPathToGUID(assetPath);
        property.stringValue = uniqueID;
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, property, label);
    }
    private void DrawLabelField(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));
    }
}
