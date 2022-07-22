#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using System;
	using Storage;

	using UnityEngine;

	[AddComponentMenu("")]
	internal class ExamplesGUI : MonoBehaviour
	{
		private enum ExamplePage
		{
			ObscuredTypes = 0,
			SavesProtection = 1,
			Detectors = 2,
			CodeHashing = 3
		}

		internal const string RedColor = "#FF4040";
		internal const string YellowColor = "#E9D604";
		internal const string GreenColor = "#02C85F";
		internal const string BlueColor = "#75C4EB";
		
		internal static GUIStyle centeredStyle;

		private ObscuredTypesExamples obscuredTypesExamples;
		private ObscuredPrefsExamples obscuredPrefsExamples;
		private ObscuredFilePrefsExamples obscuredFilePrefsExamples;
		private DetectorsExamples detectorsExamples;
		private CodeHashExamples codeHashExamples;

		private readonly string[] tabs = {"Variables protection", "Saves protection", "Cheating detectors", "Code Hashing"};
		private readonly string[] savesTabs = {"ObscuredPrefs", "ObscuredFilePrefs"};
		private ExamplePage currentPage;
		private int savesPage;

		public DetectorsExamples DetectorsExamples => detectorsExamples;

		private void Awake()
		{
			obscuredTypesExamples = GetComponent<ObscuredTypesExamples>();
			obscuredPrefsExamples = GetComponent<ObscuredPrefsExamples>();
			obscuredFilePrefsExamples = GetComponent<ObscuredFilePrefsExamples>();
			detectorsExamples = GetComponent<DetectorsExamples>();
			codeHashExamples = GetComponent<CodeHashExamples>();

		}

		private void OnGUI()
		{		
			if (centeredStyle == null)
			{
				centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperCenter};
			}

			GUILayout.BeginArea(new Rect(10, 5, Screen.width - 20, Screen.height - 10));

			GUILayout.Label("<color=\"#0287C8\"><b>Anti-Cheat Toolkit Sandbox</b></color>", centeredStyle);
			GUILayout.Label("Here you can overview common ACTk features and try to cheat something yourself.", centeredStyle);
			GUILayout.Space(5);

			currentPage = (ExamplePage)GUILayout.Toolbar((int)currentPage, tabs);

			switch (currentPage)
			{
				case ExamplePage.ObscuredTypes:
				{
					obscuredTypesExamples.DrawUI(this);
					break;
				}
				case ExamplePage.SavesProtection:
				{
					DrawSavesProtectionPage();
					break;
				}
				case ExamplePage.Detectors:
				{
					detectorsExamples.DrawUI();
					break;
				}
				case ExamplePage.CodeHashing:
				{
					codeHashExamples.DrawUI();
					break;
				}
			}
			GUILayout.EndArea();
		}

		private void DrawSavesProtectionPage()
		{
			savesPage = GUILayout.Toolbar(savesPage, savesTabs);
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope(GUILayout.MinWidth(130)))
				{
					GUILayout.Label("<b>Supported types:</b>");
					GUILayout.Label(GetAllObscuredPrefsDataTypes(), GUILayout.MaxWidth(100));
				}
				
				GUILayout.Space(10);
				
				using (new GUILayout.VerticalScope())
				{
					switch (savesPage)
					{
						case 0:
						{
							obscuredPrefsExamples.DrawUI();
							break;
						}
						case 1:
						{
							obscuredFilePrefsExamples.DrawUI();
							break;
						}
					}
				}
			}
		}

		internal static string Colorize(string stringToWrap, bool green)
		{
			return Colorize(stringToWrap, green ? GreenColor : RedColor);
		}

		private static string Colorize(string stringToWrap, string color)
		{
			return $"<color=\"{color}\">{stringToWrap}</color>";
		}
		
		private static string GetAllObscuredPrefsDataTypes()
		{
			var result = "<color=\"" + BlueColor + "\">";
			var values = Enum.GetNames(typeof(StorageDataType));

			for (var i = 0; i < values.Length; i++)
			{
				var value = values[i];
				var lowerCase = value.ToLowerInvariant();

				if (lowerCase.Contains(StorageDataType.Unknown.ToString().ToLowerInvariant()))
					continue;

				result += lowerCase;
				if (i != values.Length - 1)
					result += ", ";
			}

			result += "</color>";

			return result;
		}
    }
}