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
    
    [SerializeField] private TextMeshProUGUI strText, dexText, intText, vitText, statPointText;

    public StatDefinition _attack;
    public StatModifier statMod;

    
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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateStatUI();
        //var callBack = new UnityAction<StatRecord>((record) => { Debug.Log(record);  });

        //_player._mStatContainer.OnStatChangeSubscribe(ATTACK_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(ATTACKSPEED_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MOVESPEED_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MANA_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MAGICATTACK_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(HEALTHGEN_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(MANAGEN_ID, callBack);
        //_player._mStatContainer.OnStatChangeSubscribe(HP_ID, callBack);
        SubscribeEvents();
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
        _player._Level += 1;
        _player._statPoint += 1;
        _player.LevelUpStats();
        _player._mSubStat._HP = _player._mSubStat._MaxHP;


        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        strText.text = "Strength " + _player._mStats._Strength;
        dexText.text = "Dexterity " + _player._mStats._Dexterity;
        intText.text = "Intelligence " + _player._mStats._Intelligence;
        vitText.text = "Vitality " + _player._mStats._Vitality;
        statPointText.text = "Points left: " + _player._statPoint;
    }
    public void AddDexterity(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            return;
        }
        for (int i = 0; i < num; i++)
        {
            _player._mSubStat._Attack += 1f;
            _player._mSubStat._AttackSpeed -= 0.01f;
        }
        _player._mStats._Dexterity += num;
        _player._statPoint -= num;
        UpdateStatUI();
    }

    public void AddStrength(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            return;
        }

        _player._mStats._Strength += num;
        for (int i = 0; i < num; i++)
        {
            _player._mSubStat._Attack += 3.0f;
            _player._mSubStat._MaxHP += 10.0f;
        }
        _player._statPoint -= num;
        UpdateStatUI();
    }

    public void AddIntelligence(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            return;
        }
        _player._mStats._Intelligence += num;
        for (int i = 0; i < num; i++)
        {
            _player._mSubStat._MagicAttack += 4.0f;
            _player._mSubStat._Mana += 5.0f;
            _player._mSubStat._ManaRegen += 0.1f;
        }
        _player._statPoint -= num;
        UpdateStatUI();
    }

    public void AddVitality(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            Debug.Log(_player._statPoint + " " + num);
            return;
        }

        for (int i = 0; i < num; i++)
        {
            _player._mSubStat._MaxHP += 25.0f;
            _player._mSubStat._HealthRegen += 0.5f;
        }
        _player._mStats._Vitality += num;
        _player._statPoint -= num;
        UpdateStatUI();
    }
}
