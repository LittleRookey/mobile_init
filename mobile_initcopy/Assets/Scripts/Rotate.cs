using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 5;
    public Vector3 init = new Vector3(60, 0, 0);
    public Vector3 rotAngle = new Vector3(0, 0, 30);
    public bool applyRotation;
    Vector3 rot;
    private void OnValidate()
    {
        rot = transform.eulerAngles;
        if (applyRotation)
        {
            transform.eulerAngles = init;
        } else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.eulerAngles = init;
        rot = transform.eulerAngles;
        if (applyRotation)
        {
            transform.eulerAngles = init;
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotAngle * Time.deltaTime * speed);
    }
}
