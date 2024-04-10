using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TextureSize
{
    Texture128x128 = 128,
    Texture256x256 = 256,
    Texture512x512 = 512,
    Texture1024x1024 = 1024,
    Texture2048x2048 = 2048,
    Texture4096x4096 = 4096
}

public interface IPaintableRenderer
{
    public Texture originTexture { get; }
    public RenderTexture paintBrushTexture { get; }
    public RenderTexture paintNormalTexture { get; }
    public RenderTexture paintTexture { get; }
    public RenderTexture lastPaintTexture { get; }
    public Renderer renderer { get; }
    public FloatVariable unitBrushScaler { get; }

    public void Clear();
}
