#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !UNITY_2019_1_OR_NEWER
#define ACTK_UWP_NO_IL2CPP
#endif

namespace CodeStage.AntiCheat.Examples.Genuine
{
	using Utils;
	using UnityEngine;
	using CodeStage.AntiCheat.Genuine.CodeHash;

#if !ACTK_UWP_NO_IL2CPP
	using System;
	using System.IO;
	using ObscuredTypes;
#endif

	// use this to check hash generated with CodeHashGeneratorListener.cs example file
	// note: this is an example for the Windows Standalone platform only!
	public class GenuineValidatorExample : MonoBehaviour
	{
#if !ACTK_UWP_NO_IL2CPP
		// 💖 looks like a really lovely separator =)
		// used at CodeHashGeneratorListener
		public const string Separator = "💖";
		
		// let's choose some non-obvious file name which will not be hashed (not .dll or .exe)
		// used at CodeHashGeneratorListener
		public const string FileName = "Textures.asset";
		
		// used at CodeHashGeneratorListener
		public static readonly char[] StringKey = {'\x674', '\x345', '\x856', '\x968', '\x322'};
		
		private string status;

		// just an unoptimized example of SHA1 hashing
		public static string GetHash(string firstBuildHash)
		{
			var stringBytes = StringUtils.StringToBytes(firstBuildHash);
			var sha1 = new SHA1Wrapper();
			var hash = sha1.ComputeHash(stringBytes);
			sha1.Clear();
			return StringUtils.HashBytesToHexString(hash);
		}
#endif
		
		private void Awake()
		{
#if !ACTK_UWP_NO_IL2CPP
			CodeHashGenerator.HashGenerated += OnGotHash;
			status = "Press Check";
#endif
		}

		private void OnGUI()
		{
#if !ACTK_UWP_NO_IL2CPP
			if (GUILayout.Button("Check"))
			{
				OnCheckHashClick();
			}
			GUILayout.Label(status);
#endif
		}

#if !ACTK_UWP_NO_IL2CPP
		private void OnCheckHashClick()
		{
			status = "Checking...";
			CodeHashGenerator.Generate();
		}

		private void OnGotHash(HashGeneratorResult result)
		{
			if (!result.Success)
			{
				status = "Error: " + result.ErrorMessage;
				return;
			}

			var filePath = Path.Combine(Path.GetFullPath(Application.dataPath + @"\..\"), FileName);
			if (!File.Exists(filePath))
			{
				status = "No super secret file found, you're cheater!\n" + filePath + "\nSummaryHash: " + result.SummaryHash;
				return;
			}

			var allBytes = File.ReadAllBytes(filePath);
			var allChars = BytesToUnicodeChars(allBytes);
			var decrypted = ObscuredString.Decrypt(allChars, StringKey);

			var separatorIndex = decrypted.IndexOf(Separator, StringComparison.InvariantCulture);
			if (separatorIndex == -1)
			{
				status = "Super secret file is corrupted, you're cheater!\nSummaryHash: " + result.SummaryHash;
				return;
			}

			var whitelistedHashes = decrypted.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
			var originalSummaryHash = whitelistedHashes[0];

			// compare summary hashes first
			if (originalSummaryHash != result.SummaryHash)
			{
				// check all files against whitelisted hashes if summary differs
				// (it can differ if some files are absent due to build separation)
				for (var i = 1; i < whitelistedHashes.Length; i++)
				{
					var hash = whitelistedHashes[i];
					if (!result.HasFileHash(hash))
					{
						status = "Code hash differs, you're cheater!\nSummary hashes:\n" + originalSummaryHash + "\n" + result.SummaryHash + "\nWhitelisted hashes count: " + whitelistedHashes.Length;
						return;
					}
				}
			}

			status = "All fine!\nSummaryHash: " + result.SummaryHash;
		}

		public static char[] BytesToUnicodeChars(byte[] input)
		{
			var chars = new char[input.Length / sizeof(char)];
			Buffer.BlockCopy(input, 0, chars, 0, input.Length);
			return chars;
		}

		public static byte[] UnicodeCharsToBytes(char[] input)
		{
			var bytes = new byte[input.Length * sizeof(char)];
			Buffer.BlockCopy(input, 0, bytes, 0, bytes.Length);
			return bytes;
		}
#endif
	}
}