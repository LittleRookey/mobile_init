using System;

namespace AssetInventory
{
    [Serializable]
    public class DownloadInfoResultDetails
    {
        public DownloadInfo download;

        public override string ToString()
        {
            return "Download Info Result Details";
        }
    }
}