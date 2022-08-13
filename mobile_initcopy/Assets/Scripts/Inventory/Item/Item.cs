using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��¥ : 2021-03-07 PM 7:34:39
// �ۼ��� : Rito

namespace Litkey.InventorySystem
{
    /*
        [��� ����]
        Item : �⺻ ������
            - EquipmentItem : ��� ������
            - CountableItem : ������ �����ϴ� ������
    */
    public abstract class Item
    {
        public ItemData Data { get; private set; }

        public Item(ItemData data) => Data = data;
    }
}