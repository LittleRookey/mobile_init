using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;


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
    public GameObject _hpBarPlayer;

    
    [Header("HPBar settings")]
    public bool turnOnHPBarAlways;
    public Vector2 _hpBarPos = new Vector2(0, 0);

    [Header("ETC")]
    public Material hitMatsDefault;
    public Material enemyMatDefault;
    public int gold;
    private int prevGold;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI goldAddOnText;
    [SerializeField] private TextMeshProUGUI expText;
    
    // timer for Character's HP Bar active time
    public static readonly float ENEMYHPTIME = 3f;


    bool isAdding;

    // Stats for base values
  
    //public static readonly int MAGICDEFENSE = 0;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        
    }
    private void Start()
    {
        isAdding = false;
        prevGold =  gold;
        goldAddOnText.gameObject.SetActive(false);
        _normalDamagePrefab.PrewarmPool();
        _swordNormalDamage.PrewarmPool();
        _bowNormalDamage.PrewarmPool();
        _magicNormalDamage.PrewarmPool();
        UpdateGold();
        UpdateExpText(StatManager.Instance._player);
    }

    private void OnEnable()
    {
        UIManager.OnUpdateExpBar += UpdateExpText;
        UIManager.OnUpdateGold += AddGold;
    }

    private void OnDisable()
    {
        UIManager.OnUpdateExpBar -= UpdateExpText;
        UIManager.OnUpdateGold -= AddGold;
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

    public void AddGold(int val)
    {
        prevGold = gold;
        gold += val;

        //StartCoroutine(IncreaseGold(val));
        MT_IncreaseGold(val);
    }

    // call it before the gold add
    private IEnumerator IncreaseGold(int addOn)
    {
        isAdding = true;
        WaitForSeconds sec = new WaitForSeconds(0.5f / addOn);
        goldAddOnText.gameObject.SetActive(true);
        goldAddOnText.text = "+" + addOn.ToString("F0");
        while (prevGold < gold)
        {
            prevGold++;
            goldText.text = prevGold.ToString();
            yield return sec;
        }
        Invoke("TurnOffAddOn", 0.5f);
        isAdding = false;
        
    }

    public void MT_IncreaseGold(int val)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(IncreaseGold(val));
    }
    void TurnOffAddOn()
    {
        goldAddOnText.gameObject.SetActive(false);
    }
    public void UpdateGold()
    {

        goldText.text = gold.ToString();
    }

    public void UpdateExpText(SA_Player sa)
    {
        float tValue = (float)sa._exp / sa._maxExp;
        tValue *= 100f;
        expText.text = tValue.ToString("F2") + "%";
    }
}
