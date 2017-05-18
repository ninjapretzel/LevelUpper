#if UNITY_6 || UNITY_5 || UNITY_4
#define UNITY
using UnityEngine;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LevelUpper.RNG {

	/// <summary> Crude, simple RNG </summary>
	/// <remarks> 
	///		<para> Provides both static and instance methods for random number generation. </para>
	///		<para> Static methods can be used to generate random numbers from given information. </para>
	///		<para> An instance of this class represents a stateful sequence of numbers. </para>
	///		<para> Instances can be constructed with a given seed, so they generate the same sequence of numbers. </para>
	///		<para> Otherwise, if not given a seed, they will use the current <see cref="System.DateTime.Now.Ticks"/> as the seed. </para>
	/// </remarks>
	public class SRNG {

		/// <summary> BAAAARF </summary>
		private const long BARF = 0xFFFF8000;
		/// <summary> BRRAAAAAP </summary>
		private const long WETFART = 0x1F2FF3F4;//0x8BADF00D;
												/// <summary> PFFFFFFFT </summary>
		private const long DRYFART = 0x83828180;//0x501D1F1D;

		/// <summary> Hashes a <see cref="System.DateTime"/> by itsd Ticks </summary>
		/// <param name="seed"> <see cref="System.DateTime"/> to hash </param>
		/// <returns> Hash of <paramref name="seed"/> seed's Ticks </returns>
		public static long hash(DateTime seed) { return hash(seed.Ticks); }

		/// <summary> Gets the hash of a given <paramref name="seed"/>. </summary>
		/// <param name="seed"> Value to use as seed </param>
		/// <returns> Mostly randomly distributed value based on the input hash </returns>
		public static long hash(long seed) {
			long a1 = seed << 32;
			long s0 = seed ^ a1;

			long left = (int)s0;
			long right = (int)(s0 >> 32);
			long join = (left << 32) | right;

			s0 = join ^ (s0 << 1);
			long s1 = BARF ^ (s0 >> 1);

			bool wet = ((byte)s0) % 2 == 0;
			long fart = wet ? WETFART : DRYFART;
			fart = s1 ^ fart;

			return fart;
		}

		/// <summary> Generate a seeded int32 value in range [0, <paramref name="max"/>) using the given <paramref name="seed"/>'s Ticks </summary>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <param name="seed"> Seed value </param>
		/// <returns> 'Random' number in range [0, <paramref name="max"/>) </returns>
		public static int Int32(int max, DateTime seed) { return Int32(max, seed.Ticks); }
		/// <summary> Generate a seeded int32 value in range [0, <paramref name="max"/>) using the given <paramref name="seed"/></summary>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <param name="seed"> Seed value </param>
		/// <returns> 'Random' number in range [0, <paramref name="max"/>) </returns>
		public static int Int32(int max, long seed) {
			int i = (int)(hash(seed));
			return (i < 0 ? -i : i) % max;
		}

		/// <summary> Generate a seeded int32 value in range [<paramref name="min"/>, <paramref name="max"/>) using the given <paramref name="seed"/>'s Ticks </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <param name="seed"> Seed value </param>
		/// <returns> 'Random' number in range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static int Int32Range(int min, int max, DateTime seed) { return min + Int32(max - min, seed.Ticks); }

		/// <summary> Generate a seeded int32 value in range [<paramref name="min"/>, <paramref name="max"/>) using the given <paramref name="seed"/>'s Ticks </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <param name="seed"> Seed value </param>
		/// <returns> 'Random' number in range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static int Int32Range(int min, int max, long seed) { return min + Int32(max - min, seed); }

		/// <summary> Generate a floating point number in the range [0, 1) </summary>
		/// <param name="seed"> Seed value to use to generate the floating point value </param>
		/// <returns> float32 number between [0, 1) </returns>
		public static float Float32(long seed) {
			long s = hash(seed);
			int i = (int)(s % (int.MaxValue / 2)); // Leads to more even distribution
			if (i < 0) { i = -i; }
			float f = ((float)i / (int.MaxValue / 2)); // Again, maxvalue/2 leads to more even distribution
			return f;
		}

		/// <summary> Generate a floating point number in the range [0, 1) </summary>
		/// <param name="seed"> DateTime to use as a seed</param>
		/// <returns> Random in range [0, 1)</returns>
		public static float Float32(DateTime seed) { return Float32(seed.Ticks); }

		/// <summary> Generate a floating point number in the range [0, <paramref name="max"/>) </summary>
		/// <param name="max"> Maximum value of range </param>
		/// <param name="seed"> Seed value to use to generate the floating point value </param>
		/// <returns> float32 number between [0, 1) </returns>
		public static float Float32(float max, long seed) { return Float32(seed) * max; }

		/// <summary> Generate a floating point number in the range [<paramref name="min"/>, <paramref name="max"/>) using the given <paramref name="seed"/> </summary>
		/// <param name="min"> Lower linit, inclusive </param>
		/// <param name="max"> Upper limit, not inclusive </param>
		/// <param name="seed"> Seed value </param>
		/// <returns> 'Random' number in range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public static float Float32Range(float min, float max, long seed) { return min + Float32(max - min, seed); }

		/// <summary> Current seed value of an RNG instance </summary>
		/// <value> Current seed. Cannot be changed externally. </value>
		public long seed { get; private set; }

		/// <summary> Basic constructor, uses the current time (DateTime.Now.Ticks) to seed its initial position. </summary>
		public SRNG() { seed = DateTime.Now.Ticks; }
		/// <summary> Seeded constructor. Uses the given <paramref name="seed"/> as the starting point in the sequence. </summary>
		/// <param name="seed"> Value of the starting point of this sequence </param>
		public SRNG(long seed) { this.seed = seed; }

		/// <summary> Gets the next Int32 value from this RNG's sequence. </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> Value in range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public int NextInt(int min, int max) { return Int32Range(min, max, nextHash()); }
		/// <summary> Gets the next Int32 value from this RNG's sequence. </summary>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> Value in range [0, <paramref name="max"/>) </returns>
		public int NextInt(int max) { return Int32(max, nextHash()); }

		/// <summary> Gets the next Float32 value from this RNG's sequence. Value is from [0, 1) </summary>
		/// <returns> Value in range [0, 1) </returns>
		public float NextFloat() { return Float32(nextHash()); }
		/// <summary> Gets the next Float32 value from this RNG's sequence in a given range. </summary>
		/// <param name="min"> Lower limit, inclusive. </param>
		/// <param name="max"> Upper limit, not inclusive. </param>
		/// <returns> "Random" value in range [<paramref name="min"/>, <paramref name="max"/>) </returns>
		public float NextFloat(float min, float max) { return Float32Range(min, max, nextHash()); }

		/// <summary> Forwards the RNG to its next value, and returns the new seed value. </summary>
		/// <returns> Newly generated seed value. </returns>
		public long nextHash() { return seed = hash(seed); }

	}



}
