using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Helpers;

namespace HyrphusQ.SerializedDataStructure
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        protected List<TKey> keys = new List<TKey>();
        [SerializeField]
        protected List<TValue> values = new List<TValue>();

        protected Dictionary<TKey, TValue> serializedDictionary = new Dictionary<TKey, TValue>();

        #region IDictionary Implementation
        public TValue this[TKey key] { get => serializedDictionary[key]; set => serializedDictionary[key] = value; }

        public ICollection<TKey> Keys => serializedDictionary.Keys;

        public ICollection<TValue> Values => serializedDictionary.Values;

        public int Count => serializedDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            serializedDictionary.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            serializedDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            serializedDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if(serializedDictionary.TryGetValue(item.Key, out TValue value))
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return serializedDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            try
            {
                serializedDictionary.InvokeMethod<object>("CopyTo", array, arrayIndex);
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return serializedDictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return serializedDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return serializedDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return serializedDictionary.GetEnumerator();
        }
        #endregion

        #region ISerializationCallbackReceiver Implementation
        public void OnAfterDeserialize()
        {
            serializedDictionary.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                serializedDictionary.Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var item in serializedDictionary)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
        }
        #endregion

        #region UNITY_EDITOR
        protected Type GetKeyType()
        {
            return typeof(TKey);
        }
        protected Type GetValueType()
        {
            return typeof(TValue);
        }
        #endregion
    }
}