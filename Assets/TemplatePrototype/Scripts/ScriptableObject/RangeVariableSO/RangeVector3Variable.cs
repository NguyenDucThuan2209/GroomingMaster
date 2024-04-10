using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeVector3Variable", menuName = "HyrphusQ/RangeVariableSO/Vector3")]
public class RangeVector3Variable : RangeVariable<Vector3>
{
    public override float CalcInverseLerpValue(Vector3 value)
    {
        if (IsOutOfRange(value))
            return 0f;
        var ba = maxValue - minValue;
        var ca = value - minValue;
        return Mathf.Clamp01(Vector3.Dot(ca, ba) / Vector3.Dot(ba, ba));
    }
    public override Vector3 CalcInterpolatedValue(float weight)
    {
        return Vector3.Lerp(minValue, maxValue, weight);
    }
    public override RangeVariableReference<Vector3> CreateRangeReference(Vector3 value)
    {
        return new RangeVariableReference<Vector3>(this, value);
    }
    public override bool IsOutOfRange(Vector3 value)
    {
        // Check if vector value is collinear with vector min and max by calculate the angle of 2 vector
        var ba = maxValue - minValue;
        var ca = value - minValue;
        if (!Mathf.Approximately(Vector3.Angle(ca, ba), 0f))
            return true;
        // Check whether magnitude of vector CA(min-value) is greater than BA(min-max) or not
        if (ca.magnitude > ba.magnitude)
            return true;
        return false;
    }
}
