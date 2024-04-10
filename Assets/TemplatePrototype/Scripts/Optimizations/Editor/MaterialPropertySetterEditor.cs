using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System;
using UnityEditorInternal;
using HyrphusQ.Optimizations;

[CustomEditor(typeof(MaterialPropertySetter))]
public class MaterialPropertySetterEditor : Editor
{
    private MaterialPropertySetter data
    {
        get => target as MaterialPropertySetter;
    }

    private ReorderableList reorderablePropertiesList;
    private Dictionary<ShaderPropertyType, Tuple<List<string>, List<int>, int>> shaderPropertiesDict = new Dictionary<ShaderPropertyType, Tuple<List<string>, List<int>, int>>();
    private GenericMenu menu;

    private void OnEnable()
    {
        reorderablePropertiesList = new ReorderableList(data.materialProperties, typeof(MaterialProperty), false, true, true, true);
        reorderablePropertiesList.drawHeaderCallback += OnDrawHeader;
        reorderablePropertiesList.drawElementCallback += OnDrawElement;
        reorderablePropertiesList.onAddCallback += OnAddElement;
        reorderablePropertiesList.onRemoveCallback += OnRemoveElement;
        reorderablePropertiesList.elementHeightCallback += OnGetElementHeight;

        for (int i = 0; i < data.material.shader.GetPropertyCount(); i++)
        {
            var propertyType = data.material.shader.GetPropertyType(i);
            if (!shaderPropertiesDict.ContainsKey(propertyType))
                shaderPropertiesDict.Add(propertyType, Tuple.Create(new List<string>(), new List<int>(), i));
            var propertyName = data.material.shader.GetPropertyName(i);
            shaderPropertiesDict[propertyType].Item1.Add(propertyName);
            shaderPropertiesDict[propertyType].Item2.Add(i);
        }

        var propertyTypeValues = shaderPropertiesDict.Keys.ToArray();
        menu = new GenericMenu();
        for (int i = 0; i < propertyTypeValues.Length; i++)
            menu.AddItem(new GUIContent($"{propertyTypeValues.GetValue(i)}"), false, OnAddProperty, propertyTypeValues.GetValue(i));
    }
    private void OnDisable()
    {
        shaderPropertiesDict.Clear();
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        reorderablePropertiesList.DoLayoutList();
    }

    private void OnDrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Material Properties");
    }
    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorGUI.BeginChangeCheck();

        var property = data.materialProperties[index];
        var displayedOptions = shaderPropertiesDict[property.type].Item1;
        var optionValues = shaderPropertiesDict[property.type].Item2;
        var defaultIndex = shaderPropertiesDict[property.type].Item3;
        var topRect = new Rect(rect.position, new Vector2(rect.size.x, EditorGUIUtility.singleLineHeight));
        var bottomRect = new Rect(rect.position + Vector2.up * (EditorGUIUtility.singleLineHeight + 5f), new Vector2(rect.size.x, property.type == ShaderPropertyType.Texture ? EditorGUIUtility.singleLineHeight * 4f : EditorGUIUtility.singleLineHeight));
        if (property.propertyIndex == -1)
            property.propertyIndex = defaultIndex;
        property.propertyIndex = EditorGUI.IntPopup(topRect, data.material.shader.GetPropertyDescription(property.propertyIndex), property.propertyIndex, displayedOptions.ToArray(), optionValues.ToArray());
        property.propertyName = data.material.shader.GetPropertyName(property.propertyIndex);
        property.propertyID = data.material.shader.GetPropertyNameId(property.propertyIndex);

        switch (property.type)
        {
            case ShaderPropertyType.Color:
                property.colorValue = EditorGUI.ColorField(bottomRect, "Color", property.colorValue);
                break;
            case ShaderPropertyType.Vector:
                property.vectorValue = EditorGUI.Vector4Field(bottomRect, "Vector4", property.vectorValue);
                break;
            case ShaderPropertyType.Range:
                var range = data.material.shader.GetPropertyRangeLimits(property.propertyIndex);
                property.floatValue = EditorGUI.Slider(bottomRect, "Value", property.floatValue, range.x, range.y);
                break;
            case ShaderPropertyType.Float:
                property.floatValue = EditorGUI.FloatField(bottomRect, "Value", property.floatValue);
                break;
            case ShaderPropertyType.Texture:
                property.texture = (Texture)EditorGUI.ObjectField(bottomRect, "Texture", property.texture, typeof(Texture), false);
                break;
            default:
                break;
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);
    }
    private void OnAddElement(ReorderableList list)
    {
        menu.ShowAsContext();
    }
    private void OnRemoveElement(ReorderableList list)
    {
        data.materialProperties.RemoveAt(list.index);
    }
    private float OnGetElementHeight(int index)
    {
        if (index >= data.materialProperties.Count)
            return 0f;
        var property = data.materialProperties[index];
        switch (property.type)
        {
            case ShaderPropertyType.Color:
            case ShaderPropertyType.Vector:
            case ShaderPropertyType.Range:
            case ShaderPropertyType.Float:
                return EditorStyles.popup.fixedHeight * 2f + 10f;
            case ShaderPropertyType.Texture:
                return EditorStyles.popup.fixedHeight * 5f + 10f;
        }
        return 0f;
    }
    private void OnAddProperty(object shaderPropertyType)
    {
        if (shaderPropertyType == null)
            return;
        var materialProperty = new MaterialPropertySetter.MaterialProperty((ShaderPropertyType) shaderPropertyType);
        data.materialProperties.Add(materialProperty);
        EditorUtility.SetDirty(target);
    }
}
