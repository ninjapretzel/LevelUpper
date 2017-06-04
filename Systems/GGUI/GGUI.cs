using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using LevelUpper.Extensions;

public static class GGUI {

	public class Control {

		public Control() { }
		//public Control(string kind) { this.kind = kind; }
		public Rect? position = null;
		public Rect? offsets = null;
		public Color color = Color.white;
		public Color textColor = Color.white;
		public bool active = true;
		public string kind = "empty";
		public string content = "";

		public Delegate callback;
		
		

		public Style style = null;
		
		public List<Control> children = new List<Control>();
	}

	[CreateAssetMenu(fileName = "NewGGUIStyle", menuName = "GGUI Style", order = 50)]
	public class Style : ScriptableObject {
		public TMP_FontAsset font = null;
		public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
		public TextOverflowModes overflow = TextOverflowModes.Overflow;
		public bool autoSize = false;
		

		public Texture2D texture = null;
		public Sprite sprite = null;
		
		public Material mat = null;
		public ColorBlock? colors = null;

		public Image.Type type = Image.Type.Sliced;
		public bool preserveAspect = false;
		public bool fillCenter = true;
		

		


	}

	public class Skin : Dictionary<string, Style> {

		public Skin() : base() { }
		public Skin(Style style) : base() { defaultStyle = style; }
		public Skin(Style style, IDictionary<string, Style> data) : base(data) { defaultStyle = style; }

		public Style defaultStyle = ScriptableObject.CreateInstance<Style>();
		public new Style this[string key] {
			get {
				if (ContainsKey(key)) {
					var goy = (Dictionary<string, Style>)this;
					return goy[key];
				}
				return defaultStyle;
			}
			set {
				var goy = (Dictionary<string, Style>) this;
				goy[key] = value;
			}
		}
	}

	public static Skin LoadDefaultSkin() {
		missingSprite = Resources.Load<Sprite>("GGUI_Missing");

		Skin sk = new Skin();
		var style = sk.defaultStyle;
		style.font = Resources.Load<TMP_FontAsset>("GGUI_Default");
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


		return sk;
	}

	public static List<Control> controls = new List<Control>();
	public static Stack<Control> history = new Stack<Control>();

	public static Sprite missingSprite;
	public static Skin defaultSkin = LoadDefaultSkin();
	public static Skin skin;
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
		var root = new Control() { position = unitRect, kind = rootName };
		history.Push(root);

		guifunc();
		// Clean up last control...
		Next();
		
		// Actually do all rendering
		return Render(root, canvas);
	}

	public static RectTransform Render(Control c, RectTransform parent) {
		RectTransform obj = new GameObject(c.kind).AddComponent<RectTransform>();
		obj.SetParent(parent, false);
		
		ApplyContent(c, obj);
		
		foreach (Control child in c.children) {
			Render(child, obj);
		}
		
		return obj;
	}

	public static void ApplyContent(Control c, RectTransform obj) {
		
		if (c.kind == "Text") { AddText(c, obj); }
		if (c.kind == "Panel") { AddImage(c, obj); }
		if (c.kind == "Button") { AddButton(c, obj); }
		if (c.kind == "Toggle") { AddToggle(c, obj); }
		if (c.kind == "Box") { AddBox(c, obj); }

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

	public static void AddText(Control c, RectTransform obj) {
		var style = c.style;
		var tmp = obj.AddComponent<TextMeshProUGUI>();
		tmp.text = c.content;
		tmp.color = c.textColor;
		StyleText(style, tmp);
		//tmp.fontSize = c.st
	}

	public static void StyleText(Style style, TextMeshProUGUI tmp) {
		tmp.font = style.font;
		tmp.alignment = style.alignment;
		tmp.overflowMode = style.overflow;
		tmp.enableAutoSizing = style.autoSize;
	}

	public static void AddImage(Control c, RectTransform obj) {
		var style = c.style;
		var img = obj.AddComponent<Image>();
		img.color = c.color;
		StyleImage(style, img);
	}

	public static void StyleImage(Style style, Image img) {
		img.type = style.type;
		var sprite = style.sprite;
		if (sprite == null && style.texture != null) {
			sprite = GetSprite(style.texture);
		}
		if (sprite == null) { sprite = missingSprite; }
		img.sprite = sprite;
		img.preserveAspect = style.preserveAspect;
		img.fillCenter = style.fillCenter;
	}

	public static void AddBox(Control c, RectTransform obj) {
		AttachText(c, obj);
		AddImage(c, obj);
	}

	public static void StyleInteractables(Control c, Component obj) {
		var sel = obj.GetComponent<Selectable>();
		sel.interactable = c.active;
		if (c.style.colors != null) { sel.colors = c.style.colors.Value; }
	}

	public static void AddButton(Control c, RectTransform obj) {
		AttachText(c, obj);
		AddImage(c, obj);

		var button = obj.AddComponent<Button>();
		StyleInteractables(c, obj);

		Action callback = (Action)c.callback;
		button.onClick.AddListener(() => callback());
	}

	public static RectTransform AttachText(Control c, RectTransform obj, Rect? positionOverride = null) {
		GameObject text = new GameObject("Text");
		var rt = text.AddComponent<RectTransform>();
		AddText(c, rt);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);
		return rt;
	}

	public static RectTransform AttachImage(Control c, RectTransform obj, Rect? positionOverride = null) {
		GameObject img = new GameObject("Image");
		var rt = img.AddComponent<RectTransform>();
		AddImage(c, rt);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);

		return rt;
	}

	public static void AddToggle(Control c, RectTransform obj) {
		AttachText(c, obj, new Rect(.1f, 0f, .9f, 1f));

		GameObject bg = new GameObject("Background");



	}


	public static void TestFunc() {
		
		Box("Hello", new Rect(.0f, .0f, .5f, .5f));
		Box("World", new Rect(.5f, .0f, .5f, .5f));
		Box("Controls!", new Rect(.0f, .5f, .5f, .5f));
		Nest(()=>{
			color = Color.red;
			Button("Button 1", () => { Debug.Log("Button 1 clicked"); }, new Rect(.0f, .9f, .5f, .1f));
			textColor = Color.blue;
			Button("Button 2", () => { Debug.Log("Button 2 clicked"); }, new Rect(.5f, .9f, .5f, .1f));
			SetActive(false);
		});

		textColor = Color.white;
		color = Color.white;


		Box("Oy", new Rect(.5f, .5f, .5f, .5f));

		color = color.Alpha(.55f);
		Box("How Rude", new Rect(.33f, .33f, .33f, .33f));

	}
	
	public static readonly Rect unitRect = new Rect(0, 0, 1, 1);
	public static readonly Rect zeroRect = new Rect(0, 0, 0, 0);

	public static Control lastControl = null;

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

	public static void Panel(Rect? position = null) {
		Next();
		lastControl = new Control() { position = position, kind = "Panel", style = skin["panel"], color = color, textColor = textColor };
	}

	public static void Text(string text, Rect? position = null) {
		Next();
		lastControl = new Control() { position = position, kind = "Text", content = text, style = skin["text"], color = color, textColor = textColor };
	}

	public static void Box(string text, Rect? position = null) {
		Next();
		lastControl = new Control() { position = position, kind = "Box", content = text, style = skin["box"], color = color, textColor = textColor };
	}

	public static void Button(string text, Action callback = null, Rect? position = null) {
		Next();
		lastControl = new Control() { position = position, kind = "Button", content = text, style = skin["button"], callback = callback, color = color, textColor = textColor };
	}

	public static void Toggle(string text, Action<bool> callback = null, Rect? position = null) {
		Next();
		lastControl = new Control() { position = position, kind = "Toggle", content = text, style = skin["toggle"], callback = callback, color = color, textColor = textColor };
	}


	
}
