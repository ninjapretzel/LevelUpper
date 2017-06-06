using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using LevelUpper.Extensions;


public class GGUIControl {
	static readonly object[] empty = new object[0];

	public GGUIControl() { }
	//public Control(string kind) { this.kind = kind; }
	public Rect? position = null;
	public Rect? offsets = null;
	public Color color = Color.white;
	public Color textColor = Color.white;
	public bool active = true;
	public string kind = "empty";
	public string content = "";
	public object initialValue = null;
	public object[] info = empty;

	public Delegate onModifiedCallback;
	public Delegate onEndEditCallback;

	public float fontSize = -1;

	public GGUIStyle style = null;

	public List<GGUIControl> children = new List<GGUIControl>();

}

public class GGUISkin : Dictionary<string, GGUIStyle> {

	public GGUISkin() : base() { }
	public GGUISkin(GGUIStyle style) : base() { defaultStyle = style; }
	public GGUISkin(GGUIStyle style, IDictionary<string, GGUIStyle> data) : base(data) { defaultStyle = style; }

	public GGUIStyle defaultStyle = ScriptableObject.CreateInstance<GGUIStyle>();
	public new GGUIStyle this[string key] {
		get {
			if (ContainsKey(key)) {
				var goy = (Dictionary<string, GGUIStyle>)this;
				return goy[key];
			}
			return defaultStyle;
		}
		set {
			var goy = (Dictionary<string, GGUIStyle>)this;
			goy[key] = value;
		}
	}
}


public static partial class GGUI {


	public static GGUISkin LoadDefaultSkin() {
		missingSprite = Resources.Load<Sprite>("GGUI_Missing");

		GGUISkin sk = new GGUISkin();
		var style = sk.defaultStyle;
		style.font = Resources.Load<TMP_FontAsset>("GGUI_Default");
		style.fontLegacy = Resources.Load<Font>("GGUI_Default");
		Debug.Log(style.font);
		style.texture = Resources.Load<Texture2D>("GGUI_Default");
		style.sprite = Resources.Load<Sprite>("GGUI_Default");

		var colors = new ColorBlock();
		colors.colorMultiplier = 1f;
		colors.fadeDuration = .15f;
		colors.normalColor = new Color(.7f, .7f, .7f);
		colors.highlightedColor = Color.white;
		colors.pressedColor = new Color(.4f, .6f, .8f);
		colors.disabledColor = new Color(.3f, .3f, .3f);

		style.colors = colors;

		style.alignment = TextAlignmentOptions.Center;

		
		var button = ScriptableObject.Instantiate(style);
		button.colors = colors; // Note: Nullables don't get serialized :(
		button.sprite = Resources.Load<Sprite>("GGUI_Button");
		sk["button"] = button;

		var box = ScriptableObject.Instantiate(style);
		box.sprite = Resources.Load<Sprite>("GGUI_Default");
		box.alignment = TextAlignmentOptions.TopLeft;
		sk["box"] = box;

		var toggle = ScriptableObject.Instantiate(style);
		toggle.colors = colors; // Note: Nullables don't get serialized :(
		toggle.sprite = Resources.Load<Sprite>("GGUI_CheckBG");
		toggle.subSprites = new Sprite[1] { Resources.Load<Sprite>("GGUI_Check") };
		toggle.imageType = Image.Type.Simple;
		toggle.preserveAspect = true;
		toggle.alignment = TextAlignmentOptions.Left;
		toggle.overflow = TextOverflowModes.Masking;
		sk["toggle"] = toggle;


		var slider = ScriptableObject.Instantiate(style);
		slider.colors = colors ;// Note: Nullables don't get serialized :(
		slider.sprite = Resources.Load<Sprite>("GGUI_Slider");
		slider.subSprites = new Sprite[2] {
			Resources.Load<Sprite>("GGUI_SliderBG"),
			Resources.Load<Sprite>("GGUI_SliderFill"),
		};
		slider.alignment = TextAlignmentOptions.Center;
		slider.imageType = Image.Type.Sliced;
		// The handle is Simple/Preserve Aspect,
		// But the backgrounds are sliced/simple with fillCenter and are intended to be stretched.
		slider.preserveAspect = false; 
		sk["slider"] = slider;

		var inputField = ScriptableObject.Instantiate(style);
		inputField.colors = colors; // Note: Nullables don't get serialized :(
		inputField.imageType = Image.Type.Sliced;
		inputField.autoSize = true;
		inputField.alignment = TextAlignmentOptions.Center; // Label gets centered
		inputField.anchorLegacy = TextAnchor.MiddleLeft; // Input field is left aligned
		inputField.horWrapLegacy = HorizontalWrapMode.Wrap;
		inputField.verWrapLegacy = VerticalWrapMode.Truncate;
		sk["inputField"] = inputField;


		return sk;
	}

	public static List<GGUIControl> controls = new List<GGUIControl>();
	public static Stack<GGUIControl> history = new Stack<GGUIControl>();

	public static Sprite missingSprite;
	public static GGUISkin defaultSkin = LoadDefaultSkin();
	public static GGUISkin skin;
	public static Color color = Color.white;
	public static Color textColor = Color.white;

	
	static Dictionary<Texture2D, Sprite> spriteCache = new Dictionary<Texture2D, Sprite>();
	static Sprite GetSprite(Texture2D tex) {
		if (spriteCache.ContainsKey(tex)) {
			if (spriteCache[tex] != null) { return spriteCache[tex]; }
		}

		var sprite = Sprite.Create(tex, unitRect, Vector2.one * .5f);
		spriteCache[tex] = sprite;

		return sprite;
	}

	private static GameObject _canvas = null;
	public static RectTransform canvas { 
		get { 
			if (_canvas == null) { _canvas = GameObject.Find("Canvas"); }	
			if (_canvas == null) { return null; }
			return _canvas.GetComponent<RectTransform>();
		} 
	}

	private static T AddComponent<T>(this Component c) where T : Component { return (c == null) ? null : c.gameObject.AddComponent<T>(); }
	
	public static RectTransform Render(Action guifunc) {
		string rootName = guifunc.Method.Name;
		// TBD: More reset logic?
		color = Color.white;
		textColor = Color.white;
		skin = defaultSkin;

		history.Clear();
		controls.Clear();
		var root = new GGUIControl() { position = unitRect, kind = rootName };
		history.Push(root);

		guifunc();
		// Clean up last control...
		Next();
		
		// Actually do all rendering
		return Render(root, canvas);
	}

	public static RectTransform Render(GGUIControl c, RectTransform parent) {
		RectTransform obj = new GameObject(c.kind).AddComponent<RectTransform>();
		obj.SetParent(parent, false);
		
		ApplyContent(c, obj);
		
		foreach (GGUIControl child in c.children) {
			Render(child, obj);
		}
		
		return obj;
	}

	public static void ApplyContent(GGUIControl c, RectTransform obj) {
		
		if (c.kind == "Text") { AddText(c, obj); }
		if (c.kind == "Panel") { AddImage(c, obj); }
		if (c.kind == "Box") { AddBox(c, obj); }
		if (c.kind == "Button") { AddButton(c, obj); }
		if (c.kind == "Toggle") { AddToggle(c, obj); }
		if (c.kind == "Slider") { AddSlider(c, obj); }
		if (c.kind == "TextField") { AddTextField(c, obj); }


		// Position is done last because some components like to mess with it.
		var pos = (c.position == null) ? unitRect : c.position.Value;
		var off = (c.offsets == null) ? zeroRect : c.offsets.Value;
		Reposition(obj, pos, off);

	}

	public static void Reposition(RectTransform obj, Rect pos, Rect off) {
		float xmin = pos.x;
		float xmax = pos.x + pos.width;
		float ymax = 1f - pos.y;
		float ymin = ymax - pos.height;

		obj.anchorMin = new Vector2(xmin, ymin);
		obj.anchorMax = new Vector2(xmax, ymax);
		obj.offsetMin = new Vector2(-off.x, -off.y);
		obj.offsetMax = new Vector2(off.width, off.height);
	}

	public static void AddText(GGUIControl c, RectTransform obj) {
		var style = c.style;
		var tmp = obj.AddComponent<TextMeshProUGUI>();
		tmp.text = c.content;

		float fontSize = c.fontSize <= 0 ? c.style.fontSize : c.fontSize;
		if (fontSize > 0)  { tmp.fontSize = fontSize; }

		tmp.color = (!c.active && c.style.colors != null) ? c.textColor * c.style.colors.Value.disabledColor : c.textColor;

		StyleText(style, tmp);
	}

	public static void StyleText(GGUIStyle style, TextMeshProUGUI tmp) {
		tmp.font = style.font;
		tmp.alignment = style.alignment;
		tmp.overflowMode = style.overflow;
		tmp.enableAutoSizing = style.autoSize;
	}

	public static void AddTextLegacy(GGUIControl c, RectTransform obj) {
		var style = c.style;
		var text = obj.AddComponent<Text>();
		text.text = c.content;

		float fontSize = c.fontSize <= 0 ? c.style.fontSize : c.fontSize;
		if (fontSize > 0) { text.fontSize = (int)fontSize; }
		
		text.color = (!c.active && c.style.colors != null) ? c.textColor * c.style.colors.Value.disabledColor : c.textColor;
		StyleTextLegacy(style, text);	
	}

	public static void StyleTextLegacy(GGUIStyle style, Text text) {
		text.font = style.fontLegacy;
		text.alignment = style.anchorLegacy;
		text.horizontalOverflow = style.horWrapLegacy;
		text.verticalOverflow = style.verWrapLegacy;
		text.resizeTextForBestFit = style.autoSize;
	}

	public static void AddImage(GGUIControl c, RectTransform obj, int imageNumber = -1) {
		var style = c.style;
		var img = obj.AddComponent<Image>();
		img.color = c.color;
		StyleImage(style, img, imageNumber);
	}

	public static void StyleImage(GGUIStyle style, Image img, int imageNumber = -1) {
		img.type = style.imageType;
		var sprite = style.sprite;
		if (sprite == null && style.texture != null) {
			sprite = GetSprite(style.texture);
		}

		if (imageNumber > -1) {
			if (imageNumber < style.subSprites.Length) {
				sprite = style.subSprites[imageNumber];
			} else {
				sprite = null;
			}
		}

		if (sprite == null) { sprite = missingSprite; }

		img.sprite = sprite;
		img.preserveAspect = style.preserveAspect;
		img.fillCenter = style.fillCenter;
	}

	public static void AddBox(GGUIControl c, RectTransform obj) {
		AttachText(c, obj);
		AddImage(c, obj);
	}

	public static void StyleInteractables(GGUIControl c, Component obj) {
		var sel = obj.GetComponent<Selectable>();
		sel.interactable = c.active;
		if (c.style.colors != null) { sel.colors = c.style.colors.Value; }
	}

	public static void AddButton(GGUIControl c, RectTransform obj) {
		AddImage(c, obj);
		AttachText(c, obj);

		var button = obj.AddComponent<Button>();
		StyleInteractables(c, obj);

		if (c.onModifiedCallback != null) {
			Action callback = (Action)c.onModifiedCallback;
			button.onClick.AddListener(() => callback());
		}
	}

	public static RectTransform AttachText(GGUIControl c, RectTransform obj, Rect? positionOverride = null) {
		GameObject text = new GameObject("Text");
		var rt = text.AddComponent<RectTransform>();
		AddText(c, rt);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);
		return rt;
	}

	public static RectTransform AttachLegacyText(GGUIControl c, RectTransform obj, Rect? positionOverride = null) {
		GameObject text = new GameObject("Legacy Text");
		var rt = text.AddComponent<RectTransform>();
		AddTextLegacy(c, rt);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);
		return rt;
	}
	
	public static RectTransform AttachImage(GGUIControl c, RectTransform obj, int? imageNumberOverride = null, Rect? positionOverride = null) {
		var imageNum = (imageNumberOverride == null) ? -1 : imageNumberOverride.Value;

		GameObject img = new GameObject("Image");
		var rt = img.AddComponent<RectTransform>();
		AddImage(c, rt, imageNum);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);

		return rt;
	}

	public static void AddToggle(GGUIControl c, RectTransform obj) {
		AttachText(c, obj, new Rect(.1f, 0f, .9f, 1f));

		Rect left = new Rect(0f, 0f, .1f, 1f);
		var bg = AttachImage(c, obj, -1, left);
		var check = AttachImage(c, obj, 0, left);

		var toggle = obj.AddComponent<Toggle>();
		toggle.targetGraphic = bg.GetComponent<Image>();
		toggle.graphic = check.GetComponent<Image>();
		StyleInteractables(c, obj);

		toggle.isOn = (bool) c.initialValue;

		if (c.onModifiedCallback != null) {
			Action<bool> callback = (Action<bool>)c.onModifiedCallback;
			toggle.onValueChanged.AddListener((b)=>callback(b));
		}


	}

	public static void AddTextField(GGUIControl c, RectTransform obj) {
		var label = AttachText(c, obj, new Rect(0f, 0f, .5f, 1f));
		Rect right = new Rect(.5f, 0f, .5f, 1f);
		string initialValue = (string)c.initialValue;

		AddImage(c, obj, -1);
		var bg = obj.GetComponent<Image>();
		
		var entry = AttachLegacyText(c, obj, right);
		var entryText = entry.GetComponent<Text>();
		entryText.supportRichText = false;
		
		var place = AttachLegacyText(c, obj, right);
		var placeText = place.GetComponent<Text>();
		placeText.supportRichText = false;
		placeText.fontStyle = FontStyle.Italic;
		placeText.color = c.textColor;

		var field = obj.AddComponent<InputField>();
		field.targetGraphic = bg.GetComponent<Image>();
		field.contentType = (InputField.ContentType.Standard);
		
		field.textComponent = entryText;
		field.placeholder = placeText;

		
		placeText.text = (string) c.info[0];
		field.text = initialValue;
		entryText.text = initialValue;

		StyleInteractables(c, obj);
		
		if (c.onModifiedCallback != null) {
			Action<string> callback = (Action<string>) c.onModifiedCallback;
			field.onValueChanged.AddListener((s)=>callback(s));
		}
		if (c.onEndEditCallback != null) {
			Action<string> callback = (Action<string>)c.onEndEditCallback;
			field.onEndEdit.AddListener((s) => callback(s));
		}
	}

	public static void AddSlider(GGUIControl c, RectTransform obj) {
		if (c.content != null && c.content != "") {
			Rect above = new Rect(0, -.5f, 1, .5f);
			AttachText(c, obj, above);
		}

		AddImage(c, obj, 0);
		var fill = AttachImage(c, obj, 1);
		var thumb = AttachImage(c, obj);
		var thumbImb = thumb.GetComponent<Image>();
		thumbImb.type = Image.Type.Simple;
		thumbImb.preserveAspect = true;

		var slider = obj.AddComponent<Slider>();
		slider.fillRect = fill;
		slider.handleRect = thumb;
		slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
		slider.minValue = (float)c.info[0];
		slider.maxValue = (float)c.info[1];
		slider.value = (float)c.initialValue;

		StyleInteractables(c, obj);

		thumb.sizeDelta = new Vector2(100, 0);

		if (c.onModifiedCallback != null) {
			Action<float> callback = (Action<float>)c.onModifiedCallback;
			slider.onValueChanged.AddListener((f)=>callback(f));
		}

	}



	public static void TestFunc() {
		
		NestBox(new Rect(.0f, .5f, .5f, .5f), "Controls!", () => {
			Button(new Rect(.0f, .8f, .5f, .1f), "Button 1", () => { Debug.Log("Button 1 clicked"); });
			color = Color.red;
			Button(new Rect(.5f, .8f, .5f, .1f), "Button 2", () => { Debug.Log("Button 2 clicked"); });
			SetActive(false); // Disables previous control

			Button(new Rect(.0f, .9f, .5f, .1f), "Button 3", () => { Debug.Log("Button 3 clicked"); });
			textColor = Color.blue;
			Button(new Rect(.5f, .9f, .5f, .1f), "Button 4", () => { Debug.Log("Button 4 clicked"); });


			color = textColor = Color.white;
			Toggle(new Rect(.0f, .5f, 1f, .1f), "Toggle 1", true, (b) => { Debug.Log("Toggle 1 set to " + b); });

			Toggle(new Rect(.0f, .6f, 1f, .1f), "Toggle 2", true, (b) => { Debug.Log("Toggle 2 set to " + b); });
			SetActive(false);
			Toggle(new Rect(.0f, .7f, 1f, .1f), "Toggle 3", false, (b) => { Debug.Log("Toggle 3 set to " + b); });

			color = Color.green;
			TextField(new Rect(.0f, .4f, 1f, .1f), "String", "Some String", " Placeholder...",
				(s) => { Debug.Log("String Changed: " + s); },
				(s) => { Debug.Log("String Finished: " + s); }
			);
			color = Color.white;
			textColor = Color.blue;
			TextField(new Rect(.0f, .3f, 1f, .1f), "Text", "Some text", " Another Placeholder", 
				(s) => { Debug.Log("Text Changed: " + s); }, 
				(s) => { Debug.Log("Text Finished: " + s); }
			);

			TextField(new Rect(.0f, .2f, 1f, .1f), "What", "Can't touch this", " You can't see this ",
				(s) => { Debug.Log("What Changed: " + s); },
				(s) => { Debug.Log("What Finished: " + s); }
			);
			SetActive(false);

		});

		textColor = color = Color.white;


		NestBox(new Rect(.5f, .5f, .5f, .5f), "More Cuntrolls", ()=>{

			Slider(new Rect(.0f, .2f, 1f, .1f), "Labeled Slider", .5f, 0, 1, (f) => { Debug.Log("Labeled Slider set to " + f); });
			color = Color.green;
			Slider(new Rect(.0f, .3f, 1f, .1f), "", 5.5f, 5, 10, (f) => { Debug.Log("Slider set to " + f); });

			textColor = Color.blue;
			color = Color.white;
			Slider(new Rect(.0f, .5f, 1f, .1f), "Disabled Labeled Slider", 5.5f, 5, 10, (f) => { Debug.Log("Slider set to " + f); });
			SetActive(false);

		});

		Box(new Rect(.0f, .0f, .5f, .5f), "Hello");
		Box(new Rect(.5f, .0f, .5f, .5f), "World");

		textColor = color = color.Alpha(.55f);
		Box(new Rect(.33f, .075f, .33f, .33f), "How Rude");

	}
	
	public static readonly Rect unitRect = new Rect(0, 0, 1, 1);
	public static readonly Rect zeroRect = new Rect(0, 0, 0, 0);

	public static GGUIControl lastControl = null;

	public static void Next() {
		if (lastControl != null && history.Count > 0) {
			history.Peek().children.Add(lastControl);
			Debug.Log("Added " + lastControl.kind + " to " + history.Peek().kind);
			lastControl = null;
		}
		if (history.Count == 0) { Debug.LogWarning("GGUI.Next: Nothing to add control to!"); }
	}
	
	public static void SetActive(bool active) {
		lastControl.active = active;
	}

	public static void Push() {
		if (lastControl != null) {
			history.Peek().children.Add(lastControl);
			history.Push(lastControl);
			lastControl = null;
		} else {
			Debug.LogWarning("GGUI.Push: Control already used, can't push it!");
		}
	}

	public static void Pop() {
		if (history.Count > 1) {
			Next();
			history.Pop();
		} else {
			Debug.LogWarning("GGUI.Pop: Only root control remains, can't pop!");
		}
	}

	public static void Nest(Action inside) {
		Push();
		inside();
		Pop();
	}

	public static void NestPanel(Rect? position, Action inside) {
		Panel(position);
		Push();
		inside();
		Pop();
	}

	public static void NestBox(Rect? position, string text, Action inside) {
		Box(position, text);
		Push();
		inside();
		Pop();
	}

	public static void Panel(Rect? position = null) {
		Next();
		lastControl = new GGUIControl() { position = position, kind = "Panel", style = skin["panel"], color = color, textColor = textColor };
	}

	public static void Text(Rect? position, string text = "") {
		Next();
		lastControl = new GGUIControl() { position = position, kind = "Text", content = text, style = skin["text"], color = color, textColor = textColor };
	}

	public static void Box(Rect? position, string text) {
		Next();
		lastControl = new GGUIControl() { position = position, kind = "Box", content = text, style = skin["box"], color = color, textColor = textColor };
	}

	public static void Button(Rect? position, string text, Action callback = null) {
		Next();
		lastControl = new GGUIControl() { position = position, kind = "Button", content = text, style = skin["button"], onModifiedCallback = callback, color = color, textColor = textColor };
	}

	public static void Toggle(Rect? position, string text, bool value, Action<bool> callback = null) {
		Next();
		lastControl = new GGUIControl() { position = position, initialValue = value, kind = "Toggle", content = text, style = skin["toggle"], onModifiedCallback = callback, color = color, textColor = textColor };
	}

	public static void Slider(Rect? position, string label, float value, float min, float max, Action<float> callback = null) {
		Next();
		lastControl = new GGUIControl() { position = position, initialValue = value, info = new object[] { min, max }, kind = "Slider", content = label, style = skin["slider"], onModifiedCallback = callback, color = color, textColor = textColor };
	}
	
	public static void TextField(Rect? position, string text, string value, string placeholder, Action<string> onChanged = null, Action<string> onEndEdit = null) {
		Next();
		lastControl = new GGUIControl() { position = position, initialValue = value, info = new object[] { placeholder }, kind = "TextField", content = text, style = skin["inputField"], onModifiedCallback = onChanged, onEndEditCallback = onEndEdit, color = color, textColor = textColor, };
	}

	
}
