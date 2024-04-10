using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HyrphusQ.SerializedDataStructure.WrappedTuple<,>), true)]
public class WrappedTupleDrawer : PropertyDrawer
{
    private readonly static string HeadFieldName = "m_Head";
    private readonly static string TailFieldName = "m_Tail";
    private readonly static float[] distributeOffset = new float[2] { 20f, 5f };
    private readonly static float[] distributeWidth = new float[3] { 0.2f, 0.35f, 0.45f };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect labelRect =
            new Rect(position.position,
            new Vector2(position.width * distributeWidth[0] + distributeOffset[0] / 2f, position.height));
        Rect leftRect =
            new Rect(position.position + Vector2.right * (labelRect.width + distributeOffset[0]),
            new Vector2(position.width * distributeWidth[1] - distributeOffset[0], position.height));
        Rect rightRect =
            new Rect(position.position + Vector2.right * (labelRect.width + distributeOffset[0] + leftRect.width + distributeOffset[1]),
            new Vector2(position.width * distributeWidth[2] - distributeOffset[1] - 10f, position.height));

        var headSerializedProp = property.FindPropertyRelative(HeadFieldName);
        var tailSerializedProp = property.FindPropertyRelative(TailFieldName);
        EditorGUI.LabelField(labelRect, property.displayName);
        EditorGUI.PropertyField(leftRect, headSerializedProp, GUIContent.none, true);
        EditorGUI.PropertyField(rightRect, tailSerializedProp, GUIContent.none, true);
    }
}
