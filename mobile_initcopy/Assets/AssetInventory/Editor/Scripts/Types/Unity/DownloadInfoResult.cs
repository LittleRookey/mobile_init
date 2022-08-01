using System;

namespace AssetInventory
{
    [Serializable]
    public class DownloadInfoResult
    {
        public DownloadInfoResultDetails result;

        public override string ToString()
        {
            return "Download Info Result";
        }
    }
}