using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleTab : MonoBehaviour
{
    [Header("Settings")]
    public Button TabButton;
    public GameObject highlight;
    public bool useText;

    public Image baseImage;
    public TextMeshProUGUI baseText;
    [Header("ETC")]
    public Sprite baseicon;
    public Sprite highlighticon;

    public Color normalColor = new Color(121,110, 111, 0);
    public Color selectedColor = new Color(246, 225, 156, 0);

    string All = "ALL";
    public List<ToggleTab> friendTabs;

    private void Awake()
    {
        if (TabButton == null)
            TabButton = GetComponent<Button>();

    }

    public void SelectTab()
    {
        Debug.Log("Selected: " + name);
        highlight.gameObject.SetActive(true);
        if (useText)
        {
            baseText.color = selectedColor;
            baseText.text = All;
        } else
        {
            baseImage.sprite = highlighticon;
            baseImage.gameObject.SetActive(true);
        }
    }

    public void DeselectTab()
    {
        Debug.Log("Deselected: " + name);
        highlight.gameObject.SetActive(false);
        if (useText)
        {
            baseText.color = normalColor;
        }
        else
        {
            baseImage.sprite = baseicon;
        }
    }
}
