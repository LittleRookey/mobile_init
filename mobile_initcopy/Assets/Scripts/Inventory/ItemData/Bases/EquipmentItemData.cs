using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 장비 아이템 </summary>
    public abstract class EquipmentItemData : ItemData
    {
        /// <summary> 최대 내구도 </summary>
        public int MaxDurability => _maxDurability;

        [SerializeField] private int _maxDurability = 100;
    }
}