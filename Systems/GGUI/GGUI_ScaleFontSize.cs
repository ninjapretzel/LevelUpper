using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GGUI_ScaleFontSize : MonoBehaviour {

	public float fontSize = -1;
	private Text legacyText;
	private TextMeshProUGUI text;
	public static float baseScreenSize = 720f;
	public static float lastScreenHeight = Screen.height;

	void Awake() {
		legacyText = GetComponent<Text>();
		text = GetComponent<TextMeshProUGUI>();
	}
	
	void Start() {
		ResizeText();
		
	}
	
	void Update() {
		if (fontSize > 0 && lastScreenHeight != Screen.height) {
			ResizeText();

		}
	}

	private void ResizeText() {
		if (text != null) {
			text.enableAutoSizing = false;
			text.fontSize = fontSize * (Screen.height / baseScreenSize);
		}
		if (legacyText != null) {
			legacyText.resizeTextForBestFit = false;
			legacyText.fontSize = (int)(fontSize * (Screen.height / baseScreenSize));
		}
	}
}
