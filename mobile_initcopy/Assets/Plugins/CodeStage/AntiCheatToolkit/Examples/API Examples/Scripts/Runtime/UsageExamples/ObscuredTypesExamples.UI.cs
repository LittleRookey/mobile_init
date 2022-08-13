#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !UNITY_2019_1_OR_NEWER
#define ACTK_UWP_NO_IL2CPP
#endif

namespace CodeStage.AntiCheat.Examples
{
	using System.Linq;
	using ObscuredTypes;
	using UnityEngine;

	internal partial class ObscuredTypesExamples
	{
		private string allSimpleObscuredTypes;
		
		public void DrawUI(ExamplesGUI examplesGUIInstance)
		{
			GUILayout.Label("ACTk offers own collection of the secure types to let you protect your variables from <b>ANY</b> memory hacking tools (Cheat Engine, ArtMoney, GameCIH, Game Guardian, etc.).");
			GUILayout.Label("Below you can try to cheat few variables of the regular types and their obscured (secure) analogues (you may change initial values from Tester object inspector):");
			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label("<b>Obscured types:</b>", GUILayout.MinWidth(130));
					GUILayout.Label($"<color=\"{ExamplesGUI.BlueColor}\">{Get()}</color>", GUILayout.MinWidth(130));
				}
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					#region int
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>int:</b> {regularInt}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							regularInt += Random.Range(1, 100);
						}
						if (GUILayout.Button("Reset"))
						{
							regularInt = 0;
						}
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>ObscuredInt:</b> {obscuredInt}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredInt += Random.Range(1, 100);
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredInt = 0;
						}
					}
					#endregion

					#region float
					GUILayout.Space(10);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>float:</b> {regularFloat}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							regularFloat += Random.Range(1f, 100f);
						}
						if (GUILayout.Button("Reset"))
						{
							regularFloat = 0;
						}
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>ObscuredFloat:</b> {obscuredFloat}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredFloat += Random.Range(1f, 100f);
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredFloat = 0;
						}
					}
					#endregion

					#region Vector3
					GUILayout.Space(10);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>Vector3:</b> {regularVector3}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							regularVector3 += Random.insideUnitSphere;
						}
						if (GUILayout.Button("Reset"))
						{
							regularVector3 = Vector3.zero;
						}
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>ObscuredVector3:</b> {obscuredVector3}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredVector3 += Random.insideUnitSphere;
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredVector3 = Vector3.zero;
						}
					}

					var detected = examplesGUIInstance.DetectorsExamples.obscuredTypeCheatDetected;
					GUILayout.Label(ExamplesGUI.Colorize($"Cheating attempt detected: {detected.ToString()}", !detected));

					#endregion
				}
			}
		}
		
		private string Get()
		{
			var result = "Can't use reflection here, sorry :(";
#if ACTK_UWP_NO_IL2CPP
			return result;
#else
			var types = "";

			if (string.IsNullOrEmpty(allSimpleObscuredTypes))
			{
				var csharpAssembly = typeof(ObscuredInt).Assembly;
				var q = from t in csharpAssembly.GetTypes()
					where t.IsPublic && t.Namespace == "CodeStage.AntiCheat.ObscuredTypes" && t.Name != "IObscuredType"
					select t;
				q.ToList().ForEach(t =>
				{
					if (types.Length > 0)
					{
						types += "\n" + (t.Name);
					}
					else
					{
						types += (t.Name);
					}
				});

				if (!string.IsNullOrEmpty(types))
				{
					result = types;
					allSimpleObscuredTypes = types;
				}
				else
				{
					allSimpleObscuredTypes = result;
				}
			}
			else
			{
				result = allSimpleObscuredTypes;
			}
			return result;
#endif
		}
	}
}