﻿#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;
	using Genuine.CodeHash;

	internal partial class CodeHashExamples : MonoBehaviour
	{
#pragma warning disable 0649
		// fill this field manually after build with value you see in console
		// (don't forget to enable pre-generation in settings)
		public string savedSummaryHash;
#pragma warning restore 0649

		public HashGeneratorResult LastResult { get; private set; }

		public static bool IsBusy => CodeHashGenerator.Instance.IsBusy;

		public static bool IsSupported => CodeHashGenerator.IsTargetPlatformCompatible();

		public bool IsGenuineValueSetInInspector => !string.IsNullOrEmpty(savedSummaryHash);

		// just to make sure it's added to the scene and Instance will be not empty
		public void Init()
		{
			CodeHashGenerator.AddToSceneOrGetExisting();
		}

		public void StartGeneration()
		{
			// This is a good practice to avoid new requests while generator is busy with previous requests.
			if (CodeHashGenerator.Instance.IsBusy)
			{
				return;
			}

			// Just subscribe to generation event and start generation.
			// Generation runs in separate thread avoiding cpu spikes in main thread.
			// It generates hash only once and cache it for any new requests since compiled code does not change in runtime.
			CodeHashGenerator.HashGenerated += OnHashGenerated;
			CodeHashGenerator.Generate();
		}

		// just to demonstrate how runtime summary hash can be compared against hash you got from Editor
		// please note though SummaryHash can differ in some cases (e.g. Android App Bundles)
		// so it's a good practice to compare summary first and in case it does not match - check if LastResult.FileHashes
		// has any new hash which was not generated from editor, it's an indicator build was altered in such case
		public bool SummaryHashMatches()
		{
			if (LastResult == null)
			{
				return false;
			}

			var buildAltered = savedSummaryHash != LastResult.SummaryHash;

			// here you can perform more accurate hash checking
			// if (buildAltered)
			// {
			//   buildAltered = CheckAndRuntimeHasAnyNewHashes(LastResult.FileHashes);
			// }

			return !buildAltered;
		}

		private void OnHashGenerated(HashGeneratorResult result)
		{
			LastResult = result;
			CodeHashGenerator.HashGenerated -= OnHashGenerated;

			if (result.Success)
			{
				// Here you can upload your hashes to the server to make a validation check on the server side and punish cheater with server logic.
				//
				// This is a preferred use case since cheater will have to figure out proper hash using
				// packets sniffing (https packets harder to sniff) or debugging first to fake it on the client side requiring more
				// skills and motivation from cheater.
				//
				// check SummaryHash first and if it differs (it can if your runtime build has only portion of the initial build you made in Unity)
				// check FileHashes if SummaryHash differs to see if runtime build have any new hashes - it will indicate build is altered
				//
				// UploadHashes(result.SummaryHash, result.FileHashes);

				// Or you may compare it with hardcoded hashes if you did save them somewhere in the build previously.
				//
				// This is less preferred way since cheater can still try to hack your client-side validation check to make it always pass.
				// Anyways, this is better than nothing and will require some additional time from cheater reducing overall motivation to hack your game.
				// In case implementing it fully on the client side, make sure to compile IL2CPP build and use code
				// obfuscation which runs integrated into the Unity build process so hashing will happen AFTER code obfuscation.
				// If obfuscation will happen after hashing it will change code hash and you'll need to re-calculate it
				// using Tools > Code Stage > Anti-Cheat Toolkit > Calculate external build hash feature.
				//
				// if (!CompareHashes(result.SummaryHash, result.FileHashes))
				// {
				//		Debug.Log("You patched my code, cheater!");
				// }
			}
			else
			{
				Debug.LogError("Oh, something went wrong while getting the hash!\n" +
				               "Error message: " + result.ErrorMessage);
			}
		}
	}
}