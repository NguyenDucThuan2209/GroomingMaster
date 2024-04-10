using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class TransformHelper
    {
        #region Extension
        public static void ResetTransform(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        public static void CloneTransform(this Transform transform, Transform target, TransformationConstraint constraint = TransformationConstraint.None, Space space = Space.World)
        {
            if (space == Space.World)
            {
                // Bitwise apply
                if ((constraint & TransformationConstraint.Position) == TransformationConstraint.Position)
                    transform.position = target.position;
                if ((constraint & TransformationConstraint.Rotation) == TransformationConstraint.Rotation)
                    transform.rotation = target.rotation;
            }
            else
            {
                // Bitwise apply
                if ((constraint & TransformationConstraint.Position) == TransformationConstraint.Position)
                    transform.localPosition = target.localPosition;
                if ((constraint & TransformationConstraint.Rotation) == TransformationConstraint.Rotation)
                    transform.localRotation = target.localRotation;
            }
            if ((constraint & TransformationConstraint.Scale) == TransformationConstraint.Scale)
                transform.localScale = target.localScale;
        }
        public static void CloneRectTransform(this RectTransform transform, RectTransform target)
        {
            transform.pivot = target.pivot;
            transform.sizeDelta = target.sizeDelta;
            transform.anchorMin = target.anchorMin;
            transform.anchorMax = target.anchorMax;
            transform.offsetMin = target.offsetMin;
            transform.offsetMax = target.offsetMax;
            transform.anchoredPosition3D = target.anchoredPosition3D;
        }
        #endregion
    }
}