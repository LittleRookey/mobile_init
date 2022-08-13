using System;

namespace AssetInventory
{
    [Serializable]
    public class NameCountPair
    {
        public string name;
        public int count;

        public override string ToString()
        {
            return $"Data ({name}, {count})";
        }
    }
}