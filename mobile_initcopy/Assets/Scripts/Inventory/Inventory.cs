using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    [Item�� ��ӱ���]
    - Item
        - CountableItem
            - PortionItem : IUsableItem.Use() -> ��� �� ���� 1 �Ҹ�
        - EquipmentItem
            - WeaponItem
            - ArmorItem
    [ItemData�� ��ӱ���]
      (ItemData�� �ش� �������� �������� ���� ������ �ʵ� ����)
      (��ü���� �޶����� �ϴ� ���� ������, ��ȭ�� ���� Item Ŭ�������� ����)
    - ItemData
        - CountableItemData
            - PortionItemData : ȿ����(Value : ȸ����, ���ݷ� � ���)
        - EquipmentItemData : �ִ� ������
            - WeaponItemData : �⺻ ���ݷ�
            - ArmorItemData : �⺻ ����
*/

/*
    [API]
    - bool HasItem(int) : �ش� �ε����� ���Կ� �������� �����ϴ��� ����
    - bool IsCountableItem(int) : �ش� �ε����� �������� �� �� �ִ� ���������� ����
    - int GetCurrentAmount(int) : �ش� �ε����� ������ ����
        - -1 : �߸��� �ε���
        -  0 : �� ����
        -  1 : �� �� ���� �������̰ų� ���� 1
    - ItemData GetItemData(int) : �ش� �ε����� ������ ����
    - string GetItemName(int) : �ش� �ε����� ������ �̸�
    - int Add(ItemData, int) : �ش� Ÿ���� �������� ������ ������ŭ �κ��丮�� �߰�
        - �ڸ� �������� ������ ������ŭ ����(0�̸� ��� �߰� �����ߴٴ� �ǹ�)
    - void Remove(int) : �ش� �ε����� ���Կ� �ִ� ������ ����
    - void Swap(int, int) : �� �ε����� ������ ��ġ ���� �ٲٱ�
    - void SeparateAmount(int a, int b, int amount)
        - a �ε����� �������� �� �� �ִ� �������� ���, amount��ŭ �и��Ͽ� b �ε����� ����
    - void Use(int) : �ش� �ε����� ������ ���
    - void UpdateSlot(int) : �ش� �ε����� ���� ���� �� UI ����
    - void UpdateAllSlot() : ��� ���� ���� �� UI ����
    - void UpdateAccessibleStatesAll() : ��� ���� UI�� ���� ���� ���� ����
    - void TrimAll() : �տ������� ������ ���� ä���
    - void SortAll() : �տ������� ������ ���� ä��鼭 ����
*/

namespace Litkey.InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        /***********************************************************************
        *                               Public Properties
        ***********************************************************************/
        #region .
        /// <summary> ������ ���� �ѵ� </summary>
        public int Capacity { get; private set; }

        // /// <summary> ���� ������ ���� </summary>
        //public int ItemCount => _itemArray.Count;

        #endregion
        /***********************************************************************
        *                               Private Fields
        ***********************************************************************/
        #region .

        // �ʱ� ���� �ѵ�
        [SerializeField, Range(8, 64)]
        private int _initalCapacity = 32;

        // �ִ� ���� �ѵ�(������ �迭 ũ��)
        [SerializeField, Range(8, 64)]
        private int _maxCapacity = 64;

        [SerializeField]
        private InventoryUI _inventoryUI; // ����� �κ��丮 UI

        /// <summary> ������ ��� </summary>
        [SerializeField]
        private Item[] _items;

        /// <summary> ������Ʈ �� �ε��� ��� </summary>
        private readonly HashSet<int> _indexSetForUpdate = new HashSet<int>();

        /// <summary> ������ ������ Ÿ�Ժ� ���� ����ġ </summary>
        private readonly static Dictionary<Type, int> _sortWeightDict = new Dictionary<Type, int>
        {
            { typeof(PortionItemData), 10000 },
            { typeof(WeaponItemData),  20000 },
            { typeof(ArmorItemData),   30000 },
        };

        private class ItemComparer : IComparer<Item>
        {
            public int Compare(Item a, Item b)
            {
                return (a.Data.ID + _sortWeightDict[a.Data.GetType()])
                     - (b.Data.ID + _sortWeightDict[b.Data.GetType()]);
            }
        }
        private static readonly ItemComparer _itemComparer = new ItemComparer();

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_initalCapacity > _maxCapacity)
                _initalCapacity = _maxCapacity;
        }
#endif
        private void Awake()
        {
            _items = new Item[_maxCapacity];
            Capacity = _initalCapacity;
            _inventoryUI.SetInventoryReference(this);
        }

        private void Start()
        {
            UpdateAccessibleStatesAll();
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .
        /// <summary> �ε����� ���� ���� ���� �ִ��� �˻� </summary>
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Capacity;
        }

        /// <summary> �տ������� ����ִ� ���� �ε��� Ž�� </summary>
        private int FindEmptySlotIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < Capacity; i++)
                if (_items[i] == null)
                    return i;
            return -1;
        }

        /// <summary> �տ������� ���� ������ �ִ� Countable �������� ���� �ε��� Ž�� </summary>
        private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
        {
            for (int i = startIndex; i < Capacity; i++)
            {
                var current = _items[i];
                if (current == null)
                    continue;

                // ������ ���� ��ġ, ���� ���� Ȯ��
                if (current.Data == target && current is CountableItem ci)
                {
                    if (!ci.IsMax)
                        return i;
                }
            }

            return -1;
        }

        /// <summary> �ش��ϴ� �ε����� ���� ���� �� UI ���� </summary>
        private void UpdateSlot(int index)
        {
            if (!IsValidIndex(index)) return;

            Item item = _items[index];

            // 1. �������� ���Կ� �����ϴ� ���
            if (item != null)
            {
                // ������ ���
                _inventoryUI.SetItemIcon(index, item.Data.IconSprite);

                // 1-1. �� �� �ִ� ������
                if (item is CountableItem ci)
                {
                    // 1-1-1. ������ 0�� ���, ������ ����
                    if (ci.IsEmpty)
                    {
                        _items[index] = null;
                        RemoveIcon();
                        return;
                    }
                    // 1-1-2. ���� �ؽ�Ʈ ǥ��
                    else
                    {
                        _inventoryUI.SetItemAmountText(index, ci.Amount);
                    }
                }
                // 1-2. �� �� ���� �������� ��� ���� �ؽ�Ʈ ����
                else
                {
                    _inventoryUI.HideItemAmountText(index);
                }

                // ���� ���� ���� ������Ʈ
                _inventoryUI.UpdateSlotFilterState(index, item.Data);
            }
            // 2. �� ������ ��� : ������ ����
            else
            {
                RemoveIcon();
            }

            // ���� : ������ �����ϱ�
            void RemoveIcon()
            {
                _inventoryUI.RemoveItem(index);
                _inventoryUI.HideItemAmountText(index); // ���� �ؽ�Ʈ �����
            }
        }

        /// <summary> �ش��ϴ� �ε����� ���Ե��� ���� �� UI ���� </summary>
        private void UpdateSlot(params int[] indices)
        {
            foreach (var i in indices)
            {
                UpdateSlot(i);
            }
        }

        /// <summary> ��� ���Ե��� ���¸� UI�� ���� </summary>
        private void UpdateAllSlot()
        {
            for (int i = 0; i < Capacity; i++)
            {
                UpdateSlot(i);
            }
        }

        #endregion
        /***********************************************************************
        *                               Check & Getter Methods
        ***********************************************************************/
        #region .

        /// <summary> �ش� ������ �������� ���� �ִ��� ���� </summary>
        public bool HasItem(int index)
        {
            return IsValidIndex(index) && _items[index] != null;
        }

        /// <summary> �ش� ������ �� �� �ִ� ���������� ���� </summary>
        public bool IsCountableItem(int index)
        {
            return HasItem(index) && _items[index] is CountableItem;
        }

        /// <summary> 
        /// �ش� ������ ���� ������ ���� ����
        /// <para/> - �߸��� �ε��� : -1 ����
        /// <para/> - �� ���� : 0 ����
        /// <para/> - �� �� ���� ������ : 1 ����
        /// </summary>
        public int GetCurrentAmount(int index)
        {
            if (!IsValidIndex(index)) return -1;
            if (_items[index] == null) return 0;

            CountableItem ci = _items[index] as CountableItem;
            if (ci == null)
                return 1;

            return ci.Amount;
        }

        /// <summary> �ش� ������ ������ ���� ���� </summary>
        public ItemData GetItemData(int index)
        {
            if (!IsValidIndex(index)) return null;
            if (_items[index] == null) return null;

            return _items[index].Data;
        }

        /// <summary> �ش� ������ ������ �̸� ���� </summary>
        public string GetItemName(int index)
        {
            if (!IsValidIndex(index)) return "";
            if (_items[index] == null) return "";

            return _items[index].Data.Name;
        }

        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .
        /// <summary> �κ��丮 UI ���� </summary>
        public void ConnectUI(InventoryUI inventoryUI)
        {
            _inventoryUI = inventoryUI;
            _inventoryUI.SetInventoryReference(this);
        }

        /// <summary> �κ��丮�� ������ �߰�
        /// <para/> �ִ� �� ������ �׿� ������ ���� ����
        /// <para/> ������ 0�̸� �ִµ� ��� �����ߴٴ� �ǹ�
        /// </summary>
        public int Add(ItemData itemData, int amount = 1)
        {
            int index;

            // 1. ������ �ִ� ������
            if (itemData is CountableItemData ciData)
            {
                bool findNextCountable = true;
                index = -1;

                while (amount > 0)
                {
                    // 1-1. �̹� �ش� �������� �κ��丮 ���� �����ϰ�, ���� ���� �ִ��� �˻�
                    if (findNextCountable)
                    {
                        index = FindCountableItemSlotIndex(ciData, index + 1);

                        // ���� �����ִ� ������ ������ ���̻� ���ٰ� �Ǵܵ� ���, �� ���Ժ��� Ž�� ����
                        if (index == -1)
                        {
                            findNextCountable = false;
                        }
                        // ������ ������ ã�� ���, �� ������Ű�� �ʰ��� ���� �� amount�� �ʱ�ȭ
                        else
                        {
                            CountableItem ci = _items[index] as CountableItem;
                            amount = ci.AddAmountAndGetExcess(amount);

                            UpdateSlot(index);
                        }
                    }
                    // 1-2. �� ���� Ž��
                    else
                    {
                        index = FindEmptySlotIndex(index + 1);

                        // �� �������� ���� ��� ����
                        if (index == -1)
                        {
                            break;
                        }
                        // �� ���� �߰� ��, ���Կ� ������ �߰� �� �׿��� ���
                        else
                        {
                            // ���ο� ������ ����
                            CountableItem ci = ciData.CreateItem() as CountableItem;
                            ci.SetAmount(amount);

                            // ���Կ� �߰�
                            _items[index] = ci;

                            // ���� ���� ���
                            amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;

                            UpdateSlot(index);
                        }
                    }
                }
            }
            // 2. ������ ���� ������
            else
            {
                // 2-1. 1���� �ִ� ���, ������ ����
                if (amount == 1)
                {
                    index = FindEmptySlotIndex();
                    if (index != -1)
                    {
                        // �������� �����Ͽ� ���Կ� �߰�
                        _items[index] = itemData.CreateItem();
                        amount = 0;

                        UpdateSlot(index);
                    }
                }

                // 2-2. 2�� �̻��� ���� ���� �������� ���ÿ� �߰��ϴ� ���
                index = -1;
                for (; amount > 0; amount--)
                {
                    // ������ ���� �ε����� ���� �ε������� ���� Ž��
                    index = FindEmptySlotIndex(index + 1);

                    // �� ���� ���� ��� ���� ����
                    if (index == -1)
                    {
                        break;
                    }

                    // �������� �����Ͽ� ���Կ� �߰�
                    _items[index] = itemData.CreateItem();

                    UpdateSlot(index);
                }
            }

            return amount;
        }

        /// <summary> �ش� ������ ������ ���� </summary>
        public void Remove(int index)
        {
            if (!IsValidIndex(index)) return;

            _items[index] = null;
            _inventoryUI.RemoveItem(index);
        }

        /// <summary> �� �ε����� ������ ��ġ�� ���� ��ü </summary>
        public void Swap(int indexA, int indexB)
        {
            if (!IsValidIndex(indexA)) return;
            if (!IsValidIndex(indexB)) return;

            Item itemA = _items[indexA];
            Item itemB = _items[indexB];

            // 1. �� �� �ִ� �������̰�, ������ �������� ���
            //    indexA -> indexB�� ���� ��ġ��
            if (itemA != null && itemB != null &&
                itemA.Data == itemB.Data &&
                itemA is CountableItem ciA && itemB is CountableItem ciB)
            {
                int maxAmount = ciB.MaxAmount;
                int sum = ciA.Amount + ciB.Amount;

                if (sum <= maxAmount)
                {
                    ciA.SetAmount(0);
                    ciB.SetAmount(sum);
                }
                else
                {
                    ciA.SetAmount(sum - maxAmount);
                    ciB.SetAmount(maxAmount);
                }
            }
            // 2. �Ϲ����� ��� : ���� ��ü
            else
            {
                _items[indexA] = itemB;
                _items[indexB] = itemA;
            }

            // �� ���� ���� ����
            UpdateSlot(indexA, indexB);
        }

        /// <summary> �� �� �ִ� �������� ���� ������(A -> B ��������) </summary>
        public void SeparateAmount(int indexA, int indexB, int amount)
        {
            // amount : ���� ��ǥ ����

            if (!IsValidIndex(indexA)) return;
            if (!IsValidIndex(indexB)) return;

            Item _itemA = _items[indexA];
            Item _itemB = _items[indexB];

            CountableItem _ciA = _itemA as CountableItem;

            // ���� : A ���� - �� �� �ִ� ������ / B ���� - Null
            // ���ǿ� �´� ���, �����Ͽ� ���� B�� �߰�
            if (_ciA != null && _itemB == null)
            {
                _items[indexB] = _ciA.SeperateAndClone(amount);

                UpdateSlot(indexA, indexB);
            }
        }

        /// <summary> �ش� ������ ������ ��� </summary>
        public void Use(int index)
        {
            if (!IsValidIndex(index)) return;
            if (_items[index] == null) return;

            // ��� ������ �������� ���
            if (_items[index] is IUsableItem uItem)
            {
                // ������ ���
                bool succeeded = uItem.Use();

                if (succeeded)
                {
                    UpdateSlot(index);
                }
            }
        }

        /// <summary> ��� ���� UI�� ���� ���� ���� ������Ʈ </summary>
        public void UpdateAccessibleStatesAll()
        {
            _inventoryUI.SetAccessibleSlotRange(Capacity);
        }

        /// <summary> �� ���� ���� �տ������� ä��� </summary>
        public void TrimAll()
        {
            // ���� ���� �迭 ����� ä��� �˰���

            // i Ŀ���� j Ŀ��
            // i Ŀ�� : ���� �տ� �ִ� ��ĭ�� ã�� Ŀ��
            // j Ŀ�� : i Ŀ�� ��ġ�������� �ڷ� �̵��ϸ� ������ �������� ã�� Ŀ��

            // iĿ���� ��ĭ�� ã���� j Ŀ���� i+1 ��ġ���� Ž��
            // jĿ���� �������� ã���� �������� �ű��, i Ŀ���� i+1 ��ġ�� �̵�
            // jĿ���� Capacity�� �����ϸ� ���� ��� ����

            _indexSetForUpdate.Clear();

            int i = -1;
            while (_items[++i] != null) ;
            int j = i;

            while (true)
            {
                while (++j < Capacity && _items[j] == null) ;

                if (j == Capacity)
                    break;

                _indexSetForUpdate.Add(i);
                _indexSetForUpdate.Add(j);

                _items[i] = _items[j];
                _items[j] = null;
                i++;
            }

            foreach (var index in _indexSetForUpdate)
            {
                UpdateSlot(index);
            }
            _inventoryUI.UpdateAllSlotFilters();
        }

        /// <summary> �� ���� ���� ä��鼭 ������ �������� �����ϱ� </summary>
        public void SortAll()
        {
            // 1. Trim
            int i = -1;
            while (_items[++i] != null) ;
            int j = i;

            while (true)
            {
                while (++j < Capacity && _items[j] == null) ;

                if (j == Capacity)
                    break;

                _items[i] = _items[j];
                _items[j] = null;
                i++;
            }

            // 2. Sort
            Array.Sort(_items, 0, i, _itemComparer);

            // 3. Update
            UpdateAllSlot();
            _inventoryUI.UpdateAllSlotFilters(); // ���� ���� ������Ʈ
        }

        #endregion
    }
}