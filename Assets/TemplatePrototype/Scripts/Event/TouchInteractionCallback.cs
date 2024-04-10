using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HyrphusQ.Events
{
    public class TouchInteractionCallback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData> onBeginDrag;
        public event Action<PointerEventData> onEndDrag;
        public event Action<PointerEventData> onDrag;
        public event Action<PointerEventData> onPointerDown;
        public event Action<PointerEventData> onPointerUp;

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke(eventData);
        }
    }
}