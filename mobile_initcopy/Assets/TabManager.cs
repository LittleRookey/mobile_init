using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    [Header("Bottom Tab Refs")]
    public GameObject _CharacterTab;
    public GameObject _BagTab;
    public GameObject _ContentsTab;
    public GameObject _CompanionTab;
    public GameObject _ShopTab;

    [Header("StatWindow Ref")]
    public GameObject _StatWindow;
    public StatSlotManager statManager;

    [Header("TalentWindow Ref")]
    public GameObject _TalentWindow;

    public Tab StatTab;
    public Tab TalentTab;
    
    public void OpenCharacterTab()
    {
        if (_CharacterTab.activeInHierarchy)
        {
            _CharacterTab.SetActive(false);
            
        }
        else
        {
            _CharacterTab.SetActive(true);
            StatTab.SelectTab();
            //_TalentTabDisabled.SetActive(true);
            //_TalentWindow
            statManager.UpdateSlots();

        }
    }

    public void OpenSubtabStat()
    {

        _StatWindow.gameObject.SetActive(true);

        _TalentWindow.gameObject.SetActive(false);
    }

    public void OpenSubtabTalent()
    {
        _StatWindow.gameObject.SetActive(false);
        _TalentWindow.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        OpenCharacterTab();
    }

}



