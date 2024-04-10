using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    [AddComponentMenu("HyrphusQ/Events/LevelEventNotifier")]
    public class LevelEventNotifier : MonoBehaviour
    {
        public event Action<LevelEventNotifier> onRaiseEvent;

        [SerializeField]
        private EventCode m_EventCode;
        [SerializeField]
        private List<UnityEngine.Object> parameterObjects = new List<UnityEngine.Object>();

        private void OnValidate()
        {
            if (gameObject.name != nameof(LevelEventNotifier))
                gameObject.name = nameof(LevelEventNotifier);
        }

        public void RaiseEvent()
        {
            var eventParams = new object[parameterObjects.Count];
            for (int i = 0; i < parameterObjects.Count; i++)
            {
                eventParams[i] = parameterObjects[i];
            }
            onRaiseEvent?.Invoke(this);
            LevelEventHandler.Invoke(m_EventCode.eventCode, null, eventParams);
        }
    }
}