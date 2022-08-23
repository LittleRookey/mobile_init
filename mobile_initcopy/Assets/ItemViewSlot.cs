using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.InventorySystem;
using UnityEngine.Events;

public interface ISlot
{
    public void SetSlot(EquipmentItem item);
}
public class ItemViewSlot : MonoBehaviour
{
    [SerializeField] private ItemViewer itemViewer;
    /*
     * 노말 = 0,
        매직 = 1,
        레어 = 2,
        유니크 = 3,
        레전드 = 4,
        신화 = 5,
        초월 = 6
     */
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private TextMeshProUGUI foundMapText;
    [SerializeField] private TextMeshProUGUI explanationText;

    [SerializeField] private Image iconBG;
    [SerializeField] private Image icon;
    [SerializeField] private Image windowBG;

    private string titleString;
    private string statString;
    private string foundMapString;
    private string explanationString;


    //string empty = "";
    //string newLine = "\n";
    //private string normal = "[일반] ";
    //private string rare = "[레어] ";
    //private string magic = "[매직] ";
    //private string unique = "[유니크] ";
    //private string legend = "[레전드] ";
    //private string myth = "[신화] ";
    //private string transcend = "[초월] ";

    //private string openParenth = "[";
    //private string closeParenth = "] ";

    //private string attack = "공격력 ";
    //private string defense = "방어력 ";
    //private string hpRegen = "체력 회복 ";
    //private string attackspeed = "공격 속도 ";
    //private string movespeed = "이동 속도 ";
    //private string magicforce = "마력 ";
    //private string maxHp = "최대 체력 ";


    //private string plus = "+";
    //private string minus = "-";

    //private string percent = "%";

    //public Color item_normalColor = new Color32(211, 211, 211, 255);
    //public Color item_magicColor = new Color32(0, 191, 0, 255);
    //public Color item_rareColor = new Color32(0, 191, 255, 255);
    //public Color item_uniqueColor = new Color32(148, 0, 211, 255);
    //public Color item_legendColor = new Color32(255, 140, 0, 255);
    //public Color item_mythColor = new Color32(255, 215, 0, 255);
    //public Color item_transcendColor = new Color32(191, 191, 191, 255);

    public UnityAction OnOpenItemSlotInfo;

    public void SetSlot(ItemData item)
    {
        if (item is EquipmentItemData)
            SetInfo((EquipmentItemData)item);
        else if (item is PortionItemData)
            SetInfo((PortionItemData)item);
        //TODO countable that is not portion

        
    }

    private void ResetSlot()
    {
        titleString = itemViewer.empty;
        statString = itemViewer.empty;
        foundMapString = itemViewer.empty;
        explanationString = itemViewer.empty;
        icon.sprite = null;
    }

    private void ApplySlot()
    {
        titleText.text = titleString;
        statText.text = statString;
        explanationText.text = explanationString;

    }

    private void SetInfo(EquipmentItemData item)
    {
        ResetSlot();
        icon.sprite = item.IconSprite;
        titleString = itemViewer.openParenth + item.rarity.ToString() + itemViewer.closeParenth + item.Name;
        explanationString = item.Tooltip;

        //TODO set color of title text
        titleText.color = itemViewer.GetColorOf(item.rarity);

        iconBG.color = titleText.color;
        windowBG.color = titleText.color;

        foreach (StatModifier st in item.GetStats())
        {
            switch (st.statType)
            {
                case StatType.attack:
                    statString += itemViewer.attack;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.attackSpeed:
                    statString += itemViewer.attackspeed;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    statString += itemViewer.percent;
                    break;
                case StatType.defense:
                    statString += itemViewer.defense;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.hpRegen:
                    statString += itemViewer.hpRegen;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.magicForce:
                    statString += itemViewer.magicforce;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.maxHP:
                    statString += itemViewer.maxHp;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.moveSpeed:
                    statString += itemViewer.movespeed;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    statString += itemViewer.percent;
                    break;
            }
            statString += itemViewer.newLine;
        }
        ApplySlot();

    }

    private void SetInfo(PortionItemData item)
    {
        ResetSlot();
        icon.sprite = item.IconSprite;
        titleString = itemViewer.openParenth + item.rarity.ToString() + itemViewer.closeParenth + item.Name;
        explanationString = item.Tooltip;

        //TODO set color of title text
        titleText.color = itemViewer.GetColorOf(item.rarity);

        iconBG.color = titleText.color;
        windowBG.color = titleText.color;

        foreach (StatModifier st in item.GetStats())
        {
            switch (st.statType)
            {
                case StatType.attack:
                    statString += itemViewer.attack;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.attackSpeed:
                    statString += itemViewer.attackspeed;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    statString += itemViewer.percent;
                    break;
                case StatType.defense:
                    statString += itemViewer.defense;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.hpRegen:
                    statString += itemViewer.hpRegen;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.magicForce:
                    statString += itemViewer.magicforce;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.maxHP:
                    statString += itemViewer.maxHp;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    break;
                case StatType.moveSpeed:
                    statString += itemViewer.movespeed;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    statString += itemViewer.percent;
                    break;
                case StatType.currentHP:
                    statString += itemViewer.currentHP;
                    statString += st.value >= 0 ? itemViewer.plus : itemViewer.minus;
                    statString += st.value;
                    statString += itemViewer.percent;
                    break;
            }
            statString += itemViewer.newLine;
        }
        ApplySlot();

    }
}
