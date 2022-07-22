#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !UNITY_2019_1_OR_NEWER
#define ACTK_UWP_NO_IL2CPP
#endif

namespace CodeStage.AntiCheat.ObscuredTypes
{
	using System;
	using System.Numerics;

	public partial struct ObscuredBigInteger : IFormattable, IEquatable<ObscuredBigInteger>, IEquatable<BigInteger>, IComparable<ObscuredBigInteger>, IComparable<BigInteger>, IComparable
	{
		#region operators, overrides, interface implementations

		//! @cond
		public static implicit operator ObscuredBigInteger(BigInteger value)
		{
			return new ObscuredBigInteger(value);
		}

		public static implicit operator ObscuredBigInteger(byte value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(sbyte value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(short value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(ushort value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(int value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(uint value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(long value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(ulong value)
		{
			return new ObscuredBigInteger(value);
		}
		
		public static implicit operator ObscuredBigInteger(float value)
		{
			return new ObscuredBigInteger((BigInteger)value);
		}
		
		public static implicit operator ObscuredBigInteger(double value)
		{
			return new ObscuredBigInteger((BigInteger)value);
		}
		
		public static implicit operator ObscuredBigInteger(decimal value)
		{
			return new ObscuredBigInteger((BigInteger)value);
		}
		
		public static implicit operator BigInteger(ObscuredBigInteger value)
		{
			return value.InternalDecrypt();
		}
		
		public static explicit operator byte(ObscuredBigInteger value)
		{
			return (byte)value.InternalDecrypt();
		}
		
		public static explicit operator sbyte(ObscuredBigInteger value)
		{
			return (sbyte)value.InternalDecrypt();
		}
		
		public static explicit operator short(ObscuredBigInteger value)
		{
			return (short)value.InternalDecrypt();
		}
		
		public static explicit operator ushort(ObscuredBigInteger value)
		{
			return (ushort)value.InternalDecrypt();
		}
		
		public static explicit operator int(ObscuredBigInteger value)
		{
			return (int)value.InternalDecrypt();
		}
		
		public static explicit operator uint(ObscuredBigInteger value)
		{
			return (uint)value.InternalDecrypt();
		}
		
		public static explicit operator long(ObscuredBigInteger value)
		{
			return (long)value.InternalDecrypt();
		}
		
		public static explicit operator ulong(ObscuredBigInteger value)
		{
			return (ulong)value.InternalDecrypt();
		}
		
		public static explicit operator float(ObscuredBigInteger value)
		{
			return (float)value.InternalDecrypt();
		}
		
		public static explicit operator double(ObscuredBigInteger value)
		{
			return (double)value.InternalDecrypt();
		}
		
		public static explicit operator decimal(ObscuredBigInteger value)
		{
			return (decimal)value.InternalDecrypt();
		}
		
		public static ObscuredBigInteger operator &(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left & (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator |(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left | (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator ^(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left ^ (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator <<(ObscuredBigInteger value, int shift)
		{
			return (BigInteger)value << shift;
		}
		
		public static ObscuredBigInteger operator >>(ObscuredBigInteger value, int shift)
		{
			return (BigInteger)value >> shift;
		}
		
		public static ObscuredBigInteger operator ~(ObscuredBigInteger value)
		{
			return ~(BigInteger)value;
		}
		
		public static ObscuredBigInteger operator +(ObscuredBigInteger value)
		{
			return +(BigInteger)value;
		}
		
		public static ObscuredBigInteger operator ++(ObscuredBigInteger input)
		{
			return Increment(input, 1);
		}
		
		public static ObscuredBigInteger operator --(ObscuredBigInteger input)
		{
			return Increment(input, -1);
		}
		
		public static ObscuredBigInteger operator +(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left + (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator -(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left - (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator *(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left * (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator /(ObscuredBigInteger dividend, ObscuredBigInteger divisor)
		{
			return (BigInteger)dividend / (BigInteger)divisor;
		}
		
		public static ObscuredBigInteger operator %(ObscuredBigInteger dividend, ObscuredBigInteger divisor)
		{
			return (BigInteger)dividend % (BigInteger)divisor;
		}
		
		public static bool operator <(ObscuredBigInteger dividend, ObscuredBigInteger divisor)
		{
			return (BigInteger)dividend < (BigInteger)divisor;
		}
		
		public static bool operator <=(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left <= (BigInteger)right;
		}
		
		public static bool operator >(ObscuredBigInteger dividend, ObscuredBigInteger divisor)
		{
			return (BigInteger)dividend > (BigInteger)divisor;
		}
		
		public static bool operator >=(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left >= (BigInteger)right;
		}
		
		public static bool operator ==(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return left.hiddenValue.value == right.hiddenValue.value;
		}
		
		public static bool operator !=(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return left.hiddenValue.value != right.hiddenValue.value;
		}
		
		public static bool operator <(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left < (BigInteger)right;
		}
		
		public static bool operator <=(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left <= (BigInteger)right;
		}
		
		public static bool operator >(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left > right;
		}
		
		public static bool operator >=(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left >= right;
		}
		
		public static bool operator ==(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left == right;
		}
		
		public static bool operator !=(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left != right;
		}
		
		public static bool operator <(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left < (BigInteger)right;
		}
		
		public static bool operator <=(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left <= (BigInteger)right;
		}
		
		public static bool operator >(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left > right;
		}
		
		public static bool operator >=(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left >= right;
		}
		
		public static bool operator ==(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left == right;
		}
		
		public static bool operator !=(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left != right;
		}
		
		public static bool operator <(long left, ObscuredBigInteger right)
		{
			return left < (BigInteger)right;
		}
		
		public static bool operator <=(long left, ObscuredBigInteger right)
		{
			return left <= (BigInteger)right;
		}
		
		public static bool operator >(long left, ObscuredBigInteger right)
		{
			return left > (BigInteger)right;
		}
		
		public static bool operator >=(long left, ObscuredBigInteger right)
		{
			return left >= (BigInteger)right;
		}
		
		public static bool operator ==(long left, ObscuredBigInteger right)
		{
			return left == (BigInteger)right;
		}
		
		public static bool operator !=(long left, ObscuredBigInteger right)
		{
			return left != (BigInteger)right;
		}
		
		public static bool operator <(ulong left, ObscuredBigInteger right)
		{
			return left < (BigInteger)right;
		}
		
		public static bool operator <=(ulong left, ObscuredBigInteger right)
		{
			return left <= (BigInteger)right;
		}
		
		public static bool operator >(ulong left, ObscuredBigInteger right)
		{
			return left > (BigInteger)right;
		}
		
		public static bool operator >=(ulong left, ObscuredBigInteger right)
		{
			return left >= (BigInteger)right;
		}
		
		public static bool operator ==(ulong left, ObscuredBigInteger right)
		{
			return left == (BigInteger)right;
		}
		
		public static bool operator !=(ulong left, ObscuredBigInteger right)
		{
			return left != (BigInteger)right;
		}
		
		private static ObscuredBigInteger Increment(ObscuredBigInteger input, int increment)
		{
			var decrypted = input.InternalDecrypt() + increment;
			input.hiddenValue.value = decrypted;
			input.hiddenValue = input.hiddenValue.Encrypt(input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				input.fakeValue.value = decrypted;
				input.fakeValueActive = true;
			}
			else
			{
				input.fakeValueActive = false;
			}

			return input;
		}

		public override int GetHashCode()
		{
			return hiddenValue.GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider) ?? string.Empty;
		}

		public override bool Equals(object obj)
		{
			return obj is ObscuredBigInteger other && Equals(other);
		}

		public bool Equals(ObscuredBigInteger other)
		{
			return hiddenValue.value.Equals(other.hiddenValue.value);
		}
		
		public bool Equals(BigInteger other)
		{
			return hiddenValue.value.Equals(other);
		}
		
		public int CompareTo(ObscuredBigInteger other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(BigInteger other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
#if !ACTK_UWP_NO_IL2CPP
			return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is BigInteger)) throw new ArgumentException("Argument must be long");
			return CompareTo((BigInteger)obj);
#endif
		}

		public int CompareTo(long other)
		{
			return InternalDecrypt().CompareTo(other);
		}		
		
		public int CompareTo(ulong other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public byte[] ToByteArray()
		{
			return InternalDecrypt().ToByteArray();
		}

		#endregion
		
		//! @endcond
	}
}
