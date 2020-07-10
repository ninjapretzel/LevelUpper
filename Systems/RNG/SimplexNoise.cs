using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using static UnityEngine.Mathf;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace LevelUpper.RNG {
	/// <summary> Modified C# port of https://github.com/SRombauts/SimplexNoise/blob/master/src/SimplexNoise.cpp for unity </summary>
	[System.Serializable]
	public struct SimplexNoise {
		/// <summary> 
		/// Default permutation.
		/// Just a random list of ints.
		/// Could use a different one for slightly different output.
		/// </summary>
		public static readonly int[] stdPerm = {
			151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
			8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,
			35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,
			134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,
			55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208, 89,
			18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,
			250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,
			189,28,42,223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,
			172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,
			228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,
			107,49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
		};
		
		/// <summary> Creates a new permutation that could be used with stdPerm </summary>
		/// <param name="max"></param>
		/// <param name="length"></param>
		/// <param name="rng"></param>
		/// <returns></returns>
		private static List<int> Permutation(int max, int length = -1, SRNG rng = null) {
			if (length < 0) { length = max; }
			if (rng == null) { rng = new SRNG(); }

			List<int> nums = new List<int>(max);
			for (int i = 0; i < max; i++) { nums.Add(i); }

			List<int> chosen = new List<int>(length);
			for (int i = 0; i < length; i++) {
				int j = rng.NextInt(0, nums.Count);
				chosen.Add(nums[j]);
				nums.RemoveAt(j);
			}

			return chosen;
		}

		/// <summary> Creates a new permutation that could be used with stdPerm </summary>
		/// <param name="seed"> Seed for the permutation </param>
		/// <returns> int[256] with [0,255] shuffled based on the given seed. </returns>
		public static int[] NewPermutation(long seed) {
			List<int> nums = Permutation(256, 256, new SRNG(seed));
			return nums.ToArray();
		}

		/// <summary> Simplex byte->byte Hash function, indexes permutation array with just lowest byte </summary>
		/// <returns> hash value </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int hash(int i) {
			return stdPerm[i & 255];
		}

		/// <summary> Returns the fractional portion of the number </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		/// 
		/// <summary> Fast floor </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int FastFloor(float f) { return f > 0 ? (int)f : (int)f - 1; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure]
		public float Frac(float n) {
			return n - FastFloor(n);
		}

		/// <summary> Seed for non-simplex primitve hash </summary>
		const float DEFAULT_SEED = 31337.1337f;

		/// <summary> Simple float->float hash function, for non-simplex discontinuous primitive use </summary>
		/// <param name="n"> Value to hash </param>
		/// <param name="seed"> Seed to hash with </param>
		/// <returns> Hashed value for n </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure]
		public float Hash(float n, float seed = DEFAULT_SEED) { return Frac(Abs(Sin(n) * seed)); }

		/// <summary> Simple 3d hash function, for non-simplex discontinuous primitive use </summary>
		/// <param name="x"> x to hash </param>
		/// <param name="y"> y to hash </param>
		/// <param name="z"> z to hash </param>
		/// <returns> Hash for coordinate </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure]
		public float Hash3(float x, float y, float z, float seed = DEFAULT_SEED) { return Hash(x + y * 157.0f + 113.0f * z, seed); }
		/// <summary> Simple 3d hash function, for non-simplex discontinuous primitive use </summary>
		/// <param name="p"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure]
		public float Hash3(Vector3 p, float seed = DEFAULT_SEED) { return Hash(p.x + p.y * 157.0f + 113.0f * p.z, seed); }

		/// <summary> 1d gradient </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float Grad(int hash, float x) {
			int h = hash & 0x0f; // 4 bits -> (-8...8) (16 values)
			float grad = 1.0f + (h & 0x07);
			if ((h & 0x08) != 0) { grad = -grad; }
			return grad * x;
		}

		/// <summary> 2d gradient </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float Grad(int hash, float x, float y) {
			int h = hash & 0x07; // 3 bits -> 8 directions
			float u = (h < 4) ? x : y;
			float v = (h < 4) ? y : x;
			return (((h & 0x01) != 0) ? -u : u) // dot with (x,y)
				+ (((h & 0x02) != 0) ? -2.0f : 2.0f) * v;
		}

		/// <summary> 3d gradient </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float Grad(int hash, float x, float y, float z) {
			int h = hash & 0x0F; // 4 bits -> 12 directions, repeat thru (12-15)
			float u = h < 8 ? x : y;
			float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
			return (((h & 0x01) != 0) ? -u : u) // )
				+ (((h & 0x02) != 0) ? -v : v);
		}


		/// <summary> Number of octaves to apply </summary>
		public int octaves;
		/// <summary> Persistance of 'deep' octaves </summary>
		public float persistence;
		/// <summary> Basic noise scale </summary>
		public float scale;
		/// <summary> Scale multiplier per octave </summary>
		public float octaveScale;
		/// <summary> Offset to apply to noises </summary>
		public Vector3 noiseOffset;
		/* Removed due to problems with piping arrays to shaders.
		/// <summary> Instanced permutation. Defaults to <see cref="stdPerm"/></summary>
		private int[] perm;
		/// <summary> Used to hide perm from inspector .</summary>
		public int[] perms { get { return perm; } set { perm = value; } }
		//*/

		/// <summary> Default SimplexNoise. Typically, use this instead of an empty constructor </summary>
		public static readonly SimplexNoise Defaults = new SimplexNoise() {
			octaves = 4,
			persistence = .5f,
			scale = 1f,
			octaveScale = 2f,
			noiseOffset = Vector3.zero,
			// perm = stdPerm,
		};

		/// <summary> Unpack a Vector4 (octaves, persistance, scale, octaveScale)</summary>
		public SimplexNoise(Vector4 data) {
			octaves = (int)Mathf.Min(1, data.x);
			persistence = Mathf.Clamp01(data.y);
			scale = data.z;
			octaveScale = data.w;
			// perm = stdPerm;
			noiseOffset = Vector3.zero;
		}

		/// <summary> Explicit Copy Constructor </summary>
		public SimplexNoise(SimplexNoise other) {
			octaves = other.octaves;
			persistence = other.persistence;
			scale = other.scale;
			octaveScale = other.octaveScale;
			noiseOffset = other.noiseOffset;
			// perm = other.perm;
		}

		const int VORONI_FROM = -1;
		const int VORONI_TO = 2;
		public static readonly Vector4 DEFAULT_COMP = new Vector4(-1, 1, 0, 1);

		/// <summary> 3d Voroni noise </summary>
		/// <param name="p"> Input coordinates </param>
		/// <param name="COMP"> Composition (x, y, z) * w </param>
		/// <returns> Value of noise for input coordinate </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Voroni3D(Vector3 p, Vector4? COMP = null) {
			return Voroni3D(p.x, p.y, p.z, COMP);
		}
		/// <summary> 3d Voroni noise </summary>
		/// <param name="x"> x coord </param>
		/// <param name="y"> y coord </param>
		/// <param name="z"> z coord </param>
		/// <param name="SHIFT"> Feature grid shift direction </param>
		/// <param name="COMP"> Voroni Composition (x y z) * w </param>
		/// <returns> Value of simplex voroni </returns>
		public float Voroni3D(float x, float y, float z, Vector4? COMP = null) {
			x += noiseOffset.x;
			y += noiseOffset.y;
			z += noiseOffset.z;
			Vector4 comp = COMP.HasValue ? COMP.Value : DEFAULT_COMP;
			
			float px = Floor(x);
			float py = Floor(y);
			float pz = Floor(z);
			Vector3 p = new Vector3(px, py, pz);
			
			float fx = Abs(x - px);
			float fy = Abs(y - py);
			float fz = Abs(z - pz);
			Vector3 f = new Vector3(fx, fy, fz);

			// Tracking 3 closest point distances.
			Vector3 closest = Vector3.one;

			//Loop over 4x4x4 local neighborhood
			for (int k = VORONI_FROM; k <= VORONI_TO; k++) {
				for (int j = VORONI_FROM; j <= VORONI_TO; j++) {
					for (int i = VORONI_FROM; i <= VORONI_TO; i++) {

						Vector3 sampleOffset = new Vector3(i, j, k);
						Vector3 feature = p + sampleOffset;
						float offsetValue = 2.0f * (RawNoise3D(feature) - .5f);

						float sx = Hash(feature.x);
						float sy = Hash(feature.y); 
						float sz = Hash(feature.z); 
						float shiftValue = 1.0f - (offsetValue * offsetValue);
						Vector3 shift = new Vector3(sx, sy, sz).normalized * offsetValue;
						
						Vector3 pointToFeature = sampleOffset - f + shift;
						float dist = 0;

						// DIFFMODE: MANHATTAN
						// featurePoint = new Vector3(Abs(featurePoint.x), Abs(featurePoint.y), Abs(featurePoint.z));
						// dist = Max(Max(featurePoint.x, featurePoint.y), featurePoint.z);
						
						// DIFFMODE: EUCLID
						dist = pointToFeature.magnitude;
						
						// track the closest 3 point distances
						if (dist < closest[0]) { closest[2] = closest[1]; closest[1] = closest[0]; closest[0] = dist; } 
						else if (dist < closest[1]) { closest[2] = closest[1]; closest[1] = dist; } 
						else if (dist < closest[2]) { closest[2] = dist; }

					}
				}
			}
			
			return comp.w * Abs(comp.x * closest.x + comp.y * closest.y + comp.z * closest.z);
		}
		
		private static readonly Vector3 DEFAULT_PHASEX = new Vector3(5.1f, 1.3f, 2.4f);
		private static readonly Vector3 DEFAULT_PHASEY = new Vector3(1.7f, 9.2f, 3.5f);
		private static readonly Vector3 DEFAULT_PHASEZ = new Vector3(2.3f, 1.2f, 1.9f);

		/// <summary> Domain warping function. Outputs a position near pos  </summary>
		/// <param name="pos"> Position to warp </param>
		/// <returns> Warped version of pos </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 Warp(Vector3 pos) {
			return Warp(pos, 4, DEFAULT_PHASEX, DEFAULT_PHASEY, DEFAULT_PHASEZ);
		}
		/// <summary> Domain warp function. Outputs a position near pos. </summary>
		/// <param name="pos"> Position to warp </param>
		/// <param name="rate"> Amount to warp by </param>
		/// <param name="phaseX"> X direction phase offset </param>
		/// <param name="phaseY"> Y direction phase offset </param>
		/// <param name="phaseZ"> Z direction phase offset </param>
		/// <returns> Warped version of pos </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 Warp(Vector3 pos, float rate, Vector3 phaseX, Vector3 phaseY, Vector3 phaseZ) {
			Vector3 q = new Vector3(FBM(pos + phaseX), FBM(pos + phaseY), FBM(pos + phaseZ));
			return pos + rate * q;
		}

		/// <summary> Render 1d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-<paramref name="min"/>, <paramref name="max"/>] </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float FBM1(float x, float min = 0, float max = 1) {
			float val = Fractal(x);
			return min + ((val + 1f) / 2f) * (max - min);
		}

		/// <summary> Render 2d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-<paramref name="min"/>, <paramref name="max"/>] </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float FBM2(float x, float y, float min = 0, float max = 1) {
			float val = Fractal(x, y);
			return min + ((val + 1f) / 2f) * (max - min);
		}

		/// <summary> Render 3d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-<paramref name="min"/>, <paramref name="max"/>] </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float FBM3(float x, float y, float z, float min = 0, float max = 1) {
			float val = Fractal(x, y, z);
			return min + ((val + 1f) / 2f) * (max - min);
		}

		/// <summary> Render 2d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-<paramref name="min"/>, <paramref name="max"/>] </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float FBM(Vector3 pos, float min = 0, float max = 1) {
			return FBM3(pos.x, pos.y, pos.z, min, max);
		}

		/// <summary> Render 2d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-<paramref name="min"/>, <paramref name="max"/>] </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float FBM(Vector2 pos, float min = 0, float max = 1) {
			return FBM2(pos.x, pos.y, min, max);
		}

		/// <summary> Render 2d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-1, 1] </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Fractal(Vector2 pos) { return Fractal(pos.x, pos.y); }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		/// <summary> Render 3d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-1, 1] </returns>
		public float Fractal(Vector3 pos) { return Fractal(pos.x, pos.y, pos.z); }

		/// <summary> Render 1d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-1, 1] </returns>
		public float Fractal(float x) {
			float total = 0;
			float max = 0;
			float frequency = scale;
			float amplitude = 1;

			for (int i = 0; i < octaves; i++) {
				total += amplitude * RawNoise1D(x * frequency);
				max += amplitude;

				frequency *= octaveScale;
				amplitude *= persistence;
			}
			return total / max;
		}
		/// <summary> Render 2d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-1, 1] </returns>
		public float Fractal(float x, float y) {
			float total = 0;
			float max = 0;
			float frequency = scale;
			float amplitude = 1;

			for (int i = 0; i < octaves; i++) {
				total += amplitude * RawNoise2D(x * frequency, y * frequency);
				max += amplitude;

				frequency *= octaveScale;
				amplitude *= persistence;
			}

			return total / max;
		}

		/// <summary> Render 3d fractal noise for the given coordinates using this simplex noise's settings. </summary>
		/// <returns> Result noise value in range [-1, 1] </returns>
		public float Fractal(float x, float y, float z) {
			float total = 0;
			float max = 0;
			float frequency = scale;
			float amplitude = 1;

			for (int i = 0; i < octaves; i++) {
				total += amplitude * RawNoise3D(x * frequency, y * frequency, z * frequency);
				max += amplitude;

				frequency *= octaveScale;
				amplitude *= persistence;
			}

			return total / max;
		}
		
		/// <summary> 1D Simplex Noise </summary>
		/// <returns> Value between [-1,1] for input coordinate </returns>
		public float RawNoise1D(float x) {
			// apply noise offset
			x += noiseOffset.x;
			// 'corner' positions
			int i0 = FastFloor(x);
			int i1 = i0 + 1;
			// distances to 'corners'
			float x0 = x - i0;
			float x1 = x0 - 1.0f;

			// Contribution of corners 
			float t0 = 1.0f - x0 * x0;
			t0 *= t0;
			float n0 = t0 * t0 * Grad(hash(i0), x0);

			float t1 = 1.0f - x1 * x1;
			t1 *= t1;
			float n1 = t1 * t1 * Grad(hash(i1), x1);

			// Scale result to fit into [-1,1]
			return .0395f * (n0 + n1);
		}
		
		/// <summary> Constant used with 2d Skewing, sqrt(3) </summary>
		const float SQRT3 = 1.73205080757f;
		/// <summary> Constant for 2d noise </summary>
		const float F2 = .5f * (SQRT3 - 1.0f);
		/// <summary> Constant for 2d noise </summary>
		const float G2 = (3.0f - SQRT3) / 6.0f;
		/// <summary> 2D Simplex Noise </summary>
		/// <returns> Value between [-1,1] for input coordinates </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float RawNoise2D(Vector2 pos) { return RawNoise2D(pos.x, pos.y); }
		/// <summary> 2D Simplex Noise </summary>
		/// <returns> Value between [-1,1] for input coordinates </returns>
		public float RawNoise2D(float x, float y) {
			// Apply noise offset 
			x += noiseOffset.x;
			y += noiseOffset.y;

			//Noise contributions from the corners
			float n0, n1, n2;

			// skew input space to make math easier.
			float s = (x + y) * F2;
			int i = FastFloor(x + s);
			int j = FastFloor(y + s);

			float t = (i + j) * G2;
			//Unskew back into normal space
			float X0 = i - t;
			float Y0 = j - t;
			//The x,y distance from the cell's origin
			float x0 = x - X0;
			float y0 = y - Y0;

			// For the 2D case, the simplex shape is an equilateral triangle.
			// Determine which simplex we are in.
			int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
			if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
			else { i1 = 0; j1 = 1; } // upper triangle, YX order: (0,0)->(0,1)->(1,1)

			// A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
			// a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
			// c = (3-sqrt(3))/6
			float x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
			float y1 = y0 - j1 + G2;
			float x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
			float y2 = y0 - 1.0f + 2.0f * G2;

			// Work out the hashed gradient indices of the three simplex corners
			int gi0 = hash(i + hash(j));
			int gi1 = hash(i + i1 + hash(j + j1));
			int gi2 = hash(i + 1 + hash(j + 1));

			// Calculate the contribution from the three corners
			float t0 = 0.5f - x0 * x0 - y0 * y0;
			if (t0 < 0) { n0 = 0.0f; } else {
				t0 *= t0;
				n0 = t0 * t0 * Grad(gi0, x0, y0);
			}

			float t1 = 0.5f - x1 * x1 - y1 * y1;
			if (t1 < 0) { n1 = 0.0f; } else {
				t1 *= t1;
				n1 = t1 * t1 * Grad(gi1, x1, y1);
			}

			float t2 = 0.5f - x2 * x2 - y2 * y2;
			if (t2 < 0) { n2 = 0.0f; } else {
				t2 *= t2;
				n2 = t2 * t2 * Grad(gi2, x2, y2);
			}

			//Add contributions from each corner to get the final noise value.
			//The result is scaled to return values in the interval [-1,1].
			/// formerly 70.0f
			return 45.23065f * (n0 + n1 + n2);
		}

		/// <summary> Constant for 3D noise </summary>
		const float F3 = 1.0f / 3.0f;
		/// <summary> Constant for 3D noise </summary>
		const float G3 = 1.0f / 6.0f;

		/// <summary> 3D Simplex Noise </summary>
		/// <returns> Value between [-1,1] for input coordinates </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float RawNoise3D(Vector3 pos) { return RawNoise3D(pos.x, pos.y, pos.z); }
		/// <summary> 3D Simplex Noise </summary>
		/// <returns> Value between [-1,1] for input coordinates </returns>
		public float RawNoise3D(float x, float y, float z) {
			// Apply noise offset:
			x += noiseOffset.x;
			y += noiseOffset.y;
			z += noiseOffset.z;
			float n0, n1, n2, n3;

			// Skew/Unskew
			float s = (x + y + z) * F3;
			int i = FastFloor(x + s);
			int j = FastFloor(y + s);
			int k = FastFloor(z + s);
			float t = (i + j + k) * G3;

			float X0 = i - t;
			float Y0 = j - t;
			float Z0 = k - t;
			float x0 = x - X0;
			float y0 = y - Y0;
			float z0 = z - Z0;

			int i1, j1, k1, i2, j2, k2;
			if (x0 > y0) {
				if (y0 >= z0) {
					i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; // X Y Z
				} else if (x0 >= z0) {
					i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; // X Z Y
				} else {
					i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; // Z X Y
				}
			} else { // x0 < y0
				if (y0 < z0) {
					i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; // Z Y X
				} else if (x0 < z0) {
					i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; // Y Z X
				} else {
					i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; // Y X Z
				}
			}

			// Steps reuse G3 offset to remain at 'corners'
			float x1 = x0 - i1 + G3;
			float y1 = y0 - j1 + G3;
			float z1 = z0 - k1 + G3;
			float x2 = x0 - i2 + 2.0f * G3;
			float y2 = y0 - j2 + 2.0f * G3;
			float z2 = z0 - k2 + 2.0f * G3;
			float x3 = x0 - 1.0f + 3.0f * G3;
			float y3 = y0 - 1.0f + 3.0f * G3;
			float z3 = z0 - 1.0f + 3.0f * G3;

			// Get corner values
			int gi0 = hash(i + hash(j + hash(k)));
			int gi1 = hash(i + i1 + hash(j + j1 + hash(k + k1)));
			int gi2 = hash(i + i2 + hash(j + j2 + hash(k + k2)));
			int gi3 = hash(i + 1 + hash(j + 1 + hash(k + 1)));

			float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
			if (t0 < 0) { n0 = 0.0f; } else {
				t0 *= t0;
				n0 = t0 * t0 * Grad(gi0, x0, y0, z0);
			}

			float t1 = .6f - x1 * x1 - y1 * y1 - z1 * z1;
			if (t1 < 0) { n1 = 0.0f; } else {
				t1 *= t1;
				n1 = t1 * t1 * Grad(gi1, x1, y1, z1);
			}

			float t2 = .6f - x2 * x2 - y2 * y2 - z2 * z2;
			if (t2 < 0) { n2 = 0.0f; } else {
				t2 *= t2;
				n2 = t2 * t2 * Grad(gi2, x2, y2, z2);
			}

			float t3 = .6f - x3 * x3 - y3 * y3 - z3 * z3;
			if (t3 < 0) { n3 = 0.0f; } else {
				t3 *= t3;
				n3 = t3 * t3 * Grad(gi3, x3, y3, z3);
			}

			// Scale to stay in [-1,1]
			return 32.0f * (n0 + n1 + n2 + n3);
		}

	}
	
	public delegate float HeightFn(Vector3 position);
	public delegate Color SplatFn(Vector3 position);
}



