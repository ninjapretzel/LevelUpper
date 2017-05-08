using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LevelUpper.RNG {

	/// <summary> Class that provides similar behaviour to the UnityEngine.Random class, but with a more controlled state. </summary>
	public static class Random {
		/// <summary> Current Seed Value </summary>
		public static long seed { get; set; }
		/// <summary> Stack of seed history. </summary>
		private static Stack<long> seeds;

		#region Bookkeeping
		/// <summary> Force a function to run during static initialization </summary>
		public static bool loaded = Load();
		/// <summary> Loads this class </summary>
		/// <returns> always true </returns>
		static bool Load() {
			seed = DateTime.Now.Ticks;
			seeds = new Stack<long>();
			return true;
		}

		/// <summary> Remember the current seed, and change the sequence to use a given seed. </summary>
		/// <param name="s"> New seed to use </param>
		public static void Push(long s) { seeds.Push(seed); seed = s; }
		/// <summary> Restore the sequence to the last remembered sequence, if there are any </summary>
		/// <returns> Previous value of <see cref="seed"/> </returns>
		public static long Pop() {
			long s = seed;
			if (seeds.Count > 0) { seed = seeds.Pop(); } else { Debug.LogWarning("LevelUpper.Random: No seed to pop!"); }
			return s;
		}

		/// <summary> Forwards the random class to the next seed value </summary>
		/// <returns> The next hash value </returns>
		static long nextSeed { get { return seed = SRNG.hash(seed); } }
		#endregion

		#region Basic Properties
		/// <summary> Provides a "Random" value, evenly distributed in the range [0, 1) </summary>
		public static float value { get { return SRNG.Float32(nextSeed); } }

		/// <summary> Provides a "Random" value, evenly distributed in the range [-1, 1) </summary>
		public static float unit { get { return -1f + (2f * value); } }

		/// <summary> Provides a "Random" value, roughly normally distributed in the range [-1, 1) </summary>
		/// <remarks> Implemented by 3 samples of value, divided by 3. </remarks>
		public static float normal { get { return (-1 + (2f * (value + value + value) / 3f)); } }

		/// <summary> Returns a "Random" Int32 from [0, int.MaxValue) </summary>
		public static int rawInt { get { return SRNG.Int32(int.MaxValue, nextSeed); } }
		/// <summary> Returns a "Random" byte value from [0, 256) </summary>
		public static byte rawByte { get { return (byte)rawInt; } }

		/// <summary> Provides a "Random" character from 'a' to 'z' </summary>
		public static char alpha { get { return (char)('a' + (int)(value * 26)); } }
		/// <summary> Provides a "Random" character from 'A' to 'A' </summary>
		public static char ALPHA { get { return (char)('A' + (int)(value * 26)); } }
		/// <summary> Provides a "Random" character from '0' to '9' </summary>
		public static char numeric { get { return (char)('0' + (int)(value * 10)); } }

		/// <summary> Gets a "Random" point inside a cube centered at (0, 0, 0) with sides (1, 1, 1) </summary>
		public static Vector3 insideUnitCube { get { return new Vector3(Range(-.5f, .5f), Range(-.5f, .5f), Range(-.5f, .5f)); } }
		/// <summary> Gets a "Random" point inside a square centered at (0, 0) with sides (1, 1) </summary>
		public static Vector2 insideUnitSquare { get { return new Vector2(Range(-.5f, .5f), Range(-.5f, .5f)); } }

		/// <summary> Gets a "Random" point inside a sphere centered at (0, 0, 0) with radius 1 </summary>
		public static Vector3 insideUnitSphere { get { return insideUnitSquare.normalized * value; } }
		/// <summary> Gets a "Random" point inside a circle centered at (0, 0) with radius 1 </summary>
		public static Vector2 insideUnitCircle { get { return insideUnitSquare.normalized * value; } }

		/// <summary> Gets a "Random" point on a sphere centered at (0, 0, 0) with radius 1 </summary>
		public static Vector3 onUnitSphere { get { return insideUnitCube.normalized; } }
		/// <summary> Gets a "Random" point on a circle centered at (0, 0) with radius 1 </summary>
		public static Vector3 onUnitCircle { get { return insideUnitSquare.normalized; } }
		#endregion

		#region Basic Functions
		/// <summary> Returns an Int32 in the range [<paramref name="min"/>, <paramref name="max"/>) </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value inside range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static int Range(int min, int max) { return SRNG.Int32Range(min, max, nextSeed); }
		/// <summary> Returns a Float32 in the range [<paramref name="min"/>, <paramref name="max"/>) </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value inside range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static float Range(float min, float max) { return SRNG.Float32Range(min, max, nextSeed); }
		/// <summary> Returns a value centered at <paramref name="avg"/> with maximum deviation of +/-<paramref name="dev"/>. </summary>
		/// <param name="avg"> Average value </param>
		/// <param name="dev"> Maximum deviation </param>
		/// <returns> Value in range [<paramref name="avg"/>-<paramref name="dev"/>, <paramref name="avg"/>+<paramref name="dev"/>) </returns>
		public static float Normal(float avg, float dev) { return avg + (dev * normal); }
		#endregion

		#region Choosing
		public static int WeightedChoose(IList<float> weights) {
			float total = 0;
			int i;
			for (i = 0; i < weights.Count; i++) { total += weights[i]; }

			float choose = value * total;
			float check = 0;
			for (i = 0; i < weights.Count; i++) {
				check += weights[i];
				if (choose < check) { return i; }
			}
			return weights.Count - 1;
		}

		#endregion

	}

}
