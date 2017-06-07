using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LevelUpper.Extensions;

public class GGUI_GhostContent : MonoBehaviour {

	public RectTransform realContent;
	private RectTransform rt = null;

	public bool expandHeight = false;
	public bool expandWidth = false;

	void Awake() {
		rt = GetComponent<RectTransform>();
	}

	void Update() {
		if (!rt) { rt = GetComponent<RectTransform>(); }
		if (!realContent) { Debug.LogWarning("RealContent of GhostContent not set!"); return; }

		if (expandHeight) { ExpandHeight(); }
		if (expandWidth) { ExpandWidth(); }

		realContent.position = transform.position;

		Rect targetRect = realContent.GetWorldRect();
		Rect worldRect = rt.GetWorldRect();

		float diff = worldRect.y - targetRect.y;

		realContent.position -= Vector3.up * diff;
	}
	
	public void ExpandHeight() {
		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;
		if (realContent.childCount == 0) { minHeight = maxHeight = 0; }
		for (int i = 0; i < realContent.childCount; i++) {
			var child = realContent.GetChild(i);

			var rect = child.GetComponent<RectTransform>().GetWorldRect();
			float ymax = rect.yMax;
			float ymin = rect.yMin;
			if (ymin < minHeight) { minHeight = ymin; }
			if (ymax > maxHeight) { maxHeight = ymax; }
		}

		
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, maxHeight - minHeight);
	}

	public void ExpandWidth() {
		float maxWidth = float.MinValue;
		float minWidth = float.MaxValue;
		if (realContent.childCount == 0) { minWidth = maxWidth = 0; }
		for (int i = 0; i < realContent.childCount; i++) {
			var child = realContent.GetChild(i);

			var rect = child.GetComponent<RectTransform>().GetWorldRect();
			float xmax = rect.xMax;
			float xmin = rect.xMin;
			if (xmin < minWidth) { minWidth = xmin; }
			if (xmax > maxWidth) { maxWidth = xmax; }
		}
		
		rt.sizeDelta = new Vector2(maxWidth - minWidth, rt.sizeDelta.y);
	}
	
}
