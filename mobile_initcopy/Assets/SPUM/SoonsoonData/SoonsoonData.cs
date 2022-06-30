using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SoonsoonData
{
    
    // Start is called before the first frame updateprivate 
    static SoonsoonData instance = null;
    public static SoonsoonData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SoonsoonData();
            }
            return instance;
        }
    }

    private SoonsoonData(){}


    [Serializable]
    public class SoonData
    {
        public List<Dictionary<string,bool>> packageList = new List<Dictionary<string,bool>>();
    }

    public SoonData _soonData2 = new SoonData();

    public SPUM_Manager _spumManager;
    public bool _gifAlphaTrigger; // for using gif trigger at Soonsoon Exporter.
    public Color _gifBasicColor; //for using gif bg color at Soonsoon Exporter.
    public Color _alphaColor; // for using gif alpha color at Soonsoon Exporter.

    public SA_Manager SAM;
    public SA_MissileManager SAMM;

    public void SaveData()
    {
        // bool _saveAvailable = false;

        try
        {
            FileSaveToPrefab();
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to save the data");
            Debug.Log(e);
        }
        finally
        {
        }
    }


    private void FileSaveToPrefab()
    {
        var b = new BinaryFormatter();
        var m = new MemoryStream();
        b.Serialize(m , _soonData2);
        PlayerPrefs.SetString("SoonsoonSave2",Convert.ToBase64String(m.GetBuffer())); 
    }

    public IEnumerator LoadData()
    {
        yield return null;
        try
        {
            LoadProcess();
        }
        catch( System.Exception e)
        {
            Debug.Log(" Failed to load Data...");
            Debug.Log(e.ToString());
        }

        yield return new WaitForSecondsRealtime(0.1f);
    }
    public void LoadProcess()
    {
        Debug.Log("Trying Loading data ...");

        if(!PlayerPrefs.HasKey("SoonsoonSave2"))
        {
            Debug.Log("You don't use save data yet.");
        }
        else
        {
            string _str = PlayerPrefs.GetString("SoonsoonSave2");

            if( _str.Length > 0)
            {
                string _tmpStr = PlayerPrefs.GetString("SoonsoonSave2");
                if(!string.IsNullOrEmpty(_tmpStr)) 
                {
                    var b = new BinaryFormatter();
                    var m = new MemoryStream(Convert.FromBase64String(_tmpStr));
                    _soonData2 = (SoonData) b.Deserialize(m);
                    Debug.Log("Load Successful!!");
                }
            }
        }
    }

    public void SavePackageData()
    {
        SoonsoonData.instance._soonData2.packageList.Clear();
        #if UNITY_EDITOR
        for( var i = 0 ; i < _spumManager._textureList.Count;i++)
        {
            Dictionary<string,bool> tList = new Dictionary<string,bool>();
            for(var j = 0 ; j < _spumManager._textureList[i]._packageList.Count;j++)
            {
                tList.Add(_spumManager._textureList[i]._packageNameList[j],_spumManager._textureList[i]._packageList[j]);
            }
            SoonsoonData.instance._soonData2.packageList.Add(tList);
            
        }
        
        SaveData();
        #endif
    }

    public void LoadPackageData()
    {
        #if UNITY_EDITOR
        if(_soonData2.packageList==null) return;
        if(_soonData2.packageList.Count ==0 ) return;


        for( var i = 0 ; i < _spumManager._textureList.Count;i++)
        {
            List<bool> tList = new List<bool>();

            for(var j = 0 ; j < _spumManager._textureList[i]._packageList.Count;j++)
            {
                string tName = _spumManager._textureList[i]._packageNameList[j];
                if(_soonData2.packageList[i].ContainsKey(tName))
                {
                    tList.Add(_soonData2.packageList[i][tName]);
                }
                else
                {
                    tList.Add(true);
                }
            }

            _spumManager._textureList[i]._packageList.Clear();

            for ( var j = 0 ; j < tList.Count;j++)
            {
                 _spumManager._textureList[i]._packageList.Add(tList[j]);
            } 
        }

        _spumManager.LinkPackageList();
        #endif
    }
}
