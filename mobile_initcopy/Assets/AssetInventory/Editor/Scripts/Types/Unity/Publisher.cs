using System;

namespace AssetInventory
{
    [Serializable]
    public class Publisher
    {
        public string id;
        public string name;
        public string supportUrl;
        public string supportEmail;
        public string url;

        public override string ToString()
        {
            return $"Publisher ({name})";
        }
    }
}