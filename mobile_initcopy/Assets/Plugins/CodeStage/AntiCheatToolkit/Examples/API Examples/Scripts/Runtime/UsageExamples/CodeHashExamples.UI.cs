#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal partial class CodeHashExamples
	{
		public void DrawUI()
		{
			GUILayout.Label("ACTk is able to generate hash signature of your code included into the build. " +
							"You can compare current hash with genuine hash to figure out if your code was altered. " +
							"It's better to do this comparison on the server side so cheater couldn't alter it as well.");
			GUILayout.Space(5);

			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				if (IsSupported)
				{
					// just to make sure it's added to the scene and Instance will be not empty
					Init();

					if (!IsBusy)
					{
						if (LastResult != null)
						{
							if (LastResult.Success)
							{
								GUILayout.Label($"Generated Summary Hash: {LastResult.SummaryHash}");
								GUILayout.Label(IsGenuineValueSetInInspector
									? $"Hash matches value from inspector: {SummaryHashMatches()}"
									: "No genuine hash was set in inspector.");
							}
							else
							{
								GUILayout.Label($"Error: {LastResult.ErrorMessage}");
							}
						}
						else
						{
							if (GUILayout.Button("Generate Hash"))
							{
								StartGeneration();
							}
						}
					}
					else
					{
						GUILayout.Label("Generating...");
					}
				}
				else
				{
					GUILayout.Label("Code Hash Generator works only in Standalone Windows and Android builds, starting from Unity 2018.1.");
				}
			}
		}
	}
}