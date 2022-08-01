using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� - ���� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weaopn", order = 1)]
    public class WeaponItemData : EquipmentItemData
    {
        /// <summary> ���ݷ� </summary>
        public int Damage => _damage;

        [SerializeField] private int _damage = 1;

        public override Item CreateItem()
        {
            return new WeaponItem(this);
        }
    }
}