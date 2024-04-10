Shader "Custom/Standard/StencilMask"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [Toggle(EMISSION_ENABLE)] _EmissionEnable ("Enable Emission", Int) = 0
        _EmissionTex("Emission Texture", 2D) = "white" {}
        [HDR] _EmissionTint("Emission Tint", Color) = (0,0,0,0)
        [Toggle(STENCIL_ENABLE)] _StencilEnable("Enable Stencil", Int) = 0
        _StencilRef("Stencil ID", Range(0,255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _PassOp("Pass Operation", Float) = 0
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 0.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 300

        Stencil {
            Ref[_StencilRef]
            Comp[_StencilComp]
            Pass[_PassOp]
        }

        CGPROGRAM
        #pragma shader_feature _ EMISSION_ENABLE
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
        #pragma surface surf Standard alpha:blend fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionTex;
        };

        sampler2D _MainTex;
        sampler2D _EmissionTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Tint;
        fixed4 _EmissionTint;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Tint;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            #ifdef EMISSION_ENABLE
                o.Emission = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionTint;
            #endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}
