using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_MissileObj : MonoBehaviour
{
    public enum MissileType
    {
        arrow,
        fireball
    };

    //public List<GameObject> _missileList;

    public float _timer;
    public float _timerForLim;

    public Vector2 _startPos;
    public Vector2 _endPos;
    public MissileType _missileType;
    public SA_Unit _owner;
    public SA_Unit _target;
    public string tTag;

    //public float _timer;
    public float _yPos;
    public float _yPosSave;
    public float _speed=1;
    public float _range;

    public bool _homing; // 유도 여부
    private void OnEnable()
    {
        SetInit();
        if (_target == null|| _owner == null)
        {
            SetInit();
        }
        StartCoroutine(ReleaseIt());
    }
    public void SetInit()
    {
        //_missileList.Clear();
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    GameObject tObj = transform.GetChild(i).gameObject;
        //    _missileList.Add(tObj);

        //    if (i != (int)_missileType) tObj.SetActive(false);
        //    else tObj.SetActive(true);
        //}

        switch(_missileType)
        {
            case MissileType.arrow:
                //Debug.Log("Set Arrow");
                _speed = 10f;
                _timerForLim = 10f;
                _homing = true;
                _range = 0;
                _yPos = 2f; // ypos올리면 좀더 화살 높이 쏨
                _yPosSave = _yPos;
                break;
            case MissileType.fireball:
                //Debug.Log("Set Missile");
                _speed = 3f;
                _timerForLim = 10f;
                _homing = true;
                _range = 2f;

                _yPos = -1f;
                _yPosSave = _yPos;
                break;
        }
    }

    public void SetMissile(MissileType type, SA_Unit owner, SA_Unit target)
    {
        transform.position = owner.transform.position + new Vector3(0, 0.25f, 0);
        _startPos = transform.position;
        _target = target;
        _missileType = type;
        _owner = owner;
        tTag = _target.tag;
        _endPos = (Vector2)_target.transform.position + new Vector2(0, 0.25f);

        _timer = 0;
        SetInit();

        //SoonsoonData.Instance.SAMM._poolListUse.Add(this);
    }

    public void MissileDone()
    {
        Debug.Log("Missile Released");
        PoolManager.ReleaseObject(this.gameObject);
        // TODO hit effect 
        //SoonsoonData.Instance.SAMM._poolListUse.Remove(this);
        //gameObject.SetActive(false);
        // TODO 
    }

    public void DoMove()
    {
        if (_owner == null || _target == null)
        {
            //PoolManager.ReleaseObject(this.gameObject);
        }
        if (_homing)
        {
            if (TargetCheck())
            {
                _endPos = (Vector2)_target.transform.position + new Vector2(0, 0.25f);
            }
        }

        Vector2 tVec = _endPos - (Vector2)transform.position;

        float tDis = tVec.sqrMagnitude;
        //Debug.Log(name + ": " + tDis);
        if (tDis > 0.1f)
        {
            Vector2 tDirVec = (tVec).normalized;
            Vector3 tVVect;
            //Debug.Log("Moving Missile: " + _speed);
            if (_yPos == -1f)
            {
                // when fireball
                tVVect = (_speed * (Vector3)tDirVec);
                //Debug.Log("vect: " + tVVect);
            } else
            {
                _yPos -= _yPosSave * Time.deltaTime;
                tVVect = (_speed * (Vector3)tDirVec + new Vector3(0, _yPos, 0));
            }
            
            transform.position += tVVect * Time.deltaTime;
            transform.up = tVVect;
        }
        else
        {
            // When it get hit
            Debug.Log("DoProcess");
            DoProcess();
        }
    }

    void DoProcess()
    {
        if (_range > 0)// fireball
        {
            Debug.Log("fireball done 0");
            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 1f);
            if (hit.Length > 0)
            {
                foreach(var obj in hit)
                {
                    if (obj.CompareTag(tTag))
                    {
                        _owner.AttackDone(obj.GetComponent<SA_Unit>());
                    }
                }
            }
        } else // arrow
        {
            Debug.Log("Arrow done 0");
            if (TargetCheck())
            {
                //    (_target.transform.position - transform.position).sqrMagnitude;
                Debug.Log("Arrow done 1");
                _owner.AttackDone();
                //if (_missileType == MissileType.arrow)
                //{
                //}
            }
        }
        MissileDone();
    }

    //TODO
    bool TargetCheck()
    {
        if (_target == null) return false;
        if (_target._unitState == SA_Unit.UnitState.death) return false;
        if (_target.gameObject == null) return false;
        if (!_target.gameObject.activeInHierarchy) return false;
        // target still exists and is alive
        Debug.Log("idle from checkTarget");
        _endPos = (Vector2)_target.transform.position;

        return true;
    }

    void Release()
    {
        //gameObject.SetActive(false);
        PoolManager.ReleaseObject(this.gameObject);
    }

    IEnumerator ReleaseIt()
    {
        yield return new WaitForSeconds(5f);
        PoolManager.ReleaseObject(this.gameObject);
    }
    private void Update()
    {
        DoMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag(tTag) && _missileType == MissileType.arrow)
        //{
        //    SA_Unit sa = collision.gameObject.GetComponent<SA_Unit>();
        //    sa.AttackDone(sa, _owner._unitAttack);
        //    PoolManager.ReleaseObject(this.gameObject);
        //}
    }
}
