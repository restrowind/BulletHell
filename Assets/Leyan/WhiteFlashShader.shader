Shader"Custom/WhiteFlashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 角色贴图
        _FlashAmount ("Flash Amount", Range(0,1)) = 0 // 控制闪白强度
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        ZWrite
Off // 关闭深度写入，避免透明错误
        Blend
SrcAlpha OneMinusSrcAlpha // 透明叠加模式

Cull Off // 允许双面渲染

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
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
float _FlashAmount; // 控制闪白强度

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    col.rgb = lerp(col.rgb, float3(1, 1, 1), _FlashAmount);
    return col;
}
            ENDCG
        }
    }
}
