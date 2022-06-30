using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(SpriteSync))]
public class SPUM_Sync : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SpriteSync SPB = (SpriteSync)target;
        if(SPB._mySprite.sprite != null)
        {
            if (GUILayout.Button("Sync Pivot",GUILayout.Height(50))) 
            {
                SPB.SyncPivot();
            }
            if (GUILayout.Button("Remove Sprite",GUILayout.Height(50))) 
            {
                SPB.RemoveSprite();
            }
        }
    }
}
