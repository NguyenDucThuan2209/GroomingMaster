using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HyrphusQ.Events
{
    [DisallowMultipleComponent]
    [AddComponentMenu("HyrphusQ/Events/LevelEventListeners")]
    public class LevelEventListeners : MonoBehaviour
    {
        [Serializable]
        public class EventTuple
        {
            public string eventType;
            public string eventCode;
            public UnityEvent unityEvent;

            public EventTuple(string eventType, string eventCode)
            {
                this.eventType = eventType;
                this.eventCode = eventCode;
                this.unityEvent = new UnityEvent();
            }
        }

        [HideInInspector]
        public List<EventTuple> listEvents = new List<EventTuple>();

        private void Awake()
        {
            foreach (var item in listEvents)
                LevelEventHandler.AddNewActionEvent((Enum) Enum.Parse(Type.GetType(item.eventType), item.eventCode), item.unityEvent.Invoke);
        }
        private void OnValidate()
        {
            if (gameObject.name != typeof(LevelEventListeners).Name)
                gameObject.name = typeof(LevelEventListeners).Name;
        }

        public bool ContainsEvent(string eventCode)
        {
            return listEvents.Exists(item => item.eventCode == eventCode);
        }
    }

}