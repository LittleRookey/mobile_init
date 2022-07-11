using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subtab : Tab
{
    public Tab mainTab;

    public bool useScroll;
    public ScrollRect scroll;

    public void SelectSubTab()
    {
        if (mainTab.isSelected)
        {
            if (useScroll)
                scroll.content = OpenWindow.GetComponent<RectTransform>();
            SelectTab();    

        }
    }
}
