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
    
    /// <summary> ��� ������ </summary>
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
                    return "���ݷ� ";
                case StatType.attackSpeed:
                    return "���� �ӵ� ";
                case StatType.defense:
                    return "���� ";
                case StatType.hpRegen:
                    return "ü�� ȸ�� ";
                case StatType.magicForce:
                    return "���� ";
                case StatType.maxHP:
                    return "�ִ� ü�� ";
                case StatType.moveSpeed:
                    return "�̵� �ӵ� ";
            }
            return "";
        }
    }

}