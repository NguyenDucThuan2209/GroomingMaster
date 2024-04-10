using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    public class HashtableBundle : IBundle
    {
        private Hashtable hashTableEventData = new Hashtable();

        public void Add<T>(string key, T data)
        {
            try
            {
                if (hashTableEventData.ContainsKey(key))
                {
                    hashTableEventData[key] = data;
                    return;
                }
                hashTableEventData.Add(key, data);
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
            }
        }

        public T Get<T>(string key)
        {
            try
            {
                return (T)hashTableEventData[key];
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
            }
            return default(T);
        }

        public bool TryGet<T>(string key, out T value)
        {
            value = default(T);
            if (!hashTableEventData.Contains(key))
                return false;
            value = (T)hashTableEventData[key];
            return true;
        }

        public bool Contains(string key) => hashTableEventData.ContainsKey(key);
        public void ClearAll() => hashTableEventData.Clear();
    }
}