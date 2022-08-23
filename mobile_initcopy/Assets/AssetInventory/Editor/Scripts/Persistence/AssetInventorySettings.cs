using System;
using System.Collections.Generic;

namespace AssetInventory
{
    [Serializable]
    public class AssetInventorySettings
    {
        public int sortField;
        public int assetGrouping;
        public bool sortDescending;
        public int maxResults = 5;
        public int tileText;
        public bool indexAssetStore = true;
        public bool autoPlayAudio = true;
        public bool pingSelected = true;
        public int tileSize = 150;
        public string customStorageLocation;
        public List<FolderSpec> folders = new List<FolderSpec>();
    }
}