namespace AssetInventory
{
    public abstract class AssertImporter
    {
        public static string CurrentMain { get; protected set; }
        public static int MainCount { get; protected set; }
        public static int MainProgress { get; protected set; }
        public static string CurrentSub { get; protected set; }
        public static int SubCount { get; protected set; }
        public static int SubProgress { get; protected set; }
        public static bool CancellationRequested { get; set; }

        protected void ResetState()
        {
            CancellationRequested = false;
            CurrentMain = null;
            CurrentSub = null;
            MainCount = 0;
            MainProgress = 0;
            SubCount = 0;
            SubProgress = 0;
        }

        protected Asset Fetch(Asset asset)
        {
            return DBAdapter.DB.Find<Asset>(a => a.SafeName == asset.SafeName);
        }

        protected void Persist(Asset asset)
        {
            Asset existing = DBAdapter.DB.Find<Asset>(a => a.SafeName == asset.SafeName);
            if (existing != null)
            {
                asset.Id = existing.Id;
                existing.SafeCategory = asset.SafeCategory;
                existing.SafePublisher = asset.SafePublisher;
                existing.CurrentState = asset.CurrentState;
                existing.AssetSource = asset.AssetSource;
                existing.PackageSize = asset.PackageSize;
                existing.Location = asset.Location;
                existing.PreviewImage = asset.PreviewImage;

                DBAdapter.DB.Update(existing);
            }
            else
            {
                DBAdapter.DB.Insert(asset);
            }
        }

        protected void Persist(AssetFile file)
        {
            AssetFile existing;
            if (string.IsNullOrEmpty(file.Guid))
            {
                existing = DBAdapter.DB.Find<AssetFile>(f => f.Path == file.Path && f.AssetId == file.AssetId);
            }
            else
            {
                existing = DBAdapter.DB.Find<AssetFile>(f => f.Guid == file.Guid && f.AssetId == file.AssetId);
            }
            if (existing != null)
            {
                file.Id = existing.Id;
                DBAdapter.DB.Update(file);
            }
            else
            {
                DBAdapter.DB.Insert(file);
            }
        }
    }
}