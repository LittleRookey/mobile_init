using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Litkey.InventorySystem
{
    public enum OperatorType
    {
        plus,
        multiply,
        divide,
        subtract

    }
    
    /// <summary> 장비 아이템 </summary>
    public abstract class EquipmentItemData : ItemData
    {
        public int MaxDurability => _maxDurability;
        [SerializeField] private int _maxDurability = 100;
        [SerializeField] protected StatModifier[] stats;

        public StatModifier[] GetStats()
        {
            return stats;
        }
    }

    [System.Serializable]
    public class StatModifier
    {
        public StatType statType;
        public ObscuredFloat value;
        public OperatorType oper;

    }

}