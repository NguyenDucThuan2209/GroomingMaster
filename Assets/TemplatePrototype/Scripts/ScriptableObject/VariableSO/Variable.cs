using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Events;

public class Variable<T> : BaseVariable, ISerializationCallbackReceiver
{
    public event Action<ValueDataChanged<T>> onValueChanged;

    [SerializeField]
    private T initialValue;
    public T InitialValue => initialValue;

    [NonSerialized]
    private T runtimeValue;
    public T Value
    {
        get => runtimeValue;
        set
        {
            var oldValue = runtimeValue;
            runtimeValue = value;
            if(!IsEquals(oldValue, runtimeValue))
                onValueChanged?.Invoke(new ValueDataChanged<T>(oldValue, value));
        }
    }

    public void OnBeforeSerialize()
    {
        // Do Nothing
    }
    public void OnAfterDeserialize()
    {
        // Assign serialized value for runtime value because runtime value is not serialized.
        runtimeValue = initialValue;
    }
    /// <summary>
    /// Define override function if generic type is your class without default equality operator
    /// </summary>
    /// <returns>Return true if two value is equal</returns>
    protected virtual bool IsEquals(T valueA, T valueB)
    {
        return EqualityComparer<T>.Default.Equals(valueA, valueB);
    }
    public static implicit operator T(Variable<T> variable)
    {
        return variable.Value;
    }
}