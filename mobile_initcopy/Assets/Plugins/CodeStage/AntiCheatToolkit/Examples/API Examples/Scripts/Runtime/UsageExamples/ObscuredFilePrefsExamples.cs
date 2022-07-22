#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if !UNITY_WEBGL
#define ACTK_ASYNC
#endif

#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !UNITY_2019_1_OR_NEWER
#define ACTK_UWP_NO_IL2CPP
#endif

// add this line in order to use obscured file prefs from code
using CodeStage.AntiCheat.Storage;

namespace CodeStage.AntiCheat.Examples
{
#if !ACTK_UWP_NO_IL2CPP
	#if ACTK_ASYNC
	using System.Threading.Tasks;
	using Utils;
	#endif
	using System.Collections.Generic;
#endif
	using UnityEngine;

	internal partial class ObscuredFilePrefsExamples : MonoBehaviour
	{
#if !ACTK_UWP_NO_IL2CPP
		private const string StringKey = "string example";
		
#if ACTK_ASYNC
		private const string ByteArrayKey = "byte[] example";
#endif
		
		[SerializeField]
		private string dataToSave;
		
#if ACTK_ASYNC
		[SerializeField]
		private long bytesToSave = 16777216; // 16 MB
#endif
		
		private uint savedBytesHash;
		private uint loadedBytesHash;
		
		private string LoadedData { get; set; }
		private bool NotGenuine { get; set; }
		private bool FromAnotherDevice { get; set; }

		private bool IsExists => ObscuredFilePrefs.IsExists;
		private bool IsLoaded => ObscuredFilePrefs.IsLoaded;
		
		private bool isFileSaving;
		private bool isFileLoading;
		
#if ACTK_ASYNC
		private bool isSettingBytes;
		private bool isGettingBytes;
#endif

		private void Start()
		{
			var deviceLockSettings = new DeviceLockSettings(DeviceLockLevel.Strict);
			var encryptionSettings = new EncryptionSettings("super secret password");
			var settings = new ObscuredFileSettings(encryptionSettings, deviceLockSettings);

			// Always initialize ObscuredFilePrefs before using.
			// In this case, it's initialized with such settings:
			// - device lock enabled (Strict),
			// - encryption enabled (with "super secret password")
			// - data integrity validation: true
			// - file location: default (Application.persistentDataPath)
			// - file name: "ACTkFilePrefs.bin"
			// - immediate prefs loading: false
			ObscuredFilePrefs.Init("ACTkFilePrefs.bin", settings, false);
			
			// Subscribe to these events to detect data integrity violation and data from another devices:
			ObscuredFilePrefs.NotGenuineDataDetected += OnNotGenuineDataDetected;
			ObscuredFilePrefs.DataFromAnotherDeviceDetected += OnDataFromAnotherDeviceDetected;
			
			// Feel free to access different settings and properties like this:
			Debug.Log($"{nameof(ObscuredFilePrefs)} file path: {ObscuredFilePrefs.FilePath}\n" +
					  $"{nameof(ObscurationMode)}: {ObscuredFilePrefs.CurrentSettings.EncryptionSettings.ObscurationMode}");
#if ACTK_ASYNC
			// Call this if you're going to use ObscuredFile or ObscuredFilePrefs
			// from non-main thread while using Lock To Device feature without custom DeviceId
			// or with default file path to avoid exception due to Unity API access from non-main thread.
			Common.UnityApiResultsHolder.InitForAsyncUsage(true);
			
			PrewarmAsync();
#endif
		}
		
#if ACTK_ASYNC
		private async void PrewarmAsync()
		{
			// just to prewarm async stuff so first
			// async call would not produce spike
			await Task.Delay(500);
		}
		
		private void SetBytesPref()
		{
			isSettingBytes = true;
			
			var data = new byte[bytesToSave];
			for (var i = 0; i < bytesToSave; i++)
			{
				data[i] = (byte)ThreadSafeRandom.Next(0, 256);
			}

			savedBytesHash = xxHash.CalculateHash(data, data.Length, 4444);
			ObscuredFilePrefs.Set(ByteArrayKey, data);

			isSettingBytes = false;
		}

		private void GetBytesPref()
		{
			isGettingBytes = true;
			
			var data = ObscuredFilePrefs.Get(ByteArrayKey, System.Array.Empty<byte>());
			loadedBytesHash = xxHash.CalculateHash(data, data.Length, 4444);

			isGettingBytes = false;
		}
		
		// Here are examples of async usage from non-main thread.
		// Please note ObscuredFile and ObscuredFilePrefs are not thread-safe and
		// should not be used simultaneously from different threads.
		private async void SetBytesPrefAsync()
		{
			savedBytesHash = 0;
			await Task.Run(SetBytesPref);
		}

		private async void GetBytesPrefAsync()
		{
			loadedBytesHash = 0;
			await Task.Run(GetBytesPref);
		}

		private async void LoadPrefsAsync()
		{
			await Task.Run(LoadPrefs);
		}
		
		private async void SaveAsync()
		{
			await Task.Run(Save);
		}
#endif
		private void LoadPrefs()
		{
			if (isFileLoading)
				return;
			
			isFileLoading = true;
			ObscuredFilePrefs.LoadPrefs();
			isFileLoading = false;
		}

		private void UnloadPrefs()
		{
			ObscuredFilePrefs.UnloadPrefs(false);
		}

		private bool HasKey(string key)
		{
			return ObscuredFilePrefs.HasKey(key);
		}
		
		private ICollection<string> GetKeys()
		{
			return IsLoaded ? ObscuredFilePrefs.GetKeys() : null;
		}

		private void SetStringPref()
		{
			ObscuredFilePrefs.Set(StringKey, dataToSave);
		}

		private void GetStringPref()
		{
			LoadedData = ObscuredFilePrefs.Get(StringKey, "No data found!");
		}
		
		private void DeleteAllPrefs()
		{
			ObscuredFilePrefs.DeleteAll();
		}

		private void OnNotGenuineDataDetected()
		{
			NotGenuine = true;
		}

		private void OnDataFromAnotherDeviceDetected()
		{
			FromAnotherDevice = true;
		}

		private void Save()
		{
			if (isFileSaving)
				return;
			
			isFileSaving = true;
			
			if (!ObscuredFilePrefs.Save())
				Debug.LogError("Couldn't save prefs!");
			
			isFileSaving = false;
		}
#endif
	}
}