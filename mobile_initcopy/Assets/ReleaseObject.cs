using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseObject : MonoBehaviour
{
    [SerializeField] private float seconds = 2f;

    private void OnEnable()
    {
        Invoke("Release", seconds);
    }

    void Release()
    {
        PoolManager.ReleaseObject(gameObject);
    }
}
