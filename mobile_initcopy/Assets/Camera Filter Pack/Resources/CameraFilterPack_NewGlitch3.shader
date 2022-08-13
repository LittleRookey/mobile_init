// Camera Filter Pack v4.0.0                  
//                                     
// by VETASOFT 2020                    

Shader "CameraFilterPack/CameraFilterPack_NewGlitch3" {
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



half4 _MainTex_ST;

float hash(float2 p) 
{
float h = dot(p,float2(127.1,311.7));
return -1.0 + 2.0*frac(sin(h)*43758.5453123);
}
float noise(float2 p) 
{
float2 i = floor(p);
float2 f = frac(p);
float2 u = f*f*(3.0-2.0*f);
return lerp(lerp(hash( i + float2(0.0,0.0) ), hash( i + float2(1.0,0.0) ), u.x), lerp( hash( i + float2(0.0,1.0) ), hash( i + float2(1.0,1.0) ), u.x), u.y);
}
float noise(float2 p, int oct) 
{
float2 m = float2 ( 1.6,  1.2);
float f  = 0.0;
for(int i = 1; i < 3; i++){ float mul = 1.0/pow(2.0, float(i)); f += mul*noise(p); p = m*p; }
return f;
}
float4 frag(v2f i) : COLOR { float4 cfresult=float4(0,0,0,0); 
float2 uv = i.texcoord;
float glitch = pow(cos(_TimeX*_Speed*0.5)*1.2+1.0, 1.2);
glitch = saturate(glitch);
if(noise(_TimeX*_Speed+float2(0, 0))*glitch > 0.62){
uv.y = fmod(uv.y+noise(float2(_TimeX*_Speed*4.0, 0)), 1.0);
}
float2 hp = float2(0.0, uv.y);
float nh = noise(hp*7.0+_TimeX*_Speed*10.0, 3) * (noise(hp+_TimeX*_Speed*0.3)*0.8);
nh += noise(hp*100.0+_TimeX*_Speed*10.0, 3)*0.02;
float rnd = 0.0;
if(glitch > 0.0){ rnd = hash(uv); if(glitch < 1.0){ rnd *= glitch; } }
nh *= glitch + rnd;
float r = tex2D(_MainTex, uv+float2(nh, 0.08)*nh).r;
float g = tex2D(_MainTex, uv+float2(nh-0.07, 0.0)*nh).g;
float b = tex2D(_MainTex, uv+float2(nh, 0.0)*nh).b;
float3 col = float3(r, g, b);
cfresult = float4(col.rgb, 1.0);
return cfresult;
}

ENDCG
}

}
}
