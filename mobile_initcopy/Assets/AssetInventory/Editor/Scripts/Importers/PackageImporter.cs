using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    public class PackageImporter : AssertImporter
    {
        private const int BREAK_INTERVAL = 3;

        public IEnumerator Index(string path, bool fromAssetStore)
        {
            ResetState();
            string[] packages = Directory.GetFiles(path, "*.unitypackage", SearchOption.AllDirectories);
            MainCount = packages.Length;

            int progressId = MetaProgress.Start("Updating asset inventory index");

            for (int i = 0; i < packages.Length; i++)
            {
                string package = packages[i];
                MetaProgress.Report(progressId, i + 1, packages.Length, package);

                yield return null; // let editor breath
                long size = new FileInfo(package).Length;

                // create asset and add additional information from file system
                Asset asset = new Asset();
                asset.SafeName = Path.GetFileNameWithoutExtension(package);
                if (fromAssetStore)
                {
                    asset.AssetSource = Asset.Source.AssetStorePackage;
                    DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(package));
                    asset.SafeCategory = dirInfo.Name;
                    asset.SafePublisher = dirInfo.Parent.Name;
                    if (string.IsNullOrEmpty(asset.DisplayCategory))
                    {
                        asset.DisplayCategory = System.Text.RegularExpressions.Regex.Replace(asset.SafeCategory, "([a-z])([A-Z])", "$1/$2", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                    }
                }
                else
                {
                    asset.AssetSource = Asset.Source.CustomPackage;
                }

                // skip unchanged 
                Asset existing = Fetch(asset);
                if (existing != null && existing.CurrentState == Asset.State.Done && existing.PackageSize == size && existing.Location == package) continue;

                // update progress only if really doing work to save refresh time in UI
                CurrentMain = package + " (" + EditorUtility.FormatBytes(size) + ")";
                MainProgress = i + 1;

                asset.CurrentState = Asset.State.InProcess;
                asset.Location = package;
                asset.PackageSize = size;
                asset.ETag = null; // force rechecking of download metadata
                Persist(asset);

                Task<List<AssetFile>> thread = IndexPackage(asset, progressId);
                yield return new WaitUntil(() => thread.IsCompleted);
                if (CancellationRequested) break;

                asset.CurrentState = Asset.State.Done;
                Persist(asset);
            }
            MetaProgress.Remove(progressId);
            ResetState();
        }

        private async Task<List<AssetFile>> IndexPackage(Asset asset, int progressId)
        {
            int subProgressId = MetaProgress.Start("Indexing package", null, progressId);
            string previewPath = AssetInventory.GetPreviewFolder();
            if (!Directory.Exists(previewPath)) Directory.CreateDirectory(previewPath);

            string tempPath = await AssetInventory.ExtractAsset(asset);
            string assetPreviewFile = Path.Combine(tempPath, ".icon.png");
            if (File.Exists(assetPreviewFile))
            {
                string targetFile = Path.Combine(previewPath, "a-" + asset.Id + Path.GetExtension(assetPreviewFile));
                File.Copy(assetPreviewFile, targetFile, true);
                asset.PreviewImage = Path.GetFileName(targetFile);
            }

            List<AssetFile> files = new List<AssetFile>();

            string[] assets = Directory.GetFiles(tempPath, "pathname", SearchOption.AllDirectories);
            SubCount = assets.Length;
            for (int i = 0; i < assets.Length; i++)
            {
                string packagePath = assets[i];
                string dir = Path.GetDirectoryName(packagePath);
                string fileName = File.ReadLines(packagePath).FirstOrDefault();
                string metaFile = Path.Combine(dir, "asset.meta");
                string assetFile = Path.Combine(dir, "asset");
                string previewFile = Path.Combine(dir, "preview.png");

                CurrentSub = fileName;
                SubProgress = i + 1;
                MetaProgress.Report(subProgressId, i + 1, assets.Length, fileName);

                // skip folders
                if (!File.Exists(assetFile)) continue;

                if (i % BREAK_INTERVAL == 0) await Task.Yield(); // let editor breath

                string guid = null;
                if (File.Exists(metaFile))
                {
                    // read guid from meta
                    guid = File.ReadLines(metaFile).FirstOrDefault(line => line.StartsWith("guid:"));
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogWarning($"Could not find meta file in '{dir}'");
                        continue;
                    }
                    guid = guid.Substring(6);
                }

                // remaining info from file data (creation date is not original date anymore, ignore)
                FileInfo assetInfo = new FileInfo(assetFile);
                long size = assetInfo.Length;

                AssetFile file = new AssetFile();
                file.Guid = guid;
                file.AssetId = asset.Id;
                file.Path = fileName;
                file.SourcePath = assetFile.Substring(tempPath.Length + 1);
                file.FileName = Path.GetFileName(file.Path);
                file.Size = size;
                file.Type = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();

                // special processing for supported file types
                if (file.Type == "png" || file.Type == "jpg")
                {
                    Texture2D tmpTexture = new Texture2D(1, 1);
                    byte[] assetContent = File.ReadAllBytes(assetFile);
                    if (tmpTexture.LoadImage(assetContent))
                    {
                        file.Width = tmpTexture.width;
                        file.Height = tmpTexture.height;
                    }
                }
                if (AssetInventory.IsFileType(fileName, "Audio"))
                {
                    string contentFile = await AssetInventory.EnsureMaterializedAsset(asset, file);
                    try
                    {
                        AudioClip clip = await AssetUtils.LoadAudioFromFile(contentFile);
                        if (clip != null) file.Length = clip.length;
                    }
                    catch
                    {
                        Debug.LogWarning($"Audio file '{Path.GetFileName(contentFile)}' from asset '{asset.SafeName}' seems to have incorrect format.");
                    }
                }
                Persist(file);

                // update preview 
                if (File.Exists(previewFile))
                {
                    string targetFile = Path.Combine(previewPath, "af-" + file.Id + Path.GetExtension(previewFile));
                    File.Copy(previewFile, targetFile, true);
                    file.PreviewFile = Path.GetFileName(targetFile);
                    Persist(file);
                }

                files.Add(file);
                if (CancellationRequested) break;
            }

            // remove files again, no need to wait
            Task _ = Task.Run(() => Directory.Delete(tempPath, true));

            CurrentSub = null;
            SubCount = 0;
            SubProgress = 0;
            MetaProgress.Remove(subProgressId);

            return files;
        }
    }
}