using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class GGUIRoot : GGUIPage {
	// Reserve focus...
	// Maybe there will be something else here eventually?
	// (UGUI InputField?)
	public static Type[] focusableTypes = {
		typeof(InputField),
	};

	public static bool hasFocus { get { return main != null && main.focused; } }
	public static GGUIRoot main;

	public bool focused = false;
	public GameObject focusedObject;
	public GameObject hoverObject;
	GameObject lastHover;
	RectTransform activeTooltip;

	GraphicRaycaster raycaster;
	PointerEventData data;
	List<RaycastResult> results;
	GGUI_Control hoverControl;
	Rect ttArea;


	void Awake() {
		if (main != null) { Destroy(gameObject); return; }
		main = this;
		DontDestroyOnLoad(gameObject);
		raycaster = GetComponent<GraphicRaycaster>();
		results = new List<RaycastResult>();
	}

	
	
	void Start() {
		
	}
	
	void Update() {
		focusedObject = EventSystem.current.currentSelectedGameObject;
		if (focusedObject != null && !focusedObject.GetComponent<InputField>()) { focusedObject = null; }
		focused = focusedObject != null && focusedObject.activeInHierarchy;


		data = new PointerEventData(EventSystem.current);
		data.position = Input.mousePosition;

		results.Clear();
		raycaster.Raycast(data, results);
		
		hoverControl = null;
		hoverObject = null;

		results.Sort((a,b)=>{ return b.depth - a.depth; });

		if (results.Count > 0) {
			foreach (var result in results) {
				hoverControl = result.gameObject.GetComponent<GGUI_Control>();
				if (hoverControl != null && !hoverControl.control.ignoreHover) {
					hoverObject = result.gameObject;
					break;
				}
			}

		}

		if (Input.GetMouseButtonDown(0) || hoverObject != lastHover) {
			RefreshTooltipNow();
		}

		if (activeTooltip != null) {

			Vector3 mouse = Input.mousePosition;
			ttArea.x = mouse.x / Screen.width;
			ttArea.y = 1.0f - (mouse.y / Screen.height);
			if (ttArea.xMax > 1) { ttArea.x = 1.0f - ttArea.width; }
			if (ttArea.yMax > 1) { ttArea.y = 1.0f - ttArea.height; }

			GGUI.Reposition(activeTooltip, ttArea, new Rect(0, 0, 0, 0));
		}

		
		lastHover = hoverObject;
		
	}
	public void KillTooltip() {
		if (activeTooltip != null) {
			Destroy(activeTooltip.gameObject);
			activeTooltip = null;
		}
	}

	public void RefreshTooltip() {
		KillTooltip();
		lastHover = null;
	}
 

	public void RefreshTooltipNow() {
		KillTooltip();

		if (hoverControl != null) {
			var ttrender = hoverControl.control.__RenderTooltip;
			if (ttrender != null) {

				activeTooltip = GGUI.Render(() => { ttArea = ttrender(); });
				foreach (var ggc in activeTooltip.GetComponentsInChildren<GGUI_Control>()) {
					ggc.control.ignoreHover = true;
				}
				foreach (var graphic in activeTooltip.GetComponentsInChildren<Graphic>()) {
					graphic.raycastTarget = false;
				}
				activeTooltip.name = "Tooltip";

			}
		}
	}
	
}
