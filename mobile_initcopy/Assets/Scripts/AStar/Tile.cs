using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetColor(Color _color)
    {
        image.color = _color;
    }
}