using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GGUIRoot : GGUIPage {

	void Awake() {
		var page = GGUI.Render(GGUI.TestFunc);
		page.SetParent(transform, false);
	}

	
	
	void Start() {
		
	}
	
	void Update() {
		
	}
	
}
