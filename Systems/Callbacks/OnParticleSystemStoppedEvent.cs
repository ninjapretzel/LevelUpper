using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class OnParticleSystemStoppedEvent : MonoBehaviour {
	public UnityEvent action;
	public void OnParticleSystemStopped() {
		action.Invoke();
	}
}
