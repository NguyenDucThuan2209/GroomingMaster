using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TextureArraySO))]
public class TextureArraySOEditor : Editor
{
    static class Styles
    {
        public static GUIContent alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
        public static GUIContent RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
        public static GUIContent arrayIcon = EditorGUIUtility.IconContent("Texture2DArray");
    }

    private int m_Slice = 0;

    private SerializedProperty globalSettingSerializedProperty;
    private SerializedProperty mipmapSerializedProperty;
    private SerializedProperty anisoLevelSerializedProperty;
    private SerializedProperty filterModeSerializedProperty;
    private SerializedProperty textureWrapModeSerializedProperty;
    private SerializedProperty textureFormatSerializedProperty;
    private SerializedProperty textureArraySerializedProperty;
    private SerializedProperty propertyNameSerializedProperty;
    private SerializedProperty shaderNameSerializedProperty;
    private List<string> shaderNameOptions;
    private List<string> textureArrayPropertyNameOptions;
    private ReorderableList reorderableList;

    private TextureArraySO data
    {
        get => target as TextureArraySO;
    }

    private void OnEnable()
    {
        // Initalize properties
        globalSettingSerializedProperty = serializedObject.FindProperty("globalSetting");
        mipmapSerializedProperty = serializedObject.FindProperty("generateMipMaps");
        anisoLevelSerializedProperty = serializedObject.FindProperty("anisoLevel");
        filterModeSerializedProperty = serializedObject.FindProperty("filterMode");
        textureWrapModeSerializedProperty = serializedObject.FindProperty("textureWrapMode");
        textureFormatSerializedProperty = serializedObject.FindProperty("textureFormat");
        textureArraySerializedProperty = serializedObject.FindProperty("textureArray");
        propertyNameSerializedProperty = serializedObject.FindProperty("propertyName");
        shaderNameSerializedProperty = serializedObject.FindProperty("shaderName");

        // Instantiate reorderable list
        reorderableList = new ReorderableList(serializedObject, textureArraySerializedProperty, false, true, true, true);
        reorderableList.drawHeaderCallback += OnDrawHeaderCallback;
        reorderableList.drawElementCallback += OnDrawElementCallback;
        reorderableList.onAddCallback += OnAddElementCallback;
        reorderableList.onRemoveCallback += OnRemoveElementCallback;

        // Crawl all shader infos & Instantiate displayed options list
        shaderNameOptions = new List<string>();
        textureArrayPropertyNameOptions = new List<string>();

        var shaderInfos = ShaderUtil.GetAllShaderInfo();
        foreach (var item in shaderInfos)
        {
            var shader = Shader.Find(item.name);
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                if(ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv && ShaderUtil.GetTexDim(shader, i) == UnityEngine.Rendering.TextureDimension.Tex2DArray)
                {
                    shaderNameOptions.Add(shader.name);
                    textureArrayPropertyNameOptions.Add(ShaderUtil.GetPropertyName(shader, i));
                }
            }
        }
    }
    private void OnDisable()
    {
        shaderNameOptions.Clear();
        textureArrayPropertyNameOptions.Clear();
    }
    public override void OnInspectorGUI()
    {
        if(textureArrayPropertyNameOptions.Count <= 0)
        {
            EditorGUILayout.HelpBox("Can't find any shader has Tex2DArray property!!! Please write a custom shader define Tex2DArray property to using this asset.", MessageType.Error);
            return;
        }
        if (!SystemInfo.supports2DArrayTextures)
        {
            EditorGUI.DropShadowLabel(EditorGUILayout.GetControlRect(), "2D texture array not supported");
            return;
        }

        serializedObject.Update();
        // Initialize default value
        EditorGUILayout.PropertyField(globalSettingSerializedProperty);

        if (string.IsNullOrEmpty(propertyNameSerializedProperty.stringValue) || !textureArrayPropertyNameOptions.Contains(propertyNameSerializedProperty.stringValue))
            SetDataValue(0);
        EditorGUI.BeginChangeCheck();
        var index = EditorGUILayout.Popup(propertyNameSerializedProperty.displayName, textureArrayPropertyNameOptions.IndexOf(propertyNameSerializedProperty.stringValue), textureArrayPropertyNameOptions.ToArray());
        if (EditorGUI.EndChangeCheck())
            SetDataValue(index);

        EditorGUILayout.PropertyField(shaderNameSerializedProperty);
        EditorGUILayout.PropertyField(mipmapSerializedProperty);
        EditorGUILayout.PropertyField(anisoLevelSerializedProperty);
        EditorGUILayout.PropertyField(filterModeSerializedProperty);
        EditorGUILayout.PropertyField(textureWrapModeSerializedProperty);
        EditorGUILayout.PropertyField(textureFormatSerializedProperty);

        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        if (textureArraySerializedProperty.arraySize <= 0)
            return;
        var objectReferenceValue = textureArraySerializedProperty.GetArrayElementAtIndex(m_Slice).objectReferenceValue;
        if (objectReferenceValue == null)
        {
            EditorGUI.HelpBox(r, "Texture is null", MessageType.Warning);
            return;
        }
        var texture = (Texture2D) objectReferenceValue;
        if (!texture.isReadable)
        {
            EditorGUI.HelpBox(r, "Texture is not readable", MessageType.Warning);
            return;
        }
        GUI.DrawTexture(r, texture, ScaleMode.ScaleToFit);
    }
    public override void OnPreviewSettings()
    {
        base.OnPreviewSettings();
        if (textureArraySerializedProperty.arraySize <= 0)
            return;
        m_Slice = (int)PreviewSettingsSlider(m_Slice, 0, textureArraySerializedProperty.arraySize - 1);
    }
    public override bool HasPreviewGUI()
    {
        if (textureArraySerializedProperty.arraySize <= 0)
            return false;
        return true;
    }

    private void OnDrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, textureArraySerializedProperty.displayName, EditorStyles.centeredGreyMiniLabel);
    }

    private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorGUI.PropertyField(rect, textureArraySerializedProperty.GetArrayElementAtIndex(index), true);
    }

    private void OnAddElementCallback(ReorderableList list)
    {
        textureArraySerializedProperty.InsertArrayElementAtIndex(textureArraySerializedProperty.arraySize);
        EditorUtility.SetDirty(target);
    }

    private void OnRemoveElementCallback(ReorderableList list)
    {
        var element = textureArraySerializedProperty.GetArrayElementAtIndex(list.index);
        if (element.objectReferenceValue != null)
            element.objectReferenceValue = null;
        textureArraySerializedProperty.DeleteArrayElementAtIndex(list.index);
        EditorUtility.SetDirty(target);
    }
    private void SetDataValue(int index)
    {
        propertyNameSerializedProperty.stringValue = textureArrayPropertyNameOptions[index];
        shaderNameSerializedProperty.stringValue = shaderNameOptions[index];
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
    private float PreviewSettingsSlider(float value, float min, float max, float sliderWidth = 60, float floatFieldWidth = 30)
    {
        var controlRect = EditorGUILayout.GetControlRect(GUILayout.Width(sliderWidth + floatFieldWidth));
        var sliderRect = new Rect(controlRect.position, new Vector2(sliderWidth, controlRect.height));
        var floatFieldRect = new Rect(controlRect.position + Vector2.right * (sliderRect.width + 2), new Vector2(controlRect.width - sliderRect.width, controlRect.height));
        value = GUI.Slider(sliderRect, value, 0, min, max, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, 0);
        value = Mathf.Round(EditorGUI.IntField(floatFieldRect, Mathf.RoundToInt(value)));
        return value;
    }
}
