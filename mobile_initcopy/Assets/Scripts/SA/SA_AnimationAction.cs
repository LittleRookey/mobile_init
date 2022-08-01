using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_AnimationAction : MonoBehaviour
{
    public  SA_UnitBase _player;


    private void OnEnable()
    {
        if (_player == null)
        {
            _player = transform.parent.GetComponent<SA_Unit>();
        }
    }
    public void AttackDone()
    {
        //Debug.Log("Attack Done");
        switch(_player._ms._attackType)
        {
            case SA_Unit.AttackType.sword:
                if (_player._target != null) _player.AttackDone();
                break;

            case SA_Unit.AttackType.bow:
                if (_player._target != null)
                        _player.AttackMissile();
                break;

            case SA_Unit.AttackType.magic:
                if (_player._target != null)
                        _player.AttackMissile();
                break;
        }
    }

    public void SwordSlashV1()
    {
        EffectManager.OnPlayerAttackV1?.Invoke(_player);
    }

    public SpriteRenderer[] GetAllParts()
    {
        return GetComponentsInChildren<SpriteRenderer>();
    }

    public void AttackFinish()
    {
        _player.isAttacking = false;
    }

    public void CanMove()
    {
        _player.canMove = true;
        _player.isAttacking = false;
    }
    public void DeathDone()
    {
        _player.SetDeathDone();   
    }
}
