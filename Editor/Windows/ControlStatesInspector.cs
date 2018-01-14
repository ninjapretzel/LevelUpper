#if UNITY_EDITOR
using System.Collections.Generic;
using LevelUpper.InputSystem;
using UnityEngine;
using UnityEditor;

namespace LevelUpper.Editor {
	public class ControlStatesInspector : EditorWindow {
		[MenuItem("LevelUpper/Input/Control States Inspector...")]
		private static void Init() {
			ControlStatesInspector instance = (ControlStatesInspector)GetWindow(typeof(ControlStatesInspector));
			instance.titleContent = new GUIContent("Control States Inspector");
			instance.Show();
		}

		private void OnGUI() {
			EditorGUILayout.BeginVertical(); {
				if (!EditorApplication.isPlaying) {
					GUILayout.Label("Waiting for editor to enter play mode...");
				} else {
					Dictionary<string, string> states = ControlStates.GetAll();
					if (states.Count == 0) {
						GUILayout.Label("No control states declared");
					} else {
						foreach (var state in states) {
							EditorGUILayout.LabelField(state.Key, state.Value);
						}
					}
				}
			} EditorGUILayout.EndVertical();
		}

		private void OnInspectorUpdate() {
			if (EditorApplication.isPlaying) {
				Repaint();
			}
		}
	}
}
#endif
