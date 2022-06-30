using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SA_UnitSet))]
public class SA_UnitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SA_UnitSet SPB = (SA_UnitSet)target;
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("You can reset unit data", MessageType.Error);

        if (GUILayout.Button("Reset Unit", GUILayout.Height(50)))
        {
            SPB.UnitTypeProcess();
        }
    }
}
