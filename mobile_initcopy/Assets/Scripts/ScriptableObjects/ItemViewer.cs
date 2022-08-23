using UnityEngine;
using Litkey.InventorySystem;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "ItemDataSettings", menuName = "Litkey/ItemViewer")]
public class ItemViewer : ScriptableObject
{

    /*
     * �븻 = 0,
        ���� = 1,
        ���� = 2,
        ����ũ = 3,
        ������ = 4,
        ��ȭ = 5,
        �ʿ� = 6
     */
    public ItemViewSlot itemViewSlot;

    public readonly string empty = "";
    public readonly string newLine = "\n";
    public readonly string normal = "[�Ϲ�] ";
    public readonly string rare = "[����] ";
    public readonly string magic = "[����] ";
    public readonly string unique = "[����ũ] ";
    public readonly string legend = "[������] ";
    public readonly string myth = "[��ȭ] ";
    public readonly string transcend = "[�ʿ�] ";

    public readonly string openParenth = "[";
    public readonly string closeParenth = "] ";

    public readonly string attack = "���ݷ� ";
    public readonly string defense = "���� ";
    public readonly string hpRegen = "ü�� ȸ�� ";
    public readonly string attackspeed = "���� �ӵ� ";
    public readonly string movespeed = "�̵� �ӵ� ";
    public readonly string magicforce = "���� ";
    public readonly string maxHp = "�ִ� ü�� ";
    public readonly string currentHP = "���� ü�� ";

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
            case EquipmentRarity.�븻:
                return item_normalColor;
            case EquipmentRarity.����:
                return item_magicColor;
            case EquipmentRarity.����:
                return item_rareColor;
            case EquipmentRarity.����ũ:
                return item_uniqueColor;
            case EquipmentRarity.������:
                return item_legendColor;
            case EquipmentRarity.��ȭ:
                return item_mythColor;
            case EquipmentRarity.�ʿ�:
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
