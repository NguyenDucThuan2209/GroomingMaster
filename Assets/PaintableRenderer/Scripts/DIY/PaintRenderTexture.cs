using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Paint RenderTexture implement Double Buffering Render Texture manually. You need to sync(copy content of current paint texture to last paint texture) two texture manually using Graphics.Blit or CommandBuffer.Blit.
/// </summary>
[Serializable]
public class PaintRenderTexture : IDisposable
{
    [SerializeField, ReadOnly]
    private RenderTexture m_PaintBrushTexture;
    public RenderTexture paintBrushTexture
    {
        get => m_PaintBrushTexture;
    }
    [SerializeField, ReadOnly]
    private RenderTexture m_PaintNormalTexture;
    public RenderTexture paintNormalTexture
    {
        get => m_PaintNormalTexture;
    }
    [SerializeField, ReadOnly]
    private RenderTexture m_PaintTexture;
    public RenderTexture paintTexture
    {
        get => m_PaintTexture;
    }
    [SerializeField, ReadOnly]
    private RenderTexture m_LastPaintTexture;
    public RenderTexture lastPaintTexture
    {
        get => m_LastPaintTexture;
    }
    [SerializeField, ReadOnly]
    private RenderTexture m_TargetPaintTexture;
    public RenderTexture targetPaintTexture
    {
        get => m_TargetPaintTexture;
    }

    #region Constructors
    public PaintRenderTexture(int resolution = (int)TextureSize.Texture512x512, int depth = 0, RenderTextureFormat colorFormat = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
    {
        m_PaintBrushTexture = new RenderTexture(resolution, resolution, depth, colorFormat);
        m_PaintBrushTexture.wrapMode = wrapMode;
        m_PaintBrushTexture.filterMode = filterMode;

        m_PaintNormalTexture = new RenderTexture(resolution, resolution, depth, colorFormat);
        m_PaintNormalTexture.wrapMode = wrapMode;
        m_PaintNormalTexture.filterMode = filterMode;

        m_PaintTexture = new RenderTexture(resolution, resolution, depth, colorFormat);
        m_PaintTexture.wrapMode = wrapMode;
        m_PaintTexture.filterMode = filterMode;

        m_LastPaintTexture = new RenderTexture(resolution, resolution, depth, colorFormat);
        m_LastPaintTexture.wrapMode = wrapMode;
        m_LastPaintTexture.filterMode = filterMode;

        m_TargetPaintTexture = new RenderTexture(resolution, resolution, depth, colorFormat);
        m_TargetPaintTexture.wrapMode = wrapMode;
        m_TargetPaintTexture.filterMode = filterMode;

        Clear();
    }
    public PaintRenderTexture(RenderTexture paintBrushTexture, RenderTexture paintNormalTexture, RenderTexture paintTexture, RenderTexture lastPaintTexture, RenderTexture targetPaintTexture)
    {
        m_PaintBrushTexture = paintBrushTexture;
        m_PaintNormalTexture = paintNormalTexture;
        m_PaintTexture = paintTexture;
        m_LastPaintTexture = lastPaintTexture;
        m_TargetPaintTexture = targetPaintTexture;
    }
    #endregion

    public void Clear()
    {
        var commandBuffer = new CommandBuffer();
        commandBuffer.name = "Command Buffer - ClearPaintRenderTexture";
        commandBuffer.SetRenderTarget(m_PaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.SetRenderTarget(m_PaintNormalTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.SetRenderTarget(m_LastPaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.SetRenderTarget(m_PaintBrushTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.SetRenderTarget(m_TargetPaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
        commandBuffer.Dispose();
    }
    public void Dispose()
    {
        if(m_PaintBrushTexture)
            m_PaintBrushTexture.Release();
        if(m_PaintNormalTexture)
            m_PaintNormalTexture.Release();
        if(m_PaintTexture)
            m_PaintTexture.Release();
        if(m_LastPaintTexture)
            m_LastPaintTexture.Release();
        if(m_TargetPaintTexture)
            m_TargetPaintTexture.Release();
    }

    public static implicit operator Texture(PaintRenderTexture paintRenderTexture)
    {
        return paintRenderTexture.m_PaintTexture;
    }
}
