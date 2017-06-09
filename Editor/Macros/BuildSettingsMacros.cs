using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BuildSettingsMacros {
	public string[] scenes;
	public static string[] ReadSceneNames() {
		List<string> temp = new List<string>();
		foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) {
			if (S.enabled) {
				string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
				name = name.Substring(0, name.Length - 6);
				temp.Add(name);
			}
		}
		return temp.ToArray();
	}

	public static string[] ReadScenePaths() {
		List<string> temp = new List<string>();
		foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) {
			if (S.enabled) { temp.Add(S.path); }
		}
		return temp.ToArray();
	}
}
