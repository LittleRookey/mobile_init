using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Events;
using DG.Tweening;


public class SA_UnitBase : MonoBehaviour
{
    [Header("References")]
    public SPUM_Prefabs _spumPrefab;
    public SA_AnimationAction _animAction;
    public SA_UnitSet _UnitSet;




    [Header("Necessary Settings")]
    public string _Name;
    public bool isPlayer;
    public bool showGizmos;
    public MonsterSetting _ms;
    public ObscuredInt _level = 1;
    public bool enableKnockBack;
    public float knockbackDuration = 0.1f;
    public float knockbackPower = 0.1f;


    [Header("Player Info")]
    public ObscuredInt ID;
    public SA_UnitBase _target;
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


    public UnityAction<SA_UnitBase> OnDeath;

    /*
     *  Protected fields
     */

    protected ObscuredFloat _findTimer;
    protected ObscuredFloat _attackTimer;
    protected ObscuredFloat _hpRegenTimer;

    protected Rigidbody2D rb;
    protected CircleCollider2D circleColl;

    protected Vector2 _dirVec;
    protected Vector2 _tempDist;

    public bool isAttacking;
    public bool canMove;
    protected bool isPatrolling;
    
    public int hitTargetID;

    protected Vector2 nPos;
    public Vector2 spawnedPos;


    protected GameObject destTarg;

    protected WaitForSeconds hitDuration = new WaitForSeconds(.2f);
    protected SpriteRenderer[] spriteParts;

    protected Material spriteMat;

    [SerializeField] protected float patrolTimer = 2f;
    protected float patrolTime = 0;
    protected float turnOffTime = 2f;


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

    protected virtual void Awake()
    {
        _animAction = _spumPrefab._anim.GetComponent<SA_AnimationAction>();
        rb = GetComponent<Rigidbody2D>();
        circleColl = GetComponent<CircleCollider2D>();
        if (spriteParts == null)
            spriteParts = _animAction.GetAllParts().ToArray();
        
        spriteMat = spriteParts[0].material;
    }

    protected virtual void Update()
    {
        HPRegen();
        CheckState();
        //FindTarget();
    }

    protected void HPRegen()
    {
        _hpRegenTimer += Time.deltaTime;
        if (_hpRegenTimer >= 1.0f && !isDead && _unitHP < _unitMaxHP)
        {
            if (_unitHP + _unitHPRegen > _unitMaxHP)
            {
                _unitHP = _unitMaxHP;

            }
            else
            {
                _unitHP += _unitHPRegen;
            }
            _hpRegenTimer = 0f;
            //Actions.OnHPChange?.Invoke(0f);
            _UnitSet.CalcHPState();
        }
    }

    protected void TurnOffCharacter(SA_UnitBase sa)
    {
        Invoke("DeathEffect", 1.5f);
        
        Invoke("TurnOff", turnOffTime);
    }

    void TurnOff()
    {
        _spumPrefab._anim.Rebind();
        PoolManager.ReleaseObject(this.gameObject);
    }





    protected virtual void CheckState()
    {
        
        switch (_unitState)
        {
            case UnitState.idle:
                //isPatrolling = false;
                _attackTimer += Time.deltaTime;
                FindTarget();
                if (_target == null)
                    isPatrolling = false;
                patrolTime += Time.deltaTime;
                if (patrolTime > patrolTimer)
                {
                    MoveToPatrol();
                    patrolTime = 0f;
                    patrolTimer = Random.Range(2f, 3f);
                }
                break;

            case UnitState.run:
                _attackTimer += Time.deltaTime;
                FindTarget();
                if (_target == null)
                    DoMove(nPos);
                else
                    DoMove(_target.transform.position);
                break;

            case UnitState.attack:
                //FindTarget();
                _attackTimer += Time.deltaTime;
                CheckAttack();
                break;
            case UnitState.patrol:
                Patrol();
                break;
            case UnitState.stun:
                break;

            case UnitState.skill:
                break;

            case UnitState.death:
                break;
        }
    }

    void MoveToPatrol()
    {
        if (_target == null)
        {
            SetState(UnitState.patrol);
            Debug.Log("Called invoke");
        }
    }

    protected virtual void SetState(UnitState state)
    {
        _unitState = state;
        switch (_unitState)
        {
            case UnitState.idle:
                //Debug.Log("Idle");
                canMove = false;
                _spumPrefab.PlayAnimation(AnimType.Idle);

                break;

            case UnitState.run:
                //Debug.Log("Run");
                _spumPrefab.PlayAnimation(AnimType.Run);
                break;

            case UnitState.attack:
                //Debug.Log("Attack");
                //CheckAttack();
                // When it arrive
                _spumPrefab._anim.SetFloat("AttackSpeed", _unitAttackSpeed);
                _spumPrefab.PlayAnimation(AnimType.Idle);

                //transform.Translate(_dirVec  * Time.deltaTime);
                break;

            case UnitState.stun:
                _spumPrefab.PlayAnimation(AnimType.Stun);
                break;

            case UnitState.skill:
                _spumPrefab.PlayAnimation(AnimType.Skill_S_V1);
                break;

            case UnitState.death:
                isDead = true;
                _spumPrefab.PlayAnimation(AnimType.Die);
                break;
            case UnitState.patrol:
                Patrol();
                break;
        }
    }

    protected virtual void FindTarget()
    {

        _findTimer += Time.deltaTime;

        if (_findTimer > SoonsoonData.Instance.SAM._findTimer)
        {

            //_target = SoonsoonData.Instance.SAM.GetTarget(this);
            _target = SoonsoonData.Instance.SAM.FindTargetOf(this);

            _findTimer = 0;
            if (_target != null)
            {
                //DoMove(_target.transform.position);
                float distToTarg = (_target.transform.position - transform.position).sqrMagnitude;
                if (distToTarg <= _unitFightRange)
                {
                    if (distToTarg <= _unitAttackRange)
                    {
                        //Debug.Log(name + " 111111111111");
                        SetState(UnitState.attack);
                    }
                    else
                    {
                        DoMove(_target.transform.position);

                    }
                }
            } 

        }


    }

    protected void CheckAttack()
    {
        //Debug.Log(name + " CheckAttack");
        if (!CheckTarget()) return;
        //Debug.Log("2");
        if (!CheckDistance()) return;
        //Debug.Log("Attack1");
        //_attackTimer += Time.deltaTime;
        if (_attackTimer > _unitAttackDelay && _target != null)
        {
            DoAttack();
            _attackTimer = 0;
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

    // assume target exists
    // if enemy is within attack range: true
    // else: false
    protected bool CheckDistance()
    {
        _tempDist = (Vector2)(_target.transform.position - transform.position);

        float tDis = _tempDist.sqrMagnitude;
        //Debug.Log(gameObject.name + ": " +tDis);
        //Vector2 tVec = (Vector2)(_target.transform.localPosition - transform.position);

        if (tDis <= _unitAttackRange * _unitAttackRange) // if enemy is within range
        {

            //SetState(UnitState.attack);
            return true;
        }
        else // if enemy is not within range
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

    protected void DoAttack()
    {
        if (_target == null)
            return;
        isAttacking = true;
        canMove = false;
        hitTargetID = _target.ID;
        _dirVec = (Vector2)(_target.transform.position - transform.position).normalized;
        SetDirection();

        switch (_ms._attackType)
        {
            case AttackType.sword:
                _spumPrefab.PlayAnimation(AnimType.Attack_S_V1);
                //StartCoroutine(GiveDamage());
                break;
            case AttackType.bow:
                _spumPrefab.PlayAnimation(AnimType.Attack_B_V1);
                break;
            case AttackType.magic:
                _spumPrefab.PlayAnimation(AnimType.Attack_M_V1);
                break;
        }
    }

    // called on animation event
    public void SetAttack()
    {
        if (_target == null) return;
        Debug.Log(name + ": 0");
        float dmg = StatManager.Instance.GetFinalPlayerAttack(_level);
        _target.SetDamage(this, dmg); // this gives dmg to target
        Debug.Log(name + ": 1");
    }

    // being called on enemy
    public virtual void SetDamage(SA_UnitBase owner, float dmg)
    {
        //// when player is dead and got hit
        //if (_unitState == UnitState.death)
        //    return;
        ////if ((owner.transform.position - transform.position).sqrMagnitude > owner._unitAttackRange) return;
        ////Debug.Log(hitTargetID + ": " + _target.ID);
        //if (owner.hitTargetID != ID) return;

        //float realdmg = dmg;
        //realdmg = Random.Range((int)(realdmg * 0.85f), (int)(realdmg * 1.15f));

        //// TODO crithit 

        //_unitHP -= realdmg;

        //// hp bar
        //if (_UnitSet == null) return;
        //_UnitSet.ShowDmgText(owner._ms._attackType, realdmg);
        //_UnitSet.CalcHPState();

        //StartCoroutine(HitEffect());
        //if (enableKnockBack)
        //    StartCoroutine(KnockBack(knockbackDuration, knockbackPower, owner.transform));
        //if (!isPlayer)
        //    Actions.OnEnemyHit?.Invoke(this);

        //if (_unitHP <= 0)
        //{
        //    SetDeath();
        //}
        //isAttacking = false;
    }

    protected virtual void SetDeath()
    {
        //_unitHP = 0;
        //_UnitSet.TurnOffHPBar(1f);
        ////TODO disable character
        //SetState(UnitState.death);
        //capsuleColl.enabled = false;
        //// loot
        //switch (gameObject.tag)
        //{
        //    case "Player":
        //        //SoonsoonData.Instance.SAM._playerList.Remove(this);

        //        break;
        //    case "Enemy":
        //        //SoonsoonData.Instance.SAM._enemyList.Remove(this);

        //        OnDeath?.Invoke(this);
        //        ItemDropManager.DropItems?.Invoke(this); // drop exp, gold visuals
        //        StatManager.OnEnemyDeath?.Invoke(this); // add exp
        //        UIManager.OnUpdateGold?.Invoke(this.dropGold);

        //        break;
        //}
    }

    public virtual void AttackDone(SA_Unit target = null) { }

    protected void SetDirection()
    {
        if (_dirVec.x >= 0)
        {
            _spumPrefab._anim.transform.localScale = new Vector3(-1 * Mathf.Abs(_spumPrefab._anim.transform.localScale.x), _spumPrefab._anim.transform.localScale.y, 1);
        }
        else
        {
            _spumPrefab._anim.transform.localScale = new Vector3(Mathf.Abs(_spumPrefab._anim.transform.localScale.x), _spumPrefab._anim.transform.localScale.y, 1);
        }
    }

    

    protected virtual void Patrol() 
    {
        if (!isPatrolling)
        {
            do
            {
                nPos = Random.insideUnitCircle * 3f;
            } while ((nPos - spawnedPos).sqrMagnitude > 3f);
            isPatrolling = true;
            _dirVec = (nPos - (Vector2)transform.position).normalized;
            SetDirection();
            //SpriteRenderer circle = Resources.Load<SpriteRenderer>("Equipments/Circle");
            //destTarg = Instantiate(circle.gameObject);
            //destTarg.transform.position = nPos;


        }


        SetState(UnitState.run);
        //Debug.Log(name + ": " + transform.position + " " + ((Vector3)nPos - transform.position).sqrMagnitude);
        if (canMove)
        {
            FindTarget();
            //Debug.Log(name + ": " + transform.position + " " + ((Vector3)nPos - transform.position).sqrMagnitude);
            if (((Vector3)nPos - transform.position).sqrMagnitude <= 0.1f)
            {
                canMove = false;
                isPatrolling = false;
                // when the unit arrives at the next position
                //Destroy(destTarg);
                SetState(UnitState.idle);

            }
            else
            {
                Debug.DrawRay(transform.position, ((Vector3)nPos - transform.position).normalized, Color.blue, 1f);
                //transform.DOMove(nPos, .5f);
                transform.position += (Vector3)_dirVec * _unitMoveSpeed * Time.deltaTime;

            }
        }

        if (_target != null)
        {
            //DoMove(nPos.transform.position);
        }
    }

    public IEnumerator KnockBack(float knockDur, float knockPow, Transform obj)
    {
        float timer = 0;

        Vector2 knockbackPos = transform.position + (transform.position - obj.position).normalized * knockbackPower;
        while (knockbackDuration > timer)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, knockbackPos, .5f);
        }
        yield return 0f;
    }

    protected IEnumerator HitEffect()
    {
        foreach (SpriteRenderer sp in spriteParts)
        {
            sp.material.SetFloat("_HitEffectBlend", 1f);
        }
        yield return hitDuration;
        foreach (SpriteRenderer sp in spriteParts)
        {
            sp.material.SetFloat("_HitEffectBlend", 0f);
        }
    }

    protected void DeathEffect()
    {
        foreach (SpriteRenderer sp in spriteParts)
        {
            //DOTween.To(() =>  targObject.transform.localScale.x, x => targObject.transform.localScale = new Vector3(x, 1f, 1f), tValue, dur);
            DOTween.To(() => sp.material.GetFloat("_FadeAmount"), x =>sp.material.SetFloat("_FadeAmount", x), 1f, 1f);
            //sp.material.SetFloat("_FadeAmount", 0f);
        }
        
    }

    protected void DoMove()
    {
        
        if (!CheckTarget()) return;

        // if target exists
        // and is within attack range
        if (CheckDistance())
        {
            Debug.Log(name + " 222222222222");
            SetState(UnitState.attack);
        }
        else // if target is not within attack range
        { // run to target

            _dirVec = _tempDist.normalized;
            SetDirection();
            SetState(UnitState.run);
            if (canMove)
            {
                transform.position += (Vector3)_dirVec * _unitMoveSpeed * Time.deltaTime;

            }
        }


    }
    bool isMovingToTarget = false;
    // moves to the given position 
    protected void DoMove(Vector3 dest)
    {
        //Debug.Log(gameObject.name +"'s target: "+ CheckTarget());
        // if target is not within attack range
        // run to target
        isMovingToTarget = true;
        _tempDist = dest - transform.position;

        _dirVec = _tempDist.normalized;
        SetDirection();
        //if (!isMovingToTarget)
        SetState(UnitState.run);
        if (canMove)
        {
            FindTarget();
            if (_target != null)
            {
                if ((_target.transform.position - transform.position).sqrMagnitude <= _unitAttackRange - .1f)
                {
                    isMovingToTarget = false;
                    canMove = false;
                    isPatrolling = false;
                    // when the unit arrives at the next position
                    //Destroy(destTarg);
                    //Debug.Log(name + " 3333333333333");
                    SetState(UnitState.attack);

                }
                else
                {
                    Debug.DrawRay(transform.position, ((Vector3)nPos - transform.position).normalized, Color.blue, .1f);
                    //transform.DOMove(nPos, .5f).SetEase(Ease.Linear);
                    transform.position += (Vector3)_dirVec * _unitMoveSpeed * Time.deltaTime;

                }

            } else
            {
                SetState(UnitState.patrol);
            }
            //Debug.Log(name + ": " + transform.position + " " + ((Vector3)nPos - transform.position).sqrMagnitude);
            //Debug.Log("Moving");

        }
    }


    public void AttackMissile()
    {
        switch (_ms._attackType)
        {
            case AttackType.bow:
                SoonsoonData.Instance.SAMM.FireMissile(SA_MissileObj.MissileType.arrow, this, _target);
                break;
            case AttackType.magic:
                SoonsoonData.Instance.SAMM.FireMissile(SA_MissileObj.MissileType.fireball, this, _target);
                break;
        }
    }

    public void SetDeathDone()
    {
        Destroy(gameObject);
    }

    //public void InitStat()
    //{
    //    _ms.Init(this);

    //}
}
