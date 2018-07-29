using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LevelUpper.Extensions;

/// <summary> Generic animator interface for a given type. </summary>
/// <typeparam name="T">Type that is animated. </typeparam>
public interface ISimpleAnim<T> {
	/// <summary> Current value of animation </summary>
	T value { get; set; }
	/// <summary> Implementation should read the current <see cref="value"/> and produce the next value. </summary>
	void Update();
}

public static class SimpleAnimHelpers {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RefAnim<T>(this ISimpleAnim<T> animator, ref T value) {
		animator.value = value;
		animator.Update();
		value = animator.value;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Animate<T>(this ISimpleAnim<T> animator, T value) {
		animator.value = value;
		animator.Update();
		return animator.value;
	}
}

[Serializable]
public class Spring : ISimpleAnim<float> {
	
	public float value { get; set; }
	public float target;
	public float velocity;
	public float strength = 100;
	public float dampening = 1;
	
	public void Update() {
		velocity += (target - value) * strength * Time.deltaTime;
		velocity *= Mathf.Pow(dampening * .0001f, Time.deltaTime);
		value += velocity * Time.deltaTime;
	}
	
}

[Serializable]
public class SpringV3 : ISimpleAnim<Vector3> {

	public Vector3 value { get; set; }
	public Vector3 target;
	public Vector3 velocity;
	public float strength = 100;
	public float dampening = 1;

	public void Update() {
		velocity += (target - value) * strength * Time.deltaTime;
		velocity *= Mathf.Pow(dampening * .0001f, Time.deltaTime);
		value += velocity * Time.deltaTime;
	}

}

[Serializable]
public class SpringQ : ISimpleAnim<Quaternion> {

	public Quaternion value { get; set; }
	public Quaternion target = Quaternion.identity;
	public Vector3 angularVelocity;

	public Vector3 velAxis;
	public float velAngle;

	public float strength = 100;
	public float dampening = 1;
	public float axisDampening = 5;

	public void Update() {
		float angle;
		Vector3 axis;
		if (velAxis.x.IsNaN()) {
			velAxis = Vector3.zero;
			velAngle = 0;
			Debug.Log("vel axis triggered NAN check");
		}
		Quaternion inv = target * Quaternion.Inverse(value);
		inv.ToAngleAxis(out angle, out axis);
		//(target * Quaternion.Inverse(value)).ToAngleAxis(out angle, out axis);
		if (!axis.x.IsNAN()) {

			if (angle > 180) { angle -= 360; }
			if (angle < -180) { angle += 360; }
			velAxis = Vector3.Lerp(velAxis, axis, Time.deltaTime * axisDampening);
		} else {
			Debug.Log("new axis triggered NAN check");

		}
		velAngle += angle * strength * Time.deltaTime;
		velAngle *= Mathf.Pow(dampening * .0001f, Time.deltaTime);
		if (velAngle > 180) { velAngle = 180; }
		if (velAngle < -180) { velAngle = -180; }

		value *= Quaternion.AngleAxis(velAngle * Time.deltaTime, velAxis);
	}

}

