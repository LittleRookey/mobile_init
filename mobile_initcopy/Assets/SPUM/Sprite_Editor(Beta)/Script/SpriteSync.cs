using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SpriteSync : MonoBehaviour
{
    #if UNITY_EDITOR
    [HideInInspector]
    public SPUM_SpriteEditManager _manager;
    [HideInInspector]
    public SpriteRenderer _mySprite;
    public Sprite _nowSprite;

    void OnDrawGizmos()
    {
        if(_nowSprite==null)
        {
            Gizmos.color = new Color(1,1,1,0.5f);
            Gizmos.DrawSphere(this.transform.position, 0.2f);
            Handles.Label(transform.position, gameObject.name);
        }
        
    }
    
    void Update()
    {
        if(Selection.activeGameObject != this.gameObject) return;

        if(_mySprite==null)
        {
            _mySprite = transform.GetComponent<SpriteRenderer>();
        }
        else
        {
            _mySprite.sprite = _nowSprite;
        }

        if(_manager==null)
        {
            _manager = GameObject.FindObjectOfType<SPUM_SpriteEditManager>();
        }
    }

    public void SyncPivot()
    {
        if(_mySprite.sprite!=null)
        {
            _manager.SyncPivotProcess(_mySprite);
        }
        
    }

    public void RemoveSprite()
    {
        transform.localPosition = Vector3.zero;
        _mySprite.sprite = null;
        _nowSprite = null;
    }
    #endif
}
