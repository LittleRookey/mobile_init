using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> �Һ� ������ ���� </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
    public class PortionItemData : CountableItemData
    {
        /// <summary> ȿ����(ȸ���� ��) </summary>
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