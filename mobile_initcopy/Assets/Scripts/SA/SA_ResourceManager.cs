using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using CleverCrow.Fluid.StatsSystem.StatsContainers;

public enum DamageType
{
    normal,
    miss,
    critical
};

public struct MainStats
{
    public int _Strength;
    public int _Dexterity;
    public int _Intelligence;
    public int _Vitality;
}
public struct SubStats
{
    public float _HP;
    public float _Mana;
    public float _Attack;
    public float _MagicAttack;

    public float _HealthRegen;
    public float _ManaRegen;

    public float _AttackSpeed;

    public float _MoveSpeed;

    public float _AttackRange;
}

public class SA_ResourceManager: MonoBehaviour 
{
    public static SA_ResourceManager Instance;

    [Header("Damage Numbers")]

    public DamageNumber _normalDamagePrefab;

    public DamageNumber _swordNormalDamage, _bowNormalDamage, _magicNormalDamage;

    public GameObject _hpBar;

    [Header("Stats")]
    public StatsContainer normalCharacterStats;

    public static readonly int HP = 100;
    public static readonly int MANA = 0;
    public static readonly int ATTACK = 10;
    public static readonly int MAGICATTACK = 10;
    public static readonly int ATTACKSPEED = 2;
    public static readonly int MOVESPEED = 1;
    public static readonly float HPREGEN = 0.1f;
    public static readonly float MANAREGEN = 0f;



    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        _normalDamagePrefab.PrewarmPool();
        _swordNormalDamage.PrewarmPool();
        _bowNormalDamage.PrewarmPool();
        _magicNormalDamage.PrewarmPool();
    }

    public StatsContainer GetCharacterStat()
    {
        return normalCharacterStats.CreateRuntimeCopy();
    }
    public DamageNumber GetDamageNumber(SA_Unit.AttackType weaponType, DamageType dmgType=DamageType.normal) 
    {
        if (weaponType == SA_Unit.AttackType.sword)
        {
            return _swordNormalDamage;
        } 
        else if (weaponType == SA_Unit.AttackType.bow)
        {
            return _bowNormalDamage;
        } 
        else if (weaponType == SA_Unit.AttackType.magic)
        {
            return _magicNormalDamage;
        }

        return null;
        //switch(weaponType == SA_Unit.AttackType.)
        //{
        //    case SA_Unit.AttackType.sword:
        //        dmn = _swordNormalDamage;
        //        break;
        //    case SA_Unit.AttackType.bow:
        //        dmn = _bowNormalDamage;
        //        break;
        //    case SA_Unit.AttackType.magic:
        //        dmn = _magicNormalDamage;
        //        break;
        //}
        //return dmn;
    }


}
