using UnityEngine;
using TMPro;

public class StatSlotManager : MonoBehaviour
{
    public StatSlot _MaxHPSlot;
    public StatSlot _AttackSlot;
    public StatSlot _HPRegenSlot;
    public StatSlot _MagicForceSlot;

    public Sprite mHPIcon;
    public Sprite attackIcon;
    public Sprite _HPRegIcon;
    public Sprite _MagicForceIcon;

    public TextMeshProUGUI statText;

    private void Start()
    {
        _MaxHPSlot._statType = StatType.maxHP;
        _AttackSlot._statType = StatType.attack;
        _HPRegenSlot._statType = StatType.hpRegen;
        _MagicForceSlot._statType = StatType.magicForce;
    }

    public void UpdateStatResources(int num)
    {
        statText.text = num.ToString();
        UpdateSlots();
    }


    public void UpdateSlots()
    {
        if (StatManager.Instance._player != null)
        {
            _MaxHPSlot.UpdateStat(StatManager.Instance._player, mHPIcon);
            _AttackSlot.UpdateStat(StatManager.Instance._player, attackIcon);
            _HPRegenSlot.UpdateStat(StatManager.Instance._player, _HPRegIcon);
            _MagicForceSlot.UpdateStat(StatManager.Instance._player, _MagicForceIcon);
        }
    }


}
