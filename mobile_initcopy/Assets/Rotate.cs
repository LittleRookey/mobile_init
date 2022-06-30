using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 5;
    public Vector3 init = new Vector3(60, 0, 0);
    public Vector3 rotAngle = new Vector3(0, 0, 60);
    private void OnValidate()
    {
        transform.eulerAngles = init;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = init;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotAngle * Time.deltaTime * speed);
    }
}
