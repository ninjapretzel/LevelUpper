using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static GGUI;
using LevelUpper.Extensions;
using UnityEngine.UI;

public class GGUIBehaviour : MonoBehaviour {

	public static readonly Rect unit = new Rect(0, 0, 1, 1);


	public RectTransform gui = null;
	public bool rebuild = false;

	public virtual void OnEnable() {
		if (rebuild && gui != null) { 
			Destroy(gui.gameObject); 
			gui = null;
		}

		BuildGUI();

		if (gui != null) { gui.gameObject.SetActive(true); }

	}

	public virtual void OnDisable() {
		if (gui != null) { gui.gameObject.SetActive(false); }

	}

	public virtual void Update() {

	}


	public virtual void RenderGUI() { 
		GGUI.TestFunc();

	} 
	/// <summary> Rebuild the GUI right now. </summary>
	public void Rebuild() {
		if (gui != null) { 
			Destroy(gui.gameObject); 
		}
		gui = null;
	}

	private void BuildGUI() {

		if (gui == null) {
			gui = Render(this.RenderGUI);
		}
		
		if (gui != null) {
			gui.gameObject.name = GetType().Name + "." + gui.gameObject.name;
			gui.SetParent(canvas);
		}

	}

	public void SwitchTo<T>() where T : GGUIBehaviour {
		T target = transform.Require<T>();
		
		enabled = false;
		target.enabled = true;

	}


	public void SetupSimpleBar(GGUIControl control, Sprite sprite, Material mat, Func<float> ffill, float dampening = 5f) {
		float delta = 0;
		float fill = ffill();
		Image img = null;

		control.OnReadyRect((rt) => {
			img = rt.GetComponent<Image>();
			img.sprite = sprite;
			img.material = new Material(mat);
			img.material.SetFloat("_Fill", fill);
			img.material.SetFloat("_Fill", delta);
		});

		control.Update((rt) => {
			if (dampening > 0) {
				float nextFill = ffill();
				delta -= fill - nextFill;
				delta = Mathf.Lerp(delta, 0, Time.deltaTime * dampening);
				fill = nextFill;
			} else {
				fill = ffill();
			}
			if (img == null) { img = rt.GetComponent<Image>(); } else {
				img.material.SetFloat("_Fill", fill);
				img.material.SetFloat("_Delta", delta);

				Rect pos = rt.GetWorldRect();
				Vector4 size = new Vector4(pos.width / pos.height, 1f, 0f, 0f);
				img.material.SetVector("_SizeInfo", size);
			}
		});
	}


}
