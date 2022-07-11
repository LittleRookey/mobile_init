using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_Manager : MonoBehaviour
{

    public float _findTimer;

    public List<Transform> _unitPool = new List<Transform>();

    public List<SA_Unit> _p1UnitList = new List<SA_Unit>();

    public List<SA_Unit> _p2UnitList = new List<SA_Unit>();

    public List<SA_Unit> _p3UnitList = new List<SA_Unit>();


    private void Awake()
    {
        SoonsoonData.Instance.SAM = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetUnitList();

    }


    public SA_Unit GetTarget(SA_Unit unit)
    {
        SA_Unit tUnit = null;

        List<SA_Unit> tList = new List<SA_Unit>();
        switch(unit.tag)
        {
            case "Player": tList = _p2UnitList; break;
            case "Enemy": tList = _p1UnitList; break;
        }

        float tSDis = 999999;
        for (int i = 0; i < tList.Count; i++)
        {
            float tDis = ((Vector2)tList[i].transform.localPosition - (Vector2)unit.transform.localPosition).sqrMagnitude;
            if (tDis <= unit._unitFightRange * unit._unitAttackRange)
            {
                if (tList[i].gameObject.activeInHierarchy)
                {
                    if (tList[i]._unitState != SA_Unit.UnitState.death)
                    {
                        if (tDis < tSDis)
                        {
                            tUnit = tList[i];
                            tSDis = tDis;
                        }
                    }

                }
            }
        }
        return tUnit;
    }
    void SetUnitList()
    {
        _p1UnitList.Clear();
        _p2UnitList.Clear();
        _p3UnitList.Clear();

        for (var i = 0; i < _unitPool.Count; i++)
        {
            for (var j = 0; j < _unitPool[i].childCount; j++)
            {
                switch(i)
                {
                    case 0:
                        
                        _p1UnitList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Unit>());
                        //Debug.Log(_unitPool[i].GetChild(j).gameObject.name);
                        _unitPool[i].GetChild(j).gameObject.tag = "Player";
                        break;
                    case 1:
                        _p2UnitList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Unit>());
                        //Debug.Log(_unitPool[i].GetChild(j).gameObject.name);
                        _unitPool[i].GetChild(j).gameObject.tag = "Enemy";
                        break;

                    case 2:
                        _p3UnitList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Unit>());
                        _unitPool[i].GetChild(j).gameObject.tag = "Object";
                        break;

                }
                
            }
        }
    }
}
