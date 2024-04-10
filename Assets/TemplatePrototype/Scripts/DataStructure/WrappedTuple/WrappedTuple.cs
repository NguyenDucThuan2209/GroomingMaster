using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.SerializedDataStructure
{
    [Serializable]
    public class WrappedTuple<THead, TTail>
    {
        [SerializeField]
        private THead m_Head;
        [SerializeField]
        private TTail m_Tail;

        public THead head
        {
            get => m_Head;
            set => m_Head = value;
        }
        public TTail tail
        {
            get => m_Tail;
            set => m_Tail = value;
        }
    }
}