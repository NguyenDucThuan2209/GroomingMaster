using HyrphusQ.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeVariableReference<T>
{
    public event Action<ValueDataChanged<T>> onValueChanged;

    private RangeVariable<T> m_RangeSO;
    private T m_Value;
    private float m_InverseLerpValue;
    public RangeVariableReference(RangeVariable<T> rangeSO, T value)
    {
        m_RangeSO = rangeSO;
        m_Value = value;
        m_InverseLerpValue = rangeSO.CalcInverseLerpValue(value);
        onValueChanged = delegate { };
    }

    #region Properties
    public T minValue => m_RangeSO.minValue;
    public T maxValue => m_RangeSO.maxValue;
    public T value
    {
        get => m_Value;
        set
        {
            var oldValue = m_Value;
            m_Value = value;
            m_InverseLerpValue = m_RangeSO.CalcInverseLerpValue(value);
            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
                onValueChanged?.Invoke(new ValueDataChanged<T>(oldValue, m_Value));
        }
    }
    public float inverseLerpValue => m_InverseLerpValue;
    #endregion

    public float CalcInverseLerpValue(T value) => m_RangeSO.CalcInverseLerpValue(value);
}
public abstract class RangeVariable<T> : BaseRangeVariable, ISerializationCallbackReceiver
{
    [SerializeField]
    private T m_MinValue = default(T);
    [SerializeField]
    private T m_MaxValue = default(T);

    [NonSerialized]
    private T m_RuntimeMinValue;
    [NonSerialized]
    private T m_RuntimeMaxValue;

    public T minValue
    {
        get => m_RuntimeMinValue;
        set => m_RuntimeMinValue = value;
    }
    public T maxValue
    {
        get => m_RuntimeMaxValue;
        set => m_RuntimeMaxValue = value;
    }

    /// <summary>
    /// Calculate inverse interpolated value range (0 - 1)
    /// </summary>
    /// <param name="value">Current value in range (min-max)</param>
    /// <returns>Return inverse interpolated value range (0 - 1)</returns>
    public abstract float CalcInverseLerpValue(T value);
    /// <summary>
    /// Calculate interpolated value range (min - max) base on weight (0 - 1)
    /// </summary>
    /// <param name="weight">Weight factor range (0 - 1)</param>
    /// <returns>Return interpolated value range (min - max)</returns>
    public abstract T CalcInterpolatedValue(float weight);
    /// <summary>
    /// Create range reference
    /// </summary>
    /// <param name="value">Inital value</param>
    /// <returns>Return range reference</returns>
    public abstract RangeVariableReference<T> CreateRangeReference(T value);
    /// <summary>
    /// Check whether value is out of range or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Return true if value out of range otherwise false</returns>
    public abstract bool IsOutOfRange(T value);

    public void OnBeforeSerialize()
    {
        // Do nothing
    }
    public void OnAfterDeserialize()
    {
        // Assign serialized value for runtime value because runtime value is not serialized.
        m_RuntimeMinValue = m_MinValue;
        m_RuntimeMaxValue = m_MaxValue;
    }
}
