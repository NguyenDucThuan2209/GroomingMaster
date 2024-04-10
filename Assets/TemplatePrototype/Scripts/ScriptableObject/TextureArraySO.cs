using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureArray", menuName ="HyrphusQ/TextureArray")]
public class TextureArraySO : ScriptableObject
{
    public bool globalSetting = true;
    [Header("Texture Setting")]
    public bool generateMipMaps = true;
    [Range(0, 16)]
    public int anisoLevel = 1;
    public FilterMode filterMode = FilterMode.Bilinear;
    public TextureWrapMode textureWrapMode = TextureWrapMode.Clamp;
    public TextureFormat textureFormat = TextureFormat.RGBA32;
    
    [SerializeField, ReadOnly]
    private string shaderName;
    [SerializeField]
    private string propertyName;
    [SerializeField]
    private List<Texture2D> textureArray;

    private int m_propertyID;
    private Texture2DArray m_Texture2DArray;


    #region Properties
    public int propertyID => m_propertyID;
    public Texture2DArray texture2DArray
    {
        get
        {
            if (m_Texture2DArray == null)
            {
                if (textureArray == null || textureArray.Count <= 0)
                    Debug.LogError("TextureArray need at least 1 texture.");
                m_propertyID = Shader.PropertyToID(propertyName);
                // Instantiate texture 2d array
                int maxWidth = textureArray.Max(texture => texture.width);
                int maxHeight = textureArray.Max(texture => texture.height);
                m_Texture2DArray = new Texture2DArray(maxWidth, maxHeight, textureArray.Count, textureFormat, generateMipMaps);
                m_Texture2DArray.wrapMode = textureWrapMode;
                m_Texture2DArray.filterMode = filterMode;
                m_Texture2DArray.anisoLevel = anisoLevel;
                for (int i = 0; i < textureArray.Count; i++)
                    m_Texture2DArray.SetPixels(textureArray[i].GetPixels(), i);
                m_Texture2DArray.Apply();
                if (globalSetting)
                    Shader.SetGlobalTexture(m_propertyID, texture2DArray);
            }
            return m_Texture2DArray;
        }
    }
    #endregion

    public static implicit operator Texture2DArray(TextureArraySO textureArraySO)
    {
        return textureArraySO.texture2DArray;
    }
}
