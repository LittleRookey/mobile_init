// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Glasses_On" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float Fade;
uniform float VisionBlur;
uniform float GlassDistortion;
uniform float GlassAberration;
uniform float _Shadow;
uniform float2 _MainTex_TexelSize;
uniform float4 GlassesColor;
uniform float4 GlassesColor2;
uniform float4 GlassColor;
uniform float UseFinalGlassColor;
uniform float UseScanLine;
uniform float UseScanLineSize;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
float2 texcoord2 : TEXCOORD1;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float2 texcoord2  : TEXCOORD1;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0) IN.texcoord.y = 1 - IN.texcoord.y;
#endif
OUT.texcoord2 = IN.texcoord;
return OUT; 
}

float4 GaussianBlur(float2 uv)
{
	float kernel[6];
	float3 final_colour = 0.0;
	kernel[0] = 0;
	kernel[1] = 0;
	kernel[2] = 0;
	kernel[3] = 0;
	kernel[4] = 0;
	kernel[5] = 0;
	float Z = 0.0;
	int j = 0;
	
	for (j = 0; j <= 2; ++j)
	{
		kernel[2 + j] = kernel[2 - j] = 0.4;
	}
	for (j = 0; j < 6; ++j)
	{  
		Z += kernel[j];
	}
	for (int u = -2; u <= 2; ++u)
	{
		for (int j = -2; j <= 2; ++j)
		{
			float kernelmult = kernel[2 + j] * kernel[2 + u];
			float4 tex = tex2D(_MainTex, (uv + float2(float(u*VisionBlur), float(j*VisionBlur))) );
			final_colour += kernelmult * (tex).rgb;
		}
	}
	return float4(final_colour / (Z*Z), 1.0);
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float2 uv2 = (i.texcoord2.xy-float2(0.5,0.5))*(1-Fade)+float2(0.5,0.5);
float e = smoothstep(0.7,1,Fade);
float s = smoothstep(0,0.3,Fade);
float s2 = smoothstep(0,0.2,Fade);
uv2.y -= s2;
uv2.y = min(uv2.y,0);
float4 tg = tex2D(_MainTex2, uv2);
float4 gcol = lerp(GlassesColor, GlassesColor2,tg.g);
float dist = saturate(tg.g-tg.r);
float2 duv = float2(dist-0.5,dist-0.5)*GlassDistortion;
float4 txt = tex2D(_MainTex, uv- duv*0.2);
float aber = lerp(0, 0.25 * Fade, GlassAberration);
float4 txtl = tex2D(_MainTex, uv - duv*(0.2 - aber));
float4 txtr = tex2D(_MainTex, uv - duv*(0.2 + aber));
float4 stxt = tex2D(_MainTex, uv);
float4 tblur = GaussianBlur(uv)-(Fade*0.2);
UseScanLineSize = UseScanLineSize*(1 - Fade);
float scans = clamp(0.35 + 0.35*sin(3.5*_TimeX + uv.y*UseScanLineSize*1.5), 0.0, 1.0);
float x = pow(scans, 1.7);
float4 Fglasses = float4(txtl.r, txt.g, txtr.b, 1) + GlassColor;
Fglasses = lerp(Fglasses, Fglasses * (0.4 + 0.7 * x), UseScanLine);
txt = lerp(Fglasses, tblur, tg.b);
float addblack = 1 - smoothstep(0.1, 0.5, tg.g);
txt = lerp(txt, gcol-(addblack * 0.8), tg.r);
txt = lerp(tblur,txt,s);
stxt = lerp(stxt,stxt+ (GlassColor*0.5),UseFinalGlassColor);
txt = lerp(stxt,txt,1-e);
return txt;
}
ENDCG
}
}
}