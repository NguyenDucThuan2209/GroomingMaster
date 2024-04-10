Shader "Custom/RimLightOpaque"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HDR] _RimColor("Rim Color", Color) = (1, 0, 0, 1)
        _RimPower("Rim Power", Range(0.0, 8.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half4 _RimColor;
        float _RimPower;
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
            float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
            Out = lerp(Min, Max, randomno);
        }
        
        float Pingpong()
        {
        int remainder = fmod(floor(_Time.y), 2);
        return remainder==1?1-frac(_Time.y):frac(_Time.y);
        }
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            float3 nVwd = normalize(IN.viewDir);
            float3 NdotV = dot(nVwd, o.Normal);

            half rim = 1 - (saturate(NdotV) * Pingpong());
            o.Emission = _RimColor.rgb * pow(rim, _RimPower);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack Off
}
