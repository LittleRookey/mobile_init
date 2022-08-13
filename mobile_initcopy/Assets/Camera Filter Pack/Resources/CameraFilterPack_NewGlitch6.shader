// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Camera Filter Pack v4.0.0                  
//                                     
// by VETASOFT 2020                    

Shader "CameraFilterPack/CameraFilterPack_NewGlitch6" {
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
uniform float FadeLight;
uniform float FadeDark;


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

float hash( float2 p )
{
float h = dot(p + _TimeX*_Speed * 0.0001,float2(127.1,311.7));
return frac(sin(h)*43331.545331);
}
float hash2( float2 p )
{
float h = dot(p ,float2(127.1,311.7));
return frac(sin(h)*43758.5453123);
}
float noise( in float2 p )
{
float2 i = floor( p );
float2 f = frac( p );
float2 u = f*f*(3.0-2.0*f);
return lerp( lerp( hash( i + float2(0.0,0.0) ),
hash( i + float2(1.0,0.0) ), u.x),
lerp( hash( i + float2(0.0,1.0) ),
hash( i + float2(1.0,1.0) ), u.x), u.y);
}
float4 frag(v2f i) : COLOR { float4 cfresult=tex2D(_MainTex,i.texcoord);
float2 uv = i.texcoord;
float glitch = hash2(floor(_TimeX*_Speed * 4.0 - uv * 20.0) / 20.0);
uv += pow(frac(glitch + _TimeX*_Speed * 0.1), 30.0);
float n = pow((noise(uv * 150.0) + noise(uv * 350.0)) * 0.5, 3.0);
n *= abs(sin(uv.y * 250.0 + _TimeX*_Speed * 2.0));
float offset = sign(sin(uv.x * 6.0 + _TimeX*_Speed * 3.0)) * 0.1;
offset += sign(sin(uv.y * 10.0 + uv.x * 14.5 + _TimeX*_Speed * 20.0)) * 0.1;
offset += sign(sin(sin(_TimeX*_Speed) * uv.y * 12.0 - uv.x * 22.0 - _TimeX*_Speed * 40.0)) * 0.2;
offset = pow(offset - 0.5, 4.0);
n *= min(1.0, floor((1.5 - uv.y + offset) * 3.0) / 3.0);
float waver = abs(noise(float2(_TimeX*_Speed,_TimeX*_Speed)) * 2.0);
n *= pow(frac(floor(uv.y * 15.0) / 15.0 + _TimeX*_Speed * 0.3) * waver * 0.4 + 0.4, 0.65);
n *= 0.75 + waver * 0.125;
n = clamp(n, 0.0, 1.0);
cfresult = lerp(cfresult,cfresult+float4(n,n,n,n),FadeLight);
cfresult = lerp(cfresult,cfresult-float4(n,n,n,n)*2,FadeDark);
return cfresult;}
 
ENDCG
}

}
}
