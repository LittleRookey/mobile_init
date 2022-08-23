using System;
using SQLite;
using UnityEngine;

namespace AssetInventory
{
    [Serializable]
    public class Tag
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public override string ToString()
        {
            return $"Tag '{Name}' ({Color})";
        }
    }
}