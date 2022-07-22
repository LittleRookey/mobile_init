using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SA_UnitSubset : MonoBehaviour
{
    public List<GameObject> _hpList;
    public Image _hpBar;
    public Image _levelFrame;
    public TextMeshProUGUI _levelText;

    public bool alwaysTurnOnHPBar;

    public void ResetMonster(SA_Unit unit)
    {
        _levelText.text = unit._level.ToString();
        _hpBar.transform.localScale = Vector3.one;
    }

    
}
