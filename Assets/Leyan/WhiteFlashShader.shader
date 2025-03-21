Shader"Custom/WhiteFlashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // ��ɫ��ͼ
        _FlashAmount ("Flash Amount", Range(0,1)) = 0 // ��������ǿ��
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        ZWrite
Off // �ر����д�룬����͸������
        Blend
SrcAlpha OneMinusSrcAlpha // ͸������ģʽ

Cull Off // ����˫����Ⱦ

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
float _FlashAmount; // ��������ǿ��

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
