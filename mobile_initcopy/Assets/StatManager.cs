using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CleverCrow.Fluid.StatsSystem;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;

    public SA_Unit _player;
    [Header("MainStats")]

    [SerializeField] private TextMeshProUGUI strText;
    [SerializeField] private TextMeshProUGUI dexText;
    [SerializeField] private TextMeshProUGUI intText;
    [SerializeField] private TextMeshProUGUI vitText;
    [SerializeField] private TextMeshProUGUI statPointText;

    [Header("Substats")]
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI magicAttackText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI magicDefText;
    [SerializeField] private TextMeshProUGUI hpRegenText;
    [SerializeField] private TextMeshProUGUI manaRegenText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;

    public StatDefinition _attack;
    public StatModifier statMod;

    private int tempStatDex;
    private int tempStatStr;
    private int tempStatInt;
    private int tempStatVit;

    float tempmaxhp = 0;
    float tempmana = 0;
    float tempattack = 0;
    float tempmagicattack = 0;
    float tempdefense = 0;
    float tempmagicdefense = 0;
    float tempHPGen = 0;
    float tempManaGen = 0;
    float tempattackspeed = 0;
    float tempmovespeed = 0;

    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color addedColor = new Color(239f, 255f, 0f);



    public static readonly float STR_ATTACKGROW = 6.0f;
    public static readonly float STR_MAXHPGROW = 15.0f;

    public static readonly float INT_MAGICATTACKGROW = 4.0f;
    public static readonly float INT_MANAGROW = 5.0f;
    public static readonly float INT_MANAGENGROW = 0.1f;

    public static readonly float DEX_ATTACKGROW = 3.0f;
    public static readonly float DEX_ATTACKSPEEDGROW = 0.01f;
    public static readonly float DEX_MOVESPEEDGROW = 0.01f;

    public static readonly float VIT_MAXHPGROW = 25.0f;
    public static readonly float VIT_DEFENSEGROW = 2.0f;
    public static readonly float VIT_MAGICDEFENSEGROW = 1.0f;
    public static readonly float VIT_HPGENGROW = 0.5f;


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
    public static readonly string ko_attack = "물리 공격력: ";
    public static readonly string ko_magicattack = "마법 공격력: ";
    public static readonly string ko_defense = "물리 방어력: ";
    public static readonly string ko_magicdefense = "마법 방어력: ";
    public static readonly string ko_hpgen = "체력 회복량: ";
    public static readonly string ko_managen = "마나 회복량: ";
    public static readonly string ko_attackspeed = "공격 속도: ";
    public static readonly string ko_movespeed = "이동 속도: ";
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //tempStatLeft = _player._statPoint; // TODO when player opens the stat window
        tempStatDex = 0;

        UpdateStatUI();
        //var callBack = new UnityAction<StatRecord>((record) => { Debug.Log(record);  });
        UpdateToNormal();
        UpdateSubstatText();
        //_player._mStatContainer.OnStatChangeSubscribe(ATTACK_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(ATTACKSPEED_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MOVESPEED_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MANA_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MAGICATTACK_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(HEALTHGEN_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MANAGEN_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(HP_ID, callBack);
        //SubscribeEvents();
    }

    public void SubscribeEvents()
    {
        //var callBacks = new UnityAction<StatRecord>((record) => Debug.Log(record) );
        //_mStatContainer.OnStatChangeSubscribe(StatManager.ATTACK_ID, callBacks);
        var callBacks = new UnityAction<StatRecord>((record) => Debug.Log(record));

        _player._mStatContainer.OnStatChangeSubscribe(ATTACK_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(ATTACKSPEED_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(MOVESPEED_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(MANA_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(MAGICATTACK_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(HEALTHGEN_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(MANAGEN_ID, callBacks);
        _player._mStatContainer.OnStatChangeSubscribe(HP_ID, callBacks);
    }
    public void Display()
    {
        //_attack.Clone<StatDefinition>()
    }

    public void LevelUp()
    {
        _player._level += 1;
        _player._statPoint += 1;
        _player.LevelUpStats();
        _player._mSubStat._HP = _player._mSubStat._MaxHP;

        _player._UnitSet._UnitSubset._levelText.text = _player._level.ToString();
        // update substats and level text
        UpdateSubstatText();
    }

    public void UpdateStatUI()
    {
        strText.text = ko_strength + _player._mStats._Strength;
        dexText.text = ko_dexterity + _player._mStats._Dexterity;
        intText.text = ko_intelligence + _player._mStats._Intelligence;
        vitText.text = ko_vitality + _player._mStats._Vitality;
        statPointText.text = "Points left: " + _player._statPoint;
    }

    void UpdateToNormal()
    {
        //UpdateStatUI();
        maxHPText.text = ko_maxHP + _player._mSubStat._MaxHP;
        manaText.text = ko_mana + _player._mSubStat._Mana;
        attackText.text = ko_attack + _player._mSubStat._Attack;
        magicAttackText.text = ko_magicattack + _player._mSubStat._MagicAttack;
        defText.text = ko_defense + _player._mSubStat._Defense;
        magicDefText.text = ko_magicdefense + _player._mSubStat._MagicDefense;
        hpRegenText.text = ko_hpgen + _player._mSubStat._HealthRegen + "/초";
        manaRegenText.text = ko_managen + _player._mSubStat._ManaRegen + "/초";
        attackSpeedText.text = ko_attackspeed + _player._mSubStat._AttackSpeed;
        moveSpeedText.text = ko_movespeed + _player._mSubStat._MoveSpeed * 100 + "%";

        maxHPText.color = normalColor;
        manaText.color = normalColor;
        attackText.color = normalColor;
        magicAttackText.color = normalColor;
        defText.color = normalColor;
        magicDefText.color = normalColor;
        hpRegenText.color = normalColor;
        manaRegenText.color = normalColor;
        attackSpeedText.color = normalColor;
        moveSpeedText.color = normalColor;

        strText.color = normalColor;
        dexText.color = normalColor;
        intText.color = normalColor;
        vitText.color = normalColor;
    }



    void UpdateSubstatText()
    {
        tempattack = 0;
        tempmaxhp = 0;
        tempmana = 0;
        tempmagicattack = 0;
        tempdefense = 0;
        tempmagicdefense = 0;
        tempHPGen = 0;
        tempManaGen = 0;
        tempattackspeed = 0;
        tempmovespeed = 0;

        if (tempStatStr > 0)
        {
            tempattack += (STR_ATTACKGROW * tempStatStr);
            tempmaxhp += (STR_MAXHPGROW * tempStatStr);            
        }
        if (tempStatInt > 0)
        {
            tempmagicattack += (INT_MAGICATTACKGROW * tempStatInt);
            tempmana += (INT_MANAGROW * tempStatInt);
            tempManaGen += (INT_MANAGENGROW * tempStatInt);
        }
        if (tempStatDex > 0)
        {
            tempattack += (DEX_ATTACKGROW * tempStatDex);
            tempattackspeed += (DEX_ATTACKSPEEDGROW * tempStatDex);
            tempmovespeed += (DEX_MOVESPEEDGROW * tempStatDex);

        }
        if (tempStatVit > 0)
        {
            tempmaxhp += (VIT_MAXHPGROW * tempStatVit);
            tempdefense += (VIT_DEFENSEGROW * tempStatVit);
            tempmagicdefense += (VIT_MAGICDEFENSEGROW * tempStatVit);
            tempHPGen += (VIT_HPGENGROW * tempStatVit);

        }

        // round if 14.9999 => 15, 14.5 => 14.5, 14.49999 => 144.9999 => 145 / 10 => 14.5, 0.21, 0.2099999 => 209999 =>
        // round(val * 10)  => 145
        //tempmaxhp = Round(tempmaxhp, 2);
        tempHPGen = Round(tempHPGen, 2);
        tempattackspeed = Round(tempattackspeed, 2);
        tempmovespeed = Round(tempmovespeed, 2);

        // check tempsubstat variables and update 
        Debug.Log("tempmaxhp: " + tempmaxhp);
        if (tempmaxhp > 0f)
        {
            maxHPText.color = addedColor;
            maxHPText.text = ko_maxHP + _player._mSubStat._MaxHP + "  +" + tempmaxhp;
        } else
        {
            maxHPText.color = normalColor;
            maxHPText.text = ko_maxHP + _player._mSubStat._MaxHP;
        }
        if (tempmana > 0f)
        {
            manaText.color = addedColor;
            manaText.text = ko_mana + _player._mSubStat._Mana + "  +" + tempmana;
        } else
        {
            manaText.color = normalColor;
            manaText.text = ko_mana + _player._mSubStat._Mana;
        }
        if (tempattack > 0f)
        {
            attackText.color = addedColor;
            attackText.text = ko_attack + _player._mSubStat._Attack + "  +" + tempattack;
        } else
        {
            attackText.color = normalColor;
            attackText.text = ko_attack + _player._mSubStat._Attack;
        }
        if (tempmagicattack > 0f)
        {
            magicAttackText.color = addedColor;
            magicAttackText.text = ko_magicattack + _player._mSubStat._MagicAttack + "  +" + tempmagicattack;
        } else
        {
            magicAttackText.color = normalColor;
            magicAttackText.text = ko_magicattack + _player._mSubStat._MagicAttack;
        }
        if (tempdefense > 0f)
        {
            defText.color = addedColor;
            defText.text = ko_defense + _player._mSubStat._Defense + " +" + tempdefense;
        } else
        {
            defText.color = normalColor;
            defText.text = ko_defense + _player._mSubStat._Defense;
        }
        if (tempmagicdefense > 0f)
        {
            magicDefText.color = addedColor;
            magicDefText.text = ko_magicdefense + _player._mSubStat._MagicDefense + " +" + tempmagicdefense;
        }
        else
        {
            magicDefText.color = normalColor;
            magicDefText.text = ko_magicdefense + _player._mSubStat._MagicDefense;
        }
        if (tempHPGen > 0f)
        {
            hpRegenText.color = addedColor;
            hpRegenText.text = ko_hpgen + _player._mSubStat._HealthRegen + " +" + tempHPGen + "/초";
        }
        else
        {
            hpRegenText.color = normalColor;
            hpRegenText.text = ko_hpgen + _player._mSubStat._HealthRegen + "/초";
        }
        if (tempManaGen > 0f)
        {
            manaRegenText.color = addedColor;
            manaRegenText.text = ko_managen + _player._mSubStat._ManaRegen + " +" + tempManaGen + "/초";
        } else
        {
            manaRegenText.color = normalColor;
            manaRegenText.text = ko_managen + _player._mSubStat._ManaRegen + "/초";
        }
        if (tempattackspeed > 0f)
        {
            attackSpeedText.color = addedColor;
            attackSpeedText.text = ko_attackspeed + _player._mSubStat._AttackSpeed + " -" + tempattackspeed;
        } else
        {
            attackSpeedText.color = normalColor;
            attackSpeedText.text = ko_attackspeed + _player._mSubStat._AttackSpeed ;
        }
        if (tempmovespeed > 0f)
        {
            moveSpeedText.color = addedColor;
            moveSpeedText.text = ko_movespeed + _player._mSubStat._MoveSpeed * 100 + " +" + tempmovespeed * 100 + "%";
        }
        else
        {
            moveSpeedText.color = normalColor;
            moveSpeedText.text = ko_movespeed + _player._mSubStat._MoveSpeed * 100 + "%";
        }


    }

    // mult: 소수점 몇째자리, 
    // ex) 0.35 * 10 = 3.5 => 4 => return 0.4
    public static float Round(float num, int mult)
    {

        int multnum = 1;
        for (int i = 0; i < mult; i++)
        {
            multnum *= 10;
        }
        return Mathf.Round(num * multnum) / multnum;
    }

    public void AddDexterity(int num=1)
    {
        int sum = tempStatDex + tempStatInt + tempStatStr + tempStatVit;
        if (_player._statPoint - sum <= 0)
        {
            // Doesn't have enough stat point
            return;
        }

        // change strtext, change color of text
        // update the stats shown below
        // save

        tempStatDex += num;
        if (tempStatDex != 0)
        {
            dexText.color = addedColor;
            dexText.text = ko_dexterity + _player._mStats._Dexterity + "  +" + tempStatDex;
        }
        else
        {
            dexText.color = normalColor;
            dexText.text = ko_dexterity + _player._mStats._Dexterity;
        }

        UpdateSubstatText();
        //dexText.text = "민첩: " + _player._mStats._Dexterity + "  +" + tempStatDex;

        // update substats
        

        //for (int i = 0; i < num; i++)
        //{
        //    _player._mSubStat._Attack += 1f;
        //    _player._mSubStat._AttackSpeed -= 0.01f;
        //}

        //_player._mStats._Dexterity += num;
        //_player._statPoint -= num;
        //UpdateStatUI();
    }

    
    public void AddStrength(int num=1)
    {
        int sum = tempStatDex + tempStatInt + tempStatStr + tempStatVit;
        if (_player._statPoint - sum <= 0)
        {
            if (num == -1 && _player._mStats._Dexterity + num >= 0)
                Debug.Log("pass");
            else
                return;
            // Doesn't have enough stat point
        }
        tempStatStr += num;
        if (tempStatStr != 0)
        {
            strText.color = addedColor;
            strText.text = ko_strength + _player._mStats._Strength + "  +" + tempStatStr;
        }
        else
        {
            strText.color = normalColor;
            strText.text = ko_strength + _player._mStats._Strength;
        }
        UpdateSubstatText();

        //_player._mStats._Strength += num;
        //for (int i = 0; i < num; i++)
        //{
        //    _player._mSubStat._Attack += 3.0f;
        //    _player._mSubStat._MaxHP += 10.0f;
        //}
        //_player._statPoint -= num;
        //UpdateStatUI();
        //UpdateSubstats();
    }

    public void AddIntelligence(int num=1)
    {
        int sum = tempStatDex + tempStatInt + tempStatStr + tempStatVit;
        if (_player._statPoint - sum <= 0)
        {
            // Doesn't have enough stat point
            return;
        }
        tempStatInt += num;
        // handles mainstat + signs
        if (tempStatInt != 0)
        {
            intText.color = addedColor;
            intText.text = ko_intelligence + _player._mStats._Intelligence + "  +" + tempStatInt;
        }
        else
        {
            intText.color = normalColor;
            intText.text = ko_intelligence + _player._mStats._Intelligence;
        }
        UpdateSubstatText();
        //_player._mStats._Intelligence += num;
        //for (int i = 0; i < num; i++)
        //{
        //    _player._mSubStat._MagicAttack += 4.0f;
        //    _player._mSubStat._Mana += 5.0f;
        //    _player._mSubStat._ManaRegen += 0.1f;
        //}
        //_player._statPoint -= num;
        //UpdateStatUI();
    }

    public void AddVitality(int num=1)
    {
        int sum = tempStatDex + tempStatInt + tempStatStr + tempStatVit;
        if (_player._statPoint - sum <= 0)
        {
            // Doesn't have enough stat point
            Debug.Log(_player._statPoint + " " + sum);
            return;
        }

        tempStatVit += num;

        if (tempStatVit != 0)
        {
            vitText.color = addedColor;
            vitText.text = ko_vitality + _player._mStats._Vitality + "  +" + tempStatVit;
        }
        else
        {
            vitText.color = normalColor;
            vitText.text = ko_vitality + _player._mStats._Vitality;
        }

        UpdateSubstatText();

        //for (int i = 0; i < num; i++)
        //{
        //    _player._mSubStat._MaxHP += 25.0f;
        //    _player._mSubStat._HealthRegen += 0.5f;
        //}
        //_player._mStats._Vitality += num;
        //_player._statPoint -= num;
        //UpdateStatUI();
    }

    public void ConfirmStat()
    {
        int sum = tempStatDex + tempStatInt + tempStatStr + tempStatVit;
        _player._statPoint -= sum;
        if (tempStatDex != 0)
        {
            _player.AddDex(tempStatDex);
        }
        if (tempStatStr != 0)
        {
            _player.AddStr(tempStatStr);
        }
        if (tempStatInt != 0)
        {
            _player.AddInt(tempStatInt);
        }
        if (tempStatVit != 0)
        {
            _player.AddVit(tempStatVit);
        }

        // 
        UpdateStatUI();
        UpdateToNormal();
        tempStatDex = 0;
        tempStatInt = 0;
        tempStatStr = 0;
        tempStatVit = 0;

    }
}
