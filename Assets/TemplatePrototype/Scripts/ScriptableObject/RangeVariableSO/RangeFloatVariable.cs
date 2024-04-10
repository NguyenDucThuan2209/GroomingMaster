using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeFloatVariable", menuName = "HyrphusQ/RangeVariableSO/Float")]
public class RangeFloatVariable : RangeVariable<float>
{
    public override float CalcInverseLerpValue(float value)
    {
        return Mathf.InverseLerp(minValue, maxValue, value);
    }
    public override float CalcInterpolatedValue(float weight)
    {
        return Mathf.Lerp(minValue, maxValue, weight);
    }
    public override RangeVariableReference<float> CreateRangeReference(float value)
    {
        return new RangeVariableReference<float>(this, value);
    }

    public override bool IsOutOfRange(float value)
    {
        return value < minValue || value > maxValue;
    }
}
