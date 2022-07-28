using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SA_Manager : MonoBehaviour
{

    public float _findTimer;

    public float scanRadius = 3f;

    public List<Transform> _unitPool = new List<Transform>();

    public List<SA_Player> _playerList = new List<SA_Player>();

    public List<SA_UnitBase> _enemyList = new List<SA_UnitBase>();

    public List<SA_Unit> _objectList = new List<SA_Unit>();


    public UnityEvent<SA_Unit> OnFindTarget;

    private void Awake()
    {
        SoonsoonData.Instance.SAM = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetUnitList();

    }

    private void OnEnable()
    {
        Actions.OnEnemySpawn += AddEnemy;
    }

    private void OnDisable()
    {
        Actions.OnEnemySpawn -= AddEnemy;
    }

    public void AddEnemy(SA_Unit enemy)
    {
        enemy.tag = "Enemy";
        _enemyList.Add(enemy);

    }

    // Find Target with circle scan and return closest
    public SA_UnitBase FindTargetOf(SA_UnitBase sa)
    {
        string enemTag = "";
        switch(sa.tag)
        {
            case "Player": enemTag = "Enemy"; break;
            case "Enemy": enemTag = "Player"; break;
        }

        RaycastHit2D[] units = Physics2D.CircleCastAll(sa.transform.position, scanRadius, Vector2.zero, 0f, LayerMask.GetMask("Units"));
        if (units.Length <= 0) return null;

        SA_UnitBase retUnit = null;
        float tSDis = 999999;
        for (int i = 0; i < units.Length; i++)
        {
            float tDis = ((Vector2)units[i].transform.position - (Vector2)sa.transform.position).sqrMagnitude;

            if (units[i].transform.gameObject.activeInHierarchy && units[i].transform.CompareTag(enemTag))
            {
                SA_UnitBase enemSide = units[i].transform.GetComponent<SA_UnitBase>();
                if (enemSide._unitState != SA_Unit.UnitState.death)
                {
                    if (tDis < tSDis)
                    {
                        retUnit = enemSide;
                        tSDis = tDis;
                    }
                }

            }
            
        }
        return retUnit;
    }
    //public void ContainsEnemy()
    //public SA_Unit GetTarget(SA_Unit unit)
    //{
    //    SA_Unit tUnit = null;

    //    List<SA_UnitBase> tList = new List<SA_UnitBase>();
    //    switch(unit.tag)
    //    {
    //        case "Player": tList = _enemyList; break;
    //        case "Enemy": tList = _playerList; break;
    //    }

    //    float tSDis = 999999;
    //    for (int i = 0; i < tList.Count; i++)
    //    {
    //        float tDis = ((Vector2)tList[i].transform.localPosition - (Vector2)unit.transform.localPosition).sqrMagnitude;
    //        if (tDis <= unit._unitFightRange * unit._unitAttackRange)
    //        {
    //            if (tList[i].gameObject.activeInHierarchy)
    //            {
    //                if (tList[i]._unitState != SA_Unit.UnitState.death)
    //                {
    //                    if (tDis < tSDis)
    //                    {
    //                        tUnit = tList[i];
    //                        tSDis = tDis;
    //                    }
    //                }

    //            }
    //        }
    //    }
    //    return tUnit;
    //}

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
                        
                        _playerList.Add(_unitPool[i].GetChild(j).GetComponent<SA_Player>());
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
