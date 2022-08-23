using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.InventorySystem;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ItemViewer itemViewer;

    public Image icon;
    public GameObject onDropEffect;
    public TextMeshProUGUI itemText;
    public Color textColor;
    public string itemName;
    public int itemCount;
    public ItemData itemData;

    string num = " x";


    public void SetDropItem(Sprite icon, string itemName, int itemCount = 1)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }
    public void SetDropItem(Sprite icon, string itemName, int itemCount, EquipmentRarity rarity, ItemData itemData)
    {
        this.itemData = itemData;

        this.textColor = itemViewer.GetColorOf(rarity);
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.itemText.color = this.textColor;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;


    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, bool showCountText=false)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = itemViewer.item_normalColor;
        this.itemText.color = this.textColor;

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        if (showCountText)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, EquipmentRarity rarity, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        this.textColor = itemViewer.GetColorOf(rarity);
        this.itemText.color = this.textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = itemViewer.item_normalColor;
        this.itemText.color = this.textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }
}
