using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary> Behaviour for a GGUIControl </summary>
public class GGUI_Control : MonoBehaviour {

	/// <summary> Linked GGUIControl </summary>
	public GGUIControl control;
	
	/// <summary> Attached RectTransform </summary>
	RectTransform rt { get { return transform as RectTransform; } }
	
	void Update() {

		control?.__Live?.Invoke(rt);

		if (control?.anchorTransform != null) {
			control.Reposition();
		}

	}
	

	void OnEnable() {

		control?.__OnEnable?.Invoke(rt);


	}

}
