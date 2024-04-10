using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HyrphusQ.Helpers;
using HyrphusQ.SerializedDataStructure;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
public class SerializedDictionaryDrawer : PropertyDrawer
{
    private static readonly float dictionarySizeFieldWidth = 50f;
    private static readonly float removeButtonWidth = 35f;
    private static readonly float keyValueOffset = EditorGUIUtility.singleLineHeight * 0.25f;
    private static readonly Vector2 keyValueLabelOffset = new Vector2(5f, EditorGUIUtility.singleLineHeight * 0.5f);
    private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    static class Styles
    {
        public static GUIContent addItemGUIContent = EditorGUIUtility.TrTextContentWithIcon("Add", "CreateAddNew");
        public static GUIContent keyLabelGUIContent = EditorGUIUtility.TrTextContent("Key");
        public static GUIContent valueLabelGUIContent = EditorGUIUtility.TrTextContent("Value");
        public static GUIContent removeItemGUIContent = EditorGUIUtility.TrTextContentWithIcon(string.Empty, "TreeEditor.Trash");
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.serializedObject.Update();

        SerializedProperty keysSerializedProp = property.FindPropertyRelative("keys");
        SerializedProperty valuesSerializedProp = property.FindPropertyRelative("values");


        Rect propertyLabelRect = new Rect(
            position.position, 
            new Vector2(position.width - dictionarySizeFieldWidth, EditorGUIUtility.singleLineHeight));

        Rect dictionarySizeRect = new Rect(
            position.position + Vector2.right * propertyLabelRect.width,
            new Vector2(dictionarySizeFieldWidth, EditorGUIUtility.singleLineHeight));

        Rect keyLabelRect = new Rect(
            position.position + Vector2.up * EditorGUIUtility.singleLineHeight * 1.5f,
            new Vector2(position.width * 0.5f, EditorGUIUtility.singleLineHeight));

        Rect valueLabelRect = new Rect(
            keyLabelRect.position + Vector2.right * (position.width * 0.5f), 
            keyLabelRect.size);

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(propertyLabelRect, property.isExpanded, label);

        GUI.enabled = false;
        EditorGUI.IntField(dictionarySizeRect, GUIContent.none, keysSerializedProp.arraySize);
        GUI.enabled = true;

        if (property.isExpanded)
        {
            var keyType = GetKeyType(property);
            if (!CheckSupportKeyType(keyType))
            {
                position.y += EditorGUIUtility.singleLineHeight * 1.25f;
                position.height = EditorGUIUtility.singleLineHeight * 2f;
                EditorGUI.HelpBox(position, "Key type is not support", MessageType.Error);
                EditorGUI.EndFoldoutHeaderGroup();
                return;
            }
            // Draw toolbar label
            GUIStyle toolbarStyle = new GUIStyle(EditorStyles.toolbar);
            toolbarStyle.fontSize = 12;
            toolbarStyle.fontStyle = FontStyle.Bold;
            toolbarStyle.padding.top = 3;

            EditorGUI.LabelField(keyLabelRect, Styles.keyLabelGUIContent, toolbarStyle);
            EditorGUI.LabelField(valueLabelRect, Styles.valueLabelGUIContent, toolbarStyle);
            Rect keyRect = new Rect(keyLabelRect.position, new Vector2((position.width - removeButtonWidth - 2f * keyValueLabelOffset.x) * 0.5f, 0f));
            Rect valueRect = new Rect(keyRect.position + Vector2.right * (keyRect.width + keyValueLabelOffset.x), keyRect.size);
            Rect removeBtnRect = new Rect(valueRect.position + Vector2.right * (valueRect.width + keyValueLabelOffset.x), new Vector2(removeButtonWidth, 0f));
            if(keysSerializedProp.isArray && keysSerializedProp.arraySize > 0)
            {
                for (int i = 0; i < keysSerializedProp.arraySize; i++)
                {
                    var keyProperty = keysSerializedProp.GetArrayElementAtIndex(i);
                    var valueProperty = valuesSerializedProp.GetArrayElementAtIndex(i);
                    var height = GetHeightOfProperty(keyProperty, valueProperty) + keyValueOffset;
                    keyRect.position += Vector2.up * height;
                    keyRect.height = height;
                    valueRect.position += Vector2.up * height;
                    valueRect.height = height;
                    removeBtnRect.position += Vector2.up * height;
                    removeBtnRect.height = EditorGUIUtility.singleLineHeight;
                    DrawKeyValueProperty(keyProperty, valueProperty, keyRect, valueRect);
                    if(GUI.Button(removeBtnRect, Styles.removeItemGUIContent))
                    {
                        if(keysSerializedProp.propertyType == SerializedPropertyType.ObjectReference && keysSerializedProp.objectReferenceValue != null)
                            keysSerializedProp.DeleteArrayElementAtIndex(i);
                        keysSerializedProp.DeleteArrayElementAtIndex(i);
                        if (valuesSerializedProp.propertyType == SerializedPropertyType.ObjectReference && valuesSerializedProp.objectReferenceValue != null)
                            valuesSerializedProp.DeleteArrayElementAtIndex(i);
                        valuesSerializedProp.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            Rect addButtonRect = new Rect(
                keyRect.position + new Vector2(position.width * 0.7f, Mathf.Max(keyRect.height, keyLabelRect.height) + keyValueOffset),
                new Vector2(position.width * 0.3f, EditorGUIUtility.singleLineHeight));

            if (GUI.Button(addButtonRect, Styles.addItemGUIContent))
                OnClickAddItem(property, keysSerializedProp, valuesSerializedProp);
        }
        EditorGUI.EndFoldoutHeaderGroup();

        property.serializedObject.ApplyModifiedProperties();
    }
    private void DrawKeyValueProperty(SerializedProperty key, SerializedProperty value, Rect keyRect, Rect valueRect)
    {
        EditorGUI.PropertyField(keyRect, key, GUIContent.none, true);
        EditorGUI.PropertyField(valueRect, value, GUIContent.none, true);
    }
    private float GetHeightOfProperty(SerializedProperty key, SerializedProperty value)
    {
        var keyPropertyHeight = EditorGUI.GetPropertyHeight(key);
        var valuePropertyHeight = EditorGUI.GetPropertyHeight(value);
        return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            var keyType = GetKeyType(property);
            if (!CheckSupportKeyType(keyType))
                return EditorGUIUtility.singleLineHeight * 3.5f;
            var totalHeight = EditorGUIUtility.singleLineHeight * 4f;
            var keysSerializedProp = property.FindPropertyRelative("keys");
            var valuesSerializedProp = property.FindPropertyRelative("values");
            for (int i = 0; i < valuesSerializedProp.arraySize; i++)
            {
                var keyProperty = keysSerializedProp.GetArrayElementAtIndex(i);
                var valueProperty = valuesSerializedProp.GetArrayElementAtIndex(i);
                totalHeight += GetHeightOfProperty(keyProperty, valueProperty) + keyValueOffset;
            }
            return totalHeight;
        }
        return EditorGUIUtility.singleLineHeight;
    }
    private void OnClickAddItem(SerializedProperty property, SerializedProperty keysSerializedProp, SerializedProperty valuesSerializedProp)
    {
        if(keysSerializedProp.arraySize <= 0)
        {
            keysSerializedProp.InsertArrayElementAtIndex(0);
            valuesSerializedProp.InsertArrayElementAtIndex(0);
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            return;
        }
        if (TryGenerateKey(property, keysSerializedProp, out object generateKey))
        {
            AddNewItem(property, generateKey);
            property.serializedObject.UpdateIfRequiredOrScript();
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
    private bool TryGenerateKey(SerializedProperty property, SerializedProperty keysSerializedProp, out object generateKey)
    {
        generateKey = null;
        var firstElementSerializedProp = keysSerializedProp.GetArrayElementAtIndex(0);
        switch (firstElementSerializedProp.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
                break;
            case SerializedPropertyType.Generic:
                return false;
            case SerializedPropertyType.Integer:
                generateKey = default(int);
                break;
            case SerializedPropertyType.Boolean:
                generateKey = default(bool);
                break;
            case SerializedPropertyType.Float:
                generateKey = default(float);
                break;
            case SerializedPropertyType.String:
                generateKey = default(string);
                break;
            case SerializedPropertyType.Color:
                generateKey = default(Color);
                break;
            case SerializedPropertyType.LayerMask:
                generateKey = default(LayerMask);
                break;
            case SerializedPropertyType.Enum:
                if (keysSerializedProp.arraySize >= firstElementSerializedProp.enumDisplayNames.Length)
                    return false;
                var listEnumKeys = new List<string>();
                var enumType = GetKeyType(property);
                for (int i = 0; i < firstElementSerializedProp.enumNames.Length; i++)
                {
                    int j;
                    for (j = 0; j < keysSerializedProp.arraySize; j++)
                    {
                        var elementSerializedProp = keysSerializedProp.GetArrayElementAtIndex(j);
                        if (elementSerializedProp.enumValueIndex == i)
                            break;
                    }
                    if (j == keysSerializedProp.arraySize)
                        listEnumKeys.Add(firstElementSerializedProp.enumNames[i]);
                }
                generateKey = Enum.Parse(enumType, listEnumKeys[0]);
                break;
            case SerializedPropertyType.Vector2:
                generateKey = default(Vector2);
                break;
            case SerializedPropertyType.Vector3:
                generateKey = default(Vector3);
                break;
            case SerializedPropertyType.Vector4:
                generateKey = default(Vector4);
                break;
            case SerializedPropertyType.Rect:
                generateKey = default(Rect);
                break;
            case SerializedPropertyType.ArraySize:
                generateKey = default(int);
                break;
            case SerializedPropertyType.Character:
                generateKey = default(char);
                break;
            case SerializedPropertyType.AnimationCurve:
                generateKey = default(AnimationCurve);
                break;
            case SerializedPropertyType.Bounds:
                generateKey = default(Bounds);
                break;
            case SerializedPropertyType.Gradient:
                generateKey = default(Gradient);
                break;
            case SerializedPropertyType.Quaternion:
                generateKey = default(Quaternion);
                break;
            case SerializedPropertyType.ExposedReference:
                return false;
            case SerializedPropertyType.FixedBufferSize:
                return false;
            case SerializedPropertyType.Vector2Int:
                generateKey = default(Vector2Int);
                break;
            case SerializedPropertyType.Vector3Int:
                generateKey = default(Vector3Int);
                break;
            case SerializedPropertyType.RectInt:
                generateKey = default(RectInt);
                break;
            case SerializedPropertyType.BoundsInt:
                generateKey = default(BoundsInt);
                break;
            case SerializedPropertyType.ManagedReference:
                return false;
            default:
                return false;
        }
        return true;
    }
    private object GenerateValue(SerializedProperty property)
    {
        var valueType = GetValueType(property);
        if (valueType.IsValueType)
            return Activator.CreateInstance(valueType);
        return null;
    }
    private bool CheckSupportKeyType(Type type)
    {
        // Check property type is primitive
        if (type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(string))
            return true;
        // Check property type is UnityObject
        if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            return true;
        // Check property type is Vector
        if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4))
            return true;
        // Check property type is VectorInt
        if (type == typeof(Vector2Int) || type == typeof(Vector3Int))
            return true;
        // Check property type is AnimationCurve or LayerMask
        if (type == typeof(AnimationCurve) || type == typeof(LayerMask))
            return true;
        return false;
    }
    private void AddNewItem(SerializedProperty property, object generateKey)
    {
        var generateValue = GenerateValue(property);
        var genericDictionaryProp = property.serializedObject.targetObject.GetFieldValue<object>(property.propertyPath);
        var method = genericDictionaryProp.GetType().GetMethods(bindingFlags).Where(methodInfo => methodInfo.Name == "Add").FirstOrDefault(methodInfo => methodInfo.GetParameters().Length == 2);
        if (method == null)
            return;
        method.Invoke(genericDictionaryProp, new object[] { generateKey, generateValue });
    }
    private Type GetKeyType(SerializedProperty property) => GetType(property, "GetKeyType");
    private Type GetValueType(SerializedProperty property) => GetType(property, "GetValueType");
    private Type GetType(SerializedProperty property, string method)
    {
        var genericDictionary = property.serializedObject.targetObject.GetFieldValue<object>(property.propertyPath);
        var keyType = genericDictionary.InvokeMethod<Type>(method);
        return keyType;
    }
}
