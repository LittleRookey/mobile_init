////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Weather/New Rain FX")]
public class CameraFilterPack_Rain_RainFX : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;
     
    private Material SCMaterial;
    [Range(-8f, 8f)]
    public float Speed = 1f;
    [Range(0f, 1f)]
    public float Fade = 1f;

    [HideInInspector]
    public int Count = 0;
    private Vector4[] Coord = new Vector4[4];
    public static Color ChangeColorRGB;
    private Texture2D Texture2;
    private Texture2D Texture3;

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
        Texture2 = Resources.Load("CameraFilterPack_RainFX_Anm2") as Texture2D;
        Texture3 = Resources.Load("CameraFilterPack_RainFX_Anm") as Texture2D;
        SCShader = Shader.Find("CameraFilterPack/RainFX");

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
            material.SetFloat("_Value", Fade);
            material.SetFloat("_Speed", Speed);
            material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

            AnimationCurve curve = new AnimationCurve();
            curve = new AnimationCurve();
            curve.AddKey(0, 0.01f);
            curve.AddKey(64, 5f);
            curve.AddKey(128, 80f);
            curve.AddKey(255, 255f);
            curve.AddKey(300, 255f);
            for (int c = 0; c < 4; c++)
            {
                Coord[c].z += 0.5f;
                if (Coord[c].w == -1) Coord[c].x = -5.0f;
                if (Coord[c].z > 254) Coord[c] = new Vector4(Random.Range(0f, 0.9f), Random.Range(0.2f, 1.1f), 0, Random.Range(0, 3));
                material.SetVector("Coord" + (c + 1).ToString(), new Vector4(Coord[c].x, Coord[c].y, (int)curve.Evaluate(Coord[c].z), Coord[c].w));
            }
            material.SetTexture("Texture2", Texture2);
            material.SetTexture("Texture3", Texture3);

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
            SCShader = Shader.Find("CameraFilterPack/RainFX");
            Texture2 = Resources.Load("CameraFilterPack_RainFX_Anm2") as Texture2D;
            Texture3 = Resources.Load("CameraFilterPack_RainFX_Anm") as Texture2D;
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