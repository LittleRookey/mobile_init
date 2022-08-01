﻿using System;

namespace AssetInventory
{
    [Serializable]
    public class AssetReview
    {
        public string reviewCount;
        public string ratingAverage;
        public string ratingCount;
        public string hotness;

        public override string ToString()
        {
            return $"Asset Review ({ratingAverage})";
        }
    }
}