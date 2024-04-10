using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CopyPasteTransformShortcut
{
    struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
    }

    private static TransformData transformData;

    [MenuItem("Edit/Copy Transform Value", false)]
    public static void CopyTransformValue()
    {
        if (Selection.gameObjects.Length == 0)
            return;
        var selectionTr = Selection.gameObjects[0].transform;
        transformData = new TransformData(selectionTr.position, selectionTr.rotation, selectionTr.localScale);
    }

    [MenuItem("Edit/Paste Transform Value", false)]
    public static void PasteTransformValue()
    {
        foreach (var item in Selection.gameObjects)
        {
            Transform selectionTr = item.transform;
            Undo.RecordObject(selectionTr, "Paste Transform Value");
            selectionTr.transform.position = transformData.position;
            selectionTr.transform.rotation = transformData.rotation;
            selectionTr.transform.localScale = transformData.localScale;
        }
    }
}
