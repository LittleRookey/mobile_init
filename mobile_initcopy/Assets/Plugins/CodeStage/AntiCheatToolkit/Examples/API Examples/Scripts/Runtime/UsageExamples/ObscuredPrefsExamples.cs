#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

// add this line in order to use obscured prefs from code
using CodeStage.AntiCheat.Storage;

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal partial class ObscuredPrefsExamples : MonoBehaviour
	{
		private const string PrefsString = "name";
		private const string PrefsInt = "money";
		private const string PrefsFloat = "lifeBar";
		private const string PrefsBool = "gameComplete";
		private const string PrefsUint = "demoUint";
		private const string PrefsLong = "demoLong";
		private const string PrefsDouble = "demoDouble";
		private const string PrefsVector2 = "demoVector2";
		private const string PrefsVector3 = "demoVector3";
		private const string PrefsQuaternion = "demoQuaternion";
		private const string PrefsRect = "demoRect";
		private const string PrefsColor = "demoColor";
		private const string PrefsColor32 = "demoColor32";
		private const string PrefsByteArray = "demoByteArray";

		[ColorUsage(true, true)]
		[SerializeField]
		#pragma warning disable CS0649
		private Color hdrColor;
		#pragma warning restore CS0649

		private string regularPrefs;
		private string obscuredPrefs;

		private bool savesAlterationDetected;
		private bool foreignSavesDetected;

		// you can keep original PlayerPrefs values while
		// migrating to the ObscuredPrefs if you wish to
		private bool PreservePlayerPrefs
		{
			get => ObscuredPrefs.preservePlayerPrefs;
			set => ObscuredPrefs.preservePlayerPrefs = value;
		}

		private void Awake()
		{
			// you can detect saves alteration attempts using this event
			ObscuredPrefs.NotGenuineDataDetected += OnSavesNotGenuineDataDetected;

			// and even may react on foreign saves (from another device)
			ObscuredPrefs.DataFromAnotherDeviceDetected += OnDataFromAnotherDeviceDetected;

			MigrateFromV1();
		}

		private void OnDestroy()
		{
			DeleteRegularPrefs();
			DeleteObscuredPrefs();
		}

		private void OnSavesNotGenuineDataDetected()
		{
			savesAlterationDetected = true;
		}

		private void OnDataFromAnotherDeviceDetected()
		{
			foreignSavesDetected = true;
		}

		private void LoadRegularPrefs()
		{
			regularPrefs = "int: " + PlayerPrefs.GetInt(PrefsInt, -1) + "\n";
			regularPrefs += "float: " + PlayerPrefs.GetFloat(PrefsFloat, -1) + "\n";
			regularPrefs += "string: " + PlayerPrefs.GetString(PrefsString, "No saved PlayerPrefs!");
		}

		private void SaveRegularPrefs()
		{
			PlayerPrefs.SetInt(PrefsInt, 456);
			PlayerPrefs.SetFloat(PrefsFloat, 456.789f);
			PlayerPrefs.SetString(PrefsString, "Hey, there!");
			PlayerPrefs.Save();
		}

		private void DeleteRegularPrefs()
		{
			PlayerPrefs.DeleteKey(PrefsInt);
			PlayerPrefs.DeleteKey(PrefsFloat);
			PlayerPrefs.DeleteKey(PrefsString);
			PlayerPrefs.Save();
		}

		private void ApplyDeviceLockLevel(DeviceLockLevel level)
		{
			// you can lock saves to the device (so they can't be read on another device)
			// there are few different levels of strictness, please see DeviceLockSettings
			// API docs for additional details;
			// set to None by default (does not lock to device)
			ObscuredPrefs.DeviceLockSettings.Level = level;
		}

		private void ApplyTamperingSensitivity(DeviceLockTamperingSensitivity value)
		{
			// in case you can't read locked to the device saves for
			// some reason (e.g. device ID unexpectedly changed), you
			// can use different DeviceLockTamperingSensitivity modes
			// to bypass lock to device and recover your saves
			ObscuredPrefs.DeviceLockSettings.Sensitivity = value;
		}

		private void LoadObscuredPrefs()
		{
			// you can store typical int, float and string at the ObscuredPrefs
			obscuredPrefs = "int: " + ObscuredPrefs.Get(PrefsInt, -1) + "\n";
			obscuredPrefs += "float: " + ObscuredPrefs.Get(PrefsFloat, -1f) + "\n";
			obscuredPrefs += "string: " + ObscuredPrefs.Get(PrefsString, "No saved ObscuredPrefs!") + "\n";

			// comparing to the vanilla PlayerPrefs, you have much more freedom on what to save
			obscuredPrefs += "bool: " + ObscuredPrefs.Get(PrefsBool, false) + "\n";
			obscuredPrefs += "uint: " + ObscuredPrefs.Get(PrefsUint, 0u) + "\n";
			obscuredPrefs += "long: " + ObscuredPrefs.Get(PrefsLong, -1L) + "\n";
			obscuredPrefs += "double: " + ObscuredPrefs.Get(PrefsDouble, -1d) + "\n";
			obscuredPrefs += "Vector2: " + ObscuredPrefs.Get(PrefsVector2, Vector2.zero) + "\n";
			obscuredPrefs += "Vector3: " + ObscuredPrefs.Get(PrefsVector3, Vector3.zero) + "\n";
			obscuredPrefs += "Quaternion: " + ObscuredPrefs.Get(PrefsQuaternion, Quaternion.identity) + "\n";
			obscuredPrefs += "Rect: " + ObscuredPrefs.Get(PrefsRect, new Rect(0, 0, 0, 0)) + "\n";
			obscuredPrefs += "Color: " + ObscuredPrefs.Get(PrefsColor, Color.black) + "\n";
			obscuredPrefs += "Color32: " + ObscuredPrefs.Get(PrefsColor, (Color32)Color.black) + "\n";

			// you can even store raw byte array with any data inside
			var ba = ObscuredPrefs.Get(PrefsByteArray, new byte[4]);
			obscuredPrefs += "byte[]: {" + ba[0] + "," + ba[1] + "," + ba[2] + "," + ba[3] + "}";
		}

		private void SaveObscuredPrefs()
		{
			// same types as at the regular PlayerPrefs
			ObscuredPrefs.Set(PrefsInt, 123);
			ObscuredPrefs.Set(PrefsFloat, 123.456f);
			ObscuredPrefs.Set(PrefsString, "Goscurry is not a lie ;)");

			// some of additional types, just for example,
			// see full supported types list at StorageDataType enum
			ObscuredPrefs.Set(PrefsBool, true);
			ObscuredPrefs.Set(PrefsUint, 1234567891u);
			ObscuredPrefs.Set(PrefsLong, 1234567891234567890L);
			ObscuredPrefs.Set(PrefsDouble, 1.234567890123456d);
			ObscuredPrefs.Set(PrefsVector2, Vector2.one);
			ObscuredPrefs.Set(PrefsVector3, Vector3.one);
			ObscuredPrefs.Set(PrefsQuaternion, Quaternion.Euler(new Vector3(10, 20, 30)));
			ObscuredPrefs.Set(PrefsRect, new Rect(1.5f, 2.6f, 3.7f, 4.8f));
			ObscuredPrefs.Set(PrefsColor, hdrColor);
			ObscuredPrefs.Set<Color32>(PrefsColor32, Color.red);
			ObscuredPrefs.Set(PrefsByteArray, new byte[] { 44, 104, 43, 32 });
			
			ObscuredPrefs.Save();
		}

		private void DeleteObscuredPrefs()
		{
			ObscuredPrefs.DeleteKey(PrefsInt);
			ObscuredPrefs.DeleteKey(PrefsFloat);
			ObscuredPrefs.DeleteKey(PrefsString);
			ObscuredPrefs.DeleteKey(PrefsBool);
			ObscuredPrefs.DeleteKey(PrefsUint);
			ObscuredPrefs.DeleteKey(PrefsLong);
			ObscuredPrefs.DeleteKey(PrefsDouble);
			ObscuredPrefs.DeleteKey(PrefsVector2);
			ObscuredPrefs.DeleteKey(PrefsVector3);
			ObscuredPrefs.DeleteKey(PrefsQuaternion);
			ObscuredPrefs.DeleteKey(PrefsRect);
			ObscuredPrefs.DeleteKey(PrefsColor);
			ObscuredPrefs.DeleteKey(PrefsColor32);
			ObscuredPrefs.DeleteKey(PrefsByteArray);
			
			ObscuredPrefs.Save();
		}

		private void MigrateFromV1()
		{
			// here is a migration example for the raw values you got from ACTk v1 ObscuredPrefs.GetRawValue()
			//
			// first you need to encrypt clean prefs key value with the v1 encryption
			// (you can ignore second argument to use default Crypto Key)
			// var v1PrefsKey = ObscuredPrefs.EncryptKeyWithACTkV1Algorithm(prefsKey, cryptoKey);
			//
			// then just set it back as normal (v1RawValue - value you got from v1 GetRawValue())
			// ObscuredPrefs.SetRawValue(v1PrefsKey, v1RawValue);
			//
			// now you can read it back as usual and it will be automatically migrated to v2 format:
			// var savedData = ObscuredPrefs.GetFloat(prefsKey);
		}
	}
}