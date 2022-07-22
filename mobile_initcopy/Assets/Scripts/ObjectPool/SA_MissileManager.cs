using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SA_MissileManager : MonoBehaviour
{

    //public Transform _misslePool;
    public int _objectPoolNumber = 20;
    public SA_MissileObj _fireball, _arrow;
    
    //public List<SA_MissileObj> _poolList = new List<SA_MissileObj>();

    //public List<SA_MissileObj> _poolListUse = new List<SA_MissileObj>();

    private void Awake()
    {
        SoonsoonData.Instance.SAMM = this;

    }

    private void Start()
    {
        PoolManager.WarmPool(_fireball.gameObject, _objectPoolNumber);
        PoolManager.WarmPool(_arrow.gameObject, _objectPoolNumber);
    }
    // Start is called before the first frame update
    //void Start()
    //{
    //    GetMissleList();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //SSThread();
    //}

    //void SSThread()
    //{
    //    if (_poolListUse.Count > 0)
    //    {
    //        for (int i = 0; i < _poolListUse.Count; i++)
    //        {
    //            if (!_poolListUse[i].gameObject.activeInHierarchy) return;
    //            SA_MissileObj tsT = _poolListUse[i];
    //            tsT._timer += Time.deltaTime;

    //            if (tsT._timer > tsT._timerForLim)
    //            {
    //                tsT.MissileDone();
    //            } else
    //            {
    //                tsT.DoMove();
    //            }
    //        }
    //    }
    //}

    //public void GetMissleList()
    //{
    //    _poolList.Clear();
    //    _poolListUse.Clear();

    //    for (int i = 0; i < _misslePool.childCount; i++)
    //    {
    //        SA_MissileObj tObj = _misslePool.GetChild(i).GetComponent<SA_MissileObj>();
    //        _poolList.Add(tObj);
    //    }
    //}

    public void FireMissile(SA_MissileObj.MissileType type, SA_Unit owner, SA_Unit target)
    {

        if (type == SA_MissileObj.MissileType.fireball)
        {
            
            SA_MissileObj obj = PoolManager.SpawnObject(_fireball.gameObject, owner.transform.position, Quaternion.identity).GetComponent<SA_MissileObj>();
            obj.SetMissile(type, owner, target);
            //go.transform.position = owner.transform.position;
        }
        else if (type == SA_MissileObj.MissileType.arrow) 
        {
            //go.transform.position = owner.transform.position;
            SA_MissileObj arr = PoolManager.SpawnObject(_arrow.gameObject, owner.transform.position, Quaternion.identity).GetComponent<SA_MissileObj>();
            arr.SetMissile(type, owner, target);
        } 

        

        //foreach(var obj in _poolList)
        //{
        //    if(!obj.gameObject.activeInHierarchy)
        //    {
        //        tMissle = obj;
        //        break;
        //    }
        //}

        //if (tMissle != null)
        //{
        //    tMissle.SetMissile(type, owner, target);
        //}
    }
    
}
