using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayCastDetector : MonoBehaviour, IPaintableRendererDetector
{
    public event Action<IPaintableRenderer> onBeginPaintRenderer;
    public event Action<IPaintableRenderer, RaycastHit, bool> onPaintRenderer;
    public event Action<IPaintableRenderer> onStopPaintRenderer;

    private bool isHoldDown;
    private IPaintableRenderer paintableRenderer;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    public void Update()
    {
        if (Input.GetMouseButton(0) && !IsPressedGUIElement())
        {
            RayCastFindPaintableRenderer(OnBeginPaintRenderer);
        }
        if (Input.GetMouseButtonUp(0) && paintableRenderer != null)
        {
            OnStopPaintRenderer(paintableRenderer);
        }
        if (isHoldDown)
        {
            RayCastFindPaintableRenderer(OnPaintRenderer);
        }
    }
    private void OnBeginPaintRenderer(IPaintableRenderer paintableRenderer, RaycastHit hitInfo)
    {
        isHoldDown = true;
        this.paintableRenderer = paintableRenderer;
        onBeginPaintRenderer?.Invoke(paintableRenderer);
    }
    private void OnPaintRenderer(IPaintableRenderer paintableRenderer, RaycastHit hitInfo)
    {
        onPaintRenderer?.Invoke(paintableRenderer, hitInfo, true);
    }
    private void OnStopPaintRenderer(IPaintableRenderer paintableRenderer)
    {
        onStopPaintRenderer?.Invoke(paintableRenderer);
        isHoldDown = false;
        this.paintableRenderer = null;
    }
    private void RayCastFindPaintableRenderer(Action<IPaintableRenderer, RaycastHit> callback)
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider.TryGetComponent(out paintableRenderer))
        {
            callback.Invoke(paintableRenderer, hitInfo);
        }
    }
    private bool IsPressedGUIElement()
    {
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        foreach (var touch in Input.touches)
        {
            var touchID = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(touchID))
                return true;
        }
        return false;
#endif
    }
}
