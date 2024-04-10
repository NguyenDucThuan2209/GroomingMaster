#define GAME3D
//#define GAME2D
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if GAME3D
[RequireComponent(typeof(Collider))]
#elif GAME2D
[RequireComponent(typeof(Collider2D))]
#endif
public class OnTriggerCallback : MonoBehaviour
{
    [TagSelector]
    public List<string> tagFilter = new List<string>() { "Untagged" };

    [SerializeField]
    private UnityEvent onTriggerEnterEvent;
    [SerializeField]
    private UnityEvent onTriggerStayEvent;
    [SerializeField]
    private UnityEvent onTriggerExitEvent;

    private void OnEnable()
    {
        onTriggerEnter += _ => onTriggerEnterEvent?.Invoke();
        onTriggerStay += _ => onTriggerStayEvent?.Invoke();
        onTriggerExit += _ => onTriggerExitEvent?.Invoke();
    }
    private void OnDisable()
    {
        onTriggerEnter -= _ => onTriggerEnterEvent?.Invoke();
        onTriggerStay -= _ => onTriggerStayEvent?.Invoke();
        onTriggerExit -= _ => onTriggerExitEvent?.Invoke();
    }

#if GAME3D
    public event Action<Collider> onTriggerEnter = delegate { };
    public event Action<Collider> onTriggerStay = delegate { };
    public event Action<Collider> onTriggerExit = delegate { };

    private void OnTriggerEnter(Collider other) {
        if (tagFilter.Any(item => other.CompareTag(item)))
            onTriggerEnter?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (tagFilter.Any(item => other.CompareTag(item)))
            onTriggerStay?.Invoke(other);
    }
    private void OnTriggerExit(Collider other) {
        if (tagFilter.Any(item => other.CompareTag(item)))
            onTriggerExit?.Invoke(other);
    }
    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
#elif GAME2D
    public event Action<Collider2D> onTriggerEnter = delegate { };
    public event Action<Collider2D> onTriggerStay = delegate { };
    public event Action<Collider2D> onTriggerExit = delegate { };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tagFilter.Any(item => other.CompareTag(item)))
            onTriggerEnter?.Invoke(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (tagFilter.Any(item => other.CompareTag(item)))
            onTriggerStay?.Invoke(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (tagFilter.Any(item => other.CompareTag(item)))
            onTriggerExit?.Invoke(other);
    }
        private void OnValidate()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
#endif
}
