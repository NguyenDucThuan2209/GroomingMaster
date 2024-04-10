Shader "Hidden/BlitCopyWithMask"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _ColorChannelMask("Color Channel Mask", Vector) = (1,0,0,0)
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define ALPHA_THRESHOLD 0.01
            #include "UnityCG.cginc"

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
            sampler2D _MaskTex;
            float4 _ColorChannelMask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed calculateAlpha(fixed4 maskColor)
            {
                fixed4 maskAlpha = maskColor * _ColorChannelMask;
                fixed alpha = maskAlpha.r + maskAlpha.g + maskAlpha.b + maskAlpha.a;
                return step(ALPHA_THRESHOLD, alpha);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sourceColor = tex2D(_MainTex, i.uv);
                fixed4 maskColor = tex2D(_MaskTex, i.uv);
                fixed alpha = sourceColor.a * calculateAlpha(maskColor);
                fixed3 col = lerp(fixed3(0,0,0), sourceColor.rgb, alpha);
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}
