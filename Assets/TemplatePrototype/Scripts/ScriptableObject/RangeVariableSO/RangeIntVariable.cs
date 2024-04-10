using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeIntVariable", menuName = "HyrphusQ/RangeVariableSO/Int")]
public class RangeIntVariable : RangeVariable<int>
{
    public override float CalcInverseLerpValue(int value)
    {
        return Mathf.InverseLerp(minValue, maxValue, value);
    }
    public override int CalcInterpolatedValue(float weight)
    {
        return (int) Mathf.Lerp(minValue, maxValue, weight);
    }
    public override RangeVariableReference<int> CreateRangeReference(int value)
    {
        return new RangeVariableReference<int>(this, value);
    }

    public override bool IsOutOfRange(int value)
    {
        return value < minValue || value > maxValue;
    }
}
