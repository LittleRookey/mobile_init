using UnityEngine;


namespace Litkey.InventorySystem
{
    
    
    /// <summary> 장비 아이템 </summary>
    public abstract class EquipmentItemData : ItemData
    {

        
        public int MaxDurability => _maxDurability;

        [SerializeField] private int _maxDurability = 100;
    }
}