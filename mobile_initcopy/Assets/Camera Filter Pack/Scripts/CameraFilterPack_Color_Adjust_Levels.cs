////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Colors/Levels")]
public class CameraFilterPack_Color_Adjust_Levels : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
 
private Material SCMaterial;
   [Range(0.0f, 1.0f)]
    public float levelMinimum = 0.0f;
    [Range(0.0f, 1.0f)]
    public float levelMiddle = 0.5f;
    [Range(0.0f, 1.0f)]
    public float levelMaximum = 1.0f;
    [Range(0.0f, 1.0f)]
    public float minOutput = 0.0f;
    [Range(0.0f, 1.0f)]
    public float maxOutput = 1.0f;

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
SCShader = Shader.Find("CameraFilterPack/Color_Levels");
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

            material.SetFloat("levelMinimum", levelMinimum);
            material.SetFloat("levelMiddle", levelMiddle);
            material.SetFloat("levelMaximum", levelMaximum);
            material.SetFloat("minOutput", minOutput);
            material.SetFloat("maxOutput", maxOutput);

            material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
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
SCShader = Shader.Find("CameraFilterPack/Color_Levels");
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