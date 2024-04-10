using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Const;

public abstract class DictionaryObjectPoolSO<TKey, TValue> : BaseObjectPoolSO, IDictionaryPool<TKey, TValue> where TKey : Enum
{
    [SerializeField]
    protected int prewarmCount = 10;

    protected Dictionary<TKey, Stack<TValue>> m_PoolDictionary;

    public int DictionaryCount => m_PoolDictionary.Count;
    public int CountAll => m_PoolDictionary.Sum(keyValuePair => keyValuePair.Value.Count);
    protected Transform anchoredTransform;
    protected Action<TValue> onCreatePoolingItem;
    protected Action<TValue> onTakeFromPool;
    protected Action<TValue> onReturnToPool;
    protected Action<TValue> onDestroyPoolingItem;

    /// <summary>
    /// Initialized a pool, need to initialzed a pool before use it.
    /// </summary>
    /// <param name="anchoredTransform">An anchored transform (parent) for all item</param>
    /// <param name="onCreatePoolingItem">Create an item callback</param>
    /// <param name="onTakeFromPool">Take an item from pool callback</param>
    /// <param name="onReturnToPool">Return an item to pool callback</param>
    /// <param name="onDestroyPoolingItem">Destroy an item callback</param>
    public virtual void InitializedPool(Transform anchoredTransform, Action<TValue> onCreatePoolingItem = null, Action<TValue> onTakeFromPool = null, Action<TValue> onReturnToPool = null, Action<TValue> onDestroyPoolingItem = null)
    {
        m_PoolDictionary = new Dictionary<TKey, Stack<TValue>>();
        this.anchoredTransform = anchoredTransform;
        this.onCreatePoolingItem = onCreatePoolingItem;
        this.onTakeFromPool = onTakeFromPool;
        this.onReturnToPool = onReturnToPool;
        this.onDestroyPoolingItem = onDestroyPoolingItem;
        // TODO: Prewarm refill pool here
        RefillAllPool();
    }
    /// <summary>
    /// Destroy(release) all items(resources) in pool
    /// </summary>
    public void Clear()
    {
        foreach (var keyValuePair in m_PoolDictionary)
        {
            var pool = keyValuePair.Value;
            foreach (var item in pool)
            {
                onDestroyPoolingItem?.Invoke(item);
                DestroyMethod(keyValuePair.Key, item);
            }
            pool.Clear();
        }
        m_PoolDictionary.Clear();
        m_PoolDictionary = null;
    }
    /// <summary>
    /// Destroy(release) all items(resources) in pool of specific key
    /// </summary>
    /// <param name="key">Key of pool</param>
    public void Clear(TKey key)
    {
        if (m_PoolDictionary.TryGetValue(key, out Stack<TValue> pool))
        {
            foreach (var item in pool)
            {
                onDestroyPoolingItem?.Invoke(item);
                DestroyMethod(key, item);
            }
            pool.Clear();
            m_PoolDictionary.Remove(key);
        }
    }
    /// <summary>
    /// Take an item from the pool of specific key
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <returns>Return an item from pool</returns>
    public TValue Get(TKey key)
    {
        if (m_PoolDictionary.TryGetValue(key, out Stack<TValue> pool))
        {
            if (pool.Count <= Const.IntValue.Zero)
                RefillPool(key);
            var item = pool.Pop();
            onTakeFromPool?.Invoke(item);
            return item;
        }
        return default(TValue);
    }
    /// <summary>
    /// Return an item to pool of specific
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <param name="item">Item to return</param>
    public void Release(TKey key, TValue item)
    {
        if (!m_PoolDictionary.ContainsKey(key))
            m_PoolDictionary.Add(key, new Stack<TValue>());
        onReturnToPool?.Invoke(item);
        var stackPool = m_PoolDictionary[key];
        stackPool.Push(item);
    }
    /// <summary>
    /// Get total count of objects(items) in the pool of specific key
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <returns>Return total count of items</returns>
    public int Count(TKey key)
    {
        if (m_PoolDictionary.TryGetValue(key, out Stack<TValue> pool))
        {
            return pool.Count;
        }
        return 0;
    }

    /// <summary>
    /// Define a method to prewarm(fill) items into specificed pool when initialized a pool
    /// </summary>
    /// <param name="key">Key of pool</param>
    protected virtual void RefillPool(TKey key)
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var item = InstantiateMethod(key);
            onCreatePoolingItem?.Invoke(item);
            Release(key, item);
        }
    }
    /// <summary>
    /// Define a method to prewarm(fill) items into pool when initialized a pool
    /// </summary>
    protected abstract void RefillAllPool();
    /// <summary>
    /// Define a method to instantiate an object(item)
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <returns>Return a object(item)</returns>
    protected abstract TValue InstantiateMethod(TKey key);
    /// <summary>
    /// Define a method to destroy an object or release some resources
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <param name="item">Object(item) to destroy(release)</param>
    protected abstract void DestroyMethod(TKey key, TValue item);
}