using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HyrphusQ.Optimizations
{
    /*Note: Unity doesn’t support GPU instancing for SkinnedMeshRenderers (So we don't have it)*/
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("HyrphusQ/Optimizations/MaterialPropertySetter")]
    public class MaterialPropertySetter : MonoBehaviour
    {
        [Serializable]
        public class MaterialProperty
        {
            //
            // Summary:
            //     Constructor.
            public MaterialProperty() { }
            public MaterialProperty(ShaderPropertyType type) => this.type = type;
            public MaterialProperty(int propertyID, float floatValue = 0f, Vector4 vectorValue = new Vector4(), Color colorValue = new Color(), Texture texture = null, ShaderPropertyType type = ShaderPropertyType.Float)
            {
                this.propertyID = propertyID;
                this.floatValue = floatValue;
                this.vectorValue = vectorValue;
                this.colorValue = colorValue;
                this.texture = texture;
                this.type = type;
            }

            public ShaderPropertyType type;
            public string propertyName = string.Empty;
            public int propertyIndex = -1;
            public int propertyID = -1;

            // Value
            public float floatValue;
            public Vector4 vectorValue;
            public Color colorValue;
            public Texture texture;
        }
        [HideInInspector]
        public List<MaterialProperty> materialProperties = new List<MaterialProperty>();

        public Material material => m_MeshRenderer.sharedMaterial;

        [SerializeField]
        private MeshRenderer m_MeshRenderer;
        public MeshRenderer meshRenderer
        {
            get
            {
                if (m_MeshRenderer == null)
                    m_MeshRenderer = GetComponent<MeshRenderer>();
                return m_MeshRenderer;
            }
        }

        private MaterialPropertyBlock materialPropertyBlock;

        #region Monobehaviour Method
        private void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            for (int i = 0; i < materialProperties.Count; i++)
            {
                var property = materialProperties[i];
                property.propertyID = Shader.PropertyToID(property.propertyName);
                switch (property.type)
                {
                    case ShaderPropertyType.Color:
                        materialPropertyBlock.SetColor(property.propertyID, property.colorValue);
                        break;
                    case ShaderPropertyType.Vector:
                        materialPropertyBlock.SetVector(property.propertyID, property.vectorValue);
                        break;
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        materialPropertyBlock.SetFloat(property.propertyID, property.floatValue);
                        break;
                    case ShaderPropertyType.Texture:
                        materialPropertyBlock.SetTexture(property.propertyID, property.texture);
                        break;
                    default:
                        break;
                }
            }
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
        private void OnValidate()
        {
            if (m_MeshRenderer == null)
                m_MeshRenderer = GetComponent<MeshRenderer>();
        }
        #endregion

        public void UpdateProperties(IEnumerable<MaterialProperty> properties)
        {
            meshRenderer.GetPropertyBlock(materialPropertyBlock);
            foreach (var property in properties)
                UpdateProperty(property);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
        public void UpdateProperties(params MaterialProperty[] properties)
        {
            UpdateProperties(properties);
        }
        public void UpdateProperty(MaterialProperty property)
        {
            // Update material property block & set to renderer
            meshRenderer.GetPropertyBlock(materialPropertyBlock);
            switch (property.type)
            {
                case ShaderPropertyType.Color:
                    materialPropertyBlock.SetColor(property.propertyID, property.colorValue);
                    break;
                case ShaderPropertyType.Vector:
                    materialPropertyBlock.SetVector(property.propertyID, property.vectorValue);
                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    materialPropertyBlock.SetFloat(property.propertyID, property.floatValue);
                    break;
                case ShaderPropertyType.Texture:
                    materialPropertyBlock.SetTexture(property.propertyID, property.texture);
                    break;
                default:
                    break;
            }
            meshRenderer.SetPropertyBlock(materialPropertyBlock);

            // Update value change in list material properties
            var materialProperty = materialProperties.FirstOrDefault(item => item.propertyID == property.propertyID);
            if (materialProperty != null)
                materialProperties.Remove(materialProperty);
            materialProperties.Add(property);
        }
    }
}