using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 장비 - 방어구 아이템 </summary>
    [CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
    public class ArmorItemData : EquipmentItemData
    {
        /// <summary> 방어력 </summary>
        public override Item CreateItem()
        {
            return new ArmorItem(this);
        }

    }
}