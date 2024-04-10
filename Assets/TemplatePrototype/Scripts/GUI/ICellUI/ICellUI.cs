using HyrphusQ.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.GUI
{
    public interface ICellUI
    {
        public IBundle GetBundle();
        public T GetComponent<T>() where T : Component;
        public T GetCachedComponent<T>() where T : Component;
        public T GetCachedComponent<T>(string name) where T : Component;
    }
}
