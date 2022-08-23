using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Litkey.InventorySystem
{
    public enum EquipmentRarity
    {
        노말 = 0,
        매직 = 1,
        레어 = 2,
        유니크 = 3,
        레전드 = 4,
        신화 = 5,
        초월 = 6


    };

    /*
        [상속 구조]
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
        /// <summary> 최대 내구도 </summary>
        [SerializeField] private EquipmentRarity _rarity = EquipmentRarity.노말;
        [SerializeField] private ObscuredInt _weight = 0;
        [SerializeField] private int _id;
        [SerializeField] private string _name;    // 아이템 이름
        [Multiline]
        [SerializeField] private string _tooltip; // 아이템 설명
        [SerializeField] private Sprite _iconSprite; // 아이템 아이콘
        [SerializeField] private GameObject _dropItemPrefab; // 바닥에 떨어질 때 생성할 프리팹

        /// <summary> 타입에 맞는 새로운 아이템 생성 </summary>
        public abstract Item CreateItem();

    }
}