// Camera Filter Pack v4.0.0
//
// by VETASOFT 2020
//

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Classic/ThermalVision")]
public class CameraFilterPack_Classic_ThermalVision : MonoBehaviour
{
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;

private Material SCMaterial;
[Range(0, 1)]
public float __Speed = 1.0f;
[Range(0, 1)]
 public float _Fade = 1.0f;
[Range(0, 1)]
 public float _Crt = 1.0f;
[Range(0, 1)]
 public float _Curve = 1.0f;
[Range(0, 1)]
 public float _Color1 = 1.0f;
[Range(0, 1)]
 public float _Color2 = 1.0f;
[Range(0, 1)]
 public float _Color3 = 1.0f;

#endregion
#region Properties
Material material
{
get
{
if (SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;
}
return SCMaterial;
}
}
#endregion
void Start()
{
SCShader = Shader.Find("CameraFilterPack/CameraFilterPack_Classic_ThermalVision");
if (!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}
void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
{
if (SCShader != null)
{
TimeX += Time.deltaTime;
if (TimeX > 100) TimeX = 0;
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Speed", __Speed);
material.SetFloat("Fade", _Fade);
material.SetFloat("Crt", _Crt);
material.SetFloat("Curve", _Curve);
material.SetFloat("Color1", _Color1);
material.SetFloat("Color2", _Color2);
material.SetFloat("Color3", _Color3);

material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void Update()
{
#if UNITY_EDITOR
if (Application.isPlaying != true)
{
SCShader = Shader.Find("CameraFilterPack/CameraFilterPack_Classic_ThermalVision");
}
#endif
}
void OnDisable()
{
if (SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
