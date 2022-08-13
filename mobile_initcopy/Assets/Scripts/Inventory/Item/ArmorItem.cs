using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 장비 - 방어구 아이템 </summary>
    public class ArmorItem : EquipmentItem, IEquippable
    {
        public ArmorItem(ArmorItemData data) : base(data) { }

        public bool Equip()
        {
            return true;
        }
    }
}