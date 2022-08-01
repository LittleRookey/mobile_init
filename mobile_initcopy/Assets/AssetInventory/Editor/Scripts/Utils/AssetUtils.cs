using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetInventory
{
    public static class AssetUtils
    {
        private static Regex NO_SPECIAL_CHARS = new Regex("[^a-zA-Z0-9 -]");

        private const int TIMEOUT = 30;

        public static int GetPageCount(int resultCount, int maxResults)
        {
            return (int) Math.Ceiling((double) resultCount / (maxResults > 0 ? maxResults : int.MaxValue));
        }

        public static async Task<AudioClip> LoadAudioFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            // select appropriate audio type from extension where UNKNOWN heuristic can fail, especially for AIFF
            AudioType type = AudioType.UNKNOWN;
            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".aiff":
                    type = AudioType.AIFF;
                    break;
            }

            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, type))
            {
                ((DownloadHandlerAudioClip) uwr.downloadHandler).streamAudio = true;
                UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
                while (!request.isDone) await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result != UnityWebRequest.Result.Success)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
                {
                    Debug.LogError($"Error fetching '{filePath}': {uwr.error}");
                    return null;
                }

                DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip) uwr.downloadHandler;
                if (dlHandler.isDone) return dlHandler.audioClip;
            }

            return null;
        }

        public static IEnumerator LoadTexture(AssetInfo assetInfo)
        {
            string previewFolder = AssetInventory.GetPreviewFolder();
            if (string.IsNullOrEmpty(assetInfo.PreviewImage)) yield break;
            string previewFile = Path.Combine(previewFolder, assetInfo.PreviewImage);
            if (!File.Exists(previewFile)) yield break;

            yield return LoadTexture(previewFile, result => { assetInfo.PreviewTexture = result; });
        }

        public static IEnumerator LoadTexture(string file, Action<Texture2D> callback)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + file);
            yield return www.SendWebRequest();
            callback?.Invoke(DownloadHandlerTexture.GetContent(www));
        }

        public static async Task<T> FetchAPIData<T>(string uri, string token, string etag = null, Action<string> eTagCallback = null)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + token);
                if (!string.IsNullOrEmpty(etag)) uwr.SetRequestHeader("If-None-Match", etag);
                uwr.timeout = TIMEOUT;
                UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
                while (!request.isDone) await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result == UnityWebRequest.Result.ConnectionError)
#else
                if (uwr.isNetworkError)
#endif
                {
                    Debug.LogError($"Could not fetch API data from {uri} due to network issues: {uwr.error}");
                }
#if UNITY_2020_1_OR_NEWER
                else if (uwr.result == UnityWebRequest.Result.ProtocolError)
#else
                else if (uwr.isHttpError)
#endif
                {
                    if (uwr.responseCode == (int) HttpStatusCode.Unauthorized)
                    {
                        Debug.LogError($"Invalid or expired API Token when contacting {uri}");
                    }
                    else
                    {
                        Debug.LogError($"Error fetching API data from {uri}: {uwr.downloadHandler.text}");
                    }
                }
                else
                {
                    if (typeof(T) == typeof(string))
                    {
                        return (T) Convert.ChangeType(uwr.downloadHandler.text, typeof(T));
                    }
                    string newEtag = uwr.GetResponseHeader("ETag");
                    if (!string.IsNullOrEmpty(newEtag)) eTagCallback?.Invoke(newEtag);

                    return JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
                }
            }
            return default;
        }

        public static string GuessSafeName(string name, string replacement = "")
        {
            // remove special characters like Unity does when saving to disk
            // This will work in 99% of cases but sometimes items get renamed and
            // Unity will keep the old safe name so this needs to be synced with the 
            // download info API.
            string clean = name;

            // special characters
            clean = NO_SPECIAL_CHARS.Replace(clean, replacement);

            // duplicate spaces
            clean = Regex.Replace(clean, @"\s+", " ");

            return clean.Trim();
        }
    }
}