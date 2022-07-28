// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Lut_2_lut_Extra" {
Properties
{
_MainTex("Base (RGB)", 2D) = "" {}
}

CGINCLUDE
#include "UnityCG.cginc"

struct v2f {
float4 pos : SV_POSITION;
float2 uv  : TEXCOORD0;
};

sampler2D _MainTex;
sampler3D _LutTex;
sampler3D _LutTex2;


float _Fade;
float _Fade2;
float _Pos;
float _Smooth;


v2f vert(appdata_img v)
{
v2f o;
o.pos = UnityObjectToClipPos(v.vertex);
o.uv = v.texcoord.xy;
return o;
}

float4 frag(v2f i) : SV_Target
{
float4 c = tex2D(_MainTex, i.uv);
float3 l1 = lerp(c.rgb, tex3D(_LutTex, c.rgb).rgb, _Fade);
float3 l2 = lerp(c.rgb, tex3D(_LutTex2, c.rgb).rgb, _Fade2);
float s = smoothstep(_Pos, _Pos + _Smooth, i.uv.y);
c.rgb = lerp(l1.rgb,l2.rgb, s);
return c;
}

float4 fragLinear(v2f i) : SV_Target
{
float4 c = tex2D(_MainTex, i.uv);
c.rgb = sqrt(c.rgb);
float3 l1 = lerp(c.rgb, tex3D(_LutTex, c.rgb).rgb, _Fade);
float3 l2 = lerp(c.rgb, tex3D(_LutTex2, c.rgb).rgb, _Fade2);
float s = smoothstep(_Pos, _Pos + _Smooth, i.uv.y);
c.rgb = lerp(l1.rgb, l2.rgb, s);
c.rgb = c.rgb*c.rgb;
return c;
}

ENDCG


Subshader
{
Pass
{
Cull Off ZWrite Off ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
ENDCG
}

Pass
{
Cull Off ZWrite Off ZTest Always

CGPROGRAM
#pragma vertex vert
#pragma fragment fragLinear
#pragma target 3.0
ENDCG
}
}

Fallback off

}