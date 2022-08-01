using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� - �� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
    public class ArmorItemData : EquipmentItemData
    {
        /// <summary> ���� </summary>
        public int Defense => _defense;

        [SerializeField] private int _defense = 1;
        public override Item CreateItem()
        {
            return new ArmorItem(this);
        }
    }
}