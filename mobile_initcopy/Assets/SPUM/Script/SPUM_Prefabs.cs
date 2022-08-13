using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
public enum AnimType
{
    Idle = 0,
    Run = 1,
    Die = 2,
    Stun = 3,
    Attack_S_V1 = 4,
    Attack_B_V1 = 5,
    Attack_M_V1 = 6,
    Skill_S_V1 = 7,
    Skill_B_V1 = 8,
    Skill_M_V1 = 9,
    Attack_S_V2 = 10,
    Attack_S_Sting = 11
};

public class SPUM_Prefabs : MonoBehaviour
{
    public float _version;
    public SPUM_SpriteList _spriteOBj;
    public bool EditChk;
    public string _code;
    public Animator _anim;
    public bool _horse;

    private static readonly string attack = "Attack";
    private static readonly string die = "Die";
    private static readonly string runState = "RunState";
    private static readonly string attackState = "AttackState";
    private static readonly string weaponState = "WeaponState";
    private static readonly string skillState =  "SkillState";
    

    public bool isRideHorse{
        get => _horse;
        set {
            _horse = value;
            UnitTypeChanged?.Invoke();
        }
    }
    public string _horseString;

    public UnityEvent UnitTypeChanged = new UnityEvent();
    private AnimationClip[] _animationClips;
    public AnimationClip[] AnimationClips => _animationClips;
    private Dictionary<string, int> _nameToHashPair = new Dictionary<string, int>();
    private void InitAnimPair(){
        _nameToHashPair.Clear();
        _animationClips = _anim.runtimeAnimatorController.animationClips;
        foreach (var clip in _animationClips)
        {
            int hash = Animator.StringToHash(clip.name);
            _nameToHashPair.Add(clip.name, hash);
        }
    }
    private void Awake() {
        InitAnimPair();
        
    }
    private void Start() {
        UnitTypeChanged.AddListener(InitAnimPair);
    }
    // 이름으로 애니메이션 실행
    public void PlayAnimation(string name){
        
        foreach (var animationName in _nameToHashPair)
        {
            if(animationName.Key.ToLower().Contains(name.ToLower()) ){
                _anim.Play(animationName.Value, 0);
                break;
            }
        }
        
    }

    public void PlayAnimation(AnimType animType)
    {
        
        switch((int)animType)
        {
            case 0: // idle
                _anim.SetFloat(runState, 0f);
                break;
            case 1: //run
                _anim.SetFloat(runState, 0.5f);
                break;
            case 2: //Death
                _anim.SetTrigger(die);
                break;
            case 3: //Stun
                _anim.SetFloat(runState, 1.0f);
                break;
            case 4: // Attack Sword
                _anim.SetTrigger(attack);
                _anim.SetInteger(attackState, 0);
                _anim.SetInteger(weaponState, 0);
                
                break;
            case 5: // Attack Bow
                _anim.SetTrigger(attack);
                _anim.SetInteger(attackState, 0);
                _anim.SetInteger(weaponState, 1);
                break;
            case 6: // Attack Magic
                _anim.SetTrigger(attack);
                _anim.SetInteger(attackState, 0);
                _anim.SetInteger(weaponState, 2);
                break;
            case 7: // Skill Sword
                _anim.SetTrigger(attack);
                //_anim.SetInteger("AttackState", 1);
                _anim.SetInteger(weaponState, 0);
                _anim.SetInteger(skillState, 0);
                break;
            case 8: // Skill Bow
                _anim.SetTrigger(attack);
                //_anim.SetInteger("AttackState", 1);
                _anim.SetInteger(weaponState, 1);
                _anim.SetInteger(skillState, 0);
                break;
            case 9: // Skill Magic
                _anim.SetTrigger(attack);
                //_anim.SetInteger("AttackState", 1);
                _anim.SetInteger(weaponState, 2);
                _anim.SetInteger(skillState, 0);
                break;
            case 10: // sword attack v2
                _anim.SetTrigger(attack);
                _anim.SetInteger(weaponState, 0);
                _anim.SetInteger(attackState, 1);
                break;
            case 11:
                _anim.SetTrigger(attack);
                _anim.SetInteger(weaponState, 0);
                _anim.SetInteger(attackState, 2);
                break;
        }
    }

    //AnimationClip GetAnimationClip(string nam)
    //{
    //    for (int i = 0; i < _animationClips.Length; i++)
    //    {
    //        if (_animationClips[i])
    //    }
    //    _animationClips
    //}
    IEnumerator PlayAnim(string ani,float sec)
    {
        _anim.Play(ani, 0);
        yield return new WaitForSeconds(sec);
    }

    

    
}
