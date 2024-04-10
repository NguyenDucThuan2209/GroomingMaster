Shader "Custom/PaintableStandardShader"
{
    Properties
    {
        [MainColor] _TintColor ("Tint", Color) = (1,1,1,1)
        [MainTex] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _PaintTex ("Paint Texture", 2D) = "black"{}
        _PaintNormalTex("Paint Normal Texture", 2D) = "black" {}
        _NormalTex ("Normal Texture", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        [Toggle(EMISSION_ENABLE)] _EmissionEnable("Enable Emission", Int) = 0
        _EmissionTex("Emission Texture", 2D) = "black" {}
        [HDR] _EmissionTint("Emission Tint", Color) = (0,0,0,0)
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 0.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma shader_feature EMISSION_ENABLE
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        #define NORMAL_ALPHA_THRESHOLD 0.5
        #define ALPHA_THRESHOLD 0.01

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_PaintTex;
            float2 uv_PaintNormalTex;
            float2 uv_NormalTex;
            float2 uv_EmissionTex;
        };

        sampler2D _MainTex;
        sampler2D _PaintTex;
        sampler2D _PaintNormalTex;
        sampler2D _NormalTex;
        sampler2D _EmissionTex;
        half _Metallic;
        half _Glossiness;
        fixed4 _TintColor;
        fixed4 _EmissionTint;
        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 blendColor(fixed4 top, fixed4 bottom)
        {
            fixed3 color = top.rgb * top.a + bottom.rgb * (1.0 - top.a);
            fixed alpha = top.a + bottom.a * (1.0 - top.a);
            return fixed4(color, alpha);
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mainColor = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 paintColor = tex2D(_PaintTex, IN.uv_PaintTex);
            fixed paintNormalAlpha = tex2D(_PaintNormalTex, IN.uv_PaintNormalTex).a;
            fixed4 flatNormalColor = fixed4(0.5, 0.5, 1, 1);
            fixed4 normalColor = lerp(flatNormalColor, tex2D(_NormalTex, IN.uv_NormalTex), paintNormalAlpha <= NORMAL_ALPHA_THRESHOLD ? 0.0 : paintNormalAlpha);
            // Apply paint mask using blend color here
            fixed4 color = blendColor(paintColor, mainColor);
            o.Albedo = color.rgb * _TintColor.rgb;
            o.Normal = UnpackNormal(normalColor);
            #ifdef EMISSION_ENABLE
            o.Emission = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionTint;
            #endif
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
