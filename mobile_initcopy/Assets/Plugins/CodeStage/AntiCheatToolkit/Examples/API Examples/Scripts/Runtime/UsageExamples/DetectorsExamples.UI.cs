#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if (UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#define ACTK_INJECTION_DETECTOR_ENABLED
#endif

#define ACTK_TIME_CHEATING_DETECTOR_ENABLED

#if ACTK_PREVENT_INTERNET_PERMISSION
#undef ACTK_TIME_CHEATING_DETECTOR_ENABLED
#endif

namespace CodeStage.AntiCheat.Examples
{
	using Detectors;
	using UnityEngine;

	internal partial class DetectorsExamples
	{
		public void DrawUI()
        {
			GUILayout.Label("ACTk is able to detect some types of cheating to let you take action on the cheating players. " +
			                "This example scene has all possible detectors and they do automatically start on scene start.");
			GUILayout.Space(5);
			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				GUILayout.Label($"<b>{SpeedHackDetector.ComponentName}</b>");
				GUILayout.Label("Allows to detect speed hack applied from Cheat Engine, Game Guardian and such.");
				GUILayout.Label($"Running: {SpeedHackDetector.Instance.IsRunning}\n" +
								ExamplesGUI.Colorize($"Detected: {speedHackDetected.ToString().ToLower()}", !speedHackDetected));

#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
				GUILayout.Space(10);
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label($"<b>{TimeCheatingDetector.ComponentName}</b> (updates once per 1 min by default)");
					if (GUILayout.Button("Force check", GUILayout.Width(100)))
					{
						ForceTimeCheatingDetectorCheck();
					}
				}
#else
				GUILayout.Space(10);
				GUILayout.Label($"<b>{TimeCheatingDetector.ComponentName}</b>");
#endif
				GUILayout.Label("Allows to detect system time change to cheat " +
				                "some long-term processes (building progress, etc.).");

#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
				if (wrongTimeDetected && !timeCheatingDetected)
				{
					GUILayout.Label($"Running: {TimeCheatingDetector.Instance.IsRunning}\n" +
					                $"<color=\"{ExamplesGUI.YellowColor}\">" +
					                $"Wrong time detected (diff more than: {TimeCheatingDetector.Instance.wrongTimeThreshold} minutes)</color>");
				}
				else
				{
					GUILayout.Label($"Running: {TimeCheatingDetector.Instance.IsRunning}\n" + 
									ExamplesGUI.Colorize($"Detected: {timeCheatingDetected.ToString()}", !timeCheatingDetected));
				}
#else
				GUILayout.Label("Was disabled with ACTK_PREVENT_INTERNET_PERMISSION compilation symbol.");
#endif
				GUILayout.Space(10);
				GUILayout.Label($"<b>{ObscuredCheatingDetector.ComponentName}</b>");

				GUILayout.Label("Detects cheating of any Obscured type (except ObscuredPrefs, " +
				                "it has own detection features) used in project.");
				GUILayout.Label($"Running: {ObscuredCheatingDetector.Instance.IsRunning}\n" + 
								ExamplesGUI.Colorize($"Detected: {obscuredTypeCheatDetected.ToString()}", !obscuredTypeCheatDetected));

				GUILayout.Space(10);
				GUILayout.Label($"<b>{WallHackDetector.ComponentName}</b>");
				GUILayout.Label("Detects common types of wall hack cheating: walking through the walls " +
				                "(Rigidbody and CharacterController modules), shooting through the walls " +
				                "(Raycast module), looking through the walls (Wireframe module).");
				GUILayout.Label($"Running: {WallHackDetector.Instance.IsRunning}\n" + 
								ExamplesGUI.Colorize($"Detected: {wallHackCheatDetected.ToString()}", !wallHackCheatDetected));

				GUILayout.Space(10);
				GUILayout.Label($"<b>{InjectionDetector.ComponentName}</b>");
				GUILayout.Label("Allows to detect foreign managed assemblies in your application.");

#if ACTK_INJECTION_DETECTOR_ENABLED
				GUILayout.Label($"Running: {InjectionDetector.Instance != null && InjectionDetector.Instance.IsRunning}\n" +
								ExamplesGUI.Colorize($"Detected: {injectionDetected.ToString()}", !injectionDetected));
#else
				GUILayout.Label("Managed injections are not possible in any non-Mono builds (such as WebGL).");
#endif
			}
        }
	}
}