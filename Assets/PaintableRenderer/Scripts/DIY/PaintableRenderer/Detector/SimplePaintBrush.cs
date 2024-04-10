using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePaintBrush : MonoBehaviour
{
    [SerializeField]
    private Brush brush;
    private RaycastHit previousHitInfo;
    private IPaintableRendererDetector detector;

    private void Awake()
    {
        detector = GetComponentInChildren<IPaintableRendererDetector>();
    }
    private void OnEnable()
    {
        detector.onPaintRenderer += OnPaintRenderer;
    }
    private void OnDisable()
    {
        detector.onPaintRenderer -= OnPaintRenderer;
    }

    private void OnPaintRenderer(IPaintableRenderer paintableRenderer, RaycastHit hitInfo, bool lastSegment)
    {
        if (Vector3.Distance(previousHitInfo.point, hitInfo.point) < (paintableRenderer.unitBrushScaler * brush.radius))
            return;
        previousHitInfo = hitInfo;
        PaintRendererManager.Instance.Paint(paintableRenderer, hitInfo.point, brush);
    }
}
