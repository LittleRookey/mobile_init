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

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal partial class ObscuredFilePrefsExamples
	{
#if !ACTK_UWP_NO_IL2CPP
		private readonly string[] sizes = { "B", "KB", "MB", "GB" };

		private bool isLoadedGUI;
		private bool isFileLoadingGUI;
#if ACTK_ASYNC
		private bool isSettingBytesGUI;
		private bool isGettingBytesGUI;
#endif
		private bool isFileSavingGUI;

		private bool unloadPrefs;
		private bool loadPrefs;
		private bool loadPrefsAsync;
		private bool setStringPref;
		private bool getStringPref;
		
		public void DrawUI()
		{
			// change vars affecting OnGUI layout at the EventType.Layout
			// to avoid errors while painting OnGUI
			if (Event.current.type == EventType.Layout)
			{
				isLoadedGUI = IsLoaded;
				isFileLoadingGUI = isFileLoading;
				isFileSavingGUI = isFileSaving;

#if ACTK_ASYNC
				isSettingBytesGUI = isSettingBytes;
				isGettingBytesGUI = isGettingBytes;
#endif
			}
			
			GUILayout.Label("ACTk has secure version of traditional binary file with few helpful " +
							"APIs built on top of it to make it as easy to use as usual Player Prefs.");

			using (new GUILayout.HorizontalScope(GUI.skin.box))
			{
				using (new GUILayout.VerticalScope(GUILayout.Width(250)))
				{
					DrawStatusPanel();
				}

				// lock UI while waiting for something
				GUI.enabled = !isFileLoading && !isFileSaving;
#if ACTK_ASYNC
				GUI.enabled &= !isGettingBytes && !isSettingBytes;
#endif
				
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label("Sandbox");
					
					GUILayout.Space(5f);

					DrawLoadUnloadPanel();
					
					GUILayout.Space(5f);
					
					DrawStringPrefPanel();
					
#if ACTK_ASYNC
					GUILayout.Space(5f);
					DrawBinaryPrefPanel();
#endif
					GUILayout.Space(5f);

					if (!IsLoaded)
						GUI.enabled = false;

					DrawSavePanel();
					
					GUI.enabled = IsExists && !isFileLoading && !isFileSaving;
#if ACTK_ASYNC
					GUI.enabled &= !isGettingBytes && !isSettingBytes;
#endif

					if (GUILayout.Button("Delete all file prefs"))
					{
						DeleteAllPrefs();
					}

					GUI.enabled = true;
				}
			}

			// these are called at OnGUI end to avoid changing
			// layout state causing logic conflicts
			if (unloadPrefs)
			{
				UnloadPrefs();
				unloadPrefs = false;
			}
			
			if (loadPrefs)
			{
				LoadPrefs();
				loadPrefs = false;
			}
			
#if ACTK_ASYNC
			if (loadPrefsAsync)
			{
				LoadPrefsAsync();
				loadPrefsAsync = false;
			}
#endif
			
			if (setStringPref)
			{
				SetStringPref();
				setStringPref = false;
			}
			
			if (getStringPref)
			{
				GetStringPref();
				getStringPref = false;
			}
		}

		private void DrawStatusPanel()
		{
			var keys = GetKeys();
			var keysLabel = keys == null ? "Empty / not loaded" : string.Join(", ", keys);
			
			GUILayout.Label("Status");
			GUILayout.Label($"File exists: {ExamplesGUI.Colorize(IsExists.ToString(), IsExists)}\n" +
							$"Prefs Loaded: {ExamplesGUI.Colorize(IsLoaded.ToString(), IsLoaded)}\n" + 
							$"Prefs keys: {keysLabel}\n" + 
							$"Data corruption detected: {ExamplesGUI.Colorize(NotGenuine.ToString(), !NotGenuine)}\n" +
							$"Data from another device detected: {ExamplesGUI.Colorize(FromAnotherDevice.ToString(), !FromAnotherDevice)}");
		}
		
		private void DrawLoadUnloadPanel()
		{
			if (!isLoadedGUI)
			{
				if (isFileLoadingGUI)
				{
					GUILayout.Label("Loading...");
				}
				else
				{
					using (new GUILayout.HorizontalScope())
					{
						if (GUILayout.Button("Load prefs"))
						{
							loadPrefs = true;
						}
#if ACTK_ASYNC
						if (GUILayout.Button("Load prefs async"))
						{
							loadPrefsAsync = true;
						}
#endif
					}
				}
			}
			else
			{
				if (GUILayout.Button("Unload prefs"))
				{
					unloadPrefs = true;
				}
			}
		}

		private void DrawStringPrefPanel()
		{
			if (!isLoadedGUI)
				return;
			
			GUILayout.Label("Enter any data you wish to save:");
			dataToSave = GUILayout.TextArea(dataToSave,GUILayout.Height(40));

			if (GUILayout.Button("Set string pref"))
				setStringPref = true;

			if (!string.IsNullOrEmpty(LoadedData))
				GUILayout.Label("Loaded data: " + LoadedData);

			var guiEnabled = GUI.enabled;
			GUI.enabled = guiEnabled && HasKey(StringKey);

			if (GUILayout.Button("Load string pref"))
				getStringPref = true;

			GUI.enabled = guiEnabled;
		}
		
#if ACTK_ASYNC
		private void DrawBinaryPrefPanel()
		{
			if (!isLoadedGUI)
				return;
			
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label("Random bytes: ", GUILayout.ExpandWidth(false));
				
				var value = GUILayout.TextField(bytesToSave.ToString(), GUILayout.Width(100));
				long.TryParse(value, out bytesToSave);

				if (bytesToSave > 1073741824) // cap it to 1 GB in demo
					bytesToSave = 1073741824;
				
				GUILayout.Label($" {FormatBytes(bytesToSave)}");
			}
			
			if (isSettingBytesGUI)
				GUILayout.Label("Setting & hashing bytes...");
			else if (savedBytesHash != 0)
				GUILayout.Label($"Set byte[] with hash: {savedBytesHash}");
			
			if (isGettingBytesGUI)
				GUILayout.Label("Getting & hashing bytes...");
			else if (loadedBytesHash != 0)
				GUILayout.Label($"Got byte[] with hash: {loadedBytesHash}");

			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Set & hash byte[] pref"))
					SetBytesPref();

				if (GUILayout.Button("Set & hash byte[] pref async"))
					SetBytesPrefAsync();
			}

			var guiEnabled = GUI.enabled;
			GUI.enabled = guiEnabled && HasKey(ByteArrayKey);
			
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Get & hash byte[]"))
					GetBytesPref();

				if (GUILayout.Button("Get & hash byte[] async"))
					GetBytesPrefAsync();
			}
			
			GUI.enabled = guiEnabled;
		}
#endif
		
		private void DrawSavePanel()
		{
			if (!isLoadedGUI)
				return;
			
			if (isFileSavingGUI)
			{
				GUILayout.Label("Saving...");
				return;
			}
			
			if (GUILayout.Button("Save prefs file"))
				Save();
					
#if ACTK_ASYNC
			if (GUILayout.Button("Save prefs file async"))
				SaveAsync();
#endif
		}
		
		public string FormatBytes(double bytes)
		{
			var order = 0;

			while (bytes >= 1024 && order + 1 < 4)
			{
				order++;
				bytes /= 1024;
			}

			return $"{bytes:0.##} {sizes[order]}";
		}

#else
		public void DrawUI()
		{
			GUILayout.Label("ACTk has secure version of traditional binary file with few helpful " +
							"APIs built on top of it to make it as easy to use as usual Player Prefs.");
			GUILayout.FlexibleSpace();
			GUILayout.Label("This feature does not supports .NET UWP. Please switch to IL2CPP UWP in order to use it.");
			GUILayout.FlexibleSpace();
		}
#endif
	}
}