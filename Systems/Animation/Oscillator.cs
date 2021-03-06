using UnityEngine;
using System.Collections;

namespace LevelUpper.Animation {


	/// <summary> Oscilates a value over time. </summary>
	[System.Serializable]
	public class Oscillator {
		/// <summary> Modes of Oscillation for the Oscillator class.</summary>
		public enum Mode { Cos, Sin, Line, Square }
		
		/// <summary> Mode of Oscillation between minVal and maxVal </summary>
		[Tooltip("Mode of Oscillation between minVal and maxVal")]
		public Mode mode = Mode.Cos;

		[Tooltip("Time of oscillation")]
		/// <summary> Time of oscillation </summary>
		public float maxTime = 1.0f;

		/// <summary> Current time in oscillation</summary>
		[System.NonSerialized] public float curTime;

		/// <summary> Value at curTime == 0 </summary>
		[Tooltip("Value at curTime == 0")]
		public float minVal = 0.0f;
		/// <summary> Value at curTime == maxTime </summary>
		[Tooltip("Value at curTime == maxTime")]
		public float maxVal = 1.0f;

		/// <summary> Current value of the oscillator </summary>
		public float value { get; private set; }
		/// <summary> Direction (up/down) </summary>
		public bool up { get; private set; }
		
		public Oscillator() {
			minVal = 0.0f;
			maxVal = 1.0f;
			maxTime = 1.0f;
		}
		
		public Oscillator(float min, float max, float time) {
			minVal = min;
			maxVal = max;
			maxTime = time;
		}
		
		public Oscillator(float min, float max, float time, float start) {
			minVal = min;
			maxVal = max;
			maxTime = time;
			curTime = start;
		}

		/// <summary> Set the current position of this oscillator </summary>
		/// <param name="time">Time to set to. Will be clamped to [0...1] and scaled</param>
		/// <param name="up">Is the oscillator rising (true) or falling (false)?</param>
		public void SetTime(float time, bool up = false) {
			curTime = time;
			this.up = up;
		}
		
		/// <summary> Update the oscillator and get the next value. </summary>
		public float Update() {
			if (up) {
				curTime += Time.deltaTime;
				if (curTime > maxTime) {
					up = !up;
					curTime = maxTime * 2 - curTime;
				}
			} else {
				curTime -= Time.deltaTime;
				if (curTime < 0) {
					up = !up;
					curTime = -curTime;
				}
			}
			
			float p = curTime / maxTime;
			if (mode == Mode.Square) {
				p *= p;
			} else if (mode == Mode.Sin) {
				p = Mathf.Sin(p * Mathf.PI);
			} else if (mode == Mode.Cos) {
				p = (1.0f + Mathf.Cos(p * Mathf.PI)) / 2.0f;
			}
			
			value = (minVal + p * (maxVal - minVal));
			return value;
		}

		/// <summary> Update the oscillator with a custom deltaTime and get the next value. </summary>
		public float Update(float deltaTime) {
			if (up) {
				curTime += deltaTime;
				if (curTime > maxTime) {
					up = !up;
					curTime = maxTime * 2 - curTime;
				}
			} else {
				curTime -= deltaTime;
				if (curTime < 0) {
					up = !up;
					curTime = -curTime;
				}
			}

			float p = curTime / maxTime;
			if (mode == Mode.Square) {
				p *= p;
			} else if (mode == Mode.Sin) {
				p = Mathf.Sin(p * Mathf.PI);
			} else if (mode == Mode.Cos) {
				p = (1.0f + Mathf.Cos(p * Mathf.PI)) / 2.0f;
			}

			value = (minVal + p * (maxVal - minVal));
			return value;
		}
	}

}
