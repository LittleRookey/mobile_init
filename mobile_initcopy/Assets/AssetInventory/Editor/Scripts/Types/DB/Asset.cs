using System;
using SQLite;

namespace AssetInventory
{
    [Serializable]
    public class Asset
    {
        public enum State
        {
            New = 0,
            InProcess = 1,
            Done = 2,
            Unknown = 3
        }

        public enum Source
        {
            AssetStorePackage = 0,
            CustomPackage = 1
        }

        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public Source AssetSource { get; set; }
        public string Location { get; set; }
        public int ForeignId { get; set; }
        public long PackageSize { get; set; }
        public string PreviewImage { get; set; }
        [Indexed] public string SafeName { get; set; }
        public string DisplayName { get; set; }
        [Indexed] public string SafePublisher { get; set; }
        public string DisplayPublisher { get; set; }
        [Indexed] public string SafeCategory { get; set; }
        public string DisplayCategory { get; set; }
        public string Slug { get; set; }
        public int Revision { get; set; }
        public string Description { get; set; }
        public string KeyFeatures { get; set; }
        public string CompatibilityInfo { get; set; }
        public string SupportedUnityVersions { get; set; }
        public string Keywords { get; set; }
        public string Version { get; set; }
        public DateTime LastRelease { get; set; }
        public string AssetRating { get; set; }
        public int RatingCount { get; set; }
        public string MainImage { get; set; }
        public string Requirements { get; set; }
        public string ReleaseNotes { get; set; }
        public string ETag { get; set; }
        public State CurrentState { get; set; }

        public string GetDisplayName => string.IsNullOrEmpty(DisplayName) ? SafeName : DisplayName;

        public override string ToString()
        {
            return $"Asset '{GetDisplayName}' ({Location})";
        }
    }
}