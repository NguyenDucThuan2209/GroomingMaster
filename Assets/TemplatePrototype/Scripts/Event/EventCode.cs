using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    [Serializable]
    public class EventCode
    {
        [SerializeField]
        private string m_EventType;
        [SerializeField]
        private string m_EventCode;

        public Enum eventCode => this;

        public static implicit operator Enum(EventCode eventCode)
        {
            return (Enum)Enum.Parse(Type.GetType(eventCode.m_EventType), eventCode.m_EventCode);
        }
    }
}