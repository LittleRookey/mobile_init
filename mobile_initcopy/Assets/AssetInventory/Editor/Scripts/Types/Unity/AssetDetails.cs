﻿using System;

namespace AssetInventory
{
    [Serializable]
    public class AssetDetails
    {
        public string id;
        public DateTime createdTime;
        public DateTime updatedTime;
        public string packageId;
        public string slug;
        public int revision;
        public string name;
        public string displayName;
        public string description;
        public string elevatorPitch;
        public string keyFeatures;
        public string compatibilityInfo;
        public string[] supportedUnityVersions;
        public string[] keyWords;
        public AssetVersion version;
        public AssetReview productReview;
        public string originPrice;
        public Publisher productPublisher;
        public Category category;
        public AssetImages mainImage;
        public AssetImages mainImageWebp;
        public AssetImage[] images;
        public string publisherId;
        public string publishNotes;
        public string[] requirements;
        public string state;

        // runtime
        public string ETag { get; set; }

        public override string ToString()
        {
            return $"Asset Details ({name})";
        }
    }
}