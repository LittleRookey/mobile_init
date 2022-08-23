using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public enum MonsterSize
{
    small,
    middle,
    big
};
public enum MonsterType
{
    Attacker,
    Tanker,
    Balance
};

[CreateAssetMenu(fileName = "MonsterSetting", menuName = "Litkey/MonsterSetting")]
public class MonsterSetting : ScriptableObject
{

    [Header("Settings")]
    public AnimationCurve basicGrowth;

    public ObscuredInt monsterStatPerLevel = 3;
    public MonsterSize _monsterSize;
    public MonsterType _monsterType; // attacker, supporter, tanker,
    public SA_Unit.AttackType _attackType;

    #region static variables
    public static readonly int HP = 25;
    public static readonly int MANA = 0;
    public static readonly int ATTACK = 3;
    public static readonly int MAGICFORCE = 0;
    public static readonly int ATTACKSPEED = 1;
    public static readonly float ATTACKDELAY = 0.5f;
    public static readonly int MOVESPEED = 1;
    public static readonly float HPREGEN = 0.1f;
    public static readonly float MANAREGEN = 0f;
    public static readonly int DEFENSE = 1;

    public static readonly int ATTACKRANGE_BOW = 10;
    public static readonly int ATTACKRANGESWORD = 1;
    public static readonly int ATTACKRANGE_MAGIC = 10;

    public static readonly float ATTACKGROW = 3.0f; 
    public static readonly float MAXHPGROW = 10.0f;
    public static readonly int DEFENSEGROW = 1;
    public static readonly float HPREGENGROW = 0.2f;


    #endregion
    [Header("Static monster setting")] // may change based on the difficulty

    [SerializeField] private ObscuredInt basis = 30;
    [SerializeField] private ObscuredInt extra = 50;
    [SerializeField] private ObscuredInt acc_a = 30;
    [SerializeField] private ObscuredInt acc_b = 30;

    [SerializeField] private ObscuredInt mob_count_basis = 7;
    [SerializeField] private ObscuredInt mob_extra = 10;

    [Header("Growth Rate")]
    [SerializeField] private ObscuredFloat level_attackgrowth = 1.06f;
    [SerializeField] public ObscuredFloat level_maxhpgrowth = 1.09f;
    [SerializeField] private ObscuredInt gold_base = 5;
    [SerializeField] private ObscuredInt gold_extra = 6;
    [Range(0, 2f)]
    [SerializeField] 
    private float mob_count_growthRate = 0.3f;
    public float dropExp;

    private ObscuredInt tempStatAttack;
    private ObscuredInt tempStatMaxHP;
    private ObscuredInt tempStatHPRegen;

    private ObscuredInt mobCount;

    private float CalcRate(int level)
    {
        return Mathf.Pow(level, 2) / (Mathf.Pow(A, 2) + Mathf.Pow(level, 2));
    }

    private ObscuredInt CalcMobCount(int level)
    { 
        float num = (mob_count_basis * Mathf.Pow((level - 1),(0.9f + acc_a / 250)) * level * (level + 1)) / 
                    (7 + Mathf.Pow(level, 5) / 50 / acc_b) + (level - 1) * mob_extra + 1;
        if (level == 1)
        {
            num = (mob_count_basis * Mathf.Pow((level - 1), (0.9f + acc_a / 250)) * level * (level + 1)) /
                    (7 + Mathf.Pow(level, 5) / 50 / acc_b) + level * mob_extra + 1;
        }
        num = Round(num, 0);
        mobCount = (int)num;
        return (ObscuredInt)num;
    }

    private ObscuredInt CalcMobCountV2(int level)
    {
        return Mathf.RoundToInt(mob_count_basis + mob_count_basis * (mob_count_growthRate + CalcRate(level) * (level - 1)));
    }

    public ObscuredInt CalcMaxExp(int level)
    {
        float num = (basis * Mathf.Pow((level - 1), (0.9f + acc_a / 250)) * level * (level + 1)) /
                    (6 + Mathf.Pow(level, 2) / 50 / acc_b) + (level - 1) * extra;
        if (level == 1)
        {
            num = (basis * Mathf.Pow((level - 1), (0.9f + acc_a / 250)) * level * (level + 1)) /
                    (6 + Mathf.Pow(level, 2) / 50 / acc_b) + (level) * extra;
        }
        num = Round(num, 0);
        return (ObscuredInt)num;
    }


    // call this method to set the right drop exp based on level
    private ObscuredInt CalcDropExp(int level)
    {
        //Debug.Log(CalcMaxExp(level));
        //Debug.Log(CalcMobCount(level));
        return Mathf.RoundToInt(CalcMaxExp(level) / CalcMobCountV2(level));
    }

    [Header("V1 settings")]
    [Range(1, 50)]
    [SerializeField]
    private int A = 15;


    [Header("V2 settings")]
    [Range(0f, 1f)]
    [SerializeField]
    float p = 0.64f; // position of turnover

    [Range(0f, 1f)]
    [SerializeField]
    float s = 0.42f; // slope of the turnover curve 
    private ObscuredFloat CalcExpV2(float progress)
    {
        float c = 2 / (1 - s) - 1; // around 2.448
        float g;
        if (progress  <= p)
        {
            g = Mathf.Pow(progress, c) / Mathf.Pow(p, c - 1);
        } else
        {
            g = 1 - (Mathf.Pow(1-progress, c) / Mathf.Pow(1-p, c - 1));
        }
        return g;
    }

    private ObscuredInt CalcDropGold(int level)
    {
        int fin = gold_base + gold_extra * (level - 1);
        //Debug.Log((int)(fin * .85f) + " :::  " + (int)(fin * 1.15f));
        return Random.Range(Round(fin * .85f), Round(fin * 1.15f));
    }

    public ObscuredInt CalcMonsterAttack(int level)
    {
        int tempStatAttack = (level - 1) * monsterStatPerLevel;
        float num = ATTACK * Mathf.Pow(level_attackgrowth, level - 1) + (tempStatAttack * ATTACKGROW);
        num = Round(num, 0);
        return (ObscuredInt)num;
    }

    private ObscuredInt CalcMonsterMAXHP(int level)
    {
        int tempStatMaxHP = (level - 1) * monsterStatPerLevel;
        float num = HP * Mathf.Pow(level_maxhpgrowth, level - 1) + (tempStatMaxHP * MAXHPGROW);
        num = Round(num, 0);
        return (ObscuredInt)num;
    }

    private ObscuredInt CalcMonsterDefense(int level)
    {
        int tempStatDef = (level - 1) * monsterStatPerLevel;
        ObscuredInt num = DEFENSE + (level - 1) * DEFENSEGROW * tempStatDef;
        return num;
    }

    // 2.3 => mult = 1
    // 2 => mult = 0
    private ObscuredFloat Round(ObscuredFloat num, int mult)
    {
        int multnum = 1;
        for (int i = 0; i < mult; i++)
        {
            multnum *= 10;
        }
        return Mathf.Round(num * multnum) / multnum;
    }

    // Round to nearest Int
    private ObscuredInt Round(ObscuredFloat num)
    {
        
        return Mathf.RoundToInt(num);
    }
    // necessary preset before this method call, should be called once
    // 1: level, 2: monsterType, 3: attacktype, 4: 
    // initialize stat for monsters on spawn, not player
    public void Init(SA_Unit sa)
    {
        if (sa.isPlayer) return;

        ObscuredInt level = sa._level;
        ObscuredInt leftStatPoints = (level - 1) * monsterStatPerLevel;
        // give all statpoints to monsters based on monster type
        if (_monsterType == MonsterType.Attacker)
        {
            int temp = leftStatPoints;
            tempStatAttack += leftStatPoints;
            leftStatPoints -= temp;
        } 
        else if(_monsterType == MonsterType.Tanker)
        {
            int temp = leftStatPoints;
            tempStatMaxHP += leftStatPoints;
            leftStatPoints -= temp;
        } 
        else if (_monsterType == MonsterType.Balance)
        {
            if (leftStatPoints%2 == 0)
            {
                int temp = leftStatPoints;
                tempStatMaxHP += leftStatPoints / 2;
                tempStatAttack += leftStatPoints / 2;
                leftStatPoints -= (tempStatAttack + tempStatAttack);
            } else
            {
                int temp = leftStatPoints-1;
                tempStatMaxHP += temp / 2;
                tempStatAttack += temp / 2;
                tempStatAttack += 1;
                leftStatPoints -= (tempStatAttack + tempStatAttack + 1);
            }
        }


        sa.dropExp = CalcDropExp(level);
        sa.dropGold = CalcDropGold(level);
        //Debug.Log(sa.dropGold);
        sa._unitMaxHP = CalcMonsterMAXHP(level);
        sa._unitHP = sa._unitMaxHP;
        sa._unitMagicForce = MAGICFORCE;
        sa._unitAttack = CalcMonsterAttack(level);
        sa._unitDefense = CalcMonsterDefense(level);
        sa._unitAttackSpeed = ATTACKSPEED;
        

        // setup attacktype
        if (_attackType == SA_Unit.AttackType.sword)
        {
            sa._unitAttackRange = ATTACKRANGESWORD;
        } else if(_attackType == SA_Unit.AttackType.bow)
        {
            sa._unitAttackRange = ATTACKRANGE_BOW;
        } else if (_attackType == SA_Unit.AttackType.magic)
        {
            sa._unitAttackRange = ATTACKRANGE_MAGIC;
        }

        // setup monster size
        if (_monsterSize == MonsterSize.small)
        {
            sa._spumPrefab._anim.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            sa._unitAttackDelay = 1f;
        } else if(_monsterSize == MonsterSize.middle)
        {
            sa._spumPrefab._anim.transform.localScale = new Vector3(1f, 1f, 1f);
            sa._unitAttackDelay = 1.5f;
        } else if(_monsterSize == MonsterSize.big)
        {
            sa._spumPrefab._anim.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            sa._unitAttackDelay = 3f;
        }

        sa._unitFightRange = 50;
        sa._unitHPRegen = HPREGEN;
        sa._unitManaRegen = MANAREGEN;
        sa._unitMoveSpeed = MOVESPEED;

    }



  

    void LevelUp()
    {

    }
}
