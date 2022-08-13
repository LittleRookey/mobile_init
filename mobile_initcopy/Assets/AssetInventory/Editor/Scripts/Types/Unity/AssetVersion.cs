using System;

namespace AssetInventory
{
    [Serializable]
    public class AssetVersion
    {
        public string id;
        public string name;
        public DateTime publishedDate;

        public override string ToString()
        {
            return $"Asset Version ({name})";
        }
    }
}