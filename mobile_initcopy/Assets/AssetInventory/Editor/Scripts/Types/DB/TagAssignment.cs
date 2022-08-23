using System;
using SQLite;
using UnityEngine;

namespace AssetInventory
{
    [Serializable]
    public class TagAssignment
    {
        public enum Target
        {
            Asset = 0,
            AssetFile = 1
        }

        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] public int TagId { get; set; }
        [Indexed] public Target TagTarget { get; set; }
        [Indexed] public int TargetId { get; set; }

        public override string ToString()
        {
            return $"Tag Assignment '{TagTarget}' ({TagId})";
        }
    }
}