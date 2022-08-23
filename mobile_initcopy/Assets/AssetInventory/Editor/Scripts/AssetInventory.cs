using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JD.EditorAudioUtils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AssetInventory
{
    public static class AssetInventory
    {
        public const string ToolVersion = "1.1.0";
        public static readonly string[] ScanDependencies = {"prefab", "mat", "controller", "anim", "asset", "physicmaterial", "physicsmaterial", "sbs", "sbsar", "cubemap", "shadergraph", "shadersubgraph"};

        public static string CurrentMain { get; set; }
        public static int MainCount { get; set; }
        public static int MainProgress { get; set; }
        public static string UsedConfigLocation { get; private set; }

        private const int BreakInterval = 5;
        private const string ConfigName = "AssetInventoryConfig.json";
        private const string AssetStoreFolderName = "Asset Store-5.x";
        private static readonly Regex FileGuid = new Regex("guid: (?:([a-z0-9]*))");

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
        public static bool ClearCacheInProgress { get; private set; }

        public static Dictionary<string, string[]> TypeGroups { get; } = new Dictionary<string, string[]>
        {
            {"Audio", new[] {"wav", "mp3", "ogg", "aiff", "aif", "mod", "it", "s3m", "xm"}},
            {"Images", new[] {"png", "jpg", "jpeg", "bmp", "tga", "tif", "tiff", "psd", "svg", "webp", "ico", "exr", "gif", "hdr", "iff", "pict"}},
            {"Video", new[] {"mp4"}},
            {"Prefabs", new[] {"prefab"}},
            {"Materials", new[] {"mat", "physicmaterial", "physicsmaterial", "sbs", "sbsar", "cubemap"}},
            {"Shaders", new[] {"shader", "shadergraph", "shadersubgraph", "compute"}},
            {"Models", new[] {"fbx", "obj", "blend", "dae", "3ds", "dxf", "max", "c4d", "mb", "ma"}},
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
            if (!string.IsNullOrEmpty(Config.customStorageLocation)) return Path.GetFullPath(Config.customStorageLocation);

            return IOUtils.PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AssetInventory");
        }

        private static string GetConfigLocation()
        {
            // search for local project-specific override first
            string guid = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(ConfigName)).FirstOrDefault();
            if (guid != null) return AssetDatabase.GUIDToAssetPath(guid);

            return IOUtils.PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ConfigName);
        }

        public static string GetPreviewFolder(string customFolder = null)
        {
            return IOUtils.PathCombine(customFolder ?? GetStorageFolder(), "Previews");
        }

        private static string GetMaterializePath()
        {
            return IOUtils.PathCombine(GetStorageFolder(), "Extracted");
        }

        private static string GetMaterializedAssetPath(Asset asset)
        {
            return IOUtils.PathCombine(GetMaterializePath(), asset.SafeName);
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

        public static async Task<string> EnsureMaterializedAsset(AssetInfo info)
        {
            string targetPath = await EnsureMaterializedAsset(info.ToAsset(), info);
            info.IsMaterialized = IsMaterialized(info.ToAsset(), info);
            return targetPath;
        }

        public static async Task<string> EnsureMaterializedAsset(Asset asset, AssetFile assetFile)
        {
            string sourcePath = Path.Combine(GetMaterializedAssetPath(asset), assetFile.SourcePath);
            if (!File.Exists(sourcePath)) await ExtractAsset(asset);
            if (!File.Exists(sourcePath)) return null;

            string targetPath = Path.Combine(Path.GetDirectoryName(sourcePath), "Content", Path.GetFileName(assetFile.Path));
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
            if (!ScanDependencies.Contains(Path.GetExtension(path).Replace(".", string.Empty))) return result;

            string content = File.ReadAllText(path);
            // TODO: handle binary serialized files, e.g. prefabs
            if (!content.StartsWith("%YAML"))
            {
                assetInfo.DepState = AssetInfo.DependencyState.NotPossible;
                return result;
            }
            MatchCollection matches = FileGuid.Matches(content);

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

            string query = "select Distinct(Type) from AssetFile where Type not null and Type != \"\" order by Type";
            List<string> raw = DBAdapter.DB.QueryScalars<string>($"{query}");

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
            return await IOUtils.GetFolderSize(GetMaterializePath());
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
                if (AssertImporter.CancellationRequested) break;

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
                        bool hasAssetStoreLayout = Path.GetFileName(folder.Location) == AssetStoreFolderName;
                        yield return new PackageImporter().Index(folder.Location, hasAssetStoreLayout);
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
            return IOUtils.PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Unity", AssetStoreFolderName);
#else
            return IOUtils.PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Unity", AssetStoreFolderName);
#endif
        }

        public static void Init()
        {
            string folder = GetStorageFolder();
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            DBAdapter.InitDB();
            PerformUpgrades();
        }

        private static void PerformUpgrades()
        {
            // filename was introduced in version 2
            AppProperty dbVersion = DBAdapter.DB.Find<AppProperty>("Version");

            // Upgrade from Initial to v2
            if (dbVersion == null)
            {
                List<AssetFile> assetFiles = DBAdapter.DB.Table<AssetFile>().ToList();
                foreach (AssetFile assetFile in assetFiles)
                {
                    assetFile.FileName = Path.GetFileName(assetFile.Path);
                }
                DBAdapter.DB.UpdateAll(assetFiles);
            }

            if (dbVersion?.Value != "2")
            {
                AppProperty version = new AppProperty("Version", "2");
                DBAdapter.DB.InsertOrReplace(version);
            }
        }

        public static void ClearCache(Action callback = null)
        {
            ClearCacheInProgress = true;
            Task _ = Task.Run(() =>
            {
                if (Directory.Exists(GetMaterializePath())) Directory.Delete(GetMaterializePath(), true);
                callback?.Invoke();
                ClearCacheInProgress = false;
            });
        }

        private static void LoadConfig()
        {
            string configLocation = GetConfigLocation();
            UsedConfigLocation = configLocation;

            if (configLocation == null || !File.Exists(configLocation))
            {
                _config = new AssetInventorySettings();
                return;
            }
            _config = JsonConvert.DeserializeObject<AssetInventorySettings>(File.ReadAllText(configLocation));
            if (_config == null) _config = new AssetInventorySettings();
            if (_config.folders == null) _config.folders = new List<FolderSpec>();
        }

        public static void SaveConfig()
        {
            string configFile = GetConfigLocation();
            if (configFile == null) return;

            File.WriteAllText(configFile, JsonConvert.SerializeObject(_config));
        }

        public static void ResetConfig()
        {
            DBAdapter.Close(); // in case DB path changes

            _config = new AssetInventorySettings();
            SaveConfig();
            AssetDatabase.Refresh();
        }

        public static async Task<AssetPurchases> FetchOnlineAssets()
        {
            AssetStore.CancellationRequested = false;
            AssetPurchases assets = await AssetStore.RetrievePurchases();

            CurrentMain = "Updating assets";
            MainCount = assets.results.Count;
            MainProgress = 1;
            int progressId = MetaProgress.Start("Updating assets");

            for (int i = 0; i < MainCount; i++)
            {
                MainProgress = i + 1;
                MetaProgress.Report(progressId, i + 1, MainCount, string.Empty);
                if (i % BreakInterval == 0) await Task.Yield(); // let editor breath
                if (AssetStore.CancellationRequested) break;

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
                    existing.DisplayName = purchase.displayName.Trim();
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
                if (i % BreakInterval == 0) await Task.Yield(); // let editor breath
                if (AssetStore.CancellationRequested) break;

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

        public static int CountPurchasedAssets(IEnumerable<AssetInfo> assets)
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

        public static void MoveDatabase(string targetFolder)
        {
            string targetDBFile = Path.Combine(targetFolder, Path.GetFileName(DBAdapter.GetDBPath()));
            if (File.Exists(targetDBFile)) File.Delete(targetDBFile);
            string oldStorageFolder = GetStorageFolder();
            DBAdapter.Close();

            bool success = false;
            try
            {
                // for safety copy first, then delete old state after everything is done
                EditorUtility.DisplayProgressBar("Moving Database", "Copying database to new location...", 0.2f);
                File.Copy(DBAdapter.GetDBPath(), targetDBFile);
                EditorUtility.ClearProgressBar();

                EditorUtility.DisplayProgressBar("Moving Preview Images", "Copying preview images to new location...", 0.4f);
                IOUtils.CopyDirectory(GetPreviewFolder(), GetPreviewFolder(targetFolder));
                EditorUtility.ClearProgressBar();

                // set new location
                SwitchDatabase(targetFolder);
                success = true;
            }
            catch
            {
                EditorUtility.DisplayDialog("Error Moving Data", "There were errors moving the existing database to a new location. Check the error log for details. Current database location remains unchanged.", "OK");
            }

            if (success)
            {
                EditorUtility.DisplayProgressBar("Freeing Up Space", "Removing backup files from old location...", 0.8f);
                Directory.Delete(oldStorageFolder, true);
                EditorUtility.ClearProgressBar();
            }
        }

        public static void SwitchDatabase(string targetFolder)
        {
            DBAdapter.Close();
            Config.customStorageLocation = targetFolder;
            SaveConfig();
        }

        public static void ForgetAsset(int id)
        {
            DBAdapter.DB.Query<AssetFile>("delete from AssetFile where AssetId=?", id);
            DBAdapter.DB.Query<Asset>("delete from Asset where Id=?", id);
        }

        public static async Task<string> CopyTo(AssetInfo assetInfo, string selectedPath, bool withDependencies = false, bool withScripts = false)
        {
            string result = null;
            string sourcePath = await EnsureMaterializedAsset(assetInfo);
            if (sourcePath != null)
            {
                string finalPath = selectedPath;

                // put into subfolder if multiple files are affected
                if (withDependencies)
                {
                    finalPath = Path.Combine(finalPath, Path.GetFileNameWithoutExtension(assetInfo.FileName));
                    if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
                }

                string targetPath = Path.Combine(finalPath, Path.GetFileName(sourcePath));
                DoCopyTo(sourcePath, targetPath);
                result = targetPath;

                if (withDependencies)
                {
                    List<AssetFile> deps = withScripts ? assetInfo.Dependencies : assetInfo.MediaDependencies;
                    for (int i = 0; i < deps.Count; i++)
                    {
                        // check if already in target
                        if (!string.IsNullOrEmpty(deps[i].Guid))
                        {
                            if (!string.IsNullOrWhiteSpace(AssetDatabase.GUIDToAssetPath(deps[i].Guid))) continue;
                        }

                        sourcePath = await EnsureMaterializedAsset(assetInfo.ToAsset(), deps[i]);
                        targetPath = Path.Combine(finalPath, Path.GetFileName(deps[i].Path));
                        DoCopyTo(sourcePath, targetPath);
                    }
                }

                AssetDatabase.Refresh();
                assetInfo.ProjectPath = AssetDatabase.GUIDToAssetPath(assetInfo.Guid);
            }

            return result;
        }

        private static void DoCopyTo(string sourcePath, string targetPath)
        {
            File.Copy(sourcePath, targetPath, true);

            string sourceMetaPath = sourcePath + ".meta";
            string targetMetaPath = targetPath + ".meta";
            if (File.Exists(sourceMetaPath)) File.Copy(sourceMetaPath, targetMetaPath, true);
        }

        public static async Task PlayAudio(AssetInfo assetInfo)
        {
            string targetPath = await EnsureMaterializedAsset(assetInfo);

            EditorAudioUtility.StopAllPreviewClips();
            if (targetPath != null)
            {
                AudioClip clip = await AssetUtils.LoadAudioFromFile(targetPath);
                if (clip != null) EditorAudioUtility.PlayPreviewClip(clip);
            }
        }
    }
}