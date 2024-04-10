using HyrphusQ.Const;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CleanableMeshRenderer : MonoBehaviour, IPaintableRenderer
{
    [SerializeField]
    private TextureSize m_PaintTextureResolution = TextureSize.Texture1024x1024;
    public int paintTextureResolution => (int)m_PaintTextureResolution;
    [SerializeField]
    private Texture dirtyTexture;
    [SerializeField]
    private MeshRenderer m_MeshRenderer;
    [SerializeField]
    private FloatVariable m_UnitBrushScaler;

    public Texture originTexture => m_OriginTexture;
    public RenderTexture paintBrushTexture => m_PaintRenderTexture.paintBrushTexture;
    public RenderTexture paintNormalTexture => m_PaintRenderTexture.paintNormalTexture;
    public RenderTexture paintTexture => m_PaintRenderTexture.paintTexture;
    public RenderTexture lastPaintTexture => m_PaintRenderTexture.lastPaintTexture;
    public RenderTexture targetPaintTexture => m_PaintRenderTexture.targetPaintTexture;
    public new Renderer renderer => m_MeshRenderer;
    public FloatVariable unitBrushScaler => m_UnitBrushScaler;

    private Texture m_OriginTexture;
    private Material m_PaintableMaterial;
    [SerializeField]
    private PaintRenderTexture m_PaintRenderTexture;

    #region Monobehaviour Methods
    private void Start()
    {
        m_PaintableMaterial = m_MeshRenderer.material;
        m_OriginTexture = m_PaintableMaterial.GetTexture(Const.ShaderProperty.MainTexture_ID);

        m_PaintRenderTexture = new PaintRenderTexture(paintTextureResolution, 0, RenderTextureFormat.ARGB32, FilterMode.Bilinear, TextureWrapMode.Clamp);
        PaintRendererManager.Instance.BlitCopy(dirtyTexture, m_PaintRenderTexture.paintTexture);
        PaintRendererManager.Instance.BlitCopy(dirtyTexture, m_PaintRenderTexture.lastPaintTexture);
        PaintRendererManager.Instance.InitializeTargetCleanTexture(this, Color.clear, targetPaintTexture);

        m_PaintableMaterial.SetTexture(Const.ShaderProperty.PaintTexture_ID, m_PaintRenderTexture);

        gameObject.tag = Const.UnityTag.PaintableRendererTag;
        gameObject.layer = Const.UnityLayerMask.PaintableRendererLayer;
    }
    private void OnDestroy()
    {
        m_PaintRenderTexture.Dispose();
    }
    private void OnValidate()
    {
        if (m_MeshRenderer == null)
            m_MeshRenderer = GetComponent<MeshRenderer>();
    }
    #endregion

    [ContextMenu("Clear PaintTexture")]
    public void Clear()
    {
        m_PaintRenderTexture.Clear();
    }
}
