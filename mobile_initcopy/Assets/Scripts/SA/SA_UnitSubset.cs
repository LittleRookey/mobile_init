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
    public Image _thirdFrame;
    [SerializeField] private Sprite _eliteLevelFrame;
    [SerializeField] private Sprite _bossLevelFrame;

    public TextMeshProUGUI _levelText;


    public bool alwaysTurnOnHPBar;
    public void SetNormal()
    {
        _hpList[2].transform.localScale = Vector3.one;
        _hpList[3].transform.localScale = Vector3.one;
        _levelFrame.gameObject.SetActive(false);
        _thirdFrame.gameObject.SetActive(false);
    }

    public void SetElite()
    {
        _hpList[2].transform.localScale = Vector3.one;
        _hpList[3].transform.localScale = Vector3.one;
        _levelFrame.gameObject.SetActive(true);
        _thirdFrame.gameObject.SetActive(false);
        _levelFrame.sprite = _eliteLevelFrame;
    }

    public void SetBoss()
    {
        _hpList[2].transform.localScale = Vector3.one;
        _hpList[3].transform.localScale = Vector3.one;
        _levelFrame.gameObject.SetActive(true);
        _thirdFrame.gameObject.SetActive(true);
        _levelFrame.sprite = _eliteLevelFrame;
        _thirdFrame.sprite = _bossLevelFrame;
    }

    public void ResetMonster(SA_Unit unit)
    {
        _levelText.text = unit._level.ToString();
        _hpBar.transform.localScale = Vector3.one;
    }

    
}
