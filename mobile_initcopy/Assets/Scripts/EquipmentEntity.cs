using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnInventory.Core.MVC.Model.Data;
using CleverCrow.Fluid.StatsSystem;


[CreateAssetMenu(fileName = "Equipment", menuName = "UnInventory/Entity Equipment")]
public class EquipmentEntity : DataEntity
{
    public struct statInfo
    {
        
        public int value;
    }
    [Header("Additional Informations")]
    public int _requiredLevel;
    public int[] stats;
}
