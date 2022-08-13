using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetInventory
{
    [Serializable]
    public class AssetPurchases
    {
        public List<AssetPurchase> results;
        public int total;
        public List<NameCountPair> category;
        public List<NameCountPair> publisherSuggest;

        public override string ToString()
        {
            return $"Asset Purchases ({total})";
        }
    }
}