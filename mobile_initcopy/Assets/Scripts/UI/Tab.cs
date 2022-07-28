using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tab : MonoBehaviour
{
    [SerializeField]
    private GameObject Disabled;

    public bool isSelected;

    public GameObject OpenWindow;

    public bool canTurnOnAndOff;

    //public UnityEvent OnSelected;
    public List<Tab> friendTabs;
    public List<Subtab> subtabs;

    private void Start()
    {
        if (friendTabs.Contains(this))
            friendTabs.Remove(this);
    }



    
    public void SelectTab()
    {
        Debug.Log("Selected tab " + name);
        isSelected = true;
        if (canTurnOnAndOff)
        {
            if (OpenWindow.gameObject.activeInHierarchy)
            {
                OpenWindow.gameObject.SetActive(false);
            } else
            {
                OpenWindow.gameObject.SetActive(true);
            }
        } else
        {
            OpenWindow.gameObject.SetActive(true);
        }

        DisableTab(false);
        for (int i = 0; i < friendTabs.Count; i++)
        {
            friendTabs[i].DeselectTab();
        }
        if (subtabs.Count > 0)
            subtabs[0].SelectSubTab();

    }
    
    void DeselectTab()
    {
        
        isSelected = false;
        OpenWindow.gameObject.SetActive(false);
        DisableTab(true);
    }
    void DisableTab(bool val)
    {
        if (Disabled != null)
            Disabled.gameObject.SetActive(val);
    }


}


