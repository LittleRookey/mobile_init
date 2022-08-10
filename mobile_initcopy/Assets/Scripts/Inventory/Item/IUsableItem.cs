using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� ������ ������(����/�Ҹ�) </summary>
    public interface IUsableItem
    {
        /// <summary> ������ ����ϱ�(��� ���� ���� ����) </summary>
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