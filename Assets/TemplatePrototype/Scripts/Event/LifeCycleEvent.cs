using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HyrphusQ.Events
{
    [DisallowMultipleComponent]
    [AddComponentMenu("HyrphusQ/Events/LifeCycleEvent")]
    public class LifeCycleEvent : MonoBehaviour
    {
        public event Action<GameObject> onAwake;
        public event Action<GameObject> onEnable;
        public event Action<GameObject> onStart;
        public event Action<GameObject> onDisable;
        public event Action<GameObject> onDestroy;

        [SerializeField]
        private UnityEvent onAwakeEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent onEnableEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent onStartEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent onDisableEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent onDestroyEvent = new UnityEvent();

        private void Awake()
        {
            onAwake += _ => onAwakeEvent?.Invoke();
            onEnable += _ => onEnableEvent?.Invoke();
            onStart += _ => onStartEvent?.Invoke();
            onDisable += _ => onDisableEvent?.Invoke();
            onDestroy += _ => onDestroyEvent?.Invoke();

            onAwake?.Invoke(gameObject);
        }
        private void OnEnable() => onEnable?.Invoke(gameObject);
        private void Start() => onStart?.Invoke(gameObject);
        private void OnDisable() => onDisable?.Invoke(gameObject);
        private void OnDestroy() => onDestroy?.Invoke(gameObject);
    }
}