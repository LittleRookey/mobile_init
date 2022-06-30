using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteSyncManager : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool _use;
    public SPUM_SpriteEditManager _spriteEditor;
    public List<SpriteSync> _syncList = new List<SpriteSync>();
    public List<Vector3> _syncPos = new List<Vector3>();
    
    void Update()
    {
        if(!_use)
        {
            if(_syncPos.Count > 0 )
            {
                SyncPivotInit();
            }
        }
    }

    public void SyncPivotInit()
    {
        for(var i = 0 ; i < _syncList.Count;i++)
        {
            _syncList[i].transform.position = new Vector3(_syncPos[i].x,_syncPos[i].y,0);
            _syncList[i]._mySprite.sortingOrder = (int)(_syncPos[i].z);
        }
    }

    public void SyncPivotDataFirst()
    {
        _syncPos.Clear();
       SyncFistProcess(_syncList[0],_spriteEditor._spriteObj._hairList[0]);
       SyncFistProcess(_syncList[1],_spriteEditor._spriteObj._hairList[3]);
       SyncFistProcess(_syncList[2],_spriteEditor._spriteObj._hairList[1]);
       SyncFistProcess(_syncList[3],_spriteEditor._spriteObj._clothList[0]);
       SyncFistProcess(_syncList[4],_spriteEditor._spriteObj._clothList[2]);
       SyncFistProcess(_syncList[5],_spriteEditor._spriteObj._clothList[1]);
       SyncFistProcess(_syncList[6],_spriteEditor._spriteObj._pantList[1]);
       SyncFistProcess(_syncList[7],_spriteEditor._spriteObj._pantList[0]);
       SyncFistProcess(_syncList[8],_spriteEditor._spriteObj._armorList[0]);
       SyncFistProcess(_syncList[9],_spriteEditor._spriteObj._armorList[2]);
       SyncFistProcess(_syncList[10],_spriteEditor._spriteObj._armorList[1]);
       SyncFistProcess(_syncList[11],_spriteEditor._spriteObj._weaponList[0]);
       SyncFistProcess(_syncList[12],_spriteEditor._spriteObj._weaponList[1]);
       SyncFistProcess(_syncList[13],_spriteEditor._spriteObj._weaponList[2]);
       SyncFistProcess(_syncList[14],_spriteEditor._spriteObj._weaponList[3]);
       SyncFistProcess(_syncList[15],_spriteEditor._spriteObj._backList[0]);
    }

    void SyncFistProcess(SpriteSync spriteSync, SpriteRenderer SP)
    {
        spriteSync.transform.position = SP.transform.position;
        Vector3 tV = new Vector3(SP.transform.position.x,SP.transform.position.y,SP.sortingOrder*1f);
    }
    #endif
}
