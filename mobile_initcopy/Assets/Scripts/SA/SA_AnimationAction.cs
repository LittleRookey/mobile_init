using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_AnimationAction : MonoBehaviour
{
    public  SA_Unit _player;

    
    private void OnEnable()
    {
        if (_player == null)
        {
            _player = transform.parent.GetComponent<SA_Unit>();
        }
    }
    public void AttackDone()
    {
        switch(_player._ms._attackType)
        {
            case SA_Unit.AttackType.sword:
                if (_player._target != null) _player.AttackDone(_player._target);
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

    public void AttackFinish()
    {
        _player.isAttacking = false;

    }

    public void CanMove()
    {
        _player.canMove = true;
    }
    public void DeathDone()
    {
        _player.SetDeathDone();   
    }
}
