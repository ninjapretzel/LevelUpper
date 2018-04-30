using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.PlayModeStateChange;
#endif

/// <summary> Hack script to hook into some UnityEditor only stuff seamlessly from outside of the Unity Editor.</summary>
[ExecuteInEditMode]
public class EditorUtils : MonoBehaviour {

	/// <summary> Static reference to the live instance. </summary>
	public static EditorUtils main;
	/// <summary> Has this instance registered callbacks into unity's systems? </summary>
	private bool registered;
	
	/// <summary> Is the Editor currently in a 'Paused' State? (Pause button itself, false = 'normal') </summary>
	public static bool IsPaused { get; private set; } = false;
	/// <summary> Is the Editor currently in a 'Playing' State? (Play button itself, false = 'stopped') </summary>
	public static bool IsPlayMode { get; private set; } = false;

	#if UNITY_EDITOR
	private void OnEnable() {
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		EditorApplication.pauseStateChanged += OnPauseStateChanged;
		registered = true;
	}

	private void OnDisable() {
		if (registered) {
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.pauseStateChanged -= OnPauseStateChanged;
		}
	}
	
	private void OnPauseStateChanged(PauseState state) {
		IsPaused = state == PauseState.Paused;
	}

	private void OnPlayModeStateChanged(PlayModeStateChange change) {
		switch (change) {
			case ExitingPlayMode: IsPlayMode = false; break;
			case ExitingEditMode: IsPlayMode = true;  break;
			case EnteredEditMode: break;
			case EnteredPlayMode: break;
			default: break;
		}
	}
	#endif 

	/// <summary> Is the context of Unity attached to an Editor? </summary>
	public static bool IsInEditor {
		get {
	#if UNITY_EDITOR
			return true;
	#else
			return false;
	#endif 
		}
	}

	void Awake() {
		if (main != null) { Destroy(this); return; }
		main = this;
		if (!IsInEditor) {
			IsPlayMode = true;
			IsPaused = false;
		}
	}
	
}
