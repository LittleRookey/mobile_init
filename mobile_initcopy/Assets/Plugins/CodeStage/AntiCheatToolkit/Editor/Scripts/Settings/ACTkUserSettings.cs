using System;
using System.IO;
using CodeStage.AntiCheat.Common;
using CodeStage.EditorCommon.Tools;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode
{
    internal class ACTkUserSettings : ScriptableObject
    {
#if UNITY_2020_1_OR_NEWER
		private const string Folder = "UserSettings";
#else
        private const string Folder = "Library";
#endif
        private const string Path = Folder + "/ACTkSettings.asset";
        
        private static ACTkUserSettings instance;

        [SerializeField] 
        private bool skipProGuardNotice;

        internal bool SkipProGuardNotice
        {
	        get => skipProGuardNotice;
	        set
	        {
		        skipProGuardNotice = value;
		        Save();
	        }
        }

        [field: SerializeField] 
		private string Version { get; set; } = ACTk.Version; // for backward compatibility in future versions

		public static ACTkUserSettings Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = LoadOrCreate();
				return instance;
			}
		}
		
		public static void Delete()
		{
			instance = null;
			CSFileTools.DeleteFile(Path);
		}

		public static void Save()
		{
			SaveInstance(Instance);
		}

		private static ACTkUserSettings LoadOrCreate()
		{
			ACTkUserSettings settings;

			if (!File.Exists(Path))
			{
				settings = CreateNewSettingsFile();
			}
			else
			{
				settings = LoadInstance();

				if (settings == null)
				{
					CSFileTools.DeleteFile(Path);
					settings = CreateNewSettingsFile();
				}
			}

			settings.Version = ACTk.Version;
			return settings;
		}

		private static ACTkUserSettings CreateNewSettingsFile()
		{
			var settingsInstance = CreateInstance();
			SaveInstance(settingsInstance);
			return settingsInstance;
		}

		private static void SaveInstance(ACTkUserSettings settingsInstance)
		{
			if (!Directory.Exists(Folder)) 
				Directory.CreateDirectory(Folder);

			try
			{
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { settingsInstance }, Path, true);
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Can't save user settings!", e);
			}
		}

		private static ACTkUserSettings LoadInstance()
		{
			ACTkUserSettings settingsInstance;

			try
			{
				settingsInstance = (ACTkUserSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception)
			{
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static ACTkUserSettings CreateInstance()
		{
			var newInstance = CreateInstance<ACTkUserSettings>();
			return newInstance;
		}
    }
}