using System.Threading.Tasks;
using UnityEditor;

namespace AssetInventory
{
    public static class AssetStore
    {
        private const string URL_PURCHASES = "https://packages-v2.unity.com/-/api/purchases";
        private const string URL_TOKEN_INFO = "https://api.unity.com/v1/oauth2/tokeninfo?access_token=";
        private const string URL_USER_INFO = "https://api.unity.com/v1/users";
        private const string URL_ASSET_DETAILS = "https://packages-v2.unity.com/-/api/product";
        private const string URL_ASSET_DOWNLOAD = "https://packages-v2.unity.com/-/api/legacy-package-download-info";
        private const int PAGE_SIZE = 100;

        public static async Task<AssetPurchases> RetrievePurchases()
        {
            AssetInventory.CurrentMain = "Fetching purchases";
            AssetInventory.MainCount = 1;
            AssetInventory.MainProgress = 1;
            int progressId = MetaProgress.Start("Fetching purchases");

            string token = CloudProjectSettings.accessToken;
            AssetPurchases result = await AssetUtils.FetchAPIData<AssetPurchases>($"{URL_PURCHASES}?offset=0&limit={PAGE_SIZE}", token);

            // if more results than page size retrieve rest as well and merge
            // doing all requests in parallel is not possible with Unity's web client since they can only run on the main thread
            if (result.total > PAGE_SIZE)
            {
                int pageCount = AssetUtils.GetPageCount(result.total, PAGE_SIZE) - 1;
                AssetInventory.MainCount = pageCount + 1;
                for (int i = 1; i <= pageCount; i++)
                {
                    AssetInventory.MainProgress = i + 1;
                    MetaProgress.Report(progressId, i + 1, pageCount + 1, string.Empty);

                    AssetPurchases pageResult = await AssetUtils.FetchAPIData<AssetPurchases>($"{URL_PURCHASES}?offset={i * PAGE_SIZE}&limit={PAGE_SIZE}", token);
                    result.results.AddRange(pageResult.results);
                }
            }

            AssetInventory.CurrentMain = null;
            MetaProgress.Remove(progressId);

            return result;
        }

        public static async Task<AssetDetails> RetrieveAssetDetails(int id, string eTag)
        {
            string token = CloudProjectSettings.accessToken;
            string newEtag = eTag;
            AssetDetails result = await AssetUtils.FetchAPIData<AssetDetails>($"{URL_ASSET_DETAILS}/{id}", token, eTag, newCacheTag => newEtag = newCacheTag);
            if (result != null) result.ETag = newEtag;

            return result;
        }

        public static async Task<DownloadInfo> RetrieveAssetDownloadInfo(int id)
        {
            string token = CloudProjectSettings.accessToken;
            DownloadInfoResult result = await AssetUtils.FetchAPIData<DownloadInfoResult>($"{URL_ASSET_DOWNLOAD}/{id}", token);

            // special handling of "." also in AssetStoreDownloadInfo 
            if (result?.result?.download != null)
            {
                result.result.download.filename_safe_category_name = result.result.download.filename_safe_category_name.Replace(".", string.Empty);
                result.result.download.filename_safe_package_name = result.result.download.filename_safe_package_name.Replace(".", string.Empty);
                result.result.download.filename_safe_publisher_name = result.result.download.filename_safe_publisher_name.Replace(".", string.Empty);
            }

            return result?.result?.download;
        }
    }
}