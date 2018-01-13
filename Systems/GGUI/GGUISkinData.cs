using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGGUISkin", menuName = "GGUI SkinData", order = 50)]
public class GGUISkinData : ScriptableObject {
	[Serializable] public class StyleInfo {
		public string name;
		public GGUIStyle style;
	}
	public GGUIStyle defaultStyle;
	public StyleInfo[] styles;

	[NonSerialized] private GGUISkin _skin = null;
	public GGUISkin skin {
		get {
			if (_skin == null) {
				_skin = GGUI.MakeDefaultSkin();
				_skin.defaultStyle.CopySettings(defaultStyle);

				foreach (var pair in _skin) {
					pair.Value.CopyDefaults(_skin.defaultStyle);
				}

				foreach (var info in styles) {
					if (_skin.ContainsKey(info.name)) {
						_skin[info.name].CopySettings(info.style);
					} else {
						_skin[info.name] = Instantiate(info.style);
						_skin[info.name].CopySettings(_skin.defaultStyle);
						_skin[info.name].CopySettings(info.style);
					}
				}

				foreach (var pair in _skin) {
					pair.Value.name = _skin.name + "_" + pair.Key;
				}


			}
			return _skin;
		}

	}

	private void OnEnable() { _skin = null; }
	private void OnDisable() { _skin = null; }
	


}
