using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BaseVariable), true)]
public class BaseVariableDrawer : PropertyDrawer
{
    static class Styles
    {
        public static GUIContent createGUIContent = EditorGUIUtility.TrTextContentWithIcon("Create", "CreateAddNew");
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.objectReferenceValue != null)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }
        var leftRect = new Rect(position.position, Vector2.Scale(position.size, new Vector2(0.75f, 1f)));
        var rightRect = new Rect(position.position + Vector2.right * position.width * 0.75f, Vector2.Scale(position.size, new Vector2(0.25f, 1f)));
        EditorGUI.PropertyField(leftRect, property, label);
        if(GUI.Button(rightRect, Styles.createGUIContent))
        {
            var path = EditorUtility.SaveFilePanel("Save as", Application.dataPath, property.displayName.Replace(" ", ""), "asset");
            if (path.Length == 0 || string.IsNullOrEmpty(path))
                return;
            var length = property.type.Length - property.type.IndexOf("$") - 1;
            var className = property.type.Substring(property.type.IndexOf("$") + 1, length - 1);
            var scriptableObj = ScriptableObject.CreateInstance(className);
            AssetDatabase.CreateAsset(scriptableObj, MakeRelativePath(path));
            AssetDatabase.SaveAssets();
            property.objectReferenceValue = scriptableObj;
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    private string MakeRelativePath(string absolutePath)
    {
        return absolutePath.Substring(absolutePath.IndexOf("Assets"));
    }
}
