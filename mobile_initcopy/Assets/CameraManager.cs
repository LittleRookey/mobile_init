using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Camera mainCamera;

    [SerializeField]
    public CameraFilterPack_AAA_SuperComputer superComputerEffect;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        

    }

    // Update is called once per frame
    void Update()
    {
        //superComputerEffect._AlphaHexa = 1.0f;
    }
}
