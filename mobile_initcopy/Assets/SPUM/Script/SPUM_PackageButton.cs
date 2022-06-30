using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SPUM_PackageButton : MonoBehaviour
{
    #if UNITY_EDITOR
    public int _index;
    public Text _title;
    public Image _bgImage;
    public SPUM_Manager _spumManager;
    public List<Color> _buttonColor = new List<Color>();

    void OnEnable()
    {
        if(_spumManager._drawItemIndex == -1) return;
        if(_spumManager._drawItemIndex == 10) return;
        if(_spumManager._textureList[_spumManager._drawItemIndex]._packageList[_index])
        {
            _bgImage.color = _buttonColor[1];
        }
        else
        {
            _bgImage.color = _buttonColor[0];
        }
    }

    public void CheckPackages()
    {
        if(_spumManager._drawItemIndex == -1) return;
        if(_spumManager._drawItemIndex == 10) return;
        // _spumManager.
        bool tV = _spumManager._textureList[_spumManager._drawItemIndex]._packageList[_index];
        _spumManager._textureList[_spumManager._drawItemIndex]._packageList[_index] = !tV;
        
        if(_spumManager._textureList[_spumManager._drawItemIndex]._packageList[_index])
        {
            _bgImage.color = _buttonColor[1];
        }
        else
        {
            _bgImage.color = _buttonColor[0];
        }

        _spumManager.DrawItemProcess();
        SoonsoonData.Instance.SavePackageData();
    }
    #endif
}
