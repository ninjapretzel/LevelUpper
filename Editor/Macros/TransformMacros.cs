#if UNITY_EDITOR && !UNITY_WEBGL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace LevelUpper.Editor {

	public class TransformMacros {

		[MenuItem("LevelUpper/Macros/Toggle Active State &q")]
		public static void ToggleActive() {
			GameObject sel = Selection.activeGameObject;
			Undo.RecordObject(sel, "Toggle Active State");
			sel.SetActive(!sel.activeSelf);
		}


	}
}
#endif
