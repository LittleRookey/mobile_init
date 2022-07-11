using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.StatsSystem.StatsContainers;
using CleverCrow.Fluid.StatsSystem;
using UnityEngine.Events;

public class SA_Unit : MonoBehaviour
{

    public SPUM_Prefabs _spumPrefab;
    public SA_AnimationAction _animAction;
    public SA_UnitSet _UnitSet;

    public AttackType _attackType;
    List<Vector2> _moveList;

    public UnitState _unitState = UnitState.idle;

    public SA_Unit _target;

    [Header("Stats")]
    public int _level = 1;

    private int _exp;
    private int _maxExp;
    public StatsContainer _mStatContainer;



    public bool isPlayer;

    public float _unitMaxHP;
    public float _unitHP;
    public int _unitDefense;
    public float _unitMana;
    public float _unitAttack;

    public float _unitMagicForce; // amplifies skills by charge
    public float _unitAttackSpeed; 
    public float _unitAttackDelay; // delay time between every attack

    public float _unitAttackRange;
    public float _unitFightRange;

    public float _unitHPRegen;
    public float _unitManaRegen;

    public float _unitMoveSpeed;

    private float _findTimer;
    private float _attackTimer;
    public static float FINDRANGE;

    private int _statPoint = 0;

    Vector2 _dirVec;
    Vector2 _tempDist;


    public List<Talent> talents;

    
    public enum UnitState
    {
        idle, 
        run,
        attack,
        stun,
        skill,
        death
    }

    public enum AttackType
    {
        sword,
        bow,
        magic
    };

    private void OnValidate()
    {
#if UNITY_EDITOR
        switch(_attackType)
        {
            case AttackType.sword:
                _unitAttackRange = 1;
                break;
            case AttackType.bow:
                _unitAttackRange = 10;
                break;
            case AttackType.magic:
                _unitAttackRange = 10;
                break;
        }
#endif
    }
    private void Awake()
    {
        _animAction = _spumPrefab._anim.GetComponent<SA_AnimationAction>();
        
    }

    void Start()
    {
        _mStatContainer = SA_ResourceManager.Instance.GetCharacterStat();
        InitStat();
        _moveList = new List<Vector2>();
        //var callBacks = new UnityAction<StatRecord>((record) => Debug.Log(record));
        //_mStatContainer.OnStatChangeSubscribe(StatManager.ATTACK_ID, callBacks);
        if (isPlayer)
        {
            //StatManager.Instance.SubscribeEvents();
        }
        //_mStatContainer.
        _mStatContainer.SetModifier(OperatorType.Add, "attack", "modifierID", 100);
        //_mstatcontainer.onstatchangesubscribe(attack_id, callback);
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        
    }

    public int GetLeftStatPoint()
    {
        return _statPoint;
    }


    // 1. load basic stat 
    // 2. Add substat based on main stat 
    // 3. Save

    // Sets up the normal stat(lv 1) to the unit  
    public void InitStat()
    {
        _unitAttack = SA_ResourceManager.ATTACK;
        _unitMagicForce = SA_ResourceManager.MAGICFORCE;
        _unitHPRegen = SA_ResourceManager.HPREGEN;
        _unitDefense = SA_ResourceManager.DEFENSE;
        
        _unitManaRegen = SA_ResourceManager.MANAREGEN;
        _unitAttackSpeed = SA_ResourceManager.ATTACKSPEED;
        _unitAttackDelay = SA_ResourceManager.ATTACKDELAY;
        _unitMaxHP = SA_ResourceManager.HP;
        _unitMana = SA_ResourceManager.MANA;
        _unitMoveSpeed = SA_ResourceManager.MOVESPEED;
        _unitHP = _unitMaxHP;
        
        if (_attackType == AttackType.sword)
            _unitAttackRange = SA_ResourceManager.ATTACKRANGESWORD;
        else if(_attackType == AttackType.bow)
            _unitAttackRange = SA_ResourceManager.ATTACKRANGE_BOW;
        else if(_attackType == AttackType.magic)
            _unitAttackRange = SA_ResourceManager.ATTACKRANGE_MAGIC;
        _unitFightRange = 200;
        _level = 1;
        //_unitAttack = SA_ResourceManager.ATTACK;
        //_unitMagicAttack = SA_ResourceManager.MAGICATTACK;
        //_unitHPRegen = SA_ResourceManager.HPREGEN;
        //_unitDefense = SA_ResourceManager.DEFENSE;
        //_unitManaRegen = SA_ResourceManager.MANAREGEN;
        //_unitAttackSpeed = SA_ResourceManager.ATTACKSPEED;
        //_unitMaxHP = SA_ResourceManager.HP;
        //_unitMana = SA_ResourceManager.MANA;
        //_unitMoveSpeed = SA_ResourceManager.MOVESPEED;
        //_unitHP = _unitMaxHP;
    }

    // takes care of stat calculation
    //public void ApplyStats(int addStr, int addDex, int addInt, int addVit)
    //{
        //Debug.Log(addStr + " " + addDex + " " + addInt + " " + addVit);
        // TODO reset stat to default
        //if (addDex != 0) AddDexteritySubstats(addDex);
        //if (addStr != 0) AddStrengthSubstats(addStr);
        //if (addInt != 0) AddIntelligenceSubstats(addInt);
        //if (addVit != 0) AddVitalitySubstats(addVit);

    //}

    //public void AddDex(int val)
    //{
    //    _mStats._Dexterity += val;
    //    ApplyStats(0, val, 0, 0);
    //}

    //public void AddStr(int val)
    //{
    //    _mStats._Strength += val;
    //    ApplyStats(val, 0, 0, 0);
    //}
    //public void AddInt(int val)
    //{
    //    _mStats._Intelligence += val;
    //    ApplyStats(0, 0, val, 0);
    //}
    //public void AddVit(int val)
    //{
    //    _mStats._Vitality += val;
    //    ApplyStats(0, 0, 0, val);
    //}
    // updates stat from mstatcontainer to the variables here
    //void UpdateStats()
    //{
    //    //_ _mStats._Strength;
    //    //_mStats._Dexterity;
    //    //_mStats._Intelligence;
    //    //_mStats._Intelligence;
    //}
    //public void LevelUpStats()
    //{
    //    _mSubStat._MaxHP += 8;
    //    _mSubStat._Attack += 3;
    //    _mSubStat._Defense += 2;
    //    _mSubStat._MagicDefense += 1;
    //    _mSubStat._MagicAttack += 3;
    //    _mSubStat._HealthRegen += 0.1f;
    //    _mSubStat._HealthRegen = StatManager.Round(_mSubStat._HealthRegen, 1);

    //    //_UnitSet._UnitSubset._levelText.text = _level.ToString();
    //}
    //public void AddDexteritySubstats(int num)
    //{
    //    for (int i = 0; i < num; i++)
    //    {
    //        _mSubStat._Attack += StatManager.DEX_ATTACKGROW;
    //        _mSubStat._AttackSpeed -= StatManager.DEX_ATTACKSPEEDGROW;
    //        _mSubStat._MoveSpeed += StatManager.DEX_MOVESPEEDGROW;
    //    }
    //}

    //public void AddStrengthSubstats(int num)
    //{
    //    for (int i = 0; i < num; i++)
    //    {
    //        _mSubStat._Attack += StatManager.STR_ATTACKGROW;
    //        _mSubStat._MaxHP += StatManager.STR_MAXHPGROW;
    //    }
    //}

    //public void AddIntelligenceSubstats(int num)
    //{
    //    for (int i = 0; i < num; i++)
    //    {
    //        _mSubStat._MagicAttack += StatManager.INT_MAGICATTACKGROW;
    //        _mSubStat._Mana += StatManager.INT_MANAGROW;
    //        _mSubStat._ManaRegen += StatManager.INT_MANAGENGROW;
    //    }
    //}

    //public void AddVitalitySubstats(int num)
    //{
    //    for (int i = 0; i < num; i++)
    //    {
    //        _mSubStat._MaxHP += StatManager.VIT_MAXHPGROW;
    //        _mSubStat._HealthRegen += StatManager.VIT_HPGENGROW;
    //        _mSubStat._Defense += StatManager.VIT_DEFENSEGROW;
    //        _mSubStat._MagicDefense += StatManager.VIT_MAGICDEFENSEGROW;
    //    }
    //}



    void CheckState()
    {
        switch(_unitState)
        {
            case UnitState.idle:
                FindTarget();
                break;

            case UnitState.run:
                FindTarget();
                DoMove();
                break;

            case UnitState.attack:
                FindTarget();
                CheckAttack();
                break;

            case UnitState.stun:
                break;

            case UnitState.skill:
                break;

            case UnitState.death:
                break;
        }
    }

    void SetState(UnitState state)
    {
        _unitState = state;
        switch (_unitState)
        {
            case UnitState.idle:
                //Debug.Log("Idle");
                _spumPrefab.PlayAnimation(0);

                break;

            case UnitState.run:
                //Debug.Log("Run");
                _spumPrefab.PlayAnimation(1);
                break;

            case UnitState.attack:
                //Debug.Log("Attack");
                //CheckAttack();
                // When it arrive
                _spumPrefab._anim.SetFloat("AttackSpeed", _unitAttackSpeed);
                _spumPrefab.PlayAnimation(0);
                break;

            case UnitState.stun:
                _spumPrefab.PlayAnimation(3);
                break;

            case UnitState.skill:
                _spumPrefab.PlayAnimation(7);
                break;

            case UnitState.death:
                _spumPrefab.PlayAnimation(2);
                break;
        }
    }

    void FindTarget()
    {
        
        _findTimer += Time.deltaTime;

        if (_findTimer > SoonsoonData.Instance.SAM._findTimer)
        {
            
            _target = SoonsoonData.Instance.SAM.GetTarget(this);
            if (_target != null)
            {

                SetState(UnitState.run);
                
            } else
            {
                Debug.Log("idle from FindTarget");
                SetState(UnitState.idle);
            }
            _findTimer = 0;
            //SA_Unit tTarget = SoonsoonData.Instance.SAM.GetTarget(this);
            //if (tTarget != null)
            //{
            //    if (_target == null)
            //    {
            //        _target = tTarget;
            //    }
            //    else if (_target != tTarget) Findway();
            //    else SetState(UnitState.idle);
            //    _findTimer = 0;
            //}
        }


    }

    void DoMove()
    {
        //Debug.Log(gameObject.name +"'s target: "+ CheckTarget());
        if (!CheckTarget()) return;

        // if target exists
        // and is within attack range
        if(CheckDistance())
        {

            SetState(UnitState.attack);
        } 
        else // if target is not within attack range
        { // run to target

            _dirVec = _tempDist.normalized;
            SetDirection();
            SetState(UnitState.run);
            transform.position += (Vector3)_dirVec * _unitMoveSpeed * Time.deltaTime;
        }

        
    }

    // if enemy is within attack range: true
    // else: false
    bool CheckDistance()
    {
        _tempDist = (Vector2)(_target.transform.position - transform.position);

        float tDis = _tempDist.sqrMagnitude;
        //Debug.Log(gameObject.name + ": " +tDis);
        //Vector2 tVec = (Vector2)(_target.transform.localPosition - transform.position);

        if (tDis <= _unitAttackRange * _unitAttackRange) // if enemy is within range
        {
            
            SetState(UnitState.attack);
            return true;
        } else // if enemy is not within range
        {
            if (!CheckTarget())
            {
                Debug.Log("idle from CheckDistance");
                SetState(UnitState.idle); // if target doesn't exist
            }
            else SetState(UnitState.run); // if target exist
            return false;
        }
    }
    // Checks if target exists
    // true if target exists
    // false if target is null
    bool CheckTarget()
    {
        bool val = true;
        if (_target == null) val = false;
        if (_target._unitState == UnitState.death) val = false;
        if (_target.gameObject == null) val = false;
        if (!_target.gameObject.activeInHierarchy) val = false;
        //Debug.Log(_target.gameObject.name + "");
        // target still exists and is alive
        if (!val)// if target doesnt exist
        {
            SetState(UnitState.idle);
        } 
        //Debug.Log("idle from checkTarget");
        //SetState(UnitState.idle);
        
        return val;
    }

    void CheckAttack()
    {
        if (!CheckTarget()) return;
        //Debug.Log("2");
        if (!CheckDistance()) return;
        //Debug.Log("Attack1");
        _attackTimer += Time.deltaTime;
        if (_attackTimer > _unitAttackDelay)
        {
            DoAttack();
            _attackTimer = 0;
        }
    }
    //IEnumerator GiveDamage()
    //{
    //    yield return new WaitForSeconds(0.15f);
    //    SetAttack();
    //}

    void DoAttack()
    {
        Debug.Log("AttackAnimationplayed");
        _dirVec = (Vector2)(_target.transform.position - transform.position).normalized;
        SetDirection();
        switch(_attackType)
        {
            case AttackType.sword: 
                _spumPrefab.PlayAnimation(4);
                //StartCoroutine(GiveDamage());
                break;
            case AttackType.bow:
                _spumPrefab.PlayAnimation(5);
                break;
            case AttackType.magic:
                _spumPrefab.PlayAnimation(6);
                break;
        }
    }

    // called on animation event
    public void SetAttack()
    {
        if (_target == null) return;
        float dmg = _unitAttack;
        _target.SetDamage(this, dmg); // this gives dmg to target
    }

    public void SetDamage(SA_Unit owner, float dmg)
    {
        _unitHP -= dmg;
        // hp bar
        if (_UnitSet == null) return;
        Debug.Log(owner.name + ": " + owner._attackType + '\n' + "target " + name + ": " + _attackType);
        _UnitSet.ShowDmgText(owner._attackType, dmg);
        _UnitSet.CalcHPState();

        if (_unitHP <= 0)
        {
            SetDeath();
        }
    }

    void SetDeath()
    {
        switch(gameObject.tag)
        {
            case "P1":
                SoonsoonData.Instance.SAM._p1UnitList.Remove(this);
                break;
            case "P2":
                SoonsoonData.Instance.SAM._p2UnitList.Remove(this);
                break;
        }
        _unitHP = 0;
        SetState(UnitState.death);
    }

    public void SetDeathDone()
    {
        Destroy(gameObject);
    }

    public void AttackMissile()
    {
        switch(_attackType)
        {
            case AttackType.bow:
                SoonsoonData.Instance.SAMM.FireMissile(SA_MissileObj.MissileType.arrow, this, _target);
                break;
            case AttackType.magic:
                SoonsoonData.Instance.SAMM.FireMissile(SA_MissileObj.MissileType.fireball, this, _target);
                break;
        }
    }

    public void AttackDone(SA_Unit target=null, float damage=0)
    {
        float dmg = _unitAttack;

        if (target == null)
        {
            if (damage != 0) _target.SetDamage(this, damage);
            else _target.SetDamage(this, dmg);
        } 
        else
        {
            if (damage != 0) target.SetDamage(this, damage);
            else target.SetDamage(this, dmg);
        }
    }
    void SetDirection()
    {
        if (_dirVec.x >= 0)
        {
            _spumPrefab._anim.transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            _spumPrefab._anim.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void Findway()
    {
        _moveList.Clear();
        Vector2 tPos = transform.position;

        float tmpValue = 0;
        bool check = true;

        for (int j = 0; j < 10; j++) 
        {
            int tmpInt = (Random.Range(0, 2) == 1) ? 1 : -1;
            bool check2 = true;

            for (int i = 0; i < 4; i++)
            {
                _tempDist = (Vector2)_target.transform.position - tPos;
                Vector2 tDirVec = _tempDist.normalized;
                if (check && check2)
                {
                    tmpValue = i * tmpInt * 90f;
                    Vector3 ttDirVec = (Quaternion.AngleAxis(tmpValue, Vector3.forward) * tDirVec);
                    RaycastHit2D hit = Physics2D.CircleCast(tPos + new Vector2(0, 0.5f), 0.125f, ttDirVec, 1f);
                    //Gizmos.Draw(tPos + new Vector2(0, 0.5f), 0.125f, ttDirVec, 1f);
                    if (hit.collider == null)
                    {
                        tDirVec = (Vector2)ttDirVec;
                        tPos += (Vector2)ttDirVec;
                        if (!_moveList.Contains(tPos))
                        {
                            _moveList.Add(tPos);
                            check2 = false;
                        }
                    }
                    else if (hit.collider.CompareTag(_target.tag))
                    {
                        _target = hit.collider.GetComponent<SA_Unit>();
                        tDirVec = (Vector2)ttDirVec;
                        tPos += (Vector2)ttDirVec;
                        if (!_moveList.Contains(tPos))
                        {
                            _moveList.Add(tPos);
                            check2 = false;
                        }
                    }
                    else if (hit.collider.CompareTag(tag))
                    {
                        SA_Unit tUnit = hit.collider.GetComponent<SA_Unit>();
                        if (tUnit._unitState != UnitState.attack)
                        {
                            tDirVec = (Vector2)ttDirVec;
                            tPos += (Vector2)ttDirVec;
                            if (!_moveList.Contains(tPos))
                            {
                                _moveList.Add(tPos);
                                check2 = false;
                            }
                        }
                    }
                }
            }
        }
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tTag = "";
        switch (gameObject.tag)
        {
            case "P1":
                tTag = "P2";
                break;
            case "P2":
                tTag = "P1";
                break;
        }
        if (collision.gameObject.CompareTag(tTag))
        {
            Debug.Log("with Target");
        }
        else if (collision.gameObject.CompareTag(gameObject.tag))
        {
            Debug.Log("Stop");
            //SetState(UnitState.idle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _unitFightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _unitAttackRange);
    }
}
