using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using LevelUpper.Extensions;

namespace LevelUpper.Debugs {
	public class FPSDisplay : MonoBehaviour {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
		private static List<float> deltaTimes = new List<float>();
		private bool down = false;
		[NonSerialized] public static bool show = false;
		[SerializeField] private float barWidth = 10;
		[SerializeField] private float barSpacing = 5;
		[SerializeField] private float greenFPS = 30;
		[SerializeField] private float yellowFPS = 15;

		protected void Update() {
			if (show) {
				deltaTimes.Add(Time.unscaledDeltaTime);
				if (deltaTimes.Count > Screen.width / (barWidth + barSpacing)) {
					deltaTimes.RemoveAt(0);
				}
			}
			if (Input.GetKey(KeyCode.JoystickButton4) &&
				Input.GetKey(KeyCode.JoystickButton5) &&
				Input.GetKey(KeyCode.JoystickButton6) &&
				Input.GetKey(KeyCode.JoystickButton8) &&
				Input.GetKey(KeyCode.JoystickButton9)) {
				if (!down) {
					Toggle();
					down = true;
				}
			} else {
				down = false;
			}
		}

		public static void Toggle() {
			deltaTimes = new List<float>();
			show = !show;
		}

		protected void OnGUI() {
			if (deltaTimes.Count > 0) {
				Color backup = GUI.color;
				int sizeBackup = GUI.skin.label.fontSize;
				float last = deltaTimes[deltaTimes.Count - 1];
				GUI.skin.label.fontSize = 32;
				if (last < 1f / greenFPS) {
					GUI.color = Color.green;
				} else if (last < 1f / yellowFPS) {
					GUI.color = Color.yellow;
				} else {
					GUI.color = Color.red;
				}
				TextAnchor alignbackup = GUI.skin.label.alignment;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height).MiddleCenter(0.9f, 0.9f), "FPS: " + (1f / last).ToString() + "\nRes: " + Screen.width.ToString() + " x " + Screen.height.ToString() + " " + Screen.currentResolution.refreshRate.ToString() + "Hz\nDeltaTime: " + (Time.unscaledDeltaTime * 1000).ToString("#,##0.00") + "ms");
				GUI.skin.label.alignment = alignbackup;
				float total = 0;
				for (int i = 0; i < deltaTimes.Count; ++i) {
					float current = deltaTimes[deltaTimes.Count - 1 - i];
					total += current;
					float height = current * 1000;
					if (current < 1f / greenFPS) {
						GUI.color = Color.green;
					} else if (current < 1f / yellowFPS) {
						GUI.color = Color.yellow;
					} else {
						GUI.color = Color.red;
					}
					GUI.DrawTexture(new Rect(i * (barWidth + barSpacing), Screen.height - height, barWidth, height), Texture2D.whiteTexture);
				}
				GUI.color = Color.green;
				GUI.DrawTexture(new Rect(0, Screen.height - (1000f / greenFPS), Screen.width, 1), Texture2D.whiteTexture);
				GUI.color = Color.yellow;
				GUI.DrawTexture(new Rect(0, Screen.height - (1000f / yellowFPS), Screen.width, 1), Texture2D.whiteTexture);
				GUI.color = Color.cyan;
				float average = (total / deltaTimes.Count);
				GUI.DrawTexture(new Rect(0, Screen.height - (average * 1000), Screen.width, 1), Texture2D.whiteTexture);
				GUI.skin.label.fontSize = sizeBackup;
				GUI.color = backup;
			}
		}
#endif
	}
}
