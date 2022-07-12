using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CleverCrow.Fluid.StatsSystem;
using UnityEngine.Events;

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

    public SA_Unit _player;
    [Header("MainStats")]

    //[SerializeField] private TextMeshProUGUI strText;
    //[SerializeField] private TextMeshProUGUI dexText;
    //[SerializeField] private TextMeshProUGUI intText;
    //[SerializeField] private TextMeshProUGUI vitText;
    //[SerializeField] private TextMeshProUGUI statPointText;

    //[Header("Substats")]
    //[SerializeField] private TextMeshProUGUI maxHPText;
    //[SerializeField] private TextMeshProUGUI manaText;
    //[SerializeField] private TextMeshProUGUI attackText;
    //[SerializeField] private TextMeshProUGUI magicAttackText;
    //[SerializeField] private TextMeshProUGUI defText;
    //[SerializeField] private TextMeshProUGUI magicDefText;
    //[SerializeField] private TextMeshProUGUI hpRegenText;
    //[SerializeField] private TextMeshProUGUI manaRegenText;
    //[SerializeField] private TextMeshProUGUI attackSpeedText;
    //[SerializeField] private TextMeshProUGUI moveSpeedText;

    public StatDefinition _attack;
    public StatModifier statMod;

    //private int tempStatDex;
    //private int tempStatStr;
    //private int tempStatInt;
    //private int tempStatVit;

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


    public static readonly float ATTACKGROW = 3.0f;
    public static readonly float MAXHPGROW = 10.0f;
    public static readonly float DEFENSEGROW = 1.0f;
    public static readonly float HPREGENGROW = 0.2f;

    //public static readonly float STR_ATTACKGROW = 6.0f;
    //public static readonly float STR_MAXHPGROW = 15.0f;

    //public static readonly float INT_MAGICATTACKGROW = 4.0f;
    //public static readonly float INT_MANAGROW = 5.0f;
    //public static readonly float INT_MANAGENGROW = 0.1f;

    //public static readonly float DEX_ATTACKGROW = 3.0f;
    //public static readonly float DEX_ATTACKSPEEDGROW = 0.01f;
    //public static readonly float DEX_MOVESPEEDGROW = 0.01f;

    //public static readonly float VIT_MAXHPGROW = 25.0f;
    //public static readonly float VIT_DEFENSEGROW = 2.0f;
    //public static readonly float VIT_MAGICDEFENSEGROW = 1.0f;
    //public static readonly float VIT_HPGENGROW = 0.5f;


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
    public static readonly string ko_strength = "��: ";
    public static readonly string ko_dexterity = "��ø: ";
    public static readonly string ko_intelligence = "����: ";
    public static readonly string ko_vitality = "������: ";
    public static readonly string ko_maxHP = "�ִ� ü��: ";
    public static readonly string ko_mana = "����: ";
    public static readonly string ko_attack = "���� ���ݷ�: ";
    public static readonly string ko_magicattack = "���� ���ݷ�: ";
    public static readonly string ko_defense = "���� ����: ";
    public static readonly string ko_magicdefense = "���� ����: ";
    public static readonly string ko_hpgen = "ü�� ȸ����: ";
    public static readonly string ko_managen = "���� ȸ����: ";
    public static readonly string ko_attackspeed = "���� �ӵ�: ";
    public static readonly string ko_movespeed = "�̵� �ӵ�: ";
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //tempStatLeft = _player._statPoint; // TODO when player opens the stat window
        //tempStatDex = 0;


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

    public float GetFinalMaxHP()
    {
        // TODO add any modifiers
        // (100 + 10) * 1.3 
        return _player._unitMaxHP;
    }

    //void UpdateToNormal()
    //{
    //    //UpdateStatUI();
    //    maxHPText.text = ko_maxHP + _player._mSubStat._MaxHP;
    //    manaText.text = ko_mana + _player._mSubStat._Mana;
    //    attackText.text = ko_attack + _player._mSubStat._Attack;
    //    magicAttackText.text = ko_magicattack + _player._mSubStat._MagicAttack;
    //    defText.text = ko_defense + _player._mSubStat._Defense;
    //    magicDefText.text = ko_magicdefense + _player._mSubStat._MagicDefense;
    //    hpRegenText.text = ko_hpgen + _player._mSubStat._HealthRegen + "/��";
    //    manaRegenText.text = ko_managen + _player._mSubStat._ManaRegen + "/��";
    //    attackSpeedText.text = ko_attackspeed + _player._mSubStat._AttackSpeed;
    //    moveSpeedText.text = ko_movespeed + _player._mSubStat._MoveSpeed * 100 + "%";

    //    maxHPText.color = normalColor;
    //    manaText.color = normalColor;
    //    attackText.color = normalColor;
    //    magicAttackText.color = normalColor;
    //    defText.color = normalColor;
    //    magicDefText.color = normalColor;
    //    hpRegenText.color = normalColor;
    //    manaRegenText.color = normalColor;
    //    attackSpeedText.color = normalColor;
    //    moveSpeedText.color = normalColor;

    //    strText.color = normalColor;
    //    dexText.color = normalColor;
    //    intText.color = normalColor;
    //    vitText.color = normalColor;
    //}



    //void UpdateSubstatText()
    //{
    //    tempattack = 0;
    //    tempmaxhp = 0;
    //    tempmana = 0;
    //    tempmagicattack = 0;
    //    tempdefense = 0;
    //    tempmagicdefense = 0;
    //    tempHPGen = 0;
    //    tempManaGen = 0;
    //    tempattackspeed = 0;
    //    tempmovespeed = 0;

    //    if (tempStatStr > 0)
    //    {
    //        tempattack += (STR_ATTACKGROW * tempStatStr);
    //        tempmaxhp += (STR_MAXHPGROW * tempStatStr);            
    //    }
    //    if (tempStatInt > 0)
    //    {
    //        tempmagicattack += (INT_MAGICATTACKGROW * tempStatInt);
    //        tempmana += (INT_MANAGROW * tempStatInt);
    //        tempManaGen += (INT_MANAGENGROW * tempStatInt);
    //    }
    //    if (tempStatDex > 0)
    //    {
    //        tempattack += (DEX_ATTACKGROW * tempStatDex);
    //        tempattackspeed += (DEX_ATTACKSPEEDGROW * tempStatDex);
    //        tempmovespeed += (DEX_MOVESPEEDGROW * tempStatDex);

    //    }
    //    if (tempStatVit > 0)
    //    {
    //        tempmaxhp += (VIT_MAXHPGROW * tempStatVit);
    //        tempdefense += (VIT_DEFENSEGROW * tempStatVit);
    //        tempmagicdefense += (VIT_MAGICDEFENSEGROW * tempStatVit);
    //        tempHPGen += (VIT_HPGENGROW * tempStatVit);

    //    }

    //    // round if 14.9999 => 15, 14.5 => 14.5, 14.49999 => 144.9999 => 145 / 10 => 14.5, 0.21, 0.2099999 => 209999 =>
    //    // round(val * 10)  => 145
    //    //tempmaxhp = Round(tempmaxhp, 2);
    //    tempHPGen = Round(tempHPGen, 2);
    //    tempattackspeed = Round(tempattackspeed, 2);
    //    tempmovespeed = Round(tempmovespeed, 2);

    //    // check tempsubstat variables and update 
    //    Debug.Log("tempmaxhp: " + tempmaxhp);
    //    if (tempmaxhp > 0f)
    //    {
    //        maxHPText.color = addedColor;
    //        maxHPText.text = ko_maxHP + _player._mSubStat._MaxHP + "  +" + tempmaxhp;
    //    } else
    //    {
    //        maxHPText.color = normalColor;
    //        maxHPText.text = ko_maxHP + _player._mSubStat._MaxHP;
    //    }
    //    if (tempmana > 0f)
    //    {
    //        manaText.color = addedColor;
    //        manaText.text = ko_mana + _player._mSubStat._Mana + "  +" + tempmana;
    //    } else
    //    {
    //        manaText.color = normalColor;
    //        manaText.text = ko_mana + _player._mSubStat._Mana;
    //    }
    //    if (tempattack > 0f)
    //    {
    //        attackText.color = addedColor;
    //        attackText.text = ko_attack + _player._mSubStat._Attack + "  +" + tempattack;
    //    } else
    //    {
    //        attackText.color = normalColor;
    //        attackText.text = ko_attack + _player._mSubStat._Attack;
    //    }
    //    if (tempmagicattack > 0f)
    //    {
    //        magicAttackText.color = addedColor;
    //        magicAttackText.text = ko_magicattack + _player._mSubStat._MagicAttack + "  +" + tempmagicattack;
    //    } else
    //    {
    //        magicAttackText.color = normalColor;
    //        magicAttackText.text = ko_magicattack + _player._mSubStat._MagicAttack;
    //    }
    //    if (tempdefense > 0f)
    //    {
    //        defText.color = addedColor;
    //        defText.text = ko_defense + _player._mSubStat._Defense + " +" + tempdefense;
    //    } else
    //    {
    //        defText.color = normalColor;
    //        defText.text = ko_defense + _player._mSubStat._Defense;
    //    }
    //    if (tempmagicdefense > 0f)
    //    {
    //        magicDefText.color = addedColor;
    //        magicDefText.text = ko_magicdefense + _player._mSubStat._MagicDefense + " +" + tempmagicdefense;
    //    }
    //    else
    //    {
    //        magicDefText.color = normalColor;
    //        magicDefText.text = ko_magicdefense + _player._mSubStat._MagicDefense;
    //    }
    //    if (tempHPGen > 0f)
    //    {
    //        hpRegenText.color = addedColor;
    //        hpRegenText.text = ko_hpgen + _player._mSubStat._HealthRegen + " +" + tempHPGen + "/��";
    //    }
    //    else
    //    {
    //        hpRegenText.color = normalColor;
    //        hpRegenText.text = ko_hpgen + _player._mSubStat._HealthRegen + "/��";
    //    }
    //    if (tempManaGen > 0f)
    //    {
    //        manaRegenText.color = addedColor;
    //        manaRegenText.text = ko_managen + _player._mSubStat._ManaRegen + " +" + tempManaGen + "/��";
    //    } else
    //    {
    //        manaRegenText.color = normalColor;
    //        manaRegenText.text = ko_managen + _player._mSubStat._ManaRegen + "/��";
    //    }
    //    if (tempattackspeed > 0f)
    //    {
    //        attackSpeedText.color = addedColor;
    //        attackSpeedText.text = ko_attackspeed + _player._mSubStat._AttackSpeed + " -" + tempattackspeed;
    //    } else
    //    {
    //        attackSpeedText.color = normalColor;
    //        attackSpeedText.text = ko_attackspeed + _player._mSubStat._AttackSpeed ;
    //    }
    //    if (tempmovespeed > 0f)
    //    {
    //        moveSpeedText.color = addedColor;
    //        moveSpeedText.text = ko_movespeed + _player._mSubStat._MoveSpeed * 100 + " +" + tempmovespeed * 100 + "%";
    //    }
    //    else
    //    {
    //        moveSpeedText.color = normalColor;
    //        moveSpeedText.text = ko_movespeed + _player._mSubStat._MoveSpeed * 100 + "%";
    //    }


    //}

    // mult: �Ҽ��� ��°�ڸ�, 
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

}

//public class StatContainer
//{
//    public float _unitMaxHP;
//    public float _unitHP;
//    public int _unitDefense;
//    public float _unitMana;
//    public float _unitAttack;
//    public float _unitMagicForce;

//    public float _unitAttackSpeed;

//    public float _unitAttackRange;
//    public float _unitFightRange;

//    public float _unitHPRegen;
//    public float _unitManaRegen;

//    public float _unitMoveSpeed;

//}