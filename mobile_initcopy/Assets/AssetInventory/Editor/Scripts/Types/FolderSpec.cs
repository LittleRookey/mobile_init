using System;

namespace AssetInventory
{
    [Serializable]
    public class FolderSpec
    {
        public enum ScanType
        {
            Packages = 0,
            Pattern = 1
        }

        public ScanType ScanFor { get; set; }
        public bool Enabled { get; set; } = true;
        public string Pattern { get; set; }
        public string Location { get; set; }
    }
}