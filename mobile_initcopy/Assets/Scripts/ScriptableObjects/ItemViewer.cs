using UnityEngine;
using Litkey.InventorySystem;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "ItemDataSettings", menuName = "Litkey/ItemViewer")]
public class ItemViewer : ScriptableObject
{

    /*
     * 노말 = 0,
        매직 = 1,
        레어 = 2,
        유니크 = 3,
        레전드 = 4,
        신화 = 5,
        초월 = 6
     */
    public ItemViewSlot itemViewSlot;

    public readonly string empty = "";
    public readonly string newLine = "\n";
    public readonly string normal = "[일반] ";
    public readonly string rare = "[레어] ";
    public readonly string magic = "[매직] ";
    public readonly string unique = "[유니크] ";
    public readonly string legend = "[레전드] ";
    public readonly string myth = "[신화] ";
    public readonly string transcend = "[초월] ";

    public readonly string openParenth = "[";
    public readonly string closeParenth = "] ";

    public readonly string attack = "공격력 ";
    public readonly string defense = "방어력 ";
    public readonly string hpRegen = "체력 회복 ";
    public readonly string attackspeed = "공격 속도 ";
    public readonly string movespeed = "이동 속도 ";
    public readonly string magicforce = "마력 ";
    public readonly string maxHp = "최대 체력 ";
    public readonly string currentHP = "현재 체력 ";

    public readonly string plus = "+";
    public readonly string minus = "-";

    public readonly string percent = "%";

    public Color item_normalColor = new Color32(211, 211, 211, 255); // D3D3D3
    public Color item_magicColor = new Color32(0, 191, 0, 255); // 00BF00
    public Color item_rareColor = new Color32(0, 191, 255, 255); // 00BFFF
    public Color item_uniqueColor = new Color32(148, 0, 211, 255); // 9400D3
    public Color item_legendColor = new Color32(255, 140, 0, 255); // FF8C00
    public Color item_mythColor = new Color32(255, 215, 0, 255); // FFD700
    public Color item_transcendColor = new Color32(191, 191, 191, 255); // BFBFBF

    public ItemViewSlot itemViewGO;

    public Color GetColorOf(EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case EquipmentRarity.노말:
                return item_normalColor;
            case EquipmentRarity.매직:
                return item_magicColor;
            case EquipmentRarity.레어:
                return item_rareColor;
            case EquipmentRarity.유니크:
                return item_uniqueColor;
            case EquipmentRarity.레전드:
                return item_legendColor;
            case EquipmentRarity.신화:
                return item_mythColor;
            case EquipmentRarity.초월:
                return item_transcendColor;
        }
        return item_normalColor;
    }

    public void OpenSlot(ItemData item, GameObject parent = null)
    {
        if (itemViewGO == null)
        {
            if (parent != null)
                itemViewGO = Instantiate(itemViewSlot.gameObject, parent.transform).GetComponent<ItemViewSlot>();
            else
                itemViewGO = Instantiate(itemViewSlot.gameObject).GetComponent<ItemViewSlot>();
            
        }

        itemViewGO.SetSlot(item);
        itemViewGO.gameObject.SetActive(true);
    }
}
