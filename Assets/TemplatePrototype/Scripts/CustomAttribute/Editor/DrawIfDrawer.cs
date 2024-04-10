using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfDrawer : PropertyDrawer
{
    private DrawIfAttribute drawIfAttribute;

    private bool IsDrawable(SerializedProperty property)
    {
        drawIfAttribute = attribute as DrawIfAttribute;
        var obj = property.serializedObject.targetObject;
        var type = obj.GetType();
        var comparisonMethod = type.GetMethod(drawIfAttribute.comparisonMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (comparisonMethod == null)
        {
            Debug.LogError("Can not find method with name: " + drawIfAttribute.comparisonMethodName);
            return false;
        }
        return (bool)comparisonMethod.Invoke(obj, null);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!IsDrawable(property))
            return 0f;
        return base.GetPropertyHeight(property, label);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsDrawable(property))
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
    }
}
