///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2020 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Vision/SniperScore")]
public class CameraFilterPack_Vision_SniperScore : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
 
private Material SCMaterial;

[Range(0f, 1f)]
public float Fade = 1f;
[Range(0f, 1f)]
public float Size = 0.45f;
[Range(0.01f, 0.4f)]
public float Smooth = 0.045f;
[Range(0f, 1f)]
public float _Cible = 0.5f;
[Range(0f, 1f)]
public float _Distortion = 0.5f;
[Range(0f, 1f)]
public float _ExtraColor = 0.5f;

[Range(0f, 1f)]
public float _ExtraLight = 0.35f;
    public Color _Tint = new Color(0, 0.6f, 0, 0.25f);
[Range(0f, 10f)]
private float StretchX = 1f;
[Range(0f, 10f)]
private float StretchY = 1f;
[Range(-1f, 1f)]
public float _PosX = 0f;
[Range(-1f, 1f)]
public float _PosY = 0f;



    #endregion
    #region Properties
    Material material
{
get
{
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;
}
return SCMaterial;
}
}
#endregion
void Start ()
{

SCShader = Shader.Find("CameraFilterPack/Vision_SniperScore");
if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}

void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
material.SetFloat("_Fade", Fade);
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Value", Size);
material.SetFloat("_Value2", Smooth);
material.SetFloat("_Value3", StretchX);
material.SetFloat("_Value4", StretchY);
material.SetFloat("_Cible", _Cible);
material.SetFloat("_ExtraColor", _ExtraColor);
material.SetFloat("_Distortion", _Distortion);
material.SetFloat("_PosX", _PosX);
material.SetFloat("_PosY", _PosY);
material.SetColor("_Tint", _Tint);
material.SetFloat("_ExtraLight", _ExtraLight);
Vector2 Scr = new Vector2(Screen.width, Screen.height);
material.SetVector("_ScreenResolution",new Vector4(Scr.x,Scr.y,Scr.y/Scr.x,0));

Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void Update ()
{

#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Vision_SniperScore");
}
#endif
}
void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
