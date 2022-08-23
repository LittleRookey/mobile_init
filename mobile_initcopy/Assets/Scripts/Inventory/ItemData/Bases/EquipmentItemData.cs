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

        public override string ToString()
        {
            switch(statType)
            {
                case StatType.attack:
                    return "공격력 ";
                case StatType.attackSpeed:
                    return "공격 속도 ";
                case StatType.defense:
                    return "방어력 ";
                case StatType.hpRegen:
                    return "체력 회복 ";
                case StatType.magicForce:
                    return "마력 ";
                case StatType.maxHP:
                    return "최대 체력 ";
                case StatType.moveSpeed:
                    return "이동 속도 ";
            }
            return "";
        }
    }

}