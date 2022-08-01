using UnityEngine;


namespace Litkey.InventorySystem
{
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