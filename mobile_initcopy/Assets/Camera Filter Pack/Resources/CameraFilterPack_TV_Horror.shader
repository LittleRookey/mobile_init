// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////


Shader "CameraFilterPack/TV_Horror" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
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
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform sampler2D Texture2;
uniform float _TimeX;
uniform float Fade;
uniform float Distortion;

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
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float2 uv2 = uvst.xy;
float2 uv3 = uvst.xy;
uv2.y = floor(sin(_TimeX)*12) / 16;
float4 txt = tex2D(_MainTex, uv);
float4 memotxt = txt;
float m = Distortion;
uv2 = float2(-m*0.5, -m*0.5)+uv2+float2(txt.r, txt.g)*m;
float4 txt2 = tex2D(Texture2, uv2);
float4 txt3 = tex2D(Texture2, uv3);
float bw = (txt.r + txt.g + txt.b) / 3;
txt = bw*txt.g/txt.b+txt.r;
txt = txt*txt2;
txt *= txt3;
txt = lerp(memotxt,txt,Fade);
return  txt;
}
ENDCG
}
}
}