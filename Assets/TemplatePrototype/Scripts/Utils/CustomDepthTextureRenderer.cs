using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("HyrphusQ/Rendering/CustomDepthTextureRenderer")]
public class CustomDepthTextureRenderer : MonoBehaviour
{
    private readonly static string ShadowCasterPassName = "FORWARD";
    private readonly static string BufferUpdateDepthTextureName = "UpdateDepthTexture";
    private readonly static string BufferDepthNormalsTextureName = "UpdateDepthNormalsTexture";
    private readonly static string DepthNormalsTextureShader = "Hidden/Internal-DepthNormalsTexture";
    private readonly static int DepthTexture_ID = Shader.PropertyToID("_CustomCameraDepthTexture");
    private readonly static int DepthNormalsTexture_ID = Shader.PropertyToID("_CustomCameraDepthNormalsTexture");

    [SerializeField]
    private DepthTextureMode depthTextureMode;
    [SerializeField]
    private List<Renderer> initialRenderers;

    private Camera mainCamera;
    private Material depthNormalsMaterial;
    private SortedDictionary<int, List<Renderer>> m_RenderersDictionary;
    private SortedDictionary<int, List<Renderer>> renderersDictionary
    {
        get
        {
            if (m_RenderersDictionary == null)
                m_RenderersDictionary = new SortedDictionary<int, List<Renderer>>();
            return m_RenderersDictionary;
        }
    }
    private Dictionary<DepthTextureMode, CommandBuffer> m_CommandBufferDictionary;
    private Dictionary<DepthTextureMode, CommandBuffer> commandBufferDictionary
    {
        get
        {
            if(m_CommandBufferDictionary == null)
                m_CommandBufferDictionary = new Dictionary<DepthTextureMode, CommandBuffer>();
            return m_CommandBufferDictionary;
        }
    }
    private void Awake()
    {
        mainCamera = Camera.main;
        depthNormalsMaterial = new Material(Shader.Find(DepthNormalsTextureShader));
        if (initialRenderers != null && initialRenderers.Count > 0)
        {
            for (int i = 0; i < initialRenderers.Count; i++)
            {
                AddRenderer(initialRenderers[i]);
            }
        }
    }
    private void OnEnable()
    {
        InitializeCommandBuffer();
    }
    private void OnDisable()
    {
        RemoveCommandBuffer();
    }

    private void InitializeCommandBuffer()
    {
        if (commandBufferDictionary.Count > 0)
            commandBufferDictionary.Clear();
        if ((depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.Depth)
        {
            var command = new CommandBuffer();
            command.name = BufferUpdateDepthTextureName;
            command.GetTemporaryRT(DepthTexture_ID, -1, -1, 16, FilterMode.Bilinear, RenderTextureFormat.Depth, RenderTextureReadWrite.Default);
            command.SetRenderTarget(DepthTexture_ID);
            command.ClearRenderTarget(true, true, Color.clear);
            foreach (var item in renderersDictionary)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    DrawDepthRenderer(item.Value[i], ref command);
                }
            }
            command.ReleaseTemporaryRT(DepthTexture_ID);

            // Add command buffer to camera events
            mainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, command);

            // Cached into to dictionary
            commandBufferDictionary.Add(DepthTextureMode.Depth, command);
        }
        if ((depthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.DepthNormals)
        {
            var command = new CommandBuffer();
            command.name = BufferDepthNormalsTextureName;
            command.GetTemporaryRT(DepthNormalsTexture_ID, -1, -1, 16, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            command.SetRenderTarget(DepthNormalsTexture_ID);
            command.ClearRenderTarget(true, true, new Color(0.5f, 0.5f, 1f, 1f));
            foreach (var item in renderersDictionary)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    DrawDepthNormalsRenderer(item.Value[i], ref command);
                }
            }
            command.ReleaseTemporaryRT(DepthNormalsTexture_ID);

            // Add command buffer to camera events
            mainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, command);

            // Cached into to dictionary
            commandBufferDictionary.Add(DepthTextureMode.DepthNormals, command);
        }
    }
    private void RemoveCommandBuffer()
    {
        if (mainCamera == null)
            return;
        foreach (var item in commandBufferDictionary)
        {
            var command = item.Value;
            mainCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, command);
            command.Clear();
            command.Dispose();
        }
        commandBufferDictionary.Clear();
    }
    private void RefreshCommandBuffer()
    {
        RemoveCommandBuffer();
        InitializeCommandBuffer();
    }
    // *NOTE: This method can't handle Mesh with multiple submesh. Modify if you need handle with multiple submesh
    private void DrawDepthRenderer(Renderer renderer, ref CommandBuffer command)
    {
        int shaderPass = renderer.sharedMaterial.FindPass(ShadowCasterPassName);
        if (shaderPass == -1)
            return;
        command.DrawRenderer(renderer, renderer.sharedMaterial, 0, shaderPass);
    }
    // *NOTE: This method can't handle Mesh with multiple submesh. Modify if you need handle with multiple submesh
    private void DrawDepthNormalsRenderer(Renderer renderer, ref CommandBuffer command)
    {
        command.DrawRenderer(renderer, depthNormalsMaterial, 0);
    }
    private void AddRenderer(Renderer renderer)
    {
        if (renderer.sharedMaterial.renderQueue >= 2500)
            return;
        var renderQueue = renderer.sharedMaterial.renderQueue;
        if (!renderersDictionary.ContainsKey(renderQueue))
        {
            renderersDictionary.Add(renderQueue, new List<Renderer>());
        }
        var renderers = renderersDictionary[renderQueue];
        renderers.Add(renderer);
        renderers.Sort((Renderer x, Renderer y) => x.transform.position.z.CompareTo(y.transform.position.z));
    }
    private void OnAddRenderer(Renderer renderer)
    {
        AddRenderer(renderer);
        RefreshCommandBuffer();
    }
}
