using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    public interface IBundle
    {
        public void Add<T>(string key, T data);
        public T Get<T>(string key);
        public bool TryGet<T>(string key, out T value);
        public bool Contains(string key);
        public void ClearAll();
    }
}
