using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.InventorySystem;
using Litkey.Utility;


[CreateAssetMenu(fileName = "LootTable", menuName = "Litkey/LootTable")]
public class LootTable : ScriptableObject
{

    [System.Serializable]
    public class ItemDrop
    {
        public ItemData item;
        [Range(0f, 100f)]
        public float dropRate;
        public Vector2Int dropCount = Vector2Int.one;
    }

    public string _lootID;

    [SerializeField] private ItemDrop[] _lootTable;

    public ItemDrop[] GetLootTableInfo()
    {
        return _lootTable;
    }

    public override string ToString()
    {
        string s = "\n";
        for (int i = 0; i < _lootTable.Length; i++)
        {
            s += _lootTable[i].item.Name + ": " + _lootID + "\n";
        }
        return s;
    }
}
