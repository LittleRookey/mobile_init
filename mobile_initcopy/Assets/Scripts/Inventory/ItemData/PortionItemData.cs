using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
    public class PortionItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        [SerializeField] protected StatModifier[] stats;
        public override Item CreateItem()
        {
            return new PortionItem(this);
        }

        public StatModifier[] GetStats()
        {
            return stats;
        }

    }
}