using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    public struct ValueDataChanged<T>
    {
        public T oldValue { get; private set; }
        public T newValue { get; private set; }

        public ValueDataChanged(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
        public void SetNewValue(T newValue)
        {
            this.newValue = newValue;
        }
        public void SetOldValue(T oldValue)
        {
            this.oldValue = oldValue;
        }
    }
}