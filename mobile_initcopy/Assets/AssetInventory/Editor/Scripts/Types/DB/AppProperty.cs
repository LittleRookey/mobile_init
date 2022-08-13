using System;
using SQLite;

namespace AssetInventory
{
    [Serializable]
    public class AppProperty
    {
        [PrimaryKey] public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"DB Info ({Name})";
        }
    }
}