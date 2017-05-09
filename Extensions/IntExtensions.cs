using UnityEngine;
using System.Collections;
using System;

namespace LevelUpper.Extensions {
	/// <summary> Holds some extension methods for the int class </summary>
	public static class IntExtensions {
		/// <summary> Returns true when called on odd ints, false on even ints. </summary>
		/// <param name="i"> Int to check for parity </param>
		/// <returns> True if <paramref name="i"/> is odd parity, false if it is even parity. </returns>
		public static bool IsOdd(this int i) { return i % 2 == 1; }
		/// <summary> Returns true when called on even ints, false on odd ints. </summary>
		/// <param name="i"> Int to check for parity </param>
		/// <returns> True if <paramref name="i"/> is even parity, false if it is odd parity. </returns>
		public static bool IsEven(this int i) { return i % 2 == 0; }
		
		/// <summary> Returns a byte[] that represents <paramref name="i"/> </summary>
		/// <param name="i"> Integer parameter </param>
		/// <returns> <paramref name="i"/> as a byte[] </returns>
		public static byte[] GetBytes(this int i) { return BitConverter.GetBytes(i); }
		
	}
}
