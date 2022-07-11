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
        isSelected = true;
        OpenWindow.gameObject.SetActive(true);
        DisableTab(false);
        for (int i = 0; i < friendTabs.Count; i++)
        {
            friendTabs[i].DeselectTab();
        }
        if (subtabs.Count > 0)
            subtabs[0].SelectSubTab();
    }
    
    public void DeselectTab()
    {
        
        isSelected = false;
        OpenWindow.gameObject.SetActive(false);
        DisableTab(true);
    }
    public void DisableTab(bool val)
    {
        Disabled.gameObject.SetActive(val);
    }


}


