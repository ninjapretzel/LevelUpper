using UnityEngine;
using System.Collections;
using System.Linq;
using System;


//<summary>Holds extension methods for float data type.</summary>
namespace LevelUpper.Extensions {
	public static class FloatExtensions {

		/// <summary> Get the byte array of a float value </summary>
		public static byte[] GetBytes(this float f) { return BitConverter.GetBytes(f); }
		
		/// <summary> Standard lerp function. Lerps <paramref name="f"/> towards <paramref name="target"/> by <paramref name="v"/>% </summary>
		/// <param name="f"> Value to lerp </param>
		/// <param name="target"> Target value to move towards </param>
		/// <param name="v"> % distance to move </param>
		/// <returns> <paramref name="f"/> + (<paramref name="target"/>-<paramref name="f"/>) * <paramref name="v"/> </returns>
		public static float Lerp(this float f, float target, float v) { return Mathf.Lerp(f, target, v); }
		
		public static float Normalize(this float f, float a, float b) { return f.Position(a, b); }
		/// <summary> Get the relative position of one value inside the range (a, b) </summary>
		/// <param name="f"> Value to get </param>
		/// <param name="a"> First extent </param>
		/// <param name="b"> Second extent </param>
		/// <returns> (<paramref name="f"/>-<paramref name="a"/>) / (<paramref name="b"/>-<paramref name="a"/>)</returns>
		public static float Position(this float f, float a, float b) { return (f-a) / (b-a); }
		
		/// <summary> Wraps Mathf.Clamp(<paramref name="f"/>, <paramref name="a"/>, <paramref name="b"/>) </summary>
		/// <param name="f"> Value to clamp </param>
		/// <param name="a"> First extent </param>
		/// <param name="b"> Second extent </param>
		/// <returns> Mathf.Clamp(<paramref name="f"/>, <paramref name="a"/>, <paramref name="b"/>) </returns>
		public static float Clamp(this float f, float a, float b) { return Mathf.Clamp(f, a, b); }
		/// <summary> Wraps Mathf.Clamp01(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to clamp </param>
		/// <returns> <paramref name="f"/> clamped to range [0, 1] </returns>
		public static float Clamp01(this float f) { return Mathf.Clamp01(f); }
		
		/// <summary> Wraps Mathf.Floor(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to floor </param>
		/// <returns> Floor of <paramref name="f"/> </returns>
		public static float Floor(this float f) { return Mathf.Floor(f); }
		/// <summary> Wraps Mathf.Ceil(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to ceil </param>
		/// <returns> Ceil of <paramref name="f"/> </returns>
		public static float Ceil(this float f) { return Mathf.Ceil(f); }
		/// <summary> Wraps Mathf.Round(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to round </param>
		/// <returns> <paramref name="f"/> Rounded to closest integer </returns>
		public static float Round(this float f) { return Mathf.Round(f); }
		/// <summary> Gets Fractional value of <paramref name="f"/> </summary>
		/// <param name="f"> Value to Fract </param>
		/// <returns> Value in range [0, 1), which is fractional part of <paramref name="f"/> </returns>
		public static float Fract(this float f) { return f - f.Floor(); }
		
		/// <summary> Gets the min of two values </summary>
		/// <returns> minimum between <paramref name="a"/> and <paramref name="b"/> </returns>
		public static float Min(float a, float b) { return a < b ? a : b; }

		/// <summary> Gets the min of three values </summary>
		/// <returns> Minimum between <paramref name="a"/>, <paramref name="b"/>, and <paramref name="c"/> </returns>
		public static float Min(float a, float b, float c) { return (a < b) ? (a < c ? a : c) : (b < c ? b : c); }

		/// <summary> Gets the min of four values </summary>
		/// <returns> Minimum between <paramref name="a"/>, <paramref name="b"/>, <paramref name="c"/>, and <paramref name="d"/> </returns>
		public static float Min(float a, float b, float c, float d) {
			float lowest = a;
			if (b < lowest) { lowest = b; }
			if (c < lowest) { lowest = c; }
			if (d < lowest) { lowest = d; }
			return lowest;
		}

		/// <summary> Gets the minimum of a set of values </summary>
		/// <param name="nums"> Set of values to check </param>
		/// <returns> Minimum value in <paramref name="nums"/> </returns>
		/// <remarks> Used when the number of values is greater than 4. </remarks>
		public static float Min(params float[] nums) {
			float lowest = Single.MaxValue;
			for (int i = 0; i < nums.Length; i++) {
				if (nums[i] < lowest) { lowest = nums[i]; }
			}
			return lowest;
		}
		/// <summary> Gets the min of two values </summary>
		/// <returns> minimum between <paramref name="a"/> and <paramref name="b"/> </returns>
		public static float Max(float a, float b) { return a > b ? a : b; }

		/// <summary> Gets the min of three values </summary>
		/// <returns> Minimum between <paramref name="a"/>, <paramref name="b"/>, and <paramref name="c"/> </returns>
		public static float Max(float a, float b, float c) { return (a > b) ? (a > c ? a : c) : (b > c ? b : c); }

		/// <summary> Gets the min of four values </summary>
		/// <returns> Minimum between <paramref name="a"/>, <paramref name="b"/>, <paramref name="c"/>, and <paramref name="d"/> </returns>
		public static float Max(float a, float b, float c, float d) {
			float highest = a;
			if (b > highest) { highest = b; }
			if (c > highest) { highest = c; }
			if (d > highest) { highest = d; }
			return highest;
		}

		/// <summary> Gets the minimum of a set of values </summary>
		/// <param name="nums"> Set of values to check </param>
		/// <returns> Minimum value in <paramref name="nums"/> </returns>
		/// <remarks> Used when the number of values is greater than 4. </remarks>
		public static float Max(params float[] nums) {
			float highest = Single.MinValue;
			for (int i = 0; i < nums.Length; i++) {
				if (nums[i] > highest) { highest = nums[i]; }
			}
			return highest;
		}

		/// <summary> See if <paramref name="f"/> is NaN </summary>
		/// <param name="f"> Float to check </param>
		/// <returns> True if it was NaN, false otherwise </returns>
		public static bool IsNAN(this float f) { return float.IsNaN(f); }

		/// <summary> See if <paramref name="f"/> is NaN </summary>
		/// <param name="f"> Float to check </param>
		/// <returns> True if it was NaN, false otherwise </returns>
		public static bool IsNaN(this float f) { return float.IsNaN(f); }
		
		/// <summary> Round <paramref name="f"/> to the nearest multiple of <paramref name="v"/> </summary>
		/// <param name="f"> Value to round </param>
		/// <param name="v"> Base to round to multiple of </param>
		/// <returns> <paramref name="f"/> rounded to the nearest multiple of <paramref name="v"/> </returns>
		public static float Nearest(this float f, float v) { return f.Nearest(v, 0); }
		/// <summary> Round <paramref name="f"/> to the nearest multiple of <paramref name="v"/> + <paramref name="f"/></summary>
		/// <param name="f"> Value to round </param>
		/// <param name="v"> Base to round to multiple of </param>
		/// <param name="offset"> offset from multiple to use. </param>
		/// <returns> <paramref name="f"/> Rounded to the closest point of (<paramref name="v"/>*n+<paramref name="offset"/>) </returns>
		public static float Nearest(this float f, float v, float offset) {
			float d = (f + offset) / v;
			return Mathf.Round(d) * v;
		}
		
		/// <summary> Wraps Mathf.Sign(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to get Sign of </param>
		/// <returns> Sign of <paramref name="f"/> (-1 or 1) </returns>
		public static float Sign(this float f) { return Mathf.Sign(f); }
		/// <summary> Wraps Mathf.Abs(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to get Abs of </param>
		/// <returns> Absolute value of <paramref name="f"/> </returns>
		public static float Abs(this float f) { return Mathf.Abs(f); }

		/// <summary> Get the distance <paramref name="val"/> is outside the range [<paramref name="a"/>, <paramref name="b"/>] </summary>
		/// <param name="val"> Value to check </param>
		/// <param name="a"> First extent of range </param>
		/// <param name="b"> Second extent of range </param>
		/// <returns> 0 if <paramref name="val"/> is between [<paramref name="a"/>, <paramref name="b"/>], otherwise the distance outside the range. </returns>
		public static float Outside(this float val, float a, float b) {
			float min = Mathf.Min(a, b);
			float max = Mathf.Max(a, b);
			if (val > min && val < max) { return 0; }
			if (val > max) { return val - max; }
			return val - min;
		}
		
		/// <summary> Interpolate between two values with a cosine curve </summary>
		/// <param name="a"> First extent of range </param>
		/// <param name="b"> Second extent of range </param>
		/// <param name="x"> Position of of value in range </param>
		/// <returns> Smoothly interpolated value between <paramref name="a"/> and <paramref name="b"/> </returns>
		public static float CosInterp(float a, float b, float x) {
			float ft = x * 3.1415927f;
			float f = (1 - Mathf.Cos(ft)) * .5f;
			return  a * (1-f) + b * f;
		}
		/// <summary> Wraps Mathf.Sin(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to get Sin of </param>
		/// <returns> Sin of <paramref name="f"/> </returns>
		public static float Sin(this float f) { return Mathf.Sin(f); }
		/// <summary> Wraps Mathf.Cos(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to get Cos of </param>
		/// <returns> Cos of <paramref name="f"/> </returns>
		public static float Cos(this float f) { return Mathf.Cos(f); }
		/// <summary> Wraps Mathf.Tan(<paramref name="f"/>) </summary>
		/// <param name="f"> Value to get Tan of </param>
		/// <returns> Tan of <paramref name="f"/> </returns>
		public static float Tan(this float f) { return Mathf.Tan(f); }
		
		/// <summary> Gets the Sin of <paramref name="f"/> in the output range [0, 1] </summary>
		/// <param name="f"> Value to get Sin of </param>
		/// <returns> Sin of <paramref name="f"/> in output range [0, 1] </returns>
		public static float Sin01(this float f) { return (f.Sin() + 1f) / 2f; }
		/// <summary> Gets the Cos of <paramref name="f"/> in the output range [0, 1] </summary>
		/// <param name="f"> Value to get Cos of </param>
		/// <returns> Cos of <paramref name="f"/> in output range [0, 1] </returns>
		public static float Cos01(this float f) { return (f.Cos() + 1f) / 2f; } 
		
		
		
		
	}
}
