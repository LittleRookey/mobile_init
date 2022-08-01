using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_Player : SA_UnitBase
{
    public bool useJoyStick;
    public bool isMoving;

    [SerializeField] private MovementJoystick movementJoyStick;

    private Vector2 moveVec;


    protected override void Awake()
    {
        base.Awake();
        if (movementJoyStick == null)
            movementJoyStick = FindObjectOfType<MovementJoystick>();
    }


    private void OnEnable()
    {
        Debug.Log("player");


        SetState(UnitState.idle);
        
        enableKnockBack = false;
        isDead = false;
        circleColl.enabled = true;

        OnDeath += TurnOffCharacter;
        if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Player");
        Debug.Log(StatManager.Instance == null);
        knockbackPower = 0.1f;
        knockbackDuration = 0.1f;
        StatManager.Instance.InitPlayer(this);

        
        _attackTimer = _unitAttackDelay;
    }

    private void OnDisable()
    {
        OnDeath -= TurnOffCharacter;

        StatManager.Instance.InitPlayer(this);
        
        //PoolManager.ReleaseObject(this.gameObject);   
    }

    private void Start()
    {
        if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Player");
        Debug.Log(StatManager.Instance == null);
        knockbackPower = 0.1f;
        knockbackDuration = 0.1f;
        StatManager.Instance.InitPlayer(this);
    }

    protected override void CheckState()
    {
        switch (_unitState)
        {
            case UnitState.idle:
                _attackTimer += Time.deltaTime;
                FindTarget();
                //SetState(UnitState.idle);

                break;

            case UnitState.run:
                _attackTimer += Time.deltaTime;
                if (!isMoving)
                {
                    FindTarget();
                    if (_target != null)
                        DoMove(_target.transform.position);
                }
                break;

            case UnitState.attack:
                //FindTarget();
                _attackTimer += Time.deltaTime;
                if (!isMoving)
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

    public override void AttackDone(SA_Unit target = null)
    {

        float dmg;
        dmg = StatManager.Instance.GetFinalPlayerAttack(_level);

        if (target == null)
        {
            //Debug.Log(name + ": 2");
            _target.SetDamage(this, dmg);
        }
        else
        {
            //Debug.Log(name + ": 3");
            target.SetDamage(this, dmg);
        }

    }


    

    protected override void SetDeath()
    {
        _unitHP = 0;
        _UnitSet.TurnOffHPBar(1f);
        //TODO disable character
        SetState(UnitState.death);
        circleColl.enabled = false;
        // loot
        // TODO Auto revive window popup

    }

    public override void SetDamage(SA_UnitBase owner, float dmg)
    {
        // when player is dead and got hit
        if (_unitState == UnitState.death)
            return;

        //if ((owner.transform.position - transform.position).sqrMagnitude > owner._unitAttackRange) return;
        //Debug.Log(hitTargetID + ": " + _target.ID);
        if (owner.hitTargetID != ID) return;

        float realdmg = dmg;
        realdmg = Random.Range((int)(realdmg * 0.85f), (int)(realdmg * 1.15f));

        // TODO crithit 

        _unitHP -= realdmg;

        // hp bar
        if (_UnitSet == null) return;
        _UnitSet.ShowDmgText(owner._ms._attackType, realdmg);
        _UnitSet.CalcHPState();

        StartCoroutine(HitEffect());
        if (enableKnockBack)
            StartCoroutine(KnockBack(knockbackDuration, knockbackPower, owner.transform));

        if (_unitHP <= 0)
        {
            SetDeath();
        }
        //isAttacking = false;
    }


    protected override void SetState(UnitState state)
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
                SetState(UnitState.idle);
                break;
        }
    }

    protected void SetDirection(float val)
    {
        if (val >= 0)
        {
            _spumPrefab._anim.transform.localScale = new Vector3(-1 * Mathf.Abs(_spumPrefab._anim.transform.localScale.x), _spumPrefab._anim.transform.localScale.y, 1);
        }
        else
        {
            _spumPrefab._anim.transform.localScale = new Vector3(Mathf.Abs(_spumPrefab._anim.transform.localScale.x), _spumPrefab._anim.transform.localScale.y, 1);
        }
    }

    private void FixedUpdate()
    {
        if (useJoyStick && !isAttacking)
        {
            if (movementJoyStick.joystickVec.x != 0)
            {
                SetState(UnitState.run);
                isMoving = true;
                moveVec.Set(movementJoyStick.joystickVec.x * _unitMoveSpeed, movementJoyStick.joystickVec.y * _unitMoveSpeed);
                rb.velocity = moveVec;
                SetDirection(moveVec.x);
            }  
            else
            {
                isMoving = false;
                rb.velocity = Vector2.zero;
                if (_target == null)
                    SetState(UnitState.idle);
            }
        }
    }
    protected override void Update()
    {
        HPRegen();
        //if (!useJoyStick)
        CheckState();
    }
}
