// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Camera Filter Pack v4.0.0                  
//                                     
// by VETASOFT 2020                    

Shader "CameraFilterPack/CameraFilterPack_NewGlitch5" {
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
uniform float Parasite;
uniform float ZoomX;
uniform float ZoomY;
uniform float PosX;
uniform float PosY;


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

float rand1(in float a, in float b) { return frac((cos(dot(float2(a,b) ,float2(12.9898,78.233))) * 43758.5453));}
float rand2(float frag_x, float frag_y) { return frac(sin(frag_y+frag_x)*_TimeX*_Speed+sin(frag_y-frag_x)*_TimeX*_Speed);}
float4 frag(v2f i) : COLOR { float4 cfresult=float4(0,0,0,0); 
float zoom = ZoomX;
float2 uv = i.texcoord;
uv.x = abs(uv.x-PosX)+PosY;
uv += ZoomX;
uv = uv/(1.+ZoomY);
float2 p = uv;
p.x = uv.y;
p.y = 1.*Parasite;
float4 t1 = CFnoise( p);
p.x /= 4.0;
float4 t2 = CFnoise( p);
t1.y -= (t1.y-0.5) * 0.5;
t1.x += (t2.y-0.5) * 1.2;
float shake = sin(rand2(t2.x, t1.x) * 0.5) * .001 * frac(t1.y * _ScreenResolution.y/(1.-zoom) / rand1(_TimeX*_Speed, t2.x));
shake += sin(shake - t1.r * t1.g) * t2.g * 0.14 * frac(uv.y * _ScreenResolution.y/(1.-zoom) / 2.0);
shake *= .8*Parasite;
uv.x += shake / t1.g * 2.41*Parasite;
uv = fmod(uv,1.);
cfresult = tex2D(_MainTex, uv);
float grey = (cfresult.r + cfresult.g + cfresult.b) / 2.1;
cfresult.rgb += grey;
cfresult.rgb *= .57;
cfresult *= lerp(cfresult, CFnoise( 0.9 * uv * -150.0), shake * .09 * _TimeX*_Speed / grey * 0.08) + t1;
cfresult = lerp(cfresult, CFnoise( 0.9 * uv * -150.0), -shake * .09 * _TimeX*_Speed / grey * 0.008);
return cfresult;}

ENDCG
}

}
}
