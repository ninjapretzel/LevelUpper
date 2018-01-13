using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GGUI_Control : MonoBehaviour {

	public GGUIControl control;

	RectTransform rt { get { return transform as RectTransform; } }
	
	void Update() {

		control?.__Live?.Invoke(rt);

	}
	

	void OnEnable() {

		control?.__OnEnable?.Invoke(rt);


	}

}
