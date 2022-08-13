using UnityEngine;

namespace Litkey.InventorySystem
{
    /// <summary> �� �� �ִ� ������ ������ </summary>
    public abstract class CountableItemData : ItemData
    {
        public int MaxAmount => _maxAmount;
        [SerializeField] private int _maxAmount = 99;
    }
}