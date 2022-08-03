using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.InventorySystem;

public class DropItem : MonoBehaviour
{
    public Image icon;
    public GameObject onDropEffect;
    public TextMeshProUGUI itemText;
    public Color textColor;

    // 노말, 매직, 레어, 유니크, 레전드, 신화, 초월
    [SerializeField] private Color item_normalColor;
    [SerializeField] private Color item_magicColor;
    [SerializeField] private Color item_rareColor;
    [SerializeField] private Color item_uniqueColor;
    [SerializeField] private Color item_legendColor;
    [SerializeField] private Color item_mythColor;
    [SerializeField] private Color item_transcendColor;

    string num = " x";


    public void SetDropItem(Sprite icon, string itemName, int itemCount = 1)
    {
        this.icon.sprite = icon;

        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }
    public void SetDropItem(Sprite icon, string itemName, int itemCount, EquipmentRarity rarity)
    {
        this.icon.sprite = icon;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        switch (rarity)
        {
            case EquipmentRarity.노말:
                this.textColor = item_normalColor;
                break;
            case EquipmentRarity.매직:
                this.textColor = item_magicColor;
                break;
            case EquipmentRarity.레어:
                this.textColor = item_rareColor;
                break;
            case EquipmentRarity.유니크:
                this.textColor = item_uniqueColor;
                break;
            case EquipmentRarity.레전드:
                this.textColor = item_legendColor;
                break;
            case EquipmentRarity.신화:
                this.textColor = item_mythColor;
                break;
            case EquipmentRarity.초월:
                this.textColor = item_transcendColor;
                break;
        }

    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor)
    {
        this.icon.sprite = icon;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, bool showCountText=false)
    {
        this.icon.sprite = icon;
        this.itemText.text = itemName;
        this.itemText.color = this.textColor;

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        if (showCountText)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, EquipmentRarity rarity, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
        switch (rarity)
        {
            case EquipmentRarity.노말:
                this.textColor = item_normalColor;
                break;
            case EquipmentRarity.매직:
                this.textColor = item_magicColor;
                break;
            case EquipmentRarity.레어:
                this.textColor = item_rareColor;
                break;
            case EquipmentRarity.유니크:
                this.textColor = item_uniqueColor;
                break;
            case EquipmentRarity.레전드:
                this.textColor = item_legendColor;
                break;
            case EquipmentRarity.신화:
                this.textColor = item_mythColor;
                break;
            case EquipmentRarity.초월:
                this.textColor = item_transcendColor;
                break;
        }
        this.itemText.color = this.textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemText.text = itemName;
        this.itemText.color = this.textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }


}
