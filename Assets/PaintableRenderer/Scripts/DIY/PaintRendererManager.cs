using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Const;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintRendererManager : Singleton<PaintRendererManager>
{
     enum PaintBlendingPass
    {
        BlendingLayer = 0,
        BlendingOverlayColor = 1
    }
    enum PaintBleedingPass
    {
        Default = 0,
        BoxKernel = 1,
        GaussianKernel3x3 = 2
    }
    enum PaintBrushPass
    {
        DefaultBrush = 0,
        CircleSDFBrush = 1,
        PaintAll = 2,
    }

    private static readonly int BrushPoint_ID = Shader.PropertyToID("_BrushPoint");
    private static readonly int BrushColor_ID = Shader.PropertyToID("_BrushColor");
    private static readonly int BrushRadius_ID = Shader.PropertyToID("_BrushRadius");
    private static readonly int SplatPaintTex_ID = Shader.PropertyToID("_SplatPaintTex");
    private static readonly int PaintBrushTex_ID = Shader.PropertyToID("_PaintBrushTex");
    private static readonly int PaintBrushTemporaryTex_ID = Shader.PropertyToID("_PaintBrushTemporaryTex");
    private static readonly int PaintNormalTemporaryTex_ID = Shader.PropertyToID("_PaintNormalTemporaryTex");
    private static readonly int TemporaryTex_ID = Shader.PropertyToID("_TemporaryTex");
    private static readonly int OverlayColor_ID = Shader.PropertyToID("_OverlayColor");
    private static readonly int MaskTex_ID = Shader.PropertyToID("_MaskTex");
    private static readonly int ColorChannelMask_ID = Shader.PropertyToID("_ColorChannelMask");

    private Shader paintBrushUnlitShader;
    private Shader paintBleedingShader;
    private Shader paintBlendingShader;
    private Shader blitCopyWithMaskShader;
    private Material paintBrushUnlitMaterial;
    private Material paintBleedingMaterial;
    private Material paintBlendingMaterial;
    private Material blitCopyWithMaskMaterial;

    private Camera mainCamera;
    private CommandBuffer commandBuffer;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        // *NOTE: Need to add shaders to ProjectSettings/Graphics at Always Included Shaders or create variables reference to it
        paintBrushUnlitShader = Shader.Find("Hidden/PaintBrushUnlitShader");
        paintBrushUnlitMaterial = new Material(paintBrushUnlitShader);

        paintBleedingShader = Shader.Find("Hidden/PaintBleedingShader");
        paintBleedingMaterial = new Material(paintBleedingShader);

        paintBlendingShader = Shader.Find("Hidden/PaintBlendingShader");
        paintBlendingMaterial = new Material(paintBlendingShader);

        blitCopyWithMaskShader = Shader.Find("Hidden/BlitCopyWithMask");
        blitCopyWithMaskMaterial = new Material(blitCopyWithMaskShader);

        commandBuffer = new CommandBuffer();
        commandBuffer.name = $"Command Buffer - {gameObject.name}";
    }

    public void Paint(IPaintableRenderer paintableRenderer, Vector3 point, Brush brush)
    {
        //Debug.Log("Paint Renderer: " + paintableRenderer.renderer);
        //Debug.Log("Paint Point: " + point);

        paintBrushUnlitMaterial.SetTexture(SplatPaintTex_ID, brush.splatTexture);
        paintBrushUnlitMaterial.SetVector(BrushPoint_ID, point);
        paintBrushUnlitMaterial.SetColor(BrushColor_ID, brush.color);
        paintBrushUnlitMaterial.SetFloat(BrushRadius_ID, brush.radius * paintableRenderer.unitBrushScaler);

        paintBlendingMaterial.SetTexture(PaintBrushTex_ID, paintableRenderer.paintBrushTexture);

        // Set up View & Projection Matrix
        commandBuffer.SetViewMatrix(mainCamera.worldToCameraMatrix);
        commandBuffer.SetProjectionMatrix(mainCamera.projectionMatrix);

        // Draw a paint brush into the PaintBrushTemporary Texture
        commandBuffer.GetTemporaryRT(PaintBrushTemporaryTex_ID, paintableRenderer.paintBrushTexture.descriptor);
        commandBuffer.SetRenderTarget(PaintBrushTemporaryTex_ID);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.DrawRenderer(paintableRenderer.renderer, paintBrushUnlitMaterial, 0, (int)PaintBrushPass.DefaultBrush);

        // Using PaintBrushTemporary texture to bleeding the paint brush(extends the range of paint brush by using 8 neighbor nearest pixels) and blit into PaintBrushTexture
        commandBuffer.Blit(PaintBrushTemporaryTex_ID, paintableRenderer.paintBrushTexture, paintBleedingMaterial, (int)PaintBleedingPass.Default);
        commandBuffer.ReleaseTemporaryRT(PaintBrushTemporaryTex_ID);

        // Using PaintBrushTexture combine(blending) with LastPaintTexture and blit into PaintTexture
        commandBuffer.SetRenderTarget(paintableRenderer.paintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.lastPaintTexture, paintableRenderer.paintTexture, paintBlendingMaterial, (int)PaintBlendingPass.BlendingLayer);

        // Sync LastPaintTexture with PaintTexture(Copy content of new PaintTexture into LastPaintTexture)
        commandBuffer.SetRenderTarget(paintableRenderer.lastPaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.paintTexture, paintableRenderer.lastPaintTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void Draw(IPaintableRenderer paintableRenderer, Vector3 point, Brush brush)
    {
        //Debug.Log("Paint Renderer: " + paintableRenderer.renderer);
        //Debug.Log("Paint Point: " + point);

        paintBrushUnlitMaterial.SetTexture(SplatPaintTex_ID, brush.splatTexture);
        paintBrushUnlitMaterial.SetVector(BrushPoint_ID, point);
        paintBrushUnlitMaterial.SetColor(BrushColor_ID, brush.color);
        paintBrushUnlitMaterial.SetFloat(BrushRadius_ID, brush.radius * paintableRenderer.unitBrushScaler);

        paintBlendingMaterial.SetTexture(PaintBrushTex_ID, paintableRenderer.paintBrushTexture);

        // Set up View & Projection Matrix
        commandBuffer.SetViewMatrix(mainCamera.worldToCameraMatrix);
        commandBuffer.SetProjectionMatrix(mainCamera.projectionMatrix);

        // Draw a paint brush into the PaintBrushTemporary Texture
        commandBuffer.GetTemporaryRT(PaintBrushTemporaryTex_ID, paintableRenderer.paintBrushTexture.descriptor);
        commandBuffer.SetRenderTarget(PaintBrushTemporaryTex_ID);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.DrawRenderer(paintableRenderer.renderer, paintBrushUnlitMaterial, 0, (int)PaintBrushPass.CircleSDFBrush);

        // Using PaintBrushTemporary texture to bleeding the paint brush(extends the range of paint brush by using 8 neighbor nearest pixels) and blit into PaintBrushTexture
        commandBuffer.Blit(PaintBrushTemporaryTex_ID, paintableRenderer.paintBrushTexture);
        commandBuffer.ReleaseTemporaryRT(PaintBrushTemporaryTex_ID);

        // Using PaintBrushTexture combine(blending) with LastPaintTexture and blit into PaintTexture
        commandBuffer.SetRenderTarget(paintableRenderer.paintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.lastPaintTexture, paintableRenderer.paintTexture, paintBlendingMaterial, (int)PaintBlendingPass.BlendingLayer);

        // Paint into PaintNormalTexture
        commandBuffer.GetTemporaryRT(PaintNormalTemporaryTex_ID, paintableRenderer.paintBrushTexture.descriptor);
        commandBuffer.SetRenderTarget(PaintNormalTemporaryTex_ID);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.paintNormalTexture, PaintNormalTemporaryTex_ID, paintBlendingMaterial, (int)PaintBlendingPass.BlendingLayer);
        commandBuffer.Blit(PaintNormalTemporaryTex_ID, paintableRenderer.paintNormalTexture);
        commandBuffer.ReleaseTemporaryRT(PaintNormalTemporaryTex_ID);

        // Sync LastPaintTexture with PaintTexture(Copy content of new PaintTexture into LastPaintTexture)
        commandBuffer.SetRenderTarget(paintableRenderer.lastPaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.paintTexture, paintableRenderer.lastPaintTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void Clean(IPaintableRenderer paintableRenderer, Vector3 point, Brush brush)
    {
        //Debug.Log("Paint Renderer: " + paintableRenderer.renderer);
        //Debug.Log("Paint Point: " + point);

        paintBrushUnlitMaterial.SetTexture(SplatPaintTex_ID, brush.splatTexture);
        paintBrushUnlitMaterial.SetVector(BrushPoint_ID, point);
        paintBrushUnlitMaterial.SetColor(BrushColor_ID, brush.color);
        paintBrushUnlitMaterial.SetFloat(BrushRadius_ID, brush.radius * paintableRenderer.unitBrushScaler);

        paintBlendingMaterial.SetTexture(PaintBrushTex_ID, paintableRenderer.paintBrushTexture);
        paintBlendingMaterial.SetColor(OverlayColor_ID, brush.color);

        // Set up View & Projection Matrix
        commandBuffer.SetViewMatrix(mainCamera.worldToCameraMatrix);
        commandBuffer.SetProjectionMatrix(mainCamera.projectionMatrix);

        // Draw a paint brush into the PaintBrushTexture Texture
        commandBuffer.SetRenderTarget(paintableRenderer.paintBrushTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.DrawRenderer(paintableRenderer.renderer, paintBrushUnlitMaterial, 0, (int)PaintBrushPass.CircleSDFBrush);

        // Using PaintBrushTexture combine(blending) with LastPaintTexture and blit into PaintTexture
        commandBuffer.SetRenderTarget(paintableRenderer.paintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.lastPaintTexture, paintableRenderer.paintTexture, paintBlendingMaterial, (int)PaintBlendingPass.BlendingOverlayColor);

        // Sync LastPaintTexture with PaintTexture(Copy content of new PaintTexture into LastPaintTexture)
        commandBuffer.SetRenderTarget(paintableRenderer.lastPaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.paintTexture, paintableRenderer.lastPaintTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void InitializeTargetCleanTexture(IPaintableRenderer paintableRenderer, Color color, RenderTexture targetRenderTexture)
    {
        paintBrushUnlitMaterial.SetColor(BrushColor_ID, color);

        paintBlendingMaterial.SetTexture(PaintBrushTex_ID, targetRenderTexture);
        paintBlendingMaterial.SetColor(OverlayColor_ID, color);

        commandBuffer.SetRenderTarget(targetRenderTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.DrawRenderer(paintableRenderer.renderer, paintBrushUnlitMaterial, 0, (int)PaintBrushPass.PaintAll);

        commandBuffer.GetTemporaryRT(TemporaryTex_ID, targetRenderTexture.descriptor);
        commandBuffer.Blit(paintableRenderer.lastPaintTexture, TemporaryTex_ID, paintBlendingMaterial, (int)PaintBlendingPass.BlendingOverlayColor);
        commandBuffer.Blit(TemporaryTex_ID, targetRenderTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void BlendPaintTexture(IPaintableRenderer paintableRenderer)
    {
        commandBuffer.Blit(paintableRenderer.lastPaintTexture, paintableRenderer.paintTexture, paintBleedingMaterial, (int) PaintBleedingPass.GaussianKernel3x3);

        // Sync LastPaintTexture with PaintTexture(Copy content of new PaintTexture into LastPaintTexture)
        commandBuffer.SetRenderTarget(paintableRenderer.lastPaintTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(paintableRenderer.paintTexture, paintableRenderer.lastPaintTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void BlitCopy(Texture source, Texture dest)
    {
        commandBuffer.SetRenderTarget(dest);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(source, dest);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void BlitCopy(Texture source, Texture dest, Texture mask, Vector4 colorChannelMask)
    {
        blitCopyWithMaskMaterial.SetTexture(MaskTex_ID, mask);
        blitCopyWithMaskMaterial.SetVector(ColorChannelMask_ID, colorChannelMask);

        commandBuffer.SetRenderTarget(dest);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(source, dest, blitCopyWithMaskMaterial);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
    public void BlitCopy(Texture source, Texture dest, Material customMaterial)
    {
        commandBuffer.SetRenderTarget(dest);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.Blit(source, dest, customMaterial);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }
}
