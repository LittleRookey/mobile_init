using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class SPUM_SpriteButtonST : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool _use;
    public Image _mainSprite;
    public int index;
    public SPUM_Manager _Manager;
    public Image _colorBG;
    public List<string> _textureList = new List<string>();
    public List<GameObject> _LockBtn = new List<GameObject>();
    public List<bool> _packageList = new List<bool>();
    public List<string> _packageNameList = new List<string>();

    void Start()
    {
        if(_mainSprite == null ) _mainSprite = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        if(_Manager == null ) _Manager = FindObjectOfType<SPUM_Manager>();
        if(index < 10)
        {
            if(_colorBG == null ) _colorBG = transform.GetChild(1).GetChild(0).GetComponent<Image>();
            if(_LockBtn.Count == 0)
            {
                _LockBtn.Add(transform.GetChild(1).GetChild(3).GetChild(0).gameObject);
                _LockBtn.Add(transform.GetChild(1).GetChild(3).GetChild(1).gameObject);
            } 
        }
    }

    public void SetUse(bool value)
    {
        _use = value;
        if(_use)
        {
            _mainSprite.color = Color.red;
        }
        else
        {
            _mainSprite.color = Color.white;
        }
    }


    public void DrawItem()
    {
        if(index == 10)
        {
            _Manager.AllRandom();
        }
        else if(index == 11)
        {
            _Manager.SetInit();
        }
        else
        {
            _Manager.DrawItem(index);
        }
    }

    public void ChangeColor()
    {
        _Manager.OpenColorPick(index);
    }

    public void ChangeRandom()
    {
        _Manager.RandomSelect(index);
    }

    public void ResetSprite()
    {
        _Manager.SetSprite(index,null,"",-1);
    }

    public void ChangeLock()
    {
        if(_LockBtn[0].activeInHierarchy)
        {
            _LockBtn[0].SetActive(false);
            _LockBtn[1].SetActive(true);
        }
        else
        {
            _LockBtn[0].SetActive(true);
            _LockBtn[1].SetActive(false);
        }
    }
    #endif
}
