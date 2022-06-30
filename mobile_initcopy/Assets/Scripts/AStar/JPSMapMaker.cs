using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JPSMapMaker : MonoBehaviour
{

    bool isPathFinding = false;
    Image start, end;
    private void Update()
    {
        if (isPathFinding == true) return;

        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);

        if (Input.GetMouseButtonDown(2))
        {
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Image _img))
                {
                    if (_img.color == Color.black)
                    {
                        _img.color = Color.white;
                        _img.tag = "Untagged";
                    }
                    else
                    {
                        _img.color = Color.black;
                        _img.tag = "Wall";
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Image _img))
                {
                    if (start != null)
                    {
                        start.tag = "Untagged";
                        start.color = Color.white;
                    }

                    _img.color = Color.red;
                    _img.tag = "Start";

                    start = _img;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Image _img))
                {
                    if (end != null)
                    {
                        end.tag = "Untagged";
                        end.color = Color.white;
                    }

                    _img.color = Color.blue;
                    _img.tag = "End";

                    end = _img;
                }
            }
        }

    }
}