Shader "Hidden/PaintBrushUnlitShader"
{
    Properties
    {
        _SplatPaintTex("Splat Texture", 2D) = "black" { }
        _MaskTex("Mask Texture", 2D) = "white" {}
        _BrushColor ("Color", Color) = (1,1,1,1)
        _BrushPoint("Point", Vector) = (0,0,0,0)
        _BrushRadius("Radius", Float) = 0.0
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #define PI 3.14159
    #define ALPHA_THRESHOLD 0.01
    #define TAU 6.28318530718

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 worldPos : TEXCOORD1;
        float4 clipPos : TEXCOORD2;
        float4 viewPos : TEXCOORD3;
    };

    sampler2D _SplatPaintTex;
    sampler2D _MaskTex;
    fixed4 _BrushColor;
    float3 _BrushPoint;
    float _BrushRadius;

    v2f vert(appdata v)
    {
        v2f o;
        // Unwrap uv coordinate of model overlay the screen
        float4 uv = float4 (0, 0, 0, 1);
        uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * 2.0 - 1.0);
        o.vertex = uv;
        o.uv = v.uv.xy;
        // Convert object space(local space) to world space
        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        // Convert object space(local space) to clip space (range: -w -> w)
        o.clipPos = UnityObjectToClipPos(v.vertex);
        // Convert clip space to normalized device coordinate(NDC) (range: -1 -> 1)
        o.clipPos /= o.clipPos.w;
        // Convert clip space to screen space(viewport space) (range: 0 -> 1) 
        o.viewPos = ComputeScreenPos(o.clipPos);
        return o;
    }
    float random(float2 co)
    {
        return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
    }
    float2 rotateUV(float2 uv, float2 pivot, float rotation) {
        float cosa = cos(rotation);
        float sina = sin(rotation);
        uv -= pivot;
        return float2 (
            cosa * uv.x - sina * uv.y,
            cosa * uv.y + sina * uv.x
            ) + pivot;
    }
    float circle(float3 position, float3 center, float radius)
    {
        return 1 - smoothstep(0, 1, distance(position, center) / radius);
    }
    float2 calculateUV(float4 vertexClipPos, float4 paintClipPos)
    {
        float2 direction = vertexClipPos.xy - paintClipPos.xy;
        float2 uv = ((direction / _BrushRadius) + 1.0) / 2.0;
        #if UNITY_UV_STARTS_AT_TOP
        uv.y = 1.0 - uv.y;
        #endif
        return uv.xy;
    }
    float calculateBrushAlpha(v2f i)
    {
        float4 vertexClipPos = i.clipPos;
        // Handle problem with non-square screen resolution
        vertexClipPos.x *= (_ScreenParams.x / _ScreenParams.y);

        // Convert world space brush point to clip space brush point
        float4 paintClipPos = UnityWorldToClipPos(_BrushPoint);
        // Convert clip space to normalized device coordinate(NDC) match with clipPos
        paintClipPos /= paintClipPos.w;
        // Handle problem with non-square screen resolution
        paintClipPos.x *= (_ScreenParams.x / _ScreenParams.y);

        float cicleBrushAlpha = circle(float3(vertexClipPos.xy, 0), float3(paintClipPos.xy, 0), _BrushRadius);
        float stepCircleBrushAlpha = step(ALPHA_THRESHOLD, cicleBrushAlpha);
        float2 uv = calculateUV(vertexClipPos, paintClipPos);

        // Rotate randomly splat texture coordinate
        uv = rotateUV(uv, float2(0.5, 0.5), 2.0 * random(float2(_Time.yy)));
        float4 sampleSplatTexture = tex2D(_SplatPaintTex, uv);
        float paintSplatAlpha = sampleSplatTexture.a * stepCircleBrushAlpha;
        return paintSplatAlpha;
    }
    fixed4 fragPaintSplatTexture(v2f i) : SV_Target
    {
        float paintBrushAlpha = calculateBrushAlpha(i);
        fixed4 maskColor = tex2D(_MaskTex, i.uv);
        fixed4 color = fixed4(_BrushColor.rgb, paintBrushAlpha * step(ALPHA_THRESHOLD, maskColor.r));
        return color;
    }
    fixed4 fragPaintCircleSDF(v2f i) : SV_Target
    {
        float4 vertexClipPos = i.clipPos;
        // Handle problem with non-square screen resolution
        vertexClipPos.x *= (_ScreenParams.x / _ScreenParams.y);

        // Convert world space brush point to clip space brush point
        float4 paintClipPos = UnityWorldToClipPos(_BrushPoint);
        // Convert clip space to normalized device coordinate(NDC) match with clipPos
        paintClipPos /= paintClipPos.w;
        // Handle problem with non-square screen resolution
        paintClipPos.x *= (_ScreenParams.x / _ScreenParams.y);

        float paintBrushAlpha = circle(float3(vertexClipPos.xy, 0), float3(paintClipPos.xy, 0), _BrushRadius);
        fixed4 maskColor = tex2D(_MaskTex, i.uv);
        fixed4 color = fixed4(_BrushColor.rgb, paintBrushAlpha * step(ALPHA_THRESHOLD, maskColor.r));
        return color;
    }
    fixed4 fragPaintAllColor(v2f i) : SV_Target
    {
        fixed4 maskColor = tex2D(_MaskTex, i.uv);
        fixed4 color = fixed4(_BrushColor.rgb, step(ALPHA_THRESHOLD, maskColor.r));
        return color;
    }
    ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Cull Off
        ZWrite Off 
        ZTest Always

        //Pass 0 for Paint with SplatTexture rendering pass

        Pass
        {
            Name "PaintWithSplatTexture"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragPaintSplatTexture
            #pragma target 3.0            
            ENDCG
        }

        //Pass 1 for Paint with Circle SignedDistanceField rendering pass

        Pass
        {
            Name "PaintWithCircleSDF"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragPaintCircleSDF
            #pragma target 3.0
            ENDCG
        }

        //Pass 2 for unwrap UV with color rendering pass

        Pass
        {
            Name "PaintAllWithColor"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragPaintAllColor
            #pragma target 3.0
            ENDCG
        }
    }
}
