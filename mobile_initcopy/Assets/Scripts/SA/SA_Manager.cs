using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_Manager : MonoBehaviour
{

    public float _findTimer;

    public List<Transform> _unitPool = new List<Transform>();

    public List<SA_Unit> _playerList = new List<SA_Unit>();

    public List<SA_Unit> _enemyList = new List<SA_Unit>();

    public List<SA_Unit> _objectList = new List<SA_Unit>();


    private void Awake()
    {
        SoonsoonData.Instance.SAM = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetUnitList();

    }

    public void AddEnemy(SA_Unit enemy)
    {
        enemy.tag = "Enemy";
        _enemyList.Add(enemy);

    }

    //public void ContainsEnemy()
    public SA_Unit GetTarget(SA_Unit unit)
    {
        SA_Unit tUnit = null;

        List<SA_Unit> tList = new List<SA_Unit>();
        switch(unit.tag)
        {
            case "Player": tList = _enemyList; break;
            case "Enemy": tList = _playerList; break;
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
        _playerList.Clear();
        _enemyList.Clear();
        _objectList.Clear();

        for (var i = 0; i < _unitPool.Count; i++)
        {
            for (var j = 0; j < _unitPool[i].childCount; j++)
            {
                switch(i)
                {
                    case 0: // for players
                        
                        _playerList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Unit>());
                        //Debug.Log(_unitPool[i].GetChild(j).gameObject.name);
                        _unitPool[i].GetChild(j).gameObject.tag = "Player";
                        break;
                    case 1: // for enemies
                        _enemyList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Unit>());
                        //Debug.Log(_unitPool[i].GetChild(j).gameObject.name);
                        _unitPool[i].GetChild(j).gameObject.tag = "Enemy";
                        break;

                    case 2:
                        _objectList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Unit>());
                        _unitPool[i].GetChild(j).gameObject.tag = "Object";
                        break;

                }
                
            }
        }
    }
}
