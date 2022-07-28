// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Camera Filter Pack v4.0.0                  
//                                     
// by VETASOFT 2020                    

Shader "CameraFilterPack/CameraFilterPack_NewGlitch2" {
Properties
{
_MainTex("Base (RGB)", 2D) = "white" {}
_TimeX("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution("_ScreenResolution", Vector) = (0.,0.,0.,0.)
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
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform float _TimeX;

uniform float _Speed;
uniform float RedFade;


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

float CFhash(float n)
{
return frac(sin(n) * 43812.175489);
}

float CFnoise(float2 p)
{
p*=128;
float2 pi = floor(p);
float2 pf = frac(p);
float n = pi.x + 59.0 * pi.y;
pf = pf * pf * (3.0 - 2.0 * pf);
return lerp(lerp(CFhash(n), CFhash(n + 1.0), pf.x),
lerp(CFhash(n + 59.0), CFhash(n + 1.0 + 59.0), pf.x),
pf.y);
}


half4 _MainTex_ST;

float4 frag(v2f i) : COLOR { float4 cfresult=float4(0,0,0,0);
float2 uv = i.texcoord;
float2 block = floor(i.texcoord.xy / float2(0.02,0.02));
float2 uv_noise = block / float2(1,1);
uv_noise += floor(float2(_TimeX*_Speed,_TimeX*_Speed) * float2(1234.0, 3543.0)) / float2(64,64);
float block_thresh = pow(frac(1 * 1236.0453), 2.0) * 0.2;
float line_thresh = pow(frac(1 * 2236.0453), 3.0) * 0.7;
float2 uv_r = uv, uv_g = uv, uv_b = uv;
if (CFnoise( uv_noise) < block_thresh ||
CFnoise( float2(uv_noise.y, 0.0)) < line_thresh) {
float2 dist = (frac(uv_noise) - 0.5) * 0.3;
uv_r += dist * 0.1;
uv_g += dist * 0.2;
uv_b += dist * 0.125;
}
cfresult.r = tex2D(_MainTex, uv_r).r;
cfresult.g = tex2D(_MainTex, uv_g).g;
cfresult.b = tex2D(_MainTex, uv_b).b;
if (CFnoise( uv_noise) < block_thresh)
cfresult.rgb = cfresult.ggg;
if (CFnoise( float2(uv_noise.y, 0.0)) * 3.5 < line_thresh)
cfresult.rgb = float3(0.0, dot(cfresult.rgb, float3(1.0,1.0,1.0)), 0.0);
if (CFnoise( uv_noise) * 1.5 < block_thresh ||
CFnoise( float2(uv_noise.y, 0.0)) * 2.5 < line_thresh) {
float l = frac(i.texcoord.y / 3.0);
float3 mask = float3(3.0, 0.0, 0.0);
if (l > 0.333)
mask = float3(0.0, 3.0, 0.0);
if (l > 0.666)
mask = float3(0.0, 0.0, 3.0);
mask = lerp(1,mask,RedFade);
cfresult.xyz *= mask;
}
return cfresult;}

ENDCG
}

}
}
