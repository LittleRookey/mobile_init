using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_Player : SA_UnitBase
{

    private void OnEnable()
    {
        Debug.Log("player");


        SetState(UnitState.idle);
        
        enableKnockBack = false;
        isDead = false;
        capsuleColl.enabled = true;

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
        capsuleColl.enabled = false;
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
        isAttacking = false;
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
}
