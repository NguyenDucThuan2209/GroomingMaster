Shader "Hidden/PaintBlendingShader"
{
    Properties
    {
        _MainTex("Last Paint Texture", 2D) = "white" {}
        _PaintBrushTex("Paint Brush Texture", 2D) = "black" {}
        _OverlayColor("Paint Overlay Color", Color) = (0,0,0,0)
    }

    CGINCLUDE
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
    float4 _MainTex_ST;
    sampler2D _PaintBrushTex;
    float4 _PaintBrushTex_ST;
    fixed4 _OverlayColor;

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }
    fixed4 blendColor(fixed4 top, fixed4 bottom)
    {
        fixed3 color = top.rgb * top.a + bottom.rgb * (1.0 - top.a);
        fixed alpha = top.a + bottom.a * (1.0 - top.a);
        return fixed4(color.rgb, alpha);
    }
    fixed4 frag(v2f i) : SV_Target
    {
        fixed4 lastPaintColor = tex2D(_MainTex, i.uv);
        fixed4 paintBrushColor = tex2D(_PaintBrushTex, i.uv);
        return blendColor(paintBrushColor, lastPaintColor);
    }
    fixed4 fragOverlay(v2f i) : SV_Target
    {
        fixed4 lastPaintColor = tex2D(_MainTex, i.uv);
        fixed4 paintBrushColor = tex2D(_PaintBrushTex, i.uv);
        return lerp(lastPaintColor, _OverlayColor, paintBrushColor.a);
    }
    ENDCG

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Cull Off
        ZWrite Off
        ZTest Always

        //Pass 0 for Blending two layer rendering pass
        Pass
        {
            Name "BlendingLayer"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }

        //Pass 1 for Blending with overlay color rendering pass
        Pass
        {
            Name "BlendingOverlayColor"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOverlay
            #pragma target 3.0
            ENDCG
        }
    }
}
