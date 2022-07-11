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

[System.Serializable]
public struct MainStats
{
    public int _Strength;
    public int _Dexterity;
    public int _Intelligence;
    public int _Vitality;
}
[System.Serializable]
public struct SubStats
{
    public float _MaxHP;
    public float _HP;
    public float _Defense;

    public float _Mana;
    public float _Attack;
    public float _MagicForce; // penetrates armor

    public float _HealthRegen;
    public float _ManaRegen;

    public float _AttackSpeed;

    public float _MoveSpeed;

    public float _AttackRange;

    public float _FightRange;
}

public class SA_ResourceManager: MonoBehaviour 
{
    public static SA_ResourceManager Instance;

    [Header("Damage Numbers")]

    public DamageNumber _normalDamagePrefab;

    public DamageNumber _swordNormalDamage, _bowNormalDamage, _magicNormalDamage;

    public GameObject _hpBar;
    public GameObject _hpBarWithLevel;

    [Header("Stats")]
    public StatsContainer normalCharacterStats;

    public bool turnOnHPBarAlways;

    // Stats for base values
    public static readonly int HP = 100;
    public static readonly int MANA = 0;
    public static readonly int ATTACK = 10;
    public static readonly int MAGICFORCE = 0;
    public static readonly int ATTACKSPEED = 1;
    public static readonly float ATTACKDELAY = 0.5f;
    public static readonly int MOVESPEED = 1;
    public static readonly float HPREGEN = 0.1f;
    public static readonly float MANAREGEN = 0f;
    public static readonly int DEFENSE = 1;

    public static readonly int ATTACKRANGE_BOW = 10;
    public static readonly int ATTACKRANGESWORD= 1;
    public static readonly int ATTACKRANGE_MAGIC = 10;
    //public static readonly int MAGICDEFENSE = 0;


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
