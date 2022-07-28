////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/FX/Drunk")]
public class CameraFilterPack_FX_Drunk : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
 
private Material SCMaterial;
[HideInInspector]
[Range(0, 20)]
public float Value = 6.0f;
[Range(0, 10)]
public float Speed = 1.0f;
[Range(0, 1)]
public float Wavy = 1f;
[Range(0, 1)]
public float Distortion = 0f;
[Range(0, 1)]
public float DistortionWave = 0f;
[Range(0, 1)]
public float Fade = 1.0f;
[Range(-2, 2)]
public float ColoredSaturate = 1.0f;
[Range(-1, 2)]
public float ColoredChange = 0.0f;
[Range(-1, 1)]
public float ChangeRed = 0.0f;
[Range(-1, 1)]
public float ChangeGreen = 0.0f;
[Range(-1, 1)]
public float ChangeBlue = 0.0f;

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
SCShader = Shader.Find("CameraFilterPack/FX_Drunk");
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
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Value", Value);
material.SetFloat("_Speed", Speed);
material.SetFloat("_Distortion", Distortion);
material.SetFloat("_DistortionWave", DistortionWave);
material.SetFloat("_Wavy", Wavy);
material.SetFloat("_Fade", Fade);

material.SetFloat("_ColoredChange", ColoredChange);
material.SetFloat("_ChangeRed", ChangeRed);
material.SetFloat("_ChangeGreen", ChangeGreen);
material.SetFloat("_ChangeBlue", ChangeBlue);

material.SetFloat("_Colored", ColoredSaturate);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);	
}
}
// Update is called once per frame
void Update () 
{
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/FX_Drunk");
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