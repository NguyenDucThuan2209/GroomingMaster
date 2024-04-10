using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolSO<T> : BaseObjectPoolSO, IPool<T> where T : UnityEngine.Object
{
    [SerializeField]
    protected int prewarmCount = 10;
    [SerializeField]
    protected T prefabObject;
    protected Stack<T> m_PoolingStack;

    public int Count => m_PoolingStack.Count;
    protected Transform anchoredTransform;
    protected Action<T> onCreatePoolingItem;
    protected Action<T> onTakeFromPool;
    protected Action<T> onReturnToPool;
    protected Action<T> onDestroyPoolingItem;

    /// <summary>
    /// Initialized a pool, need to initialzed a pool before use it.
    /// </summary>
    /// <param name="anchoredTransform">An anchored transform (parent) for all item</param>
    /// <param name="onCreatePoolingItem">Create an item callback</param>
    /// <param name="onTakeFromPool">Take an item from pool callback</param>
    /// <param name="onReturnToPool">Return an item to pool callback</param>
    /// <param name="onDestroyPoolingItem">Destroy an item callback</param>
    public virtual void InitializedPool(Transform anchoredTransform, Action<T> onCreatePoolingItem = null, Action<T> onTakeFromPool = null, Action<T> onReturnToPool = null, Action<T> onDestroyPoolingItem = null)
    {
        m_PoolingStack = new Stack<T>();
        this.anchoredTransform = anchoredTransform;
        this.onCreatePoolingItem = onCreatePoolingItem;
        this.onTakeFromPool = onTakeFromPool;
        this.onReturnToPool = onReturnToPool;
        this.onDestroyPoolingItem = onDestroyPoolingItem;
        RefillPool();
    }
    /// <summary>
    /// Destroy(release) all items(resources) in pool
    /// </summary>
    public virtual void Clear()
    {
        foreach (var item in m_PoolingStack)
        {
            onDestroyPoolingItem?.Invoke(item);
            DestroyMethod(item);
        }
        m_PoolingStack?.Clear();
    }
    /// <summary>
    /// Take an item from pool
    /// </summary>
    /// <returns>Return an item from pool</returns>
    public virtual T Get()
    {
        if (m_PoolingStack.Count <= 0)
            RefillPool();
        var item = m_PoolingStack.Pop();
        onTakeFromPool?.Invoke(item);
        return item;
    }
    /// <summary>
    /// Return an item to pool
    /// </summary>
    /// <param name="item">Item to return</param>
    public virtual void Release(T item)
    {
        m_PoolingStack.Push(item);
        onReturnToPool?.Invoke(item);
    }

    /// <summary>
    /// Define a method to prewarm(fill) items into pool when initialized a pool
    /// </summary>
    protected virtual void RefillPool()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var item = InstantiateMethod();
            onCreatePoolingItem?.Invoke(item);
            Release(item);
        }
    }
    /// <summary>
    /// Define a method to instantiate an object(item)
    /// </summary>
    /// <returns>Return a object(item)</returns>
    protected abstract T InstantiateMethod();
    /// <summary>
    /// Define a method to destroy an object or release some resource
    /// </summary>
    /// <param name="item">Object(item) to destroy(release)</param>
    protected abstract void DestroyMethod(T item);
}