Shader "Hidden/PaintBleedingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #define UV_OFFSET_LENGTH 8
    #define SQRT_2 0.70710678118
    #define ONE 1.0
    #define ALPHA_THRESHOLD 0.001

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        // Consider current pixel based on 8 neighbor nearest pixels

        const float2 uvOffset[UV_OFFSET_LENGTH] = {
            float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
            float2(SQRT_2, SQRT_2), float2(-SQRT_2, SQRT_2), float2(-SQRT_2, -SQRT_2), float2(SQRT_2, -SQRT_2)
        };
        float4 color = tex2D(_MainTex, i.uv);
        for (int k = 0; k < UV_OFFSET_LENGTH; k++)
        {
            float2 neighborUV = i.uv + uvOffset[k] * _MainTex_TexelSize.xy;
            float4 neighborColor = tex2D(_MainTex, neighborUV);
            color = max(color, neighborColor);
        }
        return color;
    }

    fixed4 fragAverage(v2f i) : SV_Target
    {
        // Calculate current pixel based on average 8 neighbor nearest pixels

        const float3x3 BOX_KERNEL = float3x3(
            1, 1, 1,
            1, 1, 1,
            1, 1, 1
        ) * 1/9;
        const float2 uvOffset[UV_OFFSET_LENGTH] = {
            float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
            float2(ONE, ONE), float2(-ONE, ONE), float2(-ONE, -ONE), float2(ONE, -ONE)
        };

        float3x3 kernel = BOX_KERNEL;
        float4 color = tex2D(_MainTex, i.uv) * kernel[1][1];
        for (int k = 0; k < UV_OFFSET_LENGTH; k++)
        {
            float2 neighborUV = i.uv + uvOffset[k] * _MainTex_TexelSize.xy;
            float2 index = abs(uvOffset[k] + float2(1, -1));
            float4 neighborColor = tex2D(_MainTex, neighborUV) * kernel[index.y][index.x];
            color += neighborColor;
        }
        return color;
    }
    fixed4 fragGaussian3x3(v2f i) : SV_Target
    {
        const float3x3 GAUSSIAN_KERNEL_3x3 = float3x3(
            1, 2, 1,
            2, 4, 2,
            1, 2, 1
        ) * 1/16;
        const float2 uvOffset[UV_OFFSET_LENGTH] = {
            float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
            float2(ONE, ONE), float2(-ONE, ONE), float2(-ONE, -ONE), float2(ONE, -ONE)
        };

        float3x3 kernel = GAUSSIAN_KERNEL_3x3;
        float4 color = tex2D(_MainTex, i.uv) * kernel[1][1];
        for (int k = 0; k < UV_OFFSET_LENGTH; k++)
        {
            float2 neighborUV = i.uv + uvOffset[k] * _MainTex_TexelSize.xy;
            float2 index = abs(uvOffset[k] + float2(1, -1));
            float4 neighborColor = tex2D(_MainTex, neighborUV) * kernel[index.y][index.x];
            color += neighborColor;
        }
        return color;
    }
    ENDCG

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "DefaultBleeding"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            Name "AverageBleeding"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragAverage
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            Name "GaussianBleeding3x3"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragGaussian3x3
            #pragma target 3.0
            ENDCG
        }
    }
}
