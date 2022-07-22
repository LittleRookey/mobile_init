#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
#define ACTK_ANDROID_DEVICE
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
#define ACTK_WEBGL_BUILD
#endif

namespace CodeStage.AntiCheat.Utils
{
	using System;
	using UnityEngine;

#if ACTK_ANDROID_DEVICE
	using Common;
#endif

	internal static class TimeUtils
	{
		public const long TicksPerSecond = TimeSpan.TicksPerMillisecond * 1000;

#if ACTK_ANDROID_DEVICE
		private const string RoutinesClassPath = "net.codestage.actk.androidnative.ACTkAndroidRoutines";

		private static AndroidJavaClass routinesClass;
		private static bool androidTimeReadAttemptWasMade;
#endif
		
		public static void Uninit()
		{
#if ACTK_ANDROID_DEVICE
			routinesClass?.Dispose();
			routinesClass = null;
			androidTimeReadAttemptWasMade = false;
#endif
		}

		/// <summary>
		/// Gets speed hacks unbiased current time ticks.
		/// </summary>
		/// <returns>Reliable current time in ticks.</returns>
		public static long GetReliableTicks()
		{
			long ticks = 0;
#if ACTK_ANDROID_DEVICE
			ticks = TryReadTicksFromAndroidRoutine();
#elif ACTK_WEBGL_BUILD
			ticks = TryReadTicksFromWebGLRoutine();
#endif
			if (ticks == 0)
				ticks = DateTime.UtcNow.Ticks;

			return ticks;
		}

		public static long GetEnvironmentTicks()
		{
			return Environment.TickCount * TimeSpan.TicksPerMillisecond;
		}

		public static long GetRealtimeTicks()
		{
			return (long)(Time.realtimeSinceStartup * TicksPerSecond);
		}
		
		public static long GetDspTicks()
		{
#if UNITY_AUDIO_MODULE
			return (long)(AudioSettings.dspTime * TicksPerSecond);
#else
			return 0;
#endif
		}

#if ACTK_ANDROID_DEVICE

		private static long TryReadTicksFromAndroidRoutine()
		{
			long result = 0;

			if (!androidTimeReadAttemptWasMade)
			{
				androidTimeReadAttemptWasMade = true;

				try
				{
					routinesClass = new AndroidJavaClass(RoutinesClassPath);
				}
				catch (Exception e)
				{
					ACTk.PrintExceptionForSupport($"Couldn't create instance of the AndroidJavaClass: {RoutinesClassPath}!\n" +
												  "Please make sure you are not obfuscating public ACTk's Java Plugin classes.", e);
				}
			}

			if (routinesClass == null) 
				return result;

			try
			{
				// getting time in nanoseconds from the native Android timer
				// since some random fixed and JVM initialization point
				// (it even may be a future so value could be negative)
				result = routinesClass.CallStatic<long>("GetSystemNanoTime");
				result /= 100;
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Couldn't call static method from the Android Routines Class!", e);
			}

			return result;
		}
		
#elif ACTK_WEBGL_BUILD

		[System.Runtime.InteropServices.DllImport("__Internal")]
		private static extern double GetUTCTicks();

		private static long TryReadTicksFromWebGLRoutine()
		{
			var ticks = (long)GetUTCTicks();
			return ticks < 0 ? 0 : ticks;
		}
		
#endif
	}
}