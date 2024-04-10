Shader "Custom/Standard/GPUInstancing"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] [MainTexture] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex_ScaleOffset("Scale (XY) Offset (ZW)", Vector) = (1,1,0,0)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [Toggle(EMISSION_ENABLE)] _EmissionEnable("Enable Emission", Int) = 0
        [NoScaleOffset] _EmissionTex("Emission Texture", 2D) = "black" {}
        _EmissionTex_ScaleOffset("Scale (XY) Offset (ZW)", Vector) = (1,1,0,0)
        [HDR] _EmissionTint("Emission Tint", Color) = (0,0,0,0)
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 0.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma shader_feature EMISSION_ENABLE
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionTex;
        };

        sampler2D _MainTex;
        sampler2D _EmissionTex;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // Sorry but GPU can not do this cause only numerical value can be instanced
            //UNITY_DEFINE_INSTANCED_PROP(sampler2D, _MainTex)
            UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ScaleOffset)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(half, _Smoothness)
            UNITY_DEFINE_INSTANCED_PROP(half, _Metallic)
            // Sorry but GPU can not do this cause only numerical value can be instanced
            //UNITY_DEFINE_INSTANCED_PROP(sampler2D, _EmissionTex)
            UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionTex_ScaleOffset)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _EmissionTint)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Acess properties from buffer
            float4 mainTexScaleOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ScaleOffset);
            fixed4 tintCol = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            half smoothness = UNITY_ACCESS_INSTANCED_PROP(Props, _Smoothness);
            half metallic = UNITY_ACCESS_INSTANCED_PROP(Props, _Metallic);
            #ifdef EMISSION_ENABLE
            float4 emissionTexScaleOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionTex_ScaleOffset);
            fixed4 emissionTintCol = UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionTint);
            #endif

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex * mainTexScaleOffset.xy + mainTexScaleOffset.zw) * tintCol;
            o.Albedo = c.rgb;
            o.Metallic = metallic;
            o.Smoothness = smoothness;
            o.Alpha = c.a;
            #ifdef EMISSION_ENABLE
            o.Emission = tex2D(_EmissionTex, IN.uv_EmissionTex * emissionTexScaleOffset.xy + emissionTexScaleOffset.zw) * emissionTintCol;
            #endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}
