using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum StatType
{
    maxHP,
    magicForce,
    attack,
    hpRegen
};

public enum SlotType
{

}
public class Slot : MonoBehaviour
{
    public TextMeshProUGUI _slotName;
    
}
public class StatSlot : Slot
{
    public Image _icon;
    public StatType _statType;
    public TextMeshProUGUI _statTotalStatText;
    public TextMeshProUGUI _statExplanationText;
    public TextMeshProUGUI _statLeftText;

    public Button explanationButton;
    public Button statupButton;

    public int statLevel;

    public bool isHidden;
    public GameObject disabled;

    private static readonly string statup = "���� ����";
    private static readonly string openParenth = " (+";
    private static readonly string closeParenth = ")";

    //public void AddStat(SA_Unit sa, StatType type)
    //{
    //    StatManager.Instance.AddStat(type);
    //    UpdateStat(sa);
    //}

    private void OnEnable()
    {
        statupButton.onClick.AddListener(UpdateStat);
    }

    private void OnDisable()
    {
        statupButton.onClick.RemoveListener(UpdateStat);
    }

    public void UpdateStat()
    {
        if (isHidden)
        {
            // hide or lock the stat
            disabled.gameObject.SetActive(true);
        }
        else
        {
            disabled.gameObject.SetActive(false);
            SA_Unit tplayer = StatManager.Instance._player;
            // if stat is not locked,
            switch (_statType)
            {
                case StatType.maxHP: // TODO 
                    Debug.Log("Update slot!");
                    _slotName.text = "�ִ� ü�� +" + StatManager.Instance.statpoint_MaxHP;
                    _statTotalStatText.text = tplayer._unitMaxHP.ToString("F0") + openParenth + StatManager.MAXHPGROW + closeParenth;
                    _statExplanationText.text = statup;
                    
                    //explanationButton.onClick 
                    _statLeftText.text = 1.ToString();
                    break;
                case StatType.attack:
                    _slotName.text = "���ݷ� +" + StatManager.Instance.statpoint_Attack;
                    _statTotalStatText.text = tplayer._unitAttack.ToString("F0") + openParenth + StatManager.ATTACKGROW + closeParenth;
                    _statExplanationText.text = statup;
                    _statLeftText.text = 1.ToString();
                    break;
                case StatType.hpRegen:
                    _slotName.text = "ü�� ��� +" + StatManager.Instance.statpoint_HPRegen;
                    _statTotalStatText.text = tplayer._unitHPRegen.ToString("F1") + openParenth + StatManager.HPREGENGROW + closeParenth;
                    _statExplanationText.text = statup;
                    _statLeftText.text = 1.ToString();
                    break;
                case StatType.magicForce:
                    _slotName.text = "���� +" + StatManager.Instance.statpoint_MagicForce;
                    _statTotalStatText.text = tplayer._unitMagicForce.ToString("F1");
                    _statExplanationText.text = statup;
                    _statLeftText.text = 1.ToString();
                    break;
            }
        }
    }

    public void UpdateStat(SA_Unit player, Sprite icon)
    {
        if (isHidden)
        {
            // hide or lock the stat
            disabled.gameObject.SetActive(true);
        } else
        {
            disabled.gameObject.SetActive(false);
            SA_Unit tplayer = StatManager.Instance._player;
            _icon.sprite = icon;
            // if stat is not locked,
            switch(_statType)
            {
                case StatType.maxHP: // TODO 
                    Debug.Log("Update slot!");
                    _slotName.text = "�ִ� ü�� +" + StatManager.Instance.statpoint_MaxHP;
                    _statTotalStatText.text = tplayer._unitMaxHP.ToString("F0") + openParenth + StatManager.MAXHPGROW + closeParenth;
                    _statExplanationText.text = statup;
                    //explanationButton.onClick 
                    _statLeftText.text = 1.ToString();
                    break;
                case StatType.attack:
                    _slotName.text = "���ݷ� +" + StatManager.Instance.statpoint_Attack;
                    _statTotalStatText.text = tplayer._unitAttack.ToString("F0") + openParenth + StatManager.ATTACKGROW + closeParenth;
                    _statExplanationText.text = statup;
                    _statLeftText.text = 1.ToString();
                    break;
                case StatType.hpRegen:
                    _slotName.text = "ü�� ��� +" + StatManager.Instance.statpoint_HPRegen; 
                    _statTotalStatText.text = tplayer._unitHPRegen.ToString("F1") + openParenth + StatManager.HPREGENGROW + closeParenth;
                    _statExplanationText.text = statup;
                    _statLeftText.text = 1.ToString();
                    break;
                case StatType.magicForce:
                    _slotName.text = "���� +" + StatManager.Instance.statpoint_MagicForce;
                    _statTotalStatText.text = tplayer._unitMagicForce.ToString("F1");
                    _statExplanationText.text = statup;
                    _statLeftText.text = 1.ToString();
                    break;
            }
        }
    }
    
}
