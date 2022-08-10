using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 사용 가능한 아이템(착용/소모) </summary>
    public interface IUsableItem
    {
        /// <summary> 아이템 사용하기(사용 성공 여부 리턴) </summary>
        bool Use();
    }

    /// <summary>
    ///  Equippable item
    /// </summary>
    public interface IEquippable
    {
        bool Equip();
    }
}