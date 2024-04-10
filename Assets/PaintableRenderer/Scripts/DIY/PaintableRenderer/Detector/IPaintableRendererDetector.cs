using System;
using UnityEngine;

public interface IPaintableRendererDetector
{
    public event Action<IPaintableRenderer> onBeginPaintRenderer;
    public event Action<IPaintableRenderer, RaycastHit, bool> onPaintRenderer;
    public event Action<IPaintableRenderer> onStopPaintRenderer;
}