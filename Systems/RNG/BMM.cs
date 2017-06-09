using UnityEngine;
using System.Collections;

namespace LevelUpper.RNG {

	///<summary> Stands for 'bool min max', stores a bool and a range. Used for randomizations. </summary>
	[System.Serializable]
	public class BMM {

		static PerlinNoise noise;
		public static bool init = Init();
		public static bool Init() {
			noise = new PerlinNoise();
			noise.freqBase = 31.337f;
			noise.octaves = 1;
			noise.persistance = 1f;
			return true;
		}

		///<summary> Randomize this field? </summary>
		public bool randomize { get { return flag; } }
		/// <summary> More generic name, since this class isn't just for randomization </summary>
		public bool flag = false;
		///<summary> Lowest possible value. </summary>
		public float min = 0.9f;
		///<summary> Largest possible value. </summary>
		public float max = 1.1f;

		///<summary> Get the (next) value, evenly distributed </summary>
		public float value {
			get {
				if (!flag) { return 1.0f; }
				return Random.Range(min, max);
			}
		}

		///<summary> Get the (next) value, normally distributed </summary>
		public float normal {
			get {
				if (!flag) { return 1.0f; }
				return Random.Normal(min, max);
			}
		}

		///<summary> Get the (next) value, distributed with perlin noise </summary>
		public float Perlin(float x, float y) {
			if (!flag) { return 1.0f; }
			float f = noise.Get(x, y);
			return min + (max-min) * f; 
		}

		///<summary> Default constructor </summary>
		public BMM() {
			flag = false;
			min = 0.9f;
			max = 1.1f;
		}

		/// <summary> Two parameter constructor</summary>
		public BMM(float mmin, float mmax) {
			flag = true;
			min = mmin;
			max = mmax;
		}

		///<summary> Three parametered constructor </summary>
		public BMM(bool bb, float mmin, float mmax) {
			flag = bb;
			min = mmin;
			max = mmax;
		}
		
	}
}
