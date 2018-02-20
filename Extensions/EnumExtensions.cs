using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LevelUpper.Extensions {
	public static class EnumExtensions {

#if NET_4_6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct {
#if NET_4_6
			return Enum.TryParse<TEnum>(value, out result);
#else
			if (!typeof(TEnum).IsEnum) { throw new ArgumentException("Type provided must be an Enum."); }
			try {
				result = (TEnum)Enum.Parse(typeof(TEnum), value);
				return true;
			} catch(ArgumentException) {
				result = default(TEnum);
				return false;
			} catch(OverflowException) {
				result = default(TEnum);
				return false;
			}
#endif
		}

#if NET_4_6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct {
#if NET_4_6
			return Enum.TryParse<TEnum>(value, ignoreCase, out result);
#else
			if (!typeof(TEnum).IsEnum) { throw new ArgumentException("Type provided must be an Enum."); }
			try {
				result = (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
				return true;
			} catch (ArgumentException) {
				result = default(TEnum);
				return false;
			} catch (OverflowException) {
				result = default(TEnum);
				return false;
			}
#endif
		}


		public static TEnum[] AllMembers<TEnum>() where TEnum : struct {
			Type type = typeof(TEnum);
			if (type.IsEnum) {
				return (TEnum[]) Enum.GetValues(type);
			}
			
			return null;
		}
		
	}
}
