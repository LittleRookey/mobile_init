using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


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
    public StatType _statType;
    public TextMeshProUGUI _statTotalStatText;
    public TextMeshProUGUI _statExplanationText;
    public TextMeshProUGUI _statLeftText;
    public GameObject _expBar;

    public bool isHidden;

    private static readonly string statup = "스텟 증가 +";
    public void UpdateStat(SA_Unit player)
    {
        if (isHidden)
        {
            // hide or lock the stat
        } else
        {
            // if stat is not locked,
            switch(_statType)
            {
                case StatType.maxHP: // TODO 
                    _slotName.text = "최대 체력";
                    _statTotalStatText.text = StatManager.Instance._player._unitMaxHP.ToString("F0");
                    _statExplanationText.text = statup + StatManager.MAXHPGROW.ToString("F0");
                    _statLeftText.text = StatManager.Instance._player.GetLeftStatPoint().ToString("F0") + "/ 1";
                    break;
                case StatType.attack:
                    _slotName.text = "공격력";
                    _statTotalStatText.text = StatManager.Instance._player._unitAttack.ToString("F0");
                    _statExplanationText.text = statup + StatManager.ATTACKGROW.ToString("F0");
                    _statLeftText.text = StatManager.Instance._player.GetLeftStatPoint().ToString("F0") + "/ 1";
                    break;
                case StatType.hpRegen:
                    _slotName.text = "체력 재생";
                    _statTotalStatText.text = StatManager.Instance._player._unitHPRegen.ToString("F0");
                    _statExplanationText.text = statup + StatManager.HPREGENGROW.ToString("F0");
                    _statLeftText.text = StatManager.Instance._player.GetLeftStatPoint().ToString("F0") + "/ 1";
                    break;
                case StatType.magicForce:
                    _slotName.text = "마력";
                    _statTotalStatText.text = StatManager.Instance._player._unitMagicForce.ToString("F0");
                    _statExplanationText.text = statup + 0;
                    _statLeftText.text = StatManager.Instance._player.GetLeftStatPoint().ToString("F0") + "/ 1";
                    break;
            }
        }
    }
    
}
