using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LevelUpper.Extensions;
using LevelUpper.InputSystem;

namespace LevelUpper.Debugs {
	public class DisplayControlStates : MonoBehaviour {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
		void OnGUI() {
			GUI.skin = Resources.Load<GUISkin>("standard");
			GUI.color = Color.white.Alpha(.5f);

			Rect area = new Rect(0, 0, Screen.width * .5f, Screen.height);
			GUI.Box(area, "");
			GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 2f), GUILayout.Height(Screen.height));
			{
				int i = 0;
				foreach (var pair in ControlStates.GetAll()) {
					GUI.color = ((i++ % 2 == 0) ? Color.grey : Color.white).Alpha(.5f);

					GUILayout.BeginHorizontal("box"); {
						GUI.color = Color.white;
						GUILayout.Label(pair.Key);
						GUILayout.FlexibleSpace();
						GUILayout.Label(pair.Value);

					}
					GUILayout.EndHorizontal();

				}

			}
			GUILayout.EndVertical();


		}
#endif

	}
}
