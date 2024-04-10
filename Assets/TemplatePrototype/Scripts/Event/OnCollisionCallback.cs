#define GAME3D
//#define GAME2D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if GAME3D
[RequireComponent(typeof(Collider))]
#elif GAME2D
[RequireComponent(typeof(Collider2D))]
#endif

public class OnCollisionCallback : MonoBehaviour
{
    [TagSelector]
    public List<string> tagFilter = new List<string>() { "Untagged" };

    [SerializeField]
    private UnityEvent onCollisionEnterEvent;
    [SerializeField]
    private UnityEvent onCollisionStayEvent;
    [SerializeField]
    private UnityEvent onCollisionExitEvent;

    private void OnEnable()
    {
        onCollisionEnter += _ => onCollisionEnterEvent?.Invoke();
        onCollisionStay += _ => onCollisionStayEvent?.Invoke();
        onCollisionExit += _ => onCollisionExitEvent?.Invoke();
    }
    private void OnDisable()
    {
        onCollisionEnter -= _ => onCollisionEnterEvent?.Invoke();
        onCollisionStay -= _ => onCollisionStayEvent?.Invoke();
        onCollisionExit -= _ => onCollisionExitEvent?.Invoke();
    }

#if GAME3D
    public event Action<Collision> onCollisionEnter = delegate { };
    public event Action<Collision> onCollisionStay = delegate { };
    public event Action<Collision> onCollisionExit = delegate { };

    private void OnCollisionEnter(Collision collision)
    {
        if (tagFilter.Any(item => collision.gameObject.CompareTag(item)))
            onCollisionEnter?.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (tagFilter.Any(item => collision.gameObject.CompareTag(item)))
            onCollisionStay?.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (tagFilter.Any(item => collision.gameObject.CompareTag(item)))
            onCollisionExit?.Invoke(collision);
    }
    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = false;
    }
#elif GAME2D
    public event Action<Collision2D> onCollisionEnter = delegate { };
    public event Action<Collision2D> onCollisionStay = delegate { };
    public event Action<Collision2D> onCollisionExit = delegate { };

    private void OnCollisionEnter2D(Collision2D collision) {
        if (tagFilter.Any(item => collision.gameObject.CompareTag(item)))
            onCollisionEnter?.Invoke(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (tagFilter.Any(item => collision.gameObject.CompareTag(item)))
            onCollisionStay?.Invoke(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (tagFilter.Any(item => collision.gameObject.CompareTag(item)))
            onCollisionExit?.Invoke(collision);
    }
    private void OnValidate()
    {
        GetComponent<Collider2D>().isTrigger = false;
    }
#endif
}
