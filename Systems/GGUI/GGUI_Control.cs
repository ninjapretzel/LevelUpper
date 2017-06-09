using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GGUI_Control : MonoBehaviour {

	public Action<RectTransform> __OnEnable = null;
	

	void OnEnable() {
		if (__OnEnable != null) {
			__OnEnable(transform as RectTransform);
		}
	}

}
