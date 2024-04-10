using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using HyrphusQ.Events;
using HyrphusQ.Helpers;

[CustomEditor(typeof(LevelEventListeners))]
public class LevelEventListenersEditor : Editor
{
    private GenericMenu menu;
    private Type[] eventCodeEnumTypes;
    private SerializedProperty listEventsSerializedProp;

    private LevelEventListeners data
    {
        get
        {
            return target as LevelEventListeners;
        }
    }

    private void OnEnable()
    {
        eventCodeEnumTypes = ReflectionHelper.GetAllTypesInNamespace("HyrphusQ.Events", type => type.IsEnum && type.GetCustomAttribute(typeof(EventCodeAttribute)) != null);
        listEventsSerializedProp = serializedObject.FindProperty("listEvents");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DoLayoutListEvent();
    }

    private void CreateMenuEventCodeOptions()
    {
        menu = new GenericMenu();
        foreach (var eventCodeType in eventCodeEnumTypes)
        {
            foreach (var eventCode in Enum.GetValues(eventCodeType))
            {
                if (data.ContainsEvent(eventCode.ToString()))
                    continue;
                menu.AddItem(new GUIContent($"{eventCodeType.Name}/{eventCode}"), false, OnAddEvent, Tuple.Create(eventCodeType.AssemblyQualifiedName, Enum.GetName(eventCodeType, eventCode)));
            }
        }
        menu.ShowAsContext();
    }
    private void OnAddEvent(object userData)
    {
        var tuple = (Tuple<string, string>) userData;
        data.listEvents.Add(new LevelEventListeners.EventTuple(tuple.Item1, tuple.Item2));
        EditorUtility.SetDirty(target);
    }
    private void DoLayoutListEvent()
    {
        GUIStyle toolbarEditorStyle = EditorStyles.toolbar;
        toolbarEditorStyle.fontSize *= 2;

        // Layout header label
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("EventCode", toolbarEditorStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));
        EditorGUILayout.LabelField("EventAction", toolbarEditorStyle);
        EditorGUILayout.EndHorizontal();

        // Layout space
        EditorGUILayout.Space();

        // Layout element
        for (int i = 0; i < data.listEvents.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            var enumType = Type.GetType(data.listEvents[i].eventType);
            var displayedOptions = Enum.GetNames(enumType).ToList().FindAll(item => item == data.listEvents[i].eventCode || !data.listEvents.Exists(element => element.eventCode == item)).ToArray();
            var optionValues = Enum.GetValues(enumType).Cast<int>().Where(item => Enum.ToObject(enumType, item).ToString() == data.listEvents[i].eventCode || !data.ContainsEvent(Enum.ToObject(enumType, item).ToString())).ToArray();
            if (optionValues.Length <= 0)
                Debug.LogError("Not found any EventCode");
            var eventCodeObject = Enum.ToObject(enumType, EditorGUILayout.IntPopup((int) Enum.Parse(enumType, data.listEvents[i].eventCode), displayedOptions, optionValues, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)));
            data.listEvents[i].eventCode = Enum.GetName(enumType, eventCodeObject);
            data.listEvents[i].eventType = enumType.AssemblyQualifiedName;
            EditorGUILayout.PropertyField(listEventsSerializedProp.GetArrayElementAtIndex(i).FindPropertyRelative("unityEvent"));
            if (GUILayout.Button(EditorGUIUtility.TrTextContentWithIcon(string.Empty, "TreeEditor.Trash"), GUILayout.Width(28f), GUILayout.Height(28f)))
                data.listEvents.RemoveAt(i);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(target);

            // Save data
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.TrTextContent("Add EventAction")))
                CreateMenuEventCodeOptions();
            GUILayout.Space(18f);
        }

    }
}
