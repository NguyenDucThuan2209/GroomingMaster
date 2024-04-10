using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using HyrphusQ.Helpers;
using System.Reflection;
using System.Linq;
using HyrphusQ.Events;

[CustomPropertyDrawer(typeof(EventCode))]
public class EventCodeDrawer : PropertyDrawer
{
    private readonly static float offset = 2.5f;

    public static class Styles
    {
        public static GUIContent ChangeBtnIcon = EditorGUIUtility.TrTextContentWithIcon("Pick","d_CollabChangesDeleted Icon");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var eventTypeSerializedProp = property.FindPropertyRelative("m_EventType");
        var eventCodeSerializedProp = property.FindPropertyRelative("m_EventCode");

        var buttonRect = Rect.zero;

        if (!string.IsNullOrEmpty(eventCodeSerializedProp.stringValue) && !string.IsNullOrEmpty(eventTypeSerializedProp.stringValue))
        {
            var eventEnumRect = new Rect(position.x, position.y, position.width * 0.8f, position.height);
            buttonRect = new Rect(position.x + eventEnumRect.width + offset, position.y, position.width * 0.2f - offset, position.height);

            EditorGUI.BeginChangeCheck();
            var enumType = Type.GetType(eventTypeSerializedProp.stringValue);
            var displayedOptions = Enum.GetNames(enumType);
            var optionValues = Enum.GetValues(enumType).Cast<int>().ToArray();
            if (optionValues.Length <= 0)
                Debug.LogError("Not found any EventCode");
            var eventCodeObject = Enum.ToObject(enumType, EditorGUI.IntPopup(eventEnumRect, label.text, (int)Enum.Parse(enumType, eventCodeSerializedProp.stringValue), displayedOptions, optionValues));
            if (EditorGUI.EndChangeCheck())
            {
                eventCodeSerializedProp.stringValue = Enum.GetName(enumType, eventCodeObject);
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
        else
        {
            buttonRect = new Rect(position.x + position.width * 0.4f + 7.5f, position.y, position.width * 0.6f - 7.5f, position.height);
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            GUI.Label(labelRect, label);
        }
        if (GUI.Button(buttonRect, Styles.ChangeBtnIcon))
        {
            CreateMenuEventCodeOptions(property);
        }
    }
    private void CreateMenuEventCodeOptions(SerializedProperty property)
    {
        var eventTypeSerializedProp = property.FindPropertyRelative("m_EventType");
        var eventCodeSerializedProp = property.FindPropertyRelative("m_EventCode");

        GenericMenu menu = new GenericMenu();
        Type[] eventCodeEnumTypes = ReflectionHelper.GetAllTypesInNamespace("HyrphusQ.Events", type => type.IsEnum && type.GetCustomAttribute(typeof(EventCodeAttribute)) != null);
        foreach (var eventCodeType in eventCodeEnumTypes)
        {
            foreach (var eventCode in Enum.GetValues(eventCodeType))
            {
                menu.AddItem(new GUIContent($"{eventCodeType.Name}/{eventCode}"), false, userData => {
                    var tuple = (Tuple<string, string>) userData;
                    eventTypeSerializedProp.stringValue = tuple.Item1;
                    eventCodeSerializedProp.stringValue = tuple.Item2;
                    property.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }, Tuple.Create(eventCodeType.AssemblyQualifiedName, Enum.GetName(eventCodeType, eventCode)));
            }
        }
        menu.ShowAsContext();
    }
}
