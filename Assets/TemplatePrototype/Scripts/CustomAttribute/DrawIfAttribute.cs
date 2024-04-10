using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class DrawIfAttribute : PropertyAttribute
{
    public string comparisonMethodName { get; private set; }

    public DrawIfAttribute(string comparisonMethodName)
    {
        this.comparisonMethodName = comparisonMethodName;
    }
}
