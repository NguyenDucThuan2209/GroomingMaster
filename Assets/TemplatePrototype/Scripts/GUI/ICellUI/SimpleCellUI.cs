using HyrphusQ.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HyrphusQ.GUI
{
    [AddComponentMenu("HyrphusQ/GUI/SimpleCell")]
    public class SimpleCellUI : MonoBehaviour, ICellUI
    {
        private IBundle m_Bundle;
        private IBundle bundle
        {
            get
            {
                if (m_Bundle == null)
                    m_Bundle = new HashtableBundle();
                return m_Bundle;
            }
        }
        private Dictionary<Tuple<string, string>, Component> m_CachedComponents;
        private Dictionary<Tuple<string, string>, Component> cachedComponents
        {
            get
            {
                if (m_CachedComponents == null)
                    m_CachedComponents = new Dictionary<Tuple<string, string>, Component>();
                return m_CachedComponents;
            }
        }

        public IBundle GetBundle() => bundle;
        public new T GetComponent<T>() where T : Component
        {
            return GetComponentInChildren<T>(true);
        }
        public T GetCachedComponent<T>() where T : Component
        {
            foreach (var item in cachedComponents)
            {
                if (item.Key.Item1 == typeof(T).Name)
                    return (T) item.Value;
            }
            var component = GetComponentInChildren<T>(true);
            cachedComponents.Add(Tuple.Create(typeof(T).Name, component?.name ?? null), component);
            return component;
        }
        public T GetCachedComponent<T>(string name) where T : Component
        {
            if (cachedComponents.TryGetValue(Tuple.Create(typeof(T).Name, name), out Component component))
            {
                component = GetComponentsInChildren<T>(true)?.FirstOrDefault(item => item.name == name);
                cachedComponents.Add(Tuple.Create(typeof(T).Name, name), component);
            }
            return (T) component;
        }
    }
}
