// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Camera Filter Pack v4.0.0                  
//                                     
// by VETASOFT 2020                    

Shader "CameraFilterPack/CameraFilterPack_NewGlitch7" {
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
uniform float LightMin;
uniform float LightMax;


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

#define bs 28.0
#define bc 0.1
float sat( float t ) {
return clamp( t, 0.0, 1.0 );
}
float2 sat( float2 t ) {
return clamp( t, 0.0, 1.0 );
}
float remap  ( float t, float a, float b ) {
return sat( (t - a) / (b - a) );
}
float linterp( float t ) {
return sat( 1.0 - abs( 2.0*t - 1.0 ) );
}
float3 spectrum_offset( float t ) {
float3 ret;
float lo = step(t,0.5);
float hi = 1.0-lo;
float w = linterp( remap( t, 0.16, 0.83 ) );
float neg_w = 1.0-w;
ret = float3(lo,1.0,hi) * float3(neg_w, w, neg_w);
return pow( ret, float3(0.45,0.45,0.45) );
}
float rand(float2 co){
return frac(sin(dot(co.xy ,float2(12.989,78.233))) * 4375.545);
}
float4 frag(v2f i) : COLOR { float4 cfresult=float4(0,0,0,0); 
float2 uv = i.texcoord;
uv.y *= _ScreenResolution.y/_ScreenResolution.x;
float time = _TimeX*_Speed;
float4 sum = tex2D(_MainTex, float2(1.0,1.778)*uv);
const float amount = 6.0;
for(float i = 0.0; i < amount; i++){
uv /= pow(lerp(float2(1.0,1.0), frac(bs*uv)+0.5, clamp(pow(length(CFnoise( float2(0.06*_TimeX*_Speed,0.06*_TimeX*_Speed)) ), 15.0),0.0, 1.0)), float2(bc,bc));
sum = clamp(sum, LightMin, LightMax);
sum /= 0.1+0.9*clamp(tex2D(_MainTex, float2(1.0,1.778)*uv+float2(1.0/_ScreenResolution.x,i/_ScreenResolution.y)),0.0,2.0);
sum *= 0.1+0.9*clamp(tex2D(_MainTex, float2(1.0,1.778)*uv+float2(1.0/_ScreenResolution.x,-i/_ScreenResolution.y)),0.0,2.0);
sum *= 0.1+0.9*clamp(tex2D(_MainTex, float2(1.0,1.778)*uv+float2(-i/_ScreenResolution.x,i/_ScreenResolution.y)),0.0,2.0);
sum /= 0.1+0.9*clamp(tex2D(_MainTex, float2(1.0,1.778)*uv+float2(-i/_ScreenResolution.x,-i/_ScreenResolution.y)),0.0,2.0);
sum.xyz /= 1.01-0.025*spectrum_offset( 1.0-length(sum.xyz) );
sum.xyz *= 1.0+0.01*spectrum_offset( length(sum.xyz) );
}
sum = 0.1+0.9*sum;
float chromaf = pow(length(CFnoise( float2(0.0213*_TimeX*_Speed,0.0213*_TimeX*_Speed)) ), 2.0);
sum /= length(sum);
sum = (-0.2+2.0*sum)*0.9;
cfresult = sum;
return cfresult;}

ENDCG
}

}
}
