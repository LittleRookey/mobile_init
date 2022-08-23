using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;
using Litkey.InventorySystem;

public class Stat
{
    public string _name;
    public int _ID;
    public int _value;
    private int _startValue;
    public int _level;
    public int _maxExp;
    public int _currentExp;
    public int statsPerLevel; // level of stat
    public Stat(string name, int ID, int level, int startValue)
    {
        _name = name;
        _ID = ID;
        _level = level;
        _startValue = startValue;
    }
}
public class StatManager : MonoBehaviour
{
    public static StatManager Instance;
    public SA_Player _player;
    [Header("MainStats")]

    public ObscuredInt statpoint_MaxHP;
    public ObscuredInt statpoint_Attack;
    public ObscuredInt statpoint_HPRegen;
    public ObscuredInt statpoint_MagicForce;
    public ObscuredInt statpoint_AttackSpeed;
    public ObscuredInt statpoint_MoveSpeed;

    public ObscuredFloat equip_MaxHP;
    public ObscuredFloat equip_Attack;
    public ObscuredFloat equip_HPRegen;
    public ObscuredFloat equip_MagicForce;
    public ObscuredFloat equip_MoveSpeed;
    public ObscuredFloat equip_AttackSpeed;
    public ObscuredFloat equip_Defense;

    public ObscuredFloat equip_MaxHP_Percentage;
    public ObscuredFloat equip_Attack_Percentage;
    public ObscuredFloat equip_HPRegen_Percentage;
    public ObscuredFloat equip_MagicForce_Percentage;
    public ObscuredFloat equip_MoveSpeed_Percentage;
    public ObscuredFloat equip_AttackSpeed_Percentage;
    public ObscuredFloat equip_Defense_Percentage;

    private ObscuredFloat AttackMultiplier = 1;
    private ObscuredFloat MaxHPMultiplier = 1;

    private ObscuredFloat TempAttackMultiplier = 0;
    private ObscuredFloat TempMaxHPMultiplier = 0;

    public MonsterSetting playerSetting;

    //float tempmaxhp = 0;
    //float tempmana = 0;
    //float tempattack = 0;
    //float tempmagicattack = 0;
    //float tempdefense = 0;
    //float tempmagicdefense = 0;
    //float tempHPGen = 0;
    //float tempManaGen = 0;
    //float tempattackspeed = 0;
    //float tempmovespeed = 0;

    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color addedColor = new Color(239f, 255f, 0f);

    [SerializeField] private ObscuredFloat level_attackgrowth = 1.05f;
    [SerializeField] private ObscuredFloat level_maxhpgrowth = 1.06f;


    public static readonly ObscuredInt ATTACKGROW = 3;
    public static readonly ObscuredFloat MAXHPGROW = 10.0f;
    public static readonly ObscuredFloat DEFENSEGROW = 1.0f;
    public static readonly ObscuredFloat HPREGENGROW = 0.2f;
    public static readonly ObscuredFloat ATTACKSPEEDGROW = 0.01f;
    public static readonly ObscuredFloat MOVESPEEDGROW = 0.01f;

    public static readonly ObscuredInt HP = 100;
    public static readonly ObscuredInt MANA = 0;
    public static readonly ObscuredInt ATTACK = 8;
    public static readonly ObscuredInt MAGICFORCE = 0;
    public static readonly ObscuredInt ATTACKSPEED = 1;
    public static readonly float ATTACKDELAY = 0.5f;
    public static readonly ObscuredInt MOVESPEED = 1;
    public static readonly float HPREGEN = 0f;
    public static readonly float MANAREGEN = 0f;
    public static readonly ObscuredInt DEFENSE = 1;
    public static readonly ObscuredInt MAXEXP = 100;

    public static readonly ObscuredInt STATPERLEVEL = 3;


    public static readonly ObscuredInt ATTACKRANGE_BOW = 10;
    public static readonly ObscuredInt ATTACKRANGESWORD = 1;
    public static readonly ObscuredInt ATTACKRANGE_MAGIC = 10;


    public static readonly string ATTACK_MODID = "_attack";
    public static readonly string MAGICATTACK_MODID = "_magicattack";
    public static readonly string HEALTH_MODID = "_health";
    public static readonly string MANA_MODID = "_mana";
    public static readonly string HEALTHGEN_MODID = "_healthgen";
    public static readonly string MANAGEN_MODID = "_managen";

    #region stat definition strings
    public static readonly string STR_ID = "strength";
    public static readonly string DEX_ID = "dexterity";
    public static readonly string INT_ID = "intelligence";
    public static readonly string VIT_ID = "vitality";

    public static readonly string ATTACK_ID = "attack";
    public static readonly string ATTACKSPEED_ID = "attackspeed";
    public static readonly string MOVESPEED_ID = "movespeed";
    public static readonly string HP_ID = "health";
    public static readonly string MANA_ID = "mana";
    public static readonly string MAGICATTACK_ID = "magicattack";
    public static readonly string HEALTHGEN_ID = "healthgen";
    public static readonly string MANAGEN_ID = "managen";
    #endregion

    #region stat korean
    public static readonly string ko_strength = "힘: ";
    public static readonly string ko_dexterity = "민첩: ";
    public static readonly string ko_intelligence = "지능: ";
    public static readonly string ko_vitality = "내구력: ";
    public static readonly string ko_maxHP = "최대 체력: ";
    public static readonly string ko_mana = "마나: ";
    public static readonly string ko_attack = "공격력: ";
    public static readonly string ko_magicattack = "마법 공격력: ";
    public static readonly string ko_defense = "물리 방어력: ";
    public static readonly string ko_magicdefense = "마법 방어력: ";
    public static readonly string ko_hpgen = "체력 회복량: ";
    public static readonly string ko_managen = "마나 회복량: ";
    public static readonly string ko_attackspeed = "공격 속도: ";
    public static readonly string ko_movespeed = "이동 속도: ";
    #endregion

    public ObscuredFloat boostFactor;

    public static UnityAction<SA_Unit> OnEnemyDeath;

    public static UnityAction<EquipmentItem> OnEquipItem;
    public static UnityAction<EquipmentItem> OnUnEquipItem;

    private EquipmentItem equippedWeapon;
    private EquipmentItem equippedArmor;
    private void Awake()
    {
        Instance = this;
        if (_player == null) _player = FindObjectOfType<SA_Player>();
    }

    private void Start()
    {
        //tempStatLeft = _player._statPoint; // TODO when player opens the stat window
        //tempStatDex = 0;


    }

    private void OnEnable()
    {
        OnEnemyDeath += GiveDropItems;
        Actions.OnPlayerLevelUp += LevelUp;
        OnEquipItem += EquipStat;
        OnUnEquipItem += UnEquipStat;
    }

    private void OnDisable()
    {
        OnEnemyDeath -= GiveDropItems;
        Actions.OnPlayerLevelUp -= LevelUp;
        OnEquipItem -= EquipStat;
        OnUnEquipItem -= UnEquipStat;
    }

    public void GiveDropItems(SA_Unit enem)
    {
        AddExp(enem);

        
    }
    
    void AddExp(SA_Unit sa)
    {
        _player._exp += sa.dropExp;
        //Debug.Log("EXP added");
        UIManager.OnUpdateExpBar?.Invoke(_player);
        if (_player._exp >= _player._maxExp)
        {
            _player._exp -= _player._maxExp;
            //LevelUp();
            Actions.OnPlayerLevelUp?.Invoke();
            //UIManager.OnUpdateExpBar?.Invoke(_player);
            UIManager.OnUpdateExpBar?.Invoke(_player);
        }
    }

    public void LevelUp()
    {
        _player._level += 1;
        _player._statPoint += 3;
        // update UI

        InitPlayer(_player);
        TabManager.OnOpenCharacterTab?.Invoke(_player._statPoint);
    }

    public void AddStat(int type)
    {
        if (_player._statPoint <= 0)
        { // TODO show popup message no statpoint
            return;
        }
        
        switch ((StatType)type)
        {
            case StatType.maxHP:
                _player._statPoint -= 1;
                statpoint_MaxHP += 1;
                _player._unitMaxHP += MAXHPGROW;
                break;
            case StatType.attack:
                _player._statPoint -= 1;
                statpoint_Attack += 1;
                _player._unitAttack += ATTACKGROW;
                break;
            case StatType.hpRegen:
                _player._statPoint -= 1;
                statpoint_HPRegen += 1;
                _player._unitHPRegen += HPREGENGROW;
                break;
            case StatType.magicForce: // magicforce

                break;
            case StatType.attackSpeed: // attackspeed
                _player._statPoint -= 1;
                statpoint_AttackSpeed += 1;
                _player._unitAttackSpeed += ATTACKSPEEDGROW;
                break;
            case StatType.moveSpeed: // movespeed
                _player._statPoint -= 1;
                statpoint_MoveSpeed += 1;
                _player._unitMoveSpeed += MOVESPEEDGROW;
                break;
        }
        //StatSlot.OnStatUpdate?.Invoke(_player._statPoint);
        TabManager.OnOpenCharacterTab?.Invoke(_player._statPoint);
    }

    public ObscuredFloat GetFinalMaxHP(int level)
    {
        // TODO add any modifiers
        // (100 + 10) * 1.3 
        ObscuredFloat num = HP * Mathf.Pow(level_maxhpgrowth, level - 1) + (statpoint_MaxHP * MAXHPGROW);
        return num;
    }



    // Attack equation
    // ((base_attack + attackstat * statPoint_attack) * (AttackMultiplier + TempAttackMultiplier) + weapon_attack * weapon_attack_Percentage)
    public ObscuredInt GetFinalPlayerAttack(int level)
    {
        // base attack by levelup + (stat attacks)
        //Debug.Log(name + ": 1-4");
        float num = ATTACK * Mathf.Pow(level_attackgrowth, level - 1) + (statpoint_Attack * ATTACKGROW);

        // weapon stats 

        // attack multipliers
        num *= (AttackMultiplier + TempAttackMultiplier);
        num += (equip_Attack * (1 + equip_Attack_Percentage));

        return (ObscuredInt)Mathf.RoundToInt(num);
    }

    

    // mult: 소수점 몇째자리, 
    // ex) 0.35 * 10 = 3.5 => 4 => return 0.4
    public static ObscuredFloat Round(float num, int mult)
    {
        int multnum = 1;
        for (int i = 0; i < mult; i++)
        {
            multnum *= 10;
        }
        return (ObscuredFloat)Mathf.Round(num * multnum) / multnum;
    }


    public void InitPlayer(SA_Player sa)
    {

        int level = sa._level;

        //int totalStatPoint = (level - 1) * StatManager.STATPERLEVEL;


        sa._unitMaxHP = GetFinalMaxHP(level);
        sa._unitHP = sa._unitMaxHP;
        sa._unitMagicForce = MAGICFORCE;
        sa._unitAttack = GetFinalPlayerAttack(level);
        // TODO calc Defense
        sa._unitDefense = DEFENSE;
        sa._unitAttackSpeed = ATTACKSPEED;
        sa._unitAttackDelay = 1f;

        // setup attacktype
        if (sa._ms._attackType == SA_Unit.AttackType.sword)
        {
            sa._unitAttackRange = ATTACKRANGESWORD;
        }
        else if (sa._ms._attackType == SA_Unit.AttackType.bow)
        {
            sa._unitAttackRange = ATTACKRANGE_BOW;
        }
        else if (sa._ms._attackType == SA_Unit.AttackType.magic)
        {
            sa._unitAttackRange = ATTACKRANGE_MAGIC;
        }

        sa._unitFightRange = 30;
        sa._unitHPRegen = HPREGEN;
        sa._unitManaRegen = MANAREGEN;
        sa._unitMoveSpeed = MOVESPEED;
        sa._maxExp = sa._ms.CalcMaxExp(level);


        // setup the attributes based on stats given 
        for (int i = 0; i < statpoint_Attack; i++)
        {
            sa._unitAttack += ATTACKGROW;
        }

        for (int i = 0; i < statpoint_MaxHP; i++)
        {
            sa._unitMaxHP += MAXHPGROW;
        }

        for (int i = 0; i < statpoint_HPRegen; i++)
        {
            sa._unitHPRegen += HPREGENGROW;
        }

        for (int i = 0; i < statpoint_AttackSpeed; i++)
        {
            sa._unitAttackSpeed += ATTACKSPEEDGROW;
        }

        for (int i = 0; i < statpoint_MoveSpeed; i++)
        {
            sa._unitMoveSpeed += MOVESPEEDGROW;
        }


        //for (int i = 0; i < statpoint_MagicForce; i++)
        //{
        //    sa._unitAttack += ;
        //}

    }



    private void UnEquipStat(EquipmentItem item)
    {
        ClearEquipValues();

        if (item is WeaponItem)
        {
            if (equippedArmor != null)
            {
                foreach (StatModifier st in equippedArmor.EquipmentData.GetStats())
                {
                    CalculateStat(st);
                }
            }
            equippedWeapon = null;
        } else if (item is ArmorItem)
        {
            if (equippedWeapon != null)
            {
                foreach (StatModifier st in equippedWeapon.EquipmentData.GetStats())
                {
                    CalculateStat(st);
                }
            }
            equippedArmor = null;
        }

        
    }

    void ClearEquipValues()
    {
        equip_Attack = 0;
        equip_AttackSpeed = 0;
        equip_Defense = 0;
        equip_HPRegen = 0;
        equip_MagicForce = 0;
        equip_MaxHP = 0;
        equip_MoveSpeed = 0;

        equip_Attack_Percentage = 0;
        equip_AttackSpeed_Percentage = 0;
        equip_Defense_Percentage = 0;
        equip_HPRegen_Percentage = 0;
        equip_MagicForce_Percentage = 0;
        equip_MaxHP_Percentage = 0;
        equip_MoveSpeed_Percentage = 0;
    }

    // make all equip values to 0. 
    // if equippedweapon is not null, remove weapon and apply the new given item
    // 
    private void EquipStat(EquipmentItem item)
    {
        ClearEquipValues();
        bool isWeapon = false;
        // if given is weapon, apply weapon 
        if (item is WeaponItem wi)
        {
            equippedWeapon = wi;
            if (equippedWeapon != null) // if equipments are on
            {
                foreach (StatModifier st in wi.EquipmentData.GetStats())
                {
                    CalculateStat(st);
                }
            }
            isWeapon = true;
            // Assign weapon
            
        }
        else if(item is ArmorItem ai)
        {
            equippedArmor = ai;
            if (equippedArmor != null) // if equipments are on
            {
                foreach (StatModifier st in ai.EquipmentData.GetStats())
                {
                    CalculateStat(st);
                }
            }
            // Assign armor
        }

        // if it is weapon, check if armor is not null and add armor stat too
        if (isWeapon)
        {
            if (equippedArmor != null)
            {
                foreach (StatModifier st in equippedArmor.EquipmentData.GetStats())
                {
                    CalculateStat(st);
                }
            }
        } else
        {
            if (equippedWeapon != null)
            {
                foreach (StatModifier st in equippedWeapon.EquipmentData.GetStats())
                {
                    CalculateStat(st);
                }
            }
        }
    }

    // calculates statmodifier one time
    void CalculateStat(StatModifier st, bool reverse = false)
    {

        if (!reverse)
        {
            switch (st.statType)
            {
                case StatType.attack:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_Attack += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_Attack_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_Attack -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_Attack_Percentage -= st.value;
                    }

                    break;
                case StatType.attackSpeed:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_AttackSpeed += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_AttackSpeed_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_AttackSpeed -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_AttackSpeed_Percentage -= st.value;
                    }
                    break;
                case StatType.moveSpeed:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_MoveSpeed += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_MoveSpeed_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_MoveSpeed -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_MoveSpeed_Percentage -= st.value;
                    }
                    break;
                case StatType.hpRegen:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_HPRegen += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_HPRegen_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_HPRegen -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_HPRegen_Percentage -= st.value;
                    }
                    break;
                case StatType.magicForce:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_HPRegen += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_HPRegen_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_HPRegen -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_HPRegen_Percentage -= st.value;
                    }
                    break;
                case StatType.maxHP:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_MaxHP += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_MaxHP_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_MaxHP -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_MaxHP_Percentage -= st.value;
                    }
                    break;
                case StatType.defense:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_Defense += st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_Defense_Percentage += st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_Defense -= st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_Defense_Percentage -= st.value;
                    }

                    break;
            }
        }
        else
        {
            switch (st.statType)
            {
                case StatType.attack:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_Attack -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_Attack_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_Attack += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_Attack_Percentage += st.value;
                    }

                    break;
                case StatType.attackSpeed:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_AttackSpeed -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_AttackSpeed_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_AttackSpeed += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_AttackSpeed_Percentage += st.value;
                    }
                    break;
                case StatType.moveSpeed:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_MoveSpeed -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_MoveSpeed_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_MoveSpeed += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_MoveSpeed_Percentage += st.value;
                    }
                    break;
                case StatType.hpRegen:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_HPRegen -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_HPRegen_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_HPRegen += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_HPRegen_Percentage += st.value;
                    }
                    break;
                case StatType.magicForce:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_HPRegen -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_HPRegen_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_HPRegen += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_HPRegen_Percentage += st.value;
                    }
                    break;
                case StatType.maxHP:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_MaxHP -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_MaxHP_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_MaxHP += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_MaxHP_Percentage += st.value;
                    }
                    break;
                case StatType.defense:
                    if (st.oper == OperatorType.plus)
                    {
                        equip_Defense -= st.value;
                    }
                    else if (st.oper == OperatorType.multiply)
                    {
                        equip_Defense_Percentage -= st.value;
                    }
                    else if (st.oper == OperatorType.subtract)
                    {
                        equip_Defense += st.value;
                    }
                    else if (st.oper == OperatorType.divide)
                    {
                        equip_Defense_Percentage += st.value;
                    }

                    break;
            }
        }
        
    }
}