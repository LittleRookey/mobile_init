using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;

public class SA_Unit : SA_UnitBase
{
    public bool isElite;
    public bool isBoss;

    public ObscuredInt dropExp;
    public ObscuredInt dropGold;

    //Vector2 _dirVec;
    //Vector2 _tempDist;
    public List<Talent> talents;



    

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
        //if (isPlayer)
        //{
        //    if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Player");
        //    Debug.Log(StatManager.Instance == null);
        //    knockbackPower = 0.1f;
        //    knockbackDuration = 0.1f;
        //    StatManager.Instance.InitPlayer(this);

        //}
    }

    private void OnEnable()
    {
        enableKnockBack = false;
        SetState(UnitState.idle);
        isDead = false;
        OnDeath += TurnOffCharacter;
        circleColl.enabled = true;
        
            
        if (_ms == null) _ms = Resources.Load<MonsterSetting>("MonsterSettings/Monster");
        _ms.Init(this);
        knockbackPower = 0.4f;
        knockbackDuration = 0.1f;
        SetState(UnitState.patrol);

        
        
        _attackTimer = _unitAttackDelay;
    }


    private void OnDisable()
    {
        OnDeath -= TurnOffCharacter;
        
        _ms.Init(this);

        
        //PoolManager.ReleaseObject(this.gameObject);   
    }

    // Update is called once per frame
    


    public void Setup(MonsterSetting monst)
    {
        
    }

    // Give reward to the given target
    public void GiveReward(SA_Unit enem)
    {
        _exp += enem.dropExp;

        
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
                Patrol();
                break;
        }
    }

    //protected override void Patrol()
    //{
    //    //if (isPatrolling) return;
    //    //Debug.Log("Patrolling");
    //    if (!isPatrolling)
    //    {
    //        do
    //        {
    //            nPos = Random.insideUnitCircle * 3f;
    //        } while ((nPos - spawnedPos).sqrMagnitude > 3f);
    //        isPatrolling = true;
    //        _dirVec = (nPos - (Vector2)transform.position).normalized;
    //        SetDirection();
    //        SpriteRenderer circle = Resources.Load<SpriteRenderer>("Equipments/Circle");
    //        destTarg = Instantiate(circle.gameObject);
    //        destTarg.transform.position = nPos;


    //    }


    //    SetState(UnitState.run);
    //    //Debug.Log(name + ": " + transform.position + " " + ((Vector3)nPos - transform.position).sqrMagnitude);
    //    if (canMove)
    //    {
    //        FindTarget();
    //        //Debug.Log(name + ": " + transform.position + " " + ((Vector3)nPos - transform.position).sqrMagnitude);
    //        if (((Vector3)nPos - transform.position).sqrMagnitude <= 0.1f)
    //        {
    //            canMove = false;
    //            isPatrolling = false;
    //            // when the unit arrives at the next position
    //            Destroy(destTarg);
    //            SetState(UnitState.idle);

    //        }
    //        else
    //        {
    //            //Debug.DrawRay(transform.position, ((Vector3)nPos- transform.position).normalized, Color.blue, 1f);
    //            //transform.DOMove(nPos, .5f);
    //            //transform.position += (Vector3)_dirVec * _unitMoveSpeed * Time.deltaTime;

    //        }
    //    }

    //    if (_target != null)
    //    {
    //        //DoMove(nPos.transform.position);
    //    }

    //}

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
        //Debug.Log(owner.name + ": " + owner._attackType + '\n' + "target " + name + ": " + _attackType);
        _UnitSet.ShowDmgText(owner._ms._attackType, realdmg);
        _UnitSet.CalcHPState();

        StartCoroutine(HitEffect());
        if (enableKnockBack)
            StartCoroutine(KnockBack(knockbackDuration, knockbackPower, owner.transform));
        
        Actions.OnEnemyHit?.Invoke(this);

        if (_unitHP <= 0)
        {
            SetDeath();
        }
        isAttacking = false;
    }

    void PatrolReset()
    {
        isPatrolling = false;
    }


    protected override void SetDeath()
    {
        _unitHP = 0;
        _UnitSet.TurnOffHPBar(1f);
        //TODO disable character
        SetState(UnitState.death);
        circleColl.enabled = false;
        // loot
        

        OnDeath?.Invoke(this);
        ItemDropManager.DropItems?.Invoke(this); // drop exp, gold visuals
        StatManager.OnEnemyDeath?.Invoke(this); // add exp
        UIManager.OnUpdateGold?.Invoke(this.dropGold);

        
    }



    



    

    //public void AttackDone(SA_Unit target=null)
    //{
    //    //Debug.Log(name + ": 1-2");
    //    float dmg;
    //    if (isPlayer)
    //        dmg = StatManager.Instance.GetFinalPlayerAttack(_level);
    //    else
    //        dmg = _ms.CalcMonsterAttack(_level);
    //    //Debug.Log(name + ": 1-3");
    //    if (target == null)
    //    {
    //        //Debug.Log(name + ": 2");
    //        _target.SetDamage(this, dmg);
    //    } 
    //    else
    //    {
    //        //Debug.Log(name + ": 3");
    //        target.SetDamage(this, dmg);
    //    }
    //}

    public override void AttackDone(SA_Unit target = null)
    {

        float dmg;
        dmg = _ms.CalcMonsterAttack(_level);

        if (target == null)
        {
            _target.SetDamage(this, dmg);
        }
        else
        {
            //Debug.Log(name + ": 3");
            target.SetDamage(this, dmg);
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
            //Debug.Log("Stop");
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
