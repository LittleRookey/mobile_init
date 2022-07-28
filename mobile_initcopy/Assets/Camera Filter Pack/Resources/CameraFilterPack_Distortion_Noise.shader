// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_Noise" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
}
SubShader 
{
Pass
{
Cull Off ZWrite Off ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
uniform float4 _ScreenResolution;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}

float mod289(float x)
{
return x - floor(x * 0.0034602) * 289.0;
}

float4 mod289(float4 x)
{
return x - floor(x * 0.0034602) * 289.0;
}

float4 perm(float4 x)
{
return mod289(((x * 34.0) + 1.0) * x);
}

float noise3d(float3 p)
{
float3 a = floor(p);
float3 d = p - a;
d = d * d * (3.0 - 2.0 * d);

float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
float4 k1 = perm(b.xyxy);
float4 k2 = perm(k1.xyxy + b.zzww);

float4 c = k2 + a.zzzz;
float4 k3 = perm(c);
float4 k4 = perm(c + 1.0);

float4 o1 = frac(k3 * 0.0243902);
float4 o2 = frac(k4 * 0.0243902);

float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

return o4.y * d.y + o4.x * (1.0 - d.y);
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float v1 = noise3d(float3(uv * 10.0, 0.0));
float v2 = noise3d(float3(uv * 10.0, 1.0));

float4 color  = tex2D(_MainTex, uv + float2(v1, v2) * 0.1*_Distortion);

return color;	
}

ENDCG
}

}
}