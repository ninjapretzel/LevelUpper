using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> Commons Behaviour for 'bilboarding' an entire transform. </summary>
public class Billboard : MonoBehaviour {

	/// <summary> Set this to override what camera is going to be looked at. </summary>
	public Camera targetOverride = null;

	/// <summary> Set this to use the camera's up vector as the local up vector. </summary>
	public bool useCameraUp = false;

	/// <summary> Does this billboard match camera rotation instead of looking at the camera? </summary>
	public bool matchRotationMode = false;

	/// <summary> Flip the object 180 degrees on the Y axis after billboarding. </summary>
	public bool flip = false;

	/// <summary> Speed of rotation along the z axis. </summary>
	public float zRotationSpeed = 0;

	/// <summary> Current rotation on the z-axis.</summary>
	float zRotation = 0;

	public bool doLate = false;

	void Update() {
		if (!doLate) { DoBillboard(); }
	}

	void LateUpdate() {
		if (doLate) { DoBillboard(); }
	}

	private void DoBillboard() {
		Camera target = targetOverride;
		if (target == null) { target = Camera.main; }

		if (matchRotationMode) {
			transform.rotation = target.transform.rotation;
		} else {
			if (useCameraUp) {
				transform.LookAt(target.transform, target.transform.up);
			} else {
				transform.LookAt(target.transform);
			}
		}

		bool actuallyFlip = matchRotationMode ? !flip : flip;
		if (actuallyFlip) { transform.Rotate(0, 180, 0); }
		zRotation += zRotationSpeed * Time.deltaTime;
		transform.Rotate(0, 0, zRotation);
	}
}
