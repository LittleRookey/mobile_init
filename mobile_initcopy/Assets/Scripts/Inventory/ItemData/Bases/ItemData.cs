using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Litkey.InventorySystem
{
    public enum EquipmentRarity
    {
        �븻 = 0,
        ���� = 1,
        ���� = 2,
        ����ũ = 3,
        ������ = 4,
        ��ȭ = 5,
        �ʿ� = 6


    };

    /*
        [��� ����]
        ItemData(abstract)
            - CountableItemData(abstract)
                - PortionItemData
            - EquipmentItemData(abstract)
                - WeaponItemData
                - ArmorItemData
    */

    public abstract class ItemData : ScriptableObject
    {
        public int ID => _id;
        public string Name => _name;
        public string Tooltip => _tooltip;
        public Sprite IconSprite => _iconSprite;

        public EquipmentRarity rarity => _rarity;
        public ObscuredInt Weight => _weight;
        /// <summary> �ִ� ������ </summary>
        [SerializeField] private EquipmentRarity _rarity = EquipmentRarity.�븻;
        [SerializeField] private ObscuredInt _weight = 0;
        [SerializeField] private int _id;
        [SerializeField] private string _name;    // ������ �̸�
        [Multiline]
        [SerializeField] private string _tooltip; // ������ ����
        [SerializeField] private Sprite _iconSprite; // ������ ������
        [SerializeField] private GameObject _dropItemPrefab; // �ٴڿ� ������ �� ������ ������

        /// <summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
        public abstract Item CreateItem();

    }
}