using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
/// Not actually that cool.
/// Represents a cooldown-timer.
/// Useful for sprint-bars, or some sort of resource that can't be used for a while once used up.
/// </summary>
[Serializable]
public struct CoolTimer {
	
	/// <summary> Maximum units stored. </summary>
	public float max;
	/// <summary> Current units stored. </summary>
	public float cur;
	/// <summary> Percentage that needs to be passed to become usable again. </summary>
	public float coolPoint;

	/// <summary> Current percentage of units available. Range [0, 1] </summary> 
	public float percentage { get { return cur/max; } } 

	/// <summary> Is the item unusable? </summary>
	public bool overheated;
	/// <summary> Is the item usable? </summary>
	public bool ready { get { return !overheated; } }
	
	/// <summary> Creates a new, usable CoolTimer with the given maximum resource. </summary>
	/// <param name="cap"> Value to use as the cap. <see cref="cur"/> and <see cref="max"/> both initialized to this value. </param>
	/// <param name="coolPoint"> Coolpoint to use. Defaults to 1f. </param>
	public CoolTimer(float cap, float coolPoint = 1f) {
		max = cap;
		cur = cap;
		this.coolPoint = coolPoint;
		overheated = false;
	}
	
	/// <summary> 
	/// Cools or heats this timer based on <paramref name="delta"/>. Positive cools, negative heats. 
	/// Automatically cools if overheated.
	/// </summary>
	/// <param name="delta"> Value to cool (positive) or heat (negative) by. </param>
	/// <returns> state of <see cref="overheated"/> after applying cooling or heating. </returns>
	public bool Update(float delta) {
		if (delta > 0 || overheated) {
			return CoolUpdate(Mathf.Abs(delta));
		} else {
			return HotUpdate(Mathf.Abs(delta));
		}
	}

	/// <summary> Updates this timer, cooling it by <paramref name="delta"/>. </summary>
	/// <param name="delta"> Units to restore. </param>
	/// <returns> state of <see cref="overheated"/> </returns>
	public bool CoolUpdate(float delta) {
		cur += delta;
		if (cur > max) { cur = max; }
		
		if (overheated && percentage >= coolPoint) { 
			overheated = false; 
		}
		
		return overheated;
	}

	/// <summary> Updates this timer, heating it by <paramref name="delta"/>. </summary>
	/// <param name="delta"> Units to drain. </param>
	/// <returns> state of <see cref="overheated"/> </returns>
	public bool HotUpdate(float delta) {
		cur -= delta;
		if (cur <= 0) { 
			cur = 0;
			overheated = true; 
		}

		return overheated;
	}


	/// <summary> Regardless of the state, overheat the timer. </summary>
	public void Drain() {
		cur = 0;
		overheated = true;
	}

}
