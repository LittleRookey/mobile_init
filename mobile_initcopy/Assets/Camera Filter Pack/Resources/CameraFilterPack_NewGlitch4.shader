// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Camera Filter Pack v4.0.0                  
//                                     
// by VETASOFT 2020                    

Shader "CameraFilterPack/CameraFilterPack_NewGlitch4" {
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
uniform float Fade;


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



half4 _MainTex_ST;

float rand(float2 co)
{
return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}
float4 frag(v2f i) : COLOR { float4 cfresult=float4(0,0,0,0); 
float2 uv = i.texcoord;
if (fmod(_TimeX*_Speed, 2.0) > 1.9)
uv.x += cos(_TimeX*_Speed * 10.0 + uv.y * 1000.0) * 0.01;
if (fmod(_TimeX*_Speed, 5.0) > 3.75)
uv += 1.0 / 64.0 * (2.0 * float2(rand(floor(uv * 32.0) + float2(32.05,236.0)), rand(floor(uv.y * 32.0) + float2(-62.05,-36.0))) - 1.0);
cfresult = tex2D(_MainTex, uv);

float r=dot(cfresult.rgb, float3(0.25, 0.5, 0.25));

if (rand(float2(_TimeX*_Speed,_TimeX*_Speed)) > 0.90)
cfresult = float4(r,r,r,r);
cfresult.rgb += 0.25 * float3(rand(_TimeX*_Speed + i.texcoord / float2(-213, 5.53)), rand(_TimeX*_Speed - i.texcoord / float2(213, -5.53)), rand(_TimeX*_Speed + i.texcoord / float2(213, 5.53))) - 0.125;

cfresult = lerp(tex2D(_MainTex,i.texcoord.xy),cfresult,Fade);
return cfresult;}

ENDCG
}

}
}
 