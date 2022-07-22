// #region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
// #endregion

namespace CodeStage.AntiCheat.EditorCode.Processors
{
	using System.IO;
	using UnityEditor;
	using UnityEditor.Build;
	using UnityEditor.Build.Reporting;
	using UnityEngine;

	public class BuildPreProcessor : IPreprocessBuildWithReport
	{
		private const string ProGuardExclusion = "-keep class net.codestage.actk.androidnative.ACTkAndroidRoutines { *; }\n" + 
		                                         "-keep class net.codestage.actk.androidnative.CodeHashGenerator {public void GetCodeHash(...);}\n" + 
		                                         "-keep class net.codestage.actk.androidnative.CodeHashCallback { *; }";

		#region Implementation of IOrderedCallback

		public int callbackOrder => int.MinValue;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform == BuildTarget.Android)
				CheckProGuard();
		}
		
		internal static void CheckProGuard(bool calledFromMenu = false)
		{
			if (!calledFromMenu && ACTkUserSettings.Instance.SkipProGuardNotice)
				return;

			if (!calledFromMenu)
			{
				var devBuild = EditorUserBuildSettings.development;
				var minifyEnabled =
#if UNITY_2020_1_OR_NEWER
					(PlayerSettings.Android.minifyRelease && !devBuild) ||
					(PlayerSettings.Android.minifyDebug && devBuild);
#else
					(EditorUserBuildSettings.androidReleaseMinification != AndroidMinification.None && !devBuild) ||
					(EditorUserBuildSettings.androidDebugMinification != AndroidMinification.None && devBuild);
#endif
				if (!minifyEnabled)
					return;
			}

			ApplyProGuardConfiguration(calledFromMenu);
		}

		private static void ApplyProGuardConfiguration(bool calledFromMenu)
		{
			var proguardFilePath = "Assets/Plugins/Android/proguard-user.txt";
			var disabledProguardFilePath = "Assets/Plugins/Android/proguard-user.txt.DISABLED";
			var needsConfiguration = false;
			var activeConfigExists = false;
			
			if (!File.Exists(proguardFilePath))
			{
				needsConfiguration = true; 
			}
			else if (!FileHasACTkAdded(proguardFilePath))
			{
				activeConfigExists = true;
				needsConfiguration = true;
			}

			if (!needsConfiguration)
			{
				if (calledFromMenu)
					EditorUtility.DisplayDialog("Anti-Cheat Toolkit", 
						"ProGuard user file configuration is already applied, no action needed.", "OK");
				return;
			}

			if (!calledFromMenu)
			{
				var choice = EditorUtility.DisplayDialogComplex("Anti-Cheat Toolkit",
					"You're about to make a build with minification, but there is no active ProGuard User file configured to exclude ACTk which can cause runtime exceptions.\n\n" +
					"Would you like to apply Anti-Cheat Toolkit rules to the ProGuard User file?", "Yes", "No",
					"No and don't ask again");

				if (choice != 0)
				{
					if (choice == 2)
						ACTkUserSettings.Instance.SkipProGuardNotice = true;
					return;
				}
			}
			
			var fileNeedsUpdate = true;
			if (!activeConfigExists && File.Exists(disabledProguardFilePath))
			{
				FileUtil.MoveFileOrDirectory(disabledProguardFilePath, proguardFilePath);
				if (FileHasACTkAdded(proguardFilePath))
					fileNeedsUpdate = false;
			}

			if (fileNeedsUpdate)
				AddACTkToFile(proguardFilePath);
			
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			
			if (calledFromMenu)
				EditorUtility.DisplayDialog("Anti-Cheat Toolkit", 
				$"ProGuard user file configuration at {proguardFilePath} updated.", "OK");
		}

		private static void AddACTkToFile(string path)
		{
			var folderPath = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var file = string.Empty;
			
			if (File.Exists(path))
				file = File.ReadAllText(path);

			if (!string.IsNullOrEmpty(file))
				file += "\n";
			
			file += ProGuardExclusion;
			
			File.WriteAllText(path, file);
		}

		private static bool FileHasACTkAdded(string path)
		{
			var file = File.ReadAllText(path);
			if (file.Contains("-keep class net.codestage.actk"))
			{
				// migrating old version of the config
				if (file.Contains("-keep class net.codestage.actk.** { *; }"))
				{
					file = file.Replace("-keep class net.codestage.actk.** { *; }",ProGuardExclusion);
					File.WriteAllText(path, file);
				}
				
				return true;
			}

			return false;
		}

		#endregion
	}
}