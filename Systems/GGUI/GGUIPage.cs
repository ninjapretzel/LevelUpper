using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GGUIPage : MonoBehaviour {

	RectTransform livePage;
	Stack<RectTransform> history = new Stack<RectTransform>();

	void LateUpdate() {
		GGUI_ScaleFontSize.lastScreenHeight = Screen.height;
	}

	public void Push(Action guiFunc) {
		if (livePage != null) {
			history.Push(livePage);
			livePage.gameObject.SetActive(false);
			livePage = null;
		}
		Render(guiFunc);
	}

	public void Pop() {
		if (history.Count > 0) {
			if (livePage != null) { Destroy(livePage.gameObject); }
			livePage = history.Pop();
			livePage.gameObject.SetActive(true);

		} else {
			Debug.LogWarning("GGUIPage.Pop: history is empty! Cannot pop.");
		}
	}

	public void Render(Action guiFunc) {
		if (livePage != null) { Destroy(livePage.gameObject); }

		var page = GGUI.Render(guiFunc);
		page.SetParent(transform, false);
		livePage = page;

	}
	

	
}
