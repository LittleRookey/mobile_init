using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;

public class CopyComponet : EditorWindow
{
    [MenuItem("Component/Copy")]
    static void ShowWindow()
    {
        CopyComponet window = EditorWindow.GetWindow<CopyComponet>("Component Copy System");
        window.minSize = new Vector3(300, 260);
        window.Show();
    }

    GUIStyle titleLabelStyle = new GUIStyle();
    GUIStyle defaultLabelStyle = new GUIStyle();
    GUIStyle defualtToggleStyle;
    GUIStyle mainButtonStyle;

    GameObject srcObject, destObject;
    string destName;

    private bool pasteComponetValue = true;
    private bool overlapComponentRemove = true;
    private bool copyTag = true, copyLayer = true;

    private void TitleLabelStyleInit()
    {
        titleLabelStyle.fontSize = 20;
        titleLabelStyle.margin = new RectOffset(0, 0, 10, 10);
        titleLabelStyle.alignment = TextAnchor.MiddleCenter;
        titleLabelStyle.fontStyle = FontStyle.Bold;
        titleLabelStyle.normal.textColor = Color.white;
    }

    private void DefaultLabelStyleInit()
    {
        defaultLabelStyle.fontSize = 14;
        defaultLabelStyle.margin = new RectOffset(5, 0, 10, 5);
        defaultLabelStyle.alignment = TextAnchor.MiddleLeft;
        defaultLabelStyle.fontStyle = FontStyle.Normal;
        defaultLabelStyle.normal.textColor = Color.white;

        // toggle
        defualtToggleStyle = new GUIStyle(GUI.skin.toggle);
        defualtToggleStyle.fontSize = 14;
        defualtToggleStyle.margin = new RectOffset(5, 0, 5, 5);
        defualtToggleStyle.alignment = TextAnchor.MiddleLeft;
        defualtToggleStyle.fontStyle = FontStyle.Normal;
        defualtToggleStyle.normal.textColor = Color.white;

        // main Button
        mainButtonStyle = new GUIStyle(GUI.skin.button);
        mainButtonStyle.margin = new RectOffset(0, 0, 10, 0);
    }

    private void OnGUI()
    {
        TitleLabelStyleInit();
        DefaultLabelStyleInit();

        GUILayout.Label("Component Copy System", titleLabelStyle);

        srcObject =
            EditorGUILayout.ObjectField("Src Obejct", srcObject, typeof(GameObject), true) as GameObject;
        destObject =
            EditorGUILayout.ObjectField("Dest Obejct", destObject, typeof(GameObject), true) as GameObject;

        if (destObject != null)
        {
            if (String.IsNullOrEmpty(destName))
            {
                destName = destObject.name;
            }

            destName = EditorGUILayout.TextField("Dest Name", destName);
        }

        GUILayout.Label("", defaultLabelStyle);
        GUILayout.Label("Setting", defaultLabelStyle);

        pasteComponetValue =
            EditorGUILayout.Toggle(new GUIContent("Paste Value", "Paste src Componet Value to Dest"), pasteComponetValue, defualtToggleStyle);

        overlapComponentRemove =
            EditorGUILayout.Toggle(new GUIContent("Overlap Remove", "Overlap Component Remove"), overlapComponentRemove, defualtToggleStyle);

        copyTag =
           EditorGUILayout.Toggle(new GUIContent("Copy Tag", "Paste Src Tag to Dest"), copyTag, defualtToggleStyle);

        copyLayer =
            EditorGUILayout.Toggle(new GUIContent("Copy Layer", "Paste Src Layer to Dest"), copyLayer, defualtToggleStyle);

        if (GUILayout.Button("Copy", mainButtonStyle, GUILayout.Height(30)) == true)
        {
            if (srcObject == null)
            {
                Debug.Log("SrcObject Is Null!");
                return;
            }

            if (destObject == null)
            {
                Debug.Log("DestObject Is Null!");
                return;
            }

            CopySrcComponetToDest();
        }
    }

    private void CopySrcComponetToDest()
    {
        Component[] srcComponets = srcObject.GetComponents<Component>();

        foreach (Component srcComm in srcComponets)
        {
            Type type = srcComm.GetType();
            Component addComm = null;

            if (overlapComponentRemove == true)
            {
                if (destObject.TryGetComponent(type, out addComm) == false)
                {
                    addComm = destObject.AddComponent(type);
                }
            }
            else
            {
                addComm = destObject.AddComponent(type);
            }


            if (addComm != null && pasteComponetValue == true)
            {
                ComponentUtility.CopyComponent(srcComm);
                ComponentUtility.PasteComponentValues(addComm);
            }
        }

        destObject.name = destName;
        destObject.layer = srcObject.layer;
        destObject.tag = srcObject.tag;
    }
}