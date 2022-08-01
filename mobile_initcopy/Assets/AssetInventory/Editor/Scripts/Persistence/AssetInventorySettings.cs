using System;
using System.Collections.Generic;

namespace AssetInventory
{
    [Serializable]
    public class AssetInventorySettings
    {
        public int maxResults = 5;
        public bool indexAssetStore = true;
        public List<FolderSpec> folders = new List<FolderSpec>();
    }
}