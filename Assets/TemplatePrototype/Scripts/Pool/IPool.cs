using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPool<T>
{
    int Count
    {
        get;
    }

    void Clear();
    T Get();
    void Release(T item);
}
