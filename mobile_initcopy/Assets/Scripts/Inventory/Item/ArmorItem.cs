using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� - �� ������ </summary>
    public class ArmorItem : EquipmentItem, IEquippable
    {
        public ArmorItem(ArmorItemData data) : base(data) { }

        public bool Equip()
        {
            return true;
        }
    }
}