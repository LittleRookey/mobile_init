using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_AnimationAction : MonoBehaviour
{
    public  SA_Unit _player;


    public void AttackDone()
    {
        switch(_player._attackType)
        {
            case SA_Unit.AttackType.sword:
                if (_player._target != null) _player.AttackDone(_player._target);
                break;

            case SA_Unit.AttackType.bow:
                _player.AttackMissile();
                break;

            case SA_Unit.AttackType.magic:
                _player.AttackMissile();
                break;
        }
    }

    public void DeathDone()
    {
        _player.SetDeathDone();   
    }
}
