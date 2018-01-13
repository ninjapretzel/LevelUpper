using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LevelUpper.RNG {
	/// <summary> Class that provides similar behaviour to the UnityEngine.Random class, but with a more controlled state. </summary>
	public class Rand {
		/// <summary> Current Seed Value </summary>
		public long seed { get; set; }
		/// <summary> Stack of seed history. </summary>
		private Stack<long> seeds;

		#region Bookkeeping
		public Rand() {
			seed = DateTime.UtcNow.Ticks;
			seeds = new Stack<long>();
		}

		/// <summary> Remember the current seed, and change the sequence to use a given seed. </summary>
		/// <param name="s"> New seed to use </param>
		public void Push(long s) { seeds.Push(seed); seed = s; }
		/// <summary> Restore the sequence to the last remembered sequence, if there are any </summary>
		/// <returns> Previous value of <see cref="seed"/> </returns>
		public long Pop() {
			long s = seed;
			if (seeds.Count > 0) { seed = seeds.Pop(); } else { Debug.LogWarning("LevelUpper.Random: No seed to pop!"); }
			return s;
		}

		/// <summary> Forwards the random class to the next seed value </summary>
		/// <returns> The next hash value </returns>
		long nextSeed { get { return seed = SRNG.hash(seed); } }
		#endregion

		#region Basic Properties
		/// <summary> Provides a "Random" value, evenly distributed in the range [0, 1) </summary>
		public float value { get { return SRNG.Float32(nextSeed); } }

		/// <summary> Provides a "Random" value, evenly distributed in the range [-1, 1) </summary>
		public float unit { get { return -1f + (2f * value); } }

		/// <summary> Provides a "Random" value, roughly normally distributed in the range [-1, 1) </summary>
		/// <remarks> Implemented by 3 samples of value, divided by 3. </remarks>
		public float normal { get { return (-1 + (2f * (value + value + value) / 3f)); } }

		/// <summary> Returns a "Random" Int32 from [0, int.MaxValue) </summary>
		public int rawInt { get { return SRNG.Int32(int.MaxValue, nextSeed); } }
		/// <summary> Returns a "Random" byte value from [0, 256) </summary>
		public byte rawByte { get { return (byte)rawInt; } }

		/// <summary> Provides a "Random" character from 'a' to 'z' </summary>
		public char alpha { get { return (char)('a' + (int)(value * 26)); } }
		/// <summary> Provides a "Random" character from 'A' to 'A' </summary>
		public char ALPHA { get { return (char)('A' + (int)(value * 26)); } }
		/// <summary> Provides a "Random" character from '0' to '9' </summary>
		public char numeric { get { return (char)('0' + (int)(value * 10)); } }

		/// <summary> Gets a "Random" point inside a cube centered at (0, 0, 0) with sides (1, 1, 1) </summary>
		public Vector3 insideUnitCube { get { return new Vector3(Range(-.5f, .5f), Range(-.5f, .5f), Range(-.5f, .5f)); } }
		/// <summary> Gets a "Random" point inside a square centered at (0, 0) with sides (1, 1) </summary>
		public Vector2 insideUnitSquare { get { return new Vector2(Range(-.5f, .5f), Range(-.5f, .5f)); } }

		/// <summary> Gets a "Random" point inside a sphere centered at (0, 0, 0) with radius 1 </summary>
		public Vector3 insideUnitSphere { get { return insideUnitSquare.normalized * value; } }
		/// <summary> Gets a "Random" point inside a circle centered at (0, 0) with radius 1 </summary>
		public Vector2 insideUnitCircle { get { return insideUnitSquare.normalized * value; } }

		/// <summary> Gets a "Random" point on a sphere centered at (0, 0, 0) with radius 1 </summary>
		public Vector3 onUnitSphere { get { return insideUnitCube.normalized; } }
		/// <summary> Gets a "Random" point on a circle centered at (0, 0) with radius 1 </summary>
		public Vector3 onUnitCircle { get { return insideUnitSquare.normalized; } }

		/// <summary> Gets a "Random" color in the RGB channels, with an alpha of 1 </summary>
		public Color rgb { get { return new Color(value, value, value, 1); } }
		/// <summary> Gets a "Random" color in the RGBA channels </summary>
		public Color rgba { get { return new Color(value, value, value, value); } }

		/// <summary> Gets a "Random" Color32 in the RGB channels, with an alpha of 255. </summary>
		public Color32 RGB { 
			get { 
				byte[] bytes = BitConverter.GetBytes(rawInt);
				return new Color32(bytes[3], bytes[2], bytes[1], 255);
			}
		}

		/// <summary> Gets a "Random" Color32 in the RGBA channels </summary>
		public Color32 RGBA {
			get {
				byte[] bytes = BitConverter.GetBytes(rawInt);
				return new Color32(bytes[3], bytes[2], bytes[1], bytes[0]);
			}
		}

		/// <summary> Returns a rotation made by rotating from (-180, 180) on all of X/Y/Z axis </summary>
		public Quaternion rotation { get { return Quaternion.Euler(insideUnitCube * 360); } }
		#endregion

		#region Basic Functions
		/// <summary> Returns an Int32 in the range [<paramref name="min"/>, <paramref name="max"/>) </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value inside range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public int Range(int min, int max) { return SRNG.Int32Range(min, max, nextSeed); }
		/// <summary> Returns a Float32 in the range [<paramref name="min"/>, <paramref name="max"/>) </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value inside range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public float Range(float min, float max) { return SRNG.Float32Range(min, max, nextSeed); }
		/// <summary> Returns a value centered at <paramref name="avg"/> with maximum deviation of +/-<paramref name="dev"/>. </summary>
		/// <param name="avg"> Average value </param>
		/// <param name="dev"> Maximum deviation </param>
		/// <returns> Value in range [<paramref name="avg"/>-<paramref name="dev"/>, <paramref name="avg"/>+<paramref name="dev"/>) </returns>
		public float Normal(float avg, float dev) { return avg + (dev * normal); }
		#endregion

		#region Choosing
		/// <summary> Chooses an index from a list of weights </summary>
		/// <param name="weights"> Weights to the given choice </param>
		/// <returns> Random index from range [0, <paramref name="weights"/>.Count) </returns>
		public int WeightedChoose(IList<float> weights) {
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



	/// <summary> Class that provides similar behaviour to the UnityEngine.Random class, but with a more controlled state. </summary>
	public static class Random {
		/// <summary> Instance of random object </summary>
		private static Rand instance = new Rand();

		/// <summary> Remember the current seed, and change the sequence to use a given seed. </summary>
		/// <param name="s"> New seed to use </param>
		public static void Push(long s) { instance.Push(s); }
		/// <summary> Restore the sequence to the last remembered sequence, if there are any </summary>
		/// <returns> Previous value of <see cref="seed"/> </returns>
		public static long Pop() { return instance.Pop(); }
		
		#region Basic Properties
		/// <summary> Provides a "Random" value, evenly distributed in the range [0, 1) </summary>
		public static float value { get { return instance.value; } }

		/// <summary> Provides a "Random" value, evenly distributed in the range [-1, 1) </summary>
		public static float unit { get { return instance.unit; } }

		/// <summary> Provides a "Random" value, roughly normally distributed in the range [-1, 1) </summary>
		/// <remarks> Implemented by 3 samples of value, divided by 3. </remarks>
		public static float normal { get { return instance.normal; } }

		/// <summary> Returns a "Random" Int32 from [0, int.MaxValue) </summary>
		public static int rawInt { get { return instance.rawInt; } }
		/// <summary> Returns a "Random" byte value from [0, 256) </summary>
		public static byte rawByte { get { return instance.rawByte; } }

		/// <summary> Provides a "Random" character from 'a' to 'z' </summary>
		public static char alpha { get { return instance.alpha; } }
		/// <summary> Provides a "Random" character from 'A' to 'A' </summary>
		public static char ALPHA { get { return instance.ALPHA; } }
		/// <summary> Provides a "Random" character from '0' to '9' </summary>
		public static char numeric { get { return instance.numeric; } }

		/// <summary> Gets a "Random" point inside a cube centered at (0, 0, 0) with sides (1, 1, 1) </summary>
		public static Vector3 insideUnitCube { get { return instance.insideUnitCube; } }
		/// <summary> Gets a "Random" point inside a square centered at (0, 0) with sides (1, 1) </summary>
		public static Vector2 insideUnitSquare { get { return instance.insideUnitSquare; } }

		/// <summary> Gets a "Random" point inside a sphere centered at (0, 0, 0) with radius 1 </summary>
		public static Vector3 insideUnitSphere { get { return instance.insideUnitSphere; } }
		/// <summary> Gets a "Random" point inside a circle centered at (0, 0) with radius 1 </summary>
		public static Vector2 insideUnitCircle { get { return insideUnitCircle; } }

		/// <summary> Gets a "Random" point on a sphere centered at (0, 0, 0) with radius 1 </summary>
		public static Vector3 onUnitSphere { get { return instance.onUnitSphere; } }
		/// <summary> Gets a "Random" point on a circle centered at (0, 0) with radius 1 </summary>
		public static Vector3 onUnitCircle { get { return instance.onUnitCircle; } }

		/// <summary> Gets a "Random" color in the RGB channels, with an alpha of 1 </summary>
		public static Color rgb { get { return instance.rgb; } }
		/// <summary> Gets a "Random" color in the RGBA channels </summary>
		public static Color rgba { get { return instance.rgba; } }

		/// <summary> Gets a "Random" Color32 in the RGB channels, with an alpha of 255. </summary>
		public static Color32 RGB { get { return instance.RGB; } }
		/// <summary> Gets a "Random" Color32 in the RGBA channels </summary>
		public static Color32 RGBA { get { return instance.RGBA; } }

		/// <summary> Returns a rotation made by rotating from (-180, 180) on all of X/Y/Z axis </summary>
		public static Quaternion rotation { get { return instance.rotation; } }
		#endregion

		#region Basic Functions
		/// <summary> Returns an Int32 in the range [<paramref name="min"/>, <paramref name="max"/>) </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value inside range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static int Range(int min, int max) { return instance.Range(min, max); }
		/// <summary> Returns a Float32 in the range [<paramref name="min"/>, <paramref name="max"/>) </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value inside range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static float Range(float min, float max) { return instance.Range(min, max); }
		/// <summary> Returns a value centered at <paramref name="avg"/> with maximum deviation of +/-<paramref name="dev"/>. </summary>
		/// <param name="avg"> Average value </param>
		/// <param name="dev"> Maximum deviation </param>
		/// <returns> Value in range [<paramref name="avg"/>-<paramref name="dev"/>, <paramref name="avg"/>+<paramref name="dev"/>) </returns>
		public static float Normal(float avg, float dev) { return instance.Normal(avg, dev); }
		#endregion

		#region Choosing
		/// <summary> Chooses an index from a list of weights </summary>
		/// <param name="weights"> Weights to the given choice </param>
		/// <returns> Random index from range [0, <paramref name="weights"/>.Count) </returns>
		public static int WeightedChoose(IList<float> weights) { return instance.WeightedChoose(weights); }
		#endregion

	}

}
