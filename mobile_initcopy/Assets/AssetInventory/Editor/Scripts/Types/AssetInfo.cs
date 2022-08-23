using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetInventory
{
    [Serializable]
    // used to contain results of join calls
    public class AssetInfo : AssetFile
    {
        public enum DependencyState
        {
            Unknown = 0,
            Calculating = 1,
            Done = 2,
            NotPossible = 3
        }

        public Asset.Source AssetSource { get; set; }
        public string Location { get; set; }
        public int ForeignId { get; set; }
        public long PackageSize { get; set; }
        public string SafeName { get; set; }
        public string DisplayName { get; set; }
        public string SafePublisher { get; set; }
        public string DisplayPublisher { get; set; }
        public string SafeCategory { get; set; }
        public string DisplayCategory { get; set; }
        public string PreviewImage { get; set; }
        public Asset.State CurrentState { get; set; }
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
        public int FileCount { get; set; }
        public long UncompressedSize { get; set; }

        // runtime only
        public Texture2D PreviewTexture { get; set; }
        public string ProjectPath { get; set; }
        public bool InProject => !string.IsNullOrWhiteSpace(ProjectPath);
        public bool IsIndexed => FileCount > 0 && CurrentState == Asset.State.Done;
        public bool IsMaterialized { get; set; }

        public bool Downloaded
        {
            get
            {
                if (_downloaded == null) _downloaded = !string.IsNullOrEmpty(Location) && File.Exists(Location);
                return _downloaded.Value;
            }
        }

        private bool? _downloaded;

        public string GetDisplayName => string.IsNullOrEmpty(DisplayName) ? SafeName : DisplayName;
        public string GetDisplayPublisher => string.IsNullOrEmpty(DisplayPublisher) ? SafePublisher : DisplayPublisher;
        public string GetDisplayCategory => string.IsNullOrEmpty(DisplayCategory) ? SafeCategory : DisplayCategory;

        public DependencyState DepState { get; set; } = DependencyState.Unknown;
        public List<AssetFile> Dependencies { get; set; }
        public List<AssetFile> MediaDependencies { get; set; }
        public List<AssetFile> ScriptDependencies { get; set; }
        public long DependencySize { get; set; }

        public AssetInfo WithTreeData(string name, int id = 0, int depth = 0)
        {
            m_Name = name;
            m_ID = id;
            m_Depth = depth;

            return this;
        }

        public AssetInfo WithTreeId(int id)
        {
            m_ID = id;

            return this;
        }

        public AssetInfo WithProjectPath(string path)
        {
            ProjectPath = path;

            return this;
        }

        public Asset ToAsset()
        {
            return new Asset
            {
                AssetSource = AssetSource,
                DisplayCategory = DisplayCategory,
                SafeCategory = SafeCategory,
                CurrentState = CurrentState,
                Id = AssetId,
                Slug = Slug,
                Revision = Revision,
                Description = Description,
                KeyFeatures = KeyFeatures,
                CompatibilityInfo = CompatibilityInfo,
                SupportedUnityVersions = SupportedUnityVersions,
                Keywords = Keywords,
                Version = Version,
                LastRelease = LastRelease,
                AssetRating = AssetRating,
                RatingCount = RatingCount,
                MainImage = MainImage,
                Requirements = Requirements,
                ReleaseNotes = ReleaseNotes,
                ETag = ETag,
                Location = Location,
                ForeignId = ForeignId,
                SafeName = SafeName,
                DisplayName = DisplayName,
                PackageSize = PackageSize,
                SafePublisher = SafePublisher,
                DisplayPublisher = DisplayPublisher
            };
        }

        public override string ToString()
        {
            return $"Asset Info '{GetDisplayName}'";
        }
    }
}