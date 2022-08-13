using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AssetInventory
{
    public static class AssetInventory
    {
        public static string[] SCAN_DEPENDENCIES = {"prefab", "mat", "controller", "anim", "asset", "physicmaterial", "physicsmaterial", "sbs", "sbsar", "cubemap", "shadergraph", "shadersubgraph"};

        public static string CurrentMain { get; set; }
        public static int MainCount { get; set; }
        public static int MainProgress { get; set; }

        private const int BREAK_INTERVAL = 5;
        private const string CONFIG_NAME = "AssetInventoryConfig.json";
        private static Regex FILE_GUID = new Regex("guid: (?:([a-z0-9]*))");

        public static AssetInventorySettings Config
        {
            get
            {
                if (_config == null) LoadConfig();
                return _config;
            }
        }

        private static AssetInventorySettings _config;

        public static bool IndexingInProgress { get; private set; }

        public static Dictionary<string, string[]> TypeGroups { get; } = new Dictionary<string, string[]>
        {
            {"Audio", new[] {"wav", "mp3", "ogg", "aiff"}},
            {"Images", new[] {"png", "jpg", "jpeg", "bmp", "tga", "tif", "tiff", "psd", "svg", "webp", "ico"}},
            {"Video", new[] {"mp4"}},
            {"Prefabs", new[] {"prefab"}},
            {"Materials", new[] {"mat", "physicmaterial", "physicsmaterial", "sbs", "sbsar", "cubemap"}},
            {"Shaders", new[] {"shader", "shadergraph", "shadersubgraph", "compute"}},
            {"Models", new[] {"fbx", "obj", "blend", "dae"}},
            {"Scripts", new[] {"cs", "php"}},
            {"Libraries", new[] {"zip", "unitypackage", "so", "bundle", "dll", "jar"}},
            {"Documents", new[] {"md", "doc", "docx", "txt", "json", "rtf", "pdf", "html", "readme", "xml", "chm"}}
        };

        public static bool IsFileType(string path, string type)
        {
            if (path == null) return false;
            return TypeGroups[type].Contains(Path.GetExtension(path).ToLower().Replace(".", string.Empty));
        }

        public static string GetStorageFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AssetInventory");
        }

        public static string GetPreviewFolder()
        {
            return Path.Combine(GetStorageFolder(), "Previews");
        }

        private static string GetMaterializePath()
        {
            return Path.Combine(GetStorageFolder(), "Extracted");
        }

        private static string GetMaterializedAssetPath(Asset asset)
        {
            return Path.Combine(GetMaterializePath(), asset.SafeName);
        }

        public static async Task<string> ExtractAsset(Asset asset)
        {
            string tempPath = GetMaterializedAssetPath(asset);
            if (Directory.Exists(tempPath)) await Task.Run(() => Directory.Delete(tempPath, true));
            await Task.Run(() => TarUtil.ExtractGZ(asset.Location, tempPath));

            return tempPath;
        }

        public static bool IsMaterialized(Asset asset, AssetFile assetFile)
        {
            string sourcePath = Path.Combine(GetMaterializedAssetPath(asset), assetFile.SourcePath);
            return File.Exists(sourcePath);
        }

        public static async Task<string> EnsureMaterializedAsset(Asset asset, AssetFile assetFile)
        {
            string sourcePath = Path.Combine(GetMaterializedAssetPath(asset), assetFile.SourcePath);
            if (!File.Exists(sourcePath)) await ExtractAsset(asset);
            if (!File.Exists(sourcePath)) return null;

            string targetPath = Path.Combine(Path.Combine(Path.GetDirectoryName(sourcePath), "Content"), Path.GetFileName(assetFile.Path));
            if (!File.Exists(targetPath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(targetPath))) Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(sourcePath, targetPath);
            }
            string sourceMetaPath = sourcePath + ".meta";
            string targetMetaPath = targetPath + ".meta";
            if (File.Exists(sourceMetaPath) && !File.Exists(targetMetaPath)) File.Copy(sourceMetaPath, targetMetaPath);

            return targetPath;
        }

        public static async Task CalculateDependencies(AssetInfo assetInfo)
        {
            string targetPath = await EnsureMaterializedAsset(assetInfo.ToAsset(), assetInfo);
            if (targetPath == null) return;

            assetInfo.Dependencies = await Task.Run(() => DoCalculateDependencies(assetInfo, targetPath));
            assetInfo.DependencySize = assetInfo.Dependencies.Sum(af => af.Size);
            assetInfo.MediaDependencies = assetInfo.Dependencies.Where(af => af.Type != "cs").ToList();
            assetInfo.ScriptDependencies = assetInfo.Dependencies.Where(af => af.Type == "cs").ToList();
        }

        private static async Task<List<AssetFile>> DoCalculateDependencies(AssetInfo assetInfo, string path)
        {
            List<AssetFile> result = new List<AssetFile>();

            // only scan file types that contain guid references
            if (!SCAN_DEPENDENCIES.Contains(Path.GetExtension(path).Replace(".", string.Empty))) return result;

            string content = File.ReadAllText(path);
            // TODO: handle binary serialized files, e.g. prefabs
            if (!content.StartsWith("%YAML"))
            {
                assetInfo.DepState = AssetInfo.DependencyState.NotPossible;
                return result;
            }
            MatchCollection matches = FILE_GUID.Matches(content);

            foreach (Match match in matches)
            {
                string guid = match.Groups[1].Value;
                if (result.Any(r => r.Guid == guid)) continue; // break recursion

                AssetFile af = DBAdapter.DB.Find<AssetFile>(a => a.Guid == guid);
                if (af == null) continue; // ignore missing guids as they are not in the package so we can't do anything about them
                result.Add(af);

                // recursive
                string targetPath = await EnsureMaterializedAsset(assetInfo.ToAsset(), af);
                if (targetPath == null)
                {
                    Debug.LogWarning($"Could not materialize dependency: {af.Path}");
                    continue;
                }

                result.AddRange(await Task.Run(() => DoCalculateDependencies(assetInfo, targetPath)));
            }

            return result;
        }

        public static List<AssetInfo> LoadAssets()
        {
            string indexedQuery = "select *, Count(*) as FileCount, Sum(af.Size) as UncompressedSize from AssetFile af left join Asset on Asset.Id = af.AssetId group by af.AssetId order by Lower(Asset.SafeName)";
            List<AssetInfo> indexedResult = DBAdapter.DB.Query<AssetInfo>(indexedQuery);

            string allQuery = "select *, Id as AssetId from Asset order by Lower(SafeName)";
            List<AssetInfo> allResult = DBAdapter.DB.Query<AssetInfo>(allQuery);

            // sqlite does not support "right join", therefore merge two queries manually 
            List<AssetInfo> result = allResult;
            result.ForEach(asset =>
            {
                AssetInfo match = indexedResult.FirstOrDefault(indexedAsset => indexedAsset.Id == asset.Id);
                if (match == null) return;
                asset.FileCount = match.FileCount;
                asset.UncompressedSize = match.UncompressedSize;
            });

            return result;
        }

        public static string[] ExtractAssetNames(IEnumerable<AssetInfo> assets)
        {
            List<string> result = new List<string> {"-all-", string.Empty};
            result.AddRange(assets
                .Where(a => a.FileCount > 0)
                .Select(a => a.SafeName));
            return result.ToArray();
        }

        public static string[] ExtractPublisherNames(IEnumerable<AssetInfo> assets)
        {
            List<string> result = new List<string> {"-all-", string.Empty};
            result.AddRange(assets
                .Where(a => a.FileCount > 0)
                .Where(a => !string.IsNullOrEmpty(a.SafePublisher))
                .Select(a => a.SafePublisher)
                .Distinct()
                .OrderBy(s => s));
            return result.ToArray();
        }

        public static string[] ExtractCategoryNames(IEnumerable<AssetInfo> assets)
        {
            List<string> result = new List<string> {"-all-", string.Empty};
            result.AddRange(assets
                .Where(a => a.FileCount > 0)
                .Where(a => !string.IsNullOrEmpty(a.SafeCategory))
                .Select(a => a.SafeCategory)
                .Distinct()
                .OrderBy(s => s));
            return result.ToArray();
        }

        public static string[] LoadTypes()
        {
            List<string> result = new List<string> {"-all-", string.Empty};
            IOrderedEnumerable<string> raw = DBAdapter.DB.Table<AssetFile>().Where(a => a.Type != "").Select(a => a.Type).Distinct().OrderBy(s => s);

            List<string> groupTypes = new List<string>();
            foreach (KeyValuePair<string, string[]> group in TypeGroups)
            {
                groupTypes.AddRange(group.Value);
                foreach (string type in group.Value)
                {
                    if (raw.Contains(type))
                    {
                        result.Add($"{group.Key}");
                        break;
                    }
                }
            }
            if (result.Last() != "") result.Add(string.Empty);

            // others
            result.AddRange(raw.Where(r => !groupTypes.Contains(r)).Select(type => $"Others/{type}"));

            // all
            result.AddRange(raw.Select(type => $"All/{type}"));

            return result.ToArray();
        }

        public static async Task<long> GetCacheFolderSize()
        {
            if (!Directory.Exists(GetMaterializePath())) return 0;
            DirectoryInfo dirInfo = new DirectoryInfo(GetMaterializePath());
            try
            {
                return await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
            }
            catch
            {
                return 0;
            }
        }

        public static IEnumerator RefreshIndex()
        {
            IndexingInProgress = true;

            Init();

            // special handling for normal asset store assets since directory structure yields additional information
            if (Config.indexAssetStore)
            {
                string assetStoreDownloadCache = GetAssetDownloadPath();
                if (!Directory.Exists(assetStoreDownloadCache))
                {
                    Debug.LogWarning($"Could not find the asset download folder: {assetStoreDownloadCache}");
                    EditorUtility.DisplayDialog("Error", $"Could not find the asset download folder: {assetStoreDownloadCache}. Probably nothing was downloaded yet through the package manager.", "OK");
                    IndexingInProgress = false;
                    yield break;
                }
                yield return new PackageImporter().Index(assetStoreDownloadCache, true);
            }

            // scan custom folders
            for (int i = 0; i < Config.folders.Count; i++)
            {
                FolderSpec folder = Config.folders[i];
                if (!folder.Enabled) continue;
                if (!Directory.Exists(folder.Location))
                {
                    Debug.LogWarning($"Specified folder to scan for assets does not exist anymore: {folder.Location}");
                    continue;
                }
                switch (folder.ScanFor)
                {
                    case FolderSpec.ScanType.Packages:
                        yield return new PackageImporter().Index(folder.Location, false);
                        break;

                    default:
                        Debug.LogError($"Unsupported folder scan type: {folder.ScanFor}");
                        break;
                }
            }

            IndexingInProgress = false;
        }

        private static string GetAssetDownloadPath()
        {
#if UNITY_EDITOR_WIN
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Unity", "Asset Store-5.x");
#else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Unity", "Asset Store-5.x");
#endif
        }

        public static void Init()
        {
            string folder = GetStorageFolder();
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            DBAdapter.InitDB();
        }

        public static void ClearCache(Action callback = null)
        {
            Task _ = Task.Run(() =>
            {
                if (Directory.Exists(GetMaterializePath())) Directory.Delete(GetMaterializePath(), true);
                callback?.Invoke();
            });
        }

        private static string GetConfigFile()
        {
            string guid = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(CONFIG_NAME)).FirstOrDefault();
            if (guid == null)
            {
                Debug.LogError($"Cannot persist Asset Inventory configuration since '{CONFIG_NAME}' file is missing.");
                return null;
            }
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        private static void LoadConfig()
        {
            string configFile = GetConfigFile();
            if (configFile == null)
            {
                _config = new AssetInventorySettings();
                return;
            }
            _config = JsonConvert.DeserializeObject<AssetInventorySettings>(File.ReadAllText(configFile));
            if (_config == null) _config = new AssetInventorySettings();
            if (_config.folders == null) _config.folders = new List<FolderSpec>();
        }

        public static void SaveConfig()
        {
            string configFile = GetConfigFile();
            if (configFile == null) return;

            File.WriteAllText(configFile, JsonConvert.SerializeObject(_config));
        }

        public static void ResetConfig()
        {
            _config = new AssetInventorySettings();
            SaveConfig();
            AssetDatabase.Refresh();
        }

        public static async Task<AssetPurchases> FetchOnlineAssets()
        {
            AssetPurchases assets = await AssetStore.RetrievePurchases();

            CurrentMain = "Updating assets";
            MainCount = assets.results.Count;
            MainProgress = 1;
            int progressId = MetaProgress.Start("Updating assets");

            for (int i = 0; i < MainCount; i++)
            {
                MainProgress = i + 1;
                MetaProgress.Report(progressId, i + 1, MainCount, string.Empty);
                if (i % BREAK_INTERVAL == 0) await Task.Yield(); // let editor breath

                AssetPurchase purchase = assets.results[i];

                Asset existing = DBAdapter.DB.Find<Asset>(a => a.ForeignId == purchase.packageId);
                if (existing == null)
                {
                    // some assets actually differ in tiny capitalization aspects, e.g. Early Prototyping Material kit
                    existing = DBAdapter.DB.Find<Asset>(a => a.SafeName.ToLower() == purchase.CalculatedSafeName.ToLower());
                }

                // check if indeed same package or copy/new version of another, e.g. Buttons Switches and Toggles
                if (existing != null && existing.ForeignId != purchase.packageId && existing.ForeignId > 0) existing = null;

                // temporarily store guessed safe name to ensure locally indexed files are mapped correctly
                // will be overridden in detail run
                if (existing != null)
                {
                    existing.AssetSource = Asset.Source.AssetStorePackage;
                    existing.DisplayName = purchase.displayName;
                    existing.ForeignId = purchase.packageId;
                    if (string.IsNullOrEmpty(existing.SafeName)) existing.SafeName = purchase.CalculatedSafeName;
                    DBAdapter.DB.Update(existing);
                }
                else
                {
                    Asset asset = purchase.ToAsset();
                    asset.SafeName = purchase.CalculatedSafeName;
                    DBAdapter.DB.Insert(asset);
                }
            }

            CurrentMain = null;
            MetaProgress.Remove(progressId);

            return assets;
        }

        public static async void FetchAssetsDetails()
        {
            List<AssetInfo> assets = LoadAssets()
                .Where(a => a.AssetSource == Asset.Source.AssetStorePackage && string.IsNullOrEmpty(a.ETag))
                .ToList();

            CurrentMain = "Updating Asset Details";
            MainCount = assets.Count;
            MainProgress = 1;
            int progressId = MetaProgress.Start("Updating asset details");

            for (int i = 0; i < MainCount; i++)
            {
                int id = assets[i].ForeignId;
                if (id <= 0) continue;

                MainProgress = i + 1;
                MetaProgress.Report(progressId, i + 1, MainCount, string.Empty);
                if (i % BREAK_INTERVAL == 0) await Task.Yield(); // let editor breath

                AssetDetails details = await AssetStore.RetrieveAssetDetails(id, assets[i].ETag);
                if (details == null) continue; // happens if unchanged through etag

                // check if disabled, then download links are not available anymore, deprecated would still work
                DownloadInfo downloadDetails = null;
                if (details.state != "disabled")
                {
                    downloadDetails = await AssetStore.RetrieveAssetDownloadInfo(id);
                    if (downloadDetails == null || string.IsNullOrEmpty(downloadDetails.filename_safe_package_name))
                    {
                        Debug.Log($"Could not fetch download detail information for '{assets[i].SafeName}'");
                        continue;
                    }
                }

                // reload asset to ensure working on latest copy, otherwise might loose package size information
                Asset asset = DBAdapter.DB.Find<Asset>(a => a.ForeignId == id);
                if (asset == null)
                {
                    Debug.LogWarning($"Formerly saved asset '{assets[i].DisplayName}' disappeared.");
                    continue;
                }
                asset.ETag = details.ETag;
                asset.DisplayName = details.name;
                asset.DisplayPublisher = details.productPublisher.name;
                asset.DisplayCategory = details.category.name;
                if (downloadDetails != null)
                {
                    asset.SafeName = downloadDetails.filename_safe_package_name;
                    asset.SafeCategory = downloadDetails.filename_safe_category_name;
                    asset.SafePublisher = downloadDetails.filename_safe_publisher_name;
                }
                if (string.IsNullOrEmpty(asset.SafeName)) asset.SafeName = AssetUtils.GuessSafeName(details.name);
                asset.Description = details.description;
                asset.Requirements = string.Join(", ", details.requirements);
                asset.Keywords = string.Join(", ", details.keyWords);
                asset.SupportedUnityVersions = string.Join(", ", details.supportedUnityVersions);
                asset.Revision = details.revision;
                asset.Slug = details.slug;
                asset.Version = details.version.name;
                asset.LastRelease = details.version.publishedDate;
                if (details.productReview != null)
                {
                    asset.AssetRating = details.productReview.ratingAverage;
                    asset.RatingCount = int.Parse(details.productReview.ratingCount);
                }
                asset.CompatibilityInfo = details.compatibilityInfo;
                asset.MainImage = details.mainImage.url;
                asset.ReleaseNotes = details.publishNotes;
                asset.KeyFeatures = details.keyFeatures;

                DBAdapter.DB.Update(asset);
                await Task.Delay(Random.Range(500, 1500)); // don't flood server
            }

            CurrentMain = null;
            MetaProgress.Remove(progressId);
        }

        public static int CountPurchasedAssets(List<AssetInfo> assets)
        {
            return assets.Count(a => a.AssetSource == Asset.Source.AssetStorePackage);
        }

        public static List<AssetInfo> CalculateAssetUsage()
        {
            List<AssetInfo> result = new List<AssetInfo>();
            List<string> guids = GatherGuids(new[] {Application.dataPath});
            foreach (string guid in guids)
            {
                string query = "select * from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId where Guid=?";
                List<AssetInfo> files = DBAdapter.DB.Query<AssetInfo>($"{query}", guid);
                if (files.Count == 0)
                {
                    // found unindexed asset
                    AssetInfo ai = new AssetInfo();
                    ai.Guid = guid;
                    ai.CurrentState = Asset.State.Unknown;
                    result.Add(ai);
                    continue;
                }
                if (files.Count > 1)
                {
                    Debug.LogWarning("Duplicate guids found: " + string.Join(", ", files.Select(ai => ai.Path)));
                    continue;
                }
                result.Add(files[0]);
            }

            return result;
        }

        private static List<string> GatherGuids(IEnumerable<string> folders)
        {
            List<string> result = new List<string>();

            foreach (string folder in folders)
            {
                // scan for all meta files and return corresponding asset
                string[] assets = Directory.GetFiles(folder, "*.meta", SearchOption.AllDirectories);
                for (int i = 0; i < assets.Length; i++)
                {
                    assets[i] = assets[i].Substring(0, assets[i].Length - 5).Replace("\\", "/");
                    assets[i] = assets[i].Substring(Application.dataPath.Length - 6); // leave "Assets/" in
                }
                foreach (string asset in assets)
                {
                    string guid = GetAssetGuid(asset);
                    if (string.IsNullOrEmpty(guid)) continue;

                    result.Add(guid);
                }
            }

            return result;
        }

        private static string GetAssetGuid(string assetFile)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetFile);
            if (!string.IsNullOrEmpty(guid)) return guid;

            // hidden files might not be indexed
            string metaFile = $"{assetFile}.meta";
            if (!File.Exists(metaFile)) return null;

            using (StreamReader reader = new StreamReader(metaFile))
            {
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    if (!line.StartsWith("guid:")) continue;
                    return line.Substring(5).Trim();
                }
            }

            return null;
        }
    }
}