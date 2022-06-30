using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SpriteEyeSync : MonoBehaviour
{
    #if UNITY_EDITOR
    [HideInInspector]
    public SPUM_SpriteEditManager _manager;

    public List<SpriteRenderer> _eyeSpList = new List<SpriteRenderer>();
    public Texture2D _nowTexture;

    void OnDrawGizmos()
    {
        if(_nowTexture==null)
        {
            Gizmos.color = new Color(1,1,1,0.5f);
            Gizmos.DrawSphere(this.transform.position, 0.2f);
            Handles.Label(transform.position, gameObject.name);
        }
        
    }
    
    void Update()
    {
        if(Selection.activeGameObject != this.gameObject) return;

        TextureToSprite();

        if(_manager==null)
        {
            _manager = GameObject.FindObjectOfType<SPUM_SpriteEditManager>();
        }
    }

    void TextureToSprite()
    {
        if(_eyeSpList.Count == 0 ) return;
        if(_nowTexture != null )
        {
            string path = AssetDatabase.GetAssetPath(_nowTexture);
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);

            for( var i = 0 ; i < sprites.Length ; i++ )
            {
                if(sprites[i].GetType() == typeof(Sprite))
                {
                    switch(sprites[i].name)
                    {
                        case "Back":
                        _eyeSpList[0].sprite = (Sprite)(sprites[i]);
                        _eyeSpList[1].sprite = (Sprite)(sprites[i]);
                        break;

                        case "Front":
                        _eyeSpList[2].sprite = (Sprite)(sprites[i]);
                        _eyeSpList[3].sprite = (Sprite)(sprites[i]);
                        break;
                    }
                }
            }
        }
        else
        {
            for(var i = 0 ; i < _eyeSpList.Count ;i++)
            {
                _eyeSpList[i].sprite = null;
            }
        }
    }

    public void SyncPivot()
    {
        // if(_mySprite.sprite!=null)
        // {
        //     _manager.SyncPivotProcess(_mySprite);
        // }
        for(var i = 0 ; i < _eyeSpList.Count ; i++)
        {
            if(_eyeSpList[i].sprite != null)
            {
                _manager.SetPivot(_eyeSpList[i]);
            }
        }
        
    }

    public void RemoveSprite()
    {
        transform.localPosition = Vector3.zero;

        for(var i = 0 ; i < _eyeSpList.Count ;i++)
        {
            _eyeSpList[i].sprite = null;
        }
        _nowTexture = null;
    }
    #endif
}
