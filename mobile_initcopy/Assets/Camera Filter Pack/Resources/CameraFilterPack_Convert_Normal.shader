// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////
Shader "CameraFilterPack/Color_Convert_Normal" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.00)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_ColorRGB ("_ColorRGB", Color) = (1,1,1,1)

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
uniform float _Heigh;
uniform float _Intervale;
uniform float4 _ScreenResolution;
uniform float4 _ColorRGB;
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

float sampleHeight(float2 coord)
{
	float4 txt = tex2D(_MainTex, coord);
	float x = 0.;
	x = txt.r + txt.g + txt.b;
	x /= 3.;
	return _Heigh * x;

}


half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float2 du = float2(_Intervale, 0.0);
float2 dv = float2(0.0, _Intervale);

float hpx = sampleHeight(uv + du);
float hmx = sampleHeight(uv - du);
float hpy = sampleHeight(uv + dv);
float hmy = sampleHeight(uv - dv);

float dHdU = (hmx - hpx) / (2.0 * du.x);
float dHdV = (hmy - hpy) / (2.0 * dv.y);



float3 normal = normalize(float3(dHdU, dHdV, 1.0));

float4 ret = float4(0.5+ 0.5 * normal, 1.0);

return ret;


}

ENDCG
}

}
}