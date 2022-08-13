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

    public void FireMissile(SA_MissileObj.MissileType type, SA_UnitBase owner, SA_UnitBase target)
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
