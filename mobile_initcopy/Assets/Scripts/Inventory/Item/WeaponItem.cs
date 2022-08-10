using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� - ���� ������ </summary>
    public class WeaponItem : EquipmentItem, IEquippable
    {
        public WeaponItem(WeaponItemData data) : base(data) { }

        public bool Equip()
        {
            
            return true;
        }
    }


}