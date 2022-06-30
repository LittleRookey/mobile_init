using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(SPUM_Exporter))]
[CanEditMultipleObjects]
public class SPUM_ExporterEditor : Editor
{
    //field list
    SerializedProperty _unitPrefab;
    SerializedProperty _unitType;
    SerializedProperty _separated;
    SerializedProperty _imageName;
    SerializedProperty _imageSize;
    SerializedProperty _fullSize;
    SerializedProperty _scaleFactor;
    SerializedProperty _frameRate;
    SerializedProperty _advanced;    
    SerializedProperty _camera;
	SerializedProperty _anim;
	SerializedProperty _objectPivot;
	SerializedProperty _objectNow;
	SerializedProperty _imgBG;
	SerializedProperty _bgSet;
	SerializedProperty frameNowNumber;
	SerializedProperty _animNum;
	SerializedProperty timer;
	SerializedProperty timerForSave;
	SerializedProperty useTimer;
	SerializedProperty _netAnimClip;
	SerializedProperty animNum;
    SerializedProperty animationClips;
	SerializedProperty _animNameList;
	SerializedProperty _animNameNow;
	SerializedProperty _textSaveList;
    SerializedProperty _gifExportUse;
	SerializedProperty _gifBGColor; 
	SerializedProperty _gifUseTransparancy;
	SerializedProperty _gifAlphaBGColor;
    SerializedProperty _gifDelay; 
	SerializedProperty _gifQuality;
	SerializedProperty _gifRepeatNum;



    //parameter list
    float tValue = 0;
    SPUM_Exporter SPB;
    AnimationClip tAnimSave;
    float tAnimTimer;
    float tAnimTimerFactor;
    float timeSave;
    float tValuee = 0 ;
    bool objectSelectionFoldout = false;
    int objectSelectionToolbar = 0;

    void OnEnable()
    {

        // Fetch the objects from the GameObject script to display in the inspector
        _unitPrefab = serializedObject.FindProperty("_unitPrefab");
        _unitType = serializedObject.FindProperty("_unitType");
        _separated = serializedObject.FindProperty("_separated");
        _imageName = serializedObject.FindProperty("_imageName");
        _imageSize = serializedObject.FindProperty("_imageSize");
        _fullSize = serializedObject.FindProperty("_fullSize");
        _scaleFactor = serializedObject.FindProperty("_scaleFactor");
        _frameRate = serializedObject.FindProperty("_frameRate");
        _advanced = serializedObject.FindProperty("_advanced");
        _camera = serializedObject.FindProperty("_camera");
        _anim = serializedObject.FindProperty("_anim");
        _objectPivot = serializedObject.FindProperty("_objectPivot");
        _objectNow = serializedObject.FindProperty("_objectNow");
        _imgBG = serializedObject.FindProperty("_imgBG");
        _bgSet = serializedObject.FindProperty("_bgSet");
        _animNum = serializedObject.FindProperty("_animNum");
        timer = serializedObject.FindProperty("timer");
        timerForSave = serializedObject.FindProperty("timerForSave");
        useTimer = serializedObject.FindProperty("useTimer");
        _netAnimClip = serializedObject.FindProperty("_netAnimClip");
        animNum = serializedObject.FindProperty("animNum");
        animationClips = serializedObject.FindProperty("animationClips");
        _animNameList = serializedObject.FindProperty("_animNameList");
        _animNameNow = serializedObject.FindProperty("_animNameNow");
        _textSaveList = serializedObject.FindProperty("_textSaveList");
        _gifExportUse = serializedObject.FindProperty("_gifExportUse");
        _gifBGColor = serializedObject.FindProperty("_gifBGColor");
        _gifUseTransparancy = serializedObject.FindProperty("_gifUseTransparancy");
        _gifAlphaBGColor = serializedObject.FindProperty("_gifAlphaBGColor");
        _gifDelay = serializedObject.FindProperty("_gifDelay");
        _gifQuality = serializedObject.FindProperty("_gifQuality");
        _gifRepeatNum = serializedObject.FindProperty("_gifRepeatNum");

    }
    

    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        SPB = (SPUM_Exporter)target;
        // base.OnInspectorGUI();
        // EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_unitPrefab);
        EditorGUILayout.PropertyField(_unitType);
        EditorGUILayout.PropertyField(_separated);
        EditorGUILayout.PropertyField(_imageName);
        EditorGUILayout.PropertyField(_imageSize);
        EditorGUILayout.PropertyField(_fullSize);
        EditorGUILayout.PropertyField(_scaleFactor);
        EditorGUILayout.PropertyField(_frameRate);
        //Gif Exporter Value Sync;
        //For Gif Exporter
        if(!SPB._gifExportUse)
        {
            EditorGUILayout.PropertyField(_gifExportUse, new GUIContent("Enable Gif Export"));
        }
        else
        {
            EditorGUILayout.PropertyField(_gifExportUse, new GUIContent("Disable Gif Export"));
            EditorGUILayout.HelpBox("Gif Exporter isn't stable now (Preview Version).",MessageType.Warning);
            // EditorGUILayout.IntSlider (_gifQuality, 1, 100, new GUIContent ("Gif Quality"));
            EditorGUILayout.PropertyField(_gifDelay, new GUIContent("Animation Time Delay"));
            // EditorGUILayout.PropertyField(_gifRepeatNum, new GUIContent("Animation Repeat Number"));
            if(!SPB._gifUseTransparancy)
            {
                EditorGUILayout.PropertyField(_gifUseTransparancy, new GUIContent("Enable Backgrund Transparancy"));
                // EditorGUILayout.HelpBox("Basic BG Color is white",MessageType.Info);
                EditorGUILayout.PropertyField(_gifBGColor);
            }
            else
            {
                EditorGUILayout.PropertyField(_gifUseTransparancy, new GUIContent("Disable Backgrund Transparancy"));
                // EditorGUILayout.HelpBox("Basic Alpha BG Color is Green",MessageType.Info);
                EditorGUILayout.PropertyField(_gifAlphaBGColor);
            }
        }

        EditorGUILayout.HelpBox("Adavnced settings only for more options (not recommended)",MessageType.Info);
        if(!SPB._advanced)
        {
            EditorGUILayout.PropertyField(_advanced, new GUIContent("Advanced Settings Show"));
        }
        else
        {
            EditorGUILayout.PropertyField(_advanced, new GUIContent("Advanced Settings Off"));
            EditorGUILayout.HelpBox("Editing is not recommended.",MessageType.Warning);
            EditorGUILayout.PropertyField(_camera);
            EditorGUILayout.PropertyField(_anim);
            EditorGUILayout.PropertyField(_objectPivot);
            EditorGUILayout.PropertyField(_objectNow);
            EditorGUILayout.PropertyField(_imgBG);
            EditorGUILayout.PropertyField(_bgSet);
            EditorGUILayout.PropertyField(_animNum);
            EditorGUILayout.PropertyField(timer);
            EditorGUILayout.PropertyField(timerForSave);
            EditorGUILayout.PropertyField(useTimer);
            EditorGUILayout.PropertyField(_netAnimClip);
            EditorGUILayout.PropertyField(animNum);
            EditorGUILayout.PropertyField(animationClips);
            EditorGUILayout.PropertyField(_animNameList);
            EditorGUILayout.PropertyField(_animNameNow);
            EditorGUILayout.PropertyField(_textSaveList);
        }

        

        serializedObject.ApplyModifiedProperties();

        

        if (GUILayout.Button("Make Sprite Images",GUILayout.Height(50))) 
        {
            if(!SPB.useTimer)
            {
                Debug.Log("Starting Export Sprite Sheets...");
                SPB.StartExport();
                SPB.animNum = 0;
                SPB.animationClips = SPB._anim.runtimeAnimatorController.animationClips;
                SPB._textSaveList.Clear();
                SPB._netAnimClip = true;
            }
        }

        if(SPB._unitPrefab!=null)
        {
            if (GUILayout.Button("Remove Object",GUILayout.Height(50))) 
            {
                Debug.Log("Removed Prefab Object!!");
                SPB._unitPrefab = null;
                SPB._imageName = "";
            }
        }
        SPB._imgBG.sizeDelta = new Vector2( SPB._imageSize.x, SPB._imageSize.y );
        if(SPB._unitPrefab==null)
        {
            SPB.CheckObjNow();
        }
        else
        {
            SPB.MakeObjNow();
            SPB._objectPivot.transform.localScale = new Vector3(SPB._scaleFactor,SPB._scaleFactor,SPB._scaleFactor);
        }

        if(SPB.useTimer)
        {
            float tTimer = Time.realtimeSinceStartup - timeSave;
            timeSave = Time.realtimeSinceStartup;
            SPB.timer += tTimer;

            if(SPB.timer > SPB.timerForSave)
            {
                tValue += tAnimTimerFactor;
                
                SPB.timer = 0;
                SPB.frameNowNumber++;
                if(SPB.frameNowNumber >= SPB._frameNumber)
                {
                    SPB.frameNowNumber = 0;
                    tValue = 0;
                    SPB.animNum++;
                    SPB.useTimer = false;
                    if(SPB._separated) 
                    {
                        SPB._sepaName = tAnimSave.name;
                    }
                    if(SPB.animNum < SPB._animNameNow.Count)
                    {
                        if(SPB._separated) SPB.MakeScreenShotFile();
                        SPB._netAnimClip = true;
                    }
                    else
                    {
                        SPB.MakeScreenShotFile();
                        SPB.PrintEndMessage();
                    }
                }
                
                if(SPB.animNum >= SPB._animNameNow.Count)
                {
                    ExporterReset();
                    SPB.MakeGifAnimation();
                }
                else 
                {
                    ExporterShot();
                    SPB.SetScreenShot();
                }
                
            }
        }

        if(SPB._netAnimClip)
        {
            SPB._netAnimClip = false;
            AnimationClip tAnim = null;
            foreach( var obj in SPB.animationClips)
            {
                if(obj.name == SPB._animNameNow[SPB.animNum])
                {
                    tAnim = obj;
                }
            }
            
            if(tAnim == null) return;

            tAnimSave = tAnim;
            tAnimTimer = tAnimSave.length;
            tAnimTimerFactor = 1f / (SPB._frameRate*1f);
            SPB._frameNumber = (int)(tAnimTimer / tAnimTimerFactor);
            Debug.Log("[[Generating : "+ tAnimSave.name + " || Time Length : " + tAnimSave.length + " sec " + "|| Frame Numbers : "+ SPB._frameNumber+"]]");
            SPB.frameNowNumber = 0;
            tValue = 0;
            SPB.timer = 0;
            SPB.useTimer = true;
            timeSave = Time.realtimeSinceStartup;

            SPB._textSaveList.Clear();
            ExporterShot();
            SPB.SetScreenShot();
        }
    }

    public void ExporterShot()
    {
        AnimationMode.StartAnimationMode();
        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(SPB._anim.gameObject,tAnimSave,tValue);
        AnimationMode.EndSampling();
    }

    public void ExporterReset()
    {
        AnimationMode.StartAnimationMode();
        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(SPB._objectNow,tAnimSave,0);
        AnimationMode.EndSampling();
        AnimationMode.StopAnimationMode();
    }

}
