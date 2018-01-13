using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GGUIRoot : GGUIPage {

	public static bool hasFocus { get { return main != null && main.focused; } }
	public static GGUIRoot main;

	public bool focused = false;
	public GameObject focusedObject;

	void Awake() {
		if (main != null) { Destroy(gameObject); return; }
		main = this;
		DontDestroyOnLoad(gameObject);


	}

	
	
	void Start() {
		
	}
	
	void Update() {
		focusedObject = EventSystem.current.currentSelectedGameObject;
		focused = focusedObject != null && focusedObject.activeInHierarchy;

	}
	
}
