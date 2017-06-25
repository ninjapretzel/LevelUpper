using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GGUI_Control : MonoBehaviour {

	public Action<RectTransform> __OnEnable = null;

	public Action<RectTransform> __Live = null;

	RectTransform rt { get { return transform as RectTransform; } }



	void Update() {
		
		if (__Live != null) {
			__Live(rt);
		}

	}
	

	void OnEnable() {

		if (__OnEnable != null) {
			__OnEnable(rt);
		}

	}

}
