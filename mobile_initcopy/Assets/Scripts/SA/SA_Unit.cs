using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;


public class SA_Unit : MonoBehaviour
{
    [Header("References")]
    public SPUM_Prefabs _spumPrefab;
    public SA_AnimationAction _animAction;
    public SA_UnitSet _UnitSet;


    //List<Vector2> _moveList;
    



    [Header("Necessary Settings")]
    public string _Name;
    public bool isPlayer;
    public bool showGizmos;
    public MonsterSetting _ms;
    public ObscuredInt _level = 1;

    [Header("Player Info")]
    public ObscuredInt ID;
    public SA_Unit _target;
    public ObscuredInt _exp;
    public ObscuredInt _maxExp;
    public ObscuredInt _statPoint = 0;
    public UnitState _unitState = UnitState.idle;
    public bool isDead;




    [Header("Stats")]
    public ObscuredFloat _unitMaxHP;
    public ObscuredFloat _unitHP;
    public ObscuredInt _unitDefense;
    public ObscuredFloat _unitMana;
    public ObscuredInt _unitAttack;

    public ObscuredFloat _unitMagicForce; // amplifies skills by charge
    public ObscuredFloat _unitAttackSpeed; 
    public ObscuredFloat _unitAttackDelay; // delay time between every attack

    public ObscuredFloat _unitAttackRange;
    public ObscuredFloat _unitFightRange;

    public ObscuredFloat _unitHPRegen;
    public ObscuredFloat _unitManaRegen;

    public ObscuredFloat _unitMoveSpeed;

    private ObscuredFloat _findTimer;
    private ObscuredFloat _attackTimer;
    private ObscuredFloat _hpRegenTimer;

    public UnityAction<SA_Unit> OnDeath;

    public ObscuredInt dropExp;

    Vector2 _dirVec;
    Vector2 _tempDist;

    public bool isAttacking;
    public bool canMove;

    public List<Talent> talents;


    Vector2 nPos;
    public enum UnitState
    {
        idle, 
        patrol,
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

//    private void OnValidate()
//    {
//#if UNITY_EDITOR
//        switch(_ms._attackType)
//        {
//            case AttackType.sword:
//                _unitAttackRange = 1;
//                break;
//            case AttackType.bow:
//                _unitAttackRange = 10;
//                break;
//            case AttackType.magic:
//                _unitAttackRange = 10;
//                break;
//        }
//#endif
//    }
    private void Awake()
    {
        _animAction = _spumPrefab._anim.GetComponent<SA_AnimationAction>();
        
    }

    void Start()
    {
        //if (!isPlayer)
        //{
        //    if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Monster");
        //    _ms.Init(this);

        //}
        //else if (isPlayer)
        //{
        //    if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Player");
        //    StatManager.Instance.InitPlayer(this);
        //}

    }

    private void OnEnable()
    {
        SetState(UnitState.idle);
        isDead = false;
        OnDeath += TurnOffCharacter;
        if (!isPlayer)
        {
            Debug.Log("Not player");
            if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Monster");
            _ms.Init(this);
            SetState(UnitState.idle);

        }
        else if (isPlayer)
        {
            Debug.Log("player");
            if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Player");
            Debug.Log(StatManager.Instance == null);
            StatManager.Instance.InitPlayer(this);
        }
    }


    private void OnDisable()
    {
        OnDeath -= TurnOffCharacter;
        if (!isPlayer)
        {

            _ms.Init(this);

        }
        else if (isPlayer)
        {
            StatManager.Instance.InitPlayer(this);
        }
        //PoolManager.ReleaseObject(this.gameObject);   
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        HPRegen();
    }

    private void HPRegen()
    {
        _hpRegenTimer += Time.deltaTime;
        if (_hpRegenTimer >= 1.0f && !isDead && _unitHP < _unitMaxHP)
        {
            if (_unitHP + _unitHPRegen > _unitMaxHP)
            {
                _unitHP = _unitMaxHP;

            } else
            {
                _unitHP += _unitHPRegen;
            }
            _hpRegenTimer = 0f;
            Actions.OnHPChange?.Invoke(0f);
        }
    }
    public void Setup(MonsterSetting monst)
    {
        
    }

    // Give reward to the given target
    public void GiveReward(SA_Unit enem)
    {
        _exp += enem.dropExp;

        
    }

    public void TurnOffCharacter(SA_Unit sa)
    {
        Invoke("TurnOff", 2f);
    }

    void TurnOff()
    {
        PoolManager.ReleaseObject(this.gameObject);
    }
    public int GetLeftStatPoint()
    {
        return _statPoint;
    }


    // 1. load basic stat 
    // 2. Add substat based on main stat 
    // 3. Save
    // FOR MONSTER
    // Sets up the normal stat(lv 1) to the unit  
    public void InitStat()
    {
        _ms.Init(this);
      
    }

   
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
                //FindTarget();
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
                isDead = true;
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
            if (_target != null && !isAttacking)
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

    //public void Patrol()
    //{
    //    if (isPatrolling)
    //    {
    //        if ((nPos - (Vector2)transform.position).sqrMagnitude < 0.1f)
    //        {
    //            canMove = false;
    //            SetState(UnitState.idle);
    //            Invoke("PatrolReset", 2f); 

    //        } else
    //        {
    //            _spumPrefab.PlayAnimation(1);
    //            if (canMove)
    //                transform.position += (Vector3)_dirVec * _unitMoveSpeed * Time.deltaTime;
    //        }
    //    } 
    //    else
    //    {
    //        isPatrolling = true;
    //        nPos = Random.insideUnitCircle * 1.5f;
    //        _dirVec = (nPos - (Vector2)transform.position).normalized;
    //        SetDirection();
    //    }
        
        
        
    //}

    //void PatrolReset()
    //{
    //    isPatrolling = false;
    //}
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
            if (canMove)
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
        if (_target == null) return false;
        if (_target._unitState == UnitState.death) val = false;
        if (_target.gameObject == null) val = false;
        if (!_target.gameObject.activeInHierarchy) val = false;

        // target still exists and is alive
        if (!val)// if target doesnt exist
        {
            SetState(UnitState.idle);
        } 

        
        return val;
    }

    void CheckAttack()
    {
        //Debug.Log(name + " CheckAttack");
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


    void DoAttack()
    {
        isAttacking = true;
        canMove = false;
        //Debug.Log("AttackAnimationplayed");
        _dirVec = (Vector2)(_target.transform.position - transform.position).normalized;
        SetDirection();

        switch(_ms._attackType)
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
        float dmg = StatManager.Instance.GetFinalPlayerAttack(_level);
        _target.SetDamage(this, dmg); // this gives dmg to target
    }

    public void SetDamage(SA_Unit owner, float dmg)
    {
        // when player is dead and got hit
        if (_unitState == UnitState.death)
            return;


        float realdmg = dmg;
        realdmg = Random.Range((int)(realdmg * 0.85f), (int)(realdmg * 1.15f));
        
        // TODO crithit 

        _unitHP -= realdmg;
        // hp bar
        if (_UnitSet == null) return;
        //Debug.Log(owner.name + ": " + owner._attackType + '\n' + "target " + name + ": " + _attackType);
        _UnitSet.ShowDmgText(owner._ms._attackType, realdmg);
        _UnitSet.CalcHPState();
        isAttacking = false;

        if (_unitHP <= 0)
        {
            SetDeath();
        }
    }

    void SetDeath()
    {
        _unitHP = 0;
        _UnitSet.TurnOffHPBar(1f);
        //TODO disable character
        SetState(UnitState.death);
        // loot
        switch(gameObject.tag)
        {
            case "Player":
                //SoonsoonData.Instance.SAM._playerList.Remove(this);

                break;
            case "Enemy":
                //SoonsoonData.Instance.SAM._enemyList.Remove(this);
                OnDeath?.Invoke(this);
                StatManager.OnEnemyDeath?.Invoke(this);
                ItemDropManager.DropItems?.Invoke(this);
                break;
        }
    }

    public void SetDeathDone()
    {
        Destroy(gameObject);
    }

    public void AttackMissile()
    {
        switch(_ms._attackType)
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
            _spumPrefab._anim.transform.localScale = new Vector3(-1 * Mathf.Abs(_spumPrefab._anim.transform.localScale.x), _spumPrefab._anim.transform.localScale.y, 1);
        } else
        {
            _spumPrefab._anim.transform.localScale = new Vector3(Mathf.Abs(_spumPrefab._anim.transform.localScale.x), _spumPrefab._anim.transform.localScale.y, 1);
        }
    }

    //void Findway()
    //{
    //    _moveList.Clear();
    //    Vector2 tPos = transform.position;

    //    float tmpValue = 0;
    //    bool check = true;

    //    for (int j = 0; j < 10; j++) 
    //    {
    //        int tmpInt = (Random.Range(0, 2) == 1) ? 1 : -1;
    //        bool check2 = true;

    //        for (int i = 0; i < 4; i++)
    //        {
    //            _tempDist = (Vector2)_target.transform.position - tPos;
    //            Vector2 tDirVec = _tempDist.normalized;
    //            if (check && check2)
    //            {
    //                tmpValue = i * tmpInt * 90f;
    //                Vector3 ttDirVec = (Quaternion.AngleAxis(tmpValue, Vector3.forward) * tDirVec);
    //                RaycastHit2D hit = Physics2D.CircleCast(tPos + new Vector2(0, 0.5f), 0.125f, ttDirVec, 1f);
    //                //Gizmos.Draw(tPos + new Vector2(0, 0.5f), 0.125f, ttDirVec, 1f);
    //                if (hit.collider == null)
    //                {
    //                    tDirVec = (Vector2)ttDirVec;
    //                    tPos += (Vector2)ttDirVec;
    //                    if (!_moveList.Contains(tPos))
    //                    {
    //                        _moveList.Add(tPos);
    //                        check2 = false;
    //                    }
    //                }
    //                else if (hit.collider.CompareTag(_target.tag))
    //                {
    //                    _target = hit.collider.GetComponent<SA_Unit>();
    //                    tDirVec = (Vector2)ttDirVec;
    //                    tPos += (Vector2)ttDirVec;
    //                    if (!_moveList.Contains(tPos))
    //                    {
    //                        _moveList.Add(tPos);
    //                        check2 = false;
    //                    }
    //                }
    //                else if (hit.collider.CompareTag(tag))
    //                {
    //                    SA_Unit tUnit = hit.collider.GetComponent<SA_Unit>();
    //                    if (tUnit._unitState != UnitState.attack)
    //                    {
    //                        tDirVec = (Vector2)ttDirVec;
    //                        tPos += (Vector2)ttDirVec;
    //                        if (!_moveList.Contains(tPos))
    //                        {
    //                            _moveList.Add(tPos);
    //                            check2 = false;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tTag = "";
        switch (gameObject.tag)
        {
            case "Player":
                tTag = "Enemy";
                break;
            case "Enemy":
                tTag = "Player";
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
        if (showGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, _unitFightRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _unitAttackRange);
        }
    }
}
