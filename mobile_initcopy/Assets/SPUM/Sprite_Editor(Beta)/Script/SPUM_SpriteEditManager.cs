using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SPUM_SpriteEditManager : MonoBehaviour
{
    #if UNITY_EDITOR
    public SPUM_SpriteList _spriteObj;
    // public Sprite _hair;
    // public Sprite _mustache;
    // public Sprite _helmet1;
    // public Sprite _helmet2;
    // public Texture2D _cloth;
    // public Texture2D _pant;
    // public Texture2D _armor;
    // public Sprite _weaponRight;
    // public Sprite _weaponLeft;
    // public Sprite _back;
    public List<SpriteRenderer> _syncList = new List<SpriteRenderer>();

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(this.transform.position, 0.1f);
    }

    void Start()
    {

    }

    void Update()
    {
        // if(Selection.activeGameObject == this.gameObject)
        // {
        //     SyncSprite();
        // }
    }
    // //Sync all sprite added.
    // public void SyncSprite()
    // {
    //     _syncList[0].GetComponent<SpriteSync>()._nowSprite=_hair;
    //     _syncList[1].GetComponent<SpriteSync>()._nowSprite=_mustache;
    //     _syncList[2].GetComponent<SpriteSync>()._nowSprite=_helmet1;
    //     _syncList[3].GetComponent<SpriteSync>()._nowSprite=_helmet2;

    //     SetMultiple(_cloth,_syncList[4].GetComponent<SpriteSync>()._nowSprite,"Body");
    //     SetMultiple(_cloth,_syncList[5].GetComponent<SpriteSync>()._nowSprite,"Right");
    //     SetMultiple(_cloth,_syncList[6].GetComponent<SpriteSync>()._nowSprite,"Left");
    //     SetMultiple(_pant,_syncList[7].GetComponent<SpriteSync>()._nowSprite,"Right");
    //     SetMultiple(_pant,_syncList[8].GetComponent<SpriteSync>()._nowSprite,"Left");
    //     SetMultiple(_armor,_syncList[9].GetComponent<SpriteSync>()._nowSprite,"Body");
    //     SetMultiple(_armor,_syncList[10].GetComponent<SpriteSync>()._nowSprite,"Right");
    //     SetMultiple(_armor,_syncList[11].GetComponent<SpriteSync>()._nowSprite,"Left");

    //     SetWeapon(_weaponRight,0);  
    //     SetWeapon(_weaponLeft,1);        

    //     _syncList[16].GetComponent<SpriteSync>()._nowSprite=_back;
    // }

    public void SyncPivot()
    {
        SyncPivotProcess(_spriteObj._hairList[0]);
        SyncPivotProcess(_spriteObj._hairList[3]);
        SyncPivotProcess(_spriteObj._hairList[1]);
        SyncPivotProcess(_spriteObj._hairList[2]);
        SyncPivotProcess(_spriteObj._clothList[0]);
        SyncPivotProcess(_spriteObj._clothList[1]);
        SyncPivotProcess(_spriteObj._clothList[2]);
        SyncPivotProcess(_spriteObj._pantList[0]);
        SyncPivotProcess(_spriteObj._pantList[1]);
        SyncPivotProcess(_spriteObj._armorList[0]);
        SyncPivotProcess(_spriteObj._armorList[1]);
        SyncPivotProcess(_spriteObj._armorList[2]);
        SyncPivotProcess(_spriteObj._weaponList[0]);
        SyncPivotProcess(_spriteObj._weaponList[1]);
        SyncPivotProcess(_spriteObj._weaponList[2]);
        SyncPivotProcess(_spriteObj._weaponList[3]);
        SyncPivotProcess(_spriteObj._backList[0]);
    }

    public void SyncPivotProcess(SpriteRenderer SR)
    {
        if(SR.sprite!=null) SetPivot(SR);
    }

    public void SetMultiple(Texture2D sp, Sprite ttSP, string nameCode)
    {
        if(sp==null) return;

        string spritePath = AssetDatabase.GetAssetPath( sp );
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);

        foreach(object obj in sprites)
        {
            if(obj.GetType() == typeof(Sprite))
            {
                Sprite tSP = (Sprite)obj;
                if(tSP.name == nameCode)
                {
                    ttSP = tSP;
                }
            }
        }
    }

    public void SetWeapon(Sprite sp, int Type)
    {
        if(sp!=null)
        {
            string tRWName = sp.name;
            if(Type == 0) //오른쪽
            {
                if(tRWName.Contains("Shield"))
                {
                    //방패
                    _syncList[12].GetComponent<SpriteSync>()._nowSprite = null;
                    _syncList[13].GetComponent<SpriteSync>()._nowSprite = sp;
                }
                else
                {
                    _syncList[12].GetComponent<SpriteSync>()._nowSprite = sp;
                    _syncList[13].GetComponent<SpriteSync>()._nowSprite = null;
                }
            }
            else //오른쪽
            {
                if(tRWName.Contains("Shield"))
                {
                    //방패
                    _syncList[14].GetComponent<SpriteSync>()._nowSprite = null;
                    _syncList[15].GetComponent<SpriteSync>()._nowSprite = sp;
                }
                else
                {
                    _syncList[14].GetComponent<SpriteSync>()._nowSprite = sp;
                    _syncList[15].GetComponent<SpriteSync>()._nowSprite = null;
                }
            }
            
        }
    }

    public void EmptyAllSprite()
    {
        for(var i = 0 ; i < _syncList.Count;i++)
        {
            _syncList[i].sprite = null;
            _syncList[i].GetComponent<SpriteSync>()._nowSprite = null;
        }        
    }
    //Reset all sprite added.

    public void SetPivot(SpriteRenderer _sprite)
    {
        if( _sprite.transform.localPosition.x ==0 && _sprite.transform.localPosition.y ==0) return;
        string path = AssetDatabase.GetAssetPath(_sprite.sprite.texture);
        

        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
        if(ti.spritesheet.Length > 1 )
        {
            ti.isReadable = true;
            List<SpriteMetaData> newData = new List<SpriteMetaData>();
            for(var i = 0 ; i < ti.spritesheet.Length;i++)
            {
                SpriteMetaData tD = ti.spritesheet[i];
                tD.alignment = (int)SpriteAlignment.Custom;
                if( _sprite.sprite.name == tD.name)
                {
                    float tXSize = tD.rect.size.x;
                    float tYSize = tD.rect.size.y;

                    float tX = _sprite.transform.localPosition.x / 0.015625f;
                    float tY = _sprite.transform.localPosition.y / 0.015625f;

                    float ttX = tX / tXSize * 0.5f;
                    float ttY = tY / tYSize * 0.5f;

                    float rX = 0.5f-ttX;
                    float rY = 0.5f-ttY;
                    tD.pivot = new Vector2(rX,rY);
                    ti.spritesheet[i]  = tD;
                }
                newData.Add(tD);
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            
            ti.isReadable = false;
        }
        else
        {
            float tXSize = _sprite.sprite.rect.size.x;
            float tYSize = _sprite.sprite.rect.size.y;

            float tX = _sprite.transform.localPosition.x / 0.015625f;
            float tY = _sprite.transform.localPosition.y / 0.015625f;

            float ttX = tX / tXSize * 0.5f;
            float ttY = tY / tYSize * 0.5f;

            float rX = 0.5f-ttX;
            float rY = 0.5f-ttY;

            Vector2 newPivot = new Vector2(rX,rY);
            ti.spritePivot = newPivot;
            TextureImporterSettings texSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(texSettings);
            texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
            ti.SetTextureSettings(texSettings);
            ti.SaveAndReimport();
        }

        _sprite.transform.localPosition = new Vector3(0,0,0);
    }
    #endif
}
