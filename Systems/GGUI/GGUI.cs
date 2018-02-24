using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using LevelUpper.Extensions;
using LevelUpper.RNG;

/// <summary> Contains information about a single UI Control </summary>
public class GGUIControl {
	/// <summary> Empty object[] to reuse </summary>
	static readonly object[] empty = new object[0];

	static readonly Rect DEFAULT_TOOLTIP_SIZE = new Rect(0,0,.25f,.25f);

	/// <summary> Empty Constructor. For this type, it's expected to use the initializer. </summary>
	/// <remarks> Does grab some override information from GGUI, such as color/textColor/fontSize. </remarks>
	public GGUIControl() { 
		color = GGUI.color;
		textColor = GGUI.textColor;
		fontSize = GGUI.fontSize;
		alignment = GGUI.alignment;
		anchorLegacy = GGUI.anchorLegacy;
	}

	/// <summary> Reference to the RectTransform of the created transform, once it exists. </summary>
	public RectTransform liveObject = null;
	
	/// <summary> Transform to anchor to. </summary>
	public Transform anchorTransform = null;

	/// <summary> Optional position in space relative to parent </summary>
	public Rect? position = null;
	/// <summary> Optional offsets, pixels to expand relative to parent </summary>
	public Rect? offsets = null;
	
	/// <summary> Color of this control's images </summary>
	public Color color = Color.white;
	/// <summary> Color of this control's text </summary>
	public Color textColor = Color.white;
	/// <summary> FontSize override if greater than 0 </summary>
	public float fontSize = -1;
	/// <summary> TextMeshPro alignment override. </summary>
	public TextAlignmentOptions? alignment;
	/// <summary> Legacy Text alignment override. </summary>
	public TextAnchor? anchorLegacy;

	/// <summary> Is this control active? </summary>
	public bool active = true;
	/// <summary> Kind of this control, eg Button, Text, Panel, Slider, etc. </summary>
	public string kind = "empty";
	/// <summary> Text content of this control </summary>
	public string content = "";
	/// <summary> Initial value for editable controls </summary>
	public object initialValue = null;
	/// <summary> Arbitrary extra information about the control </summary>
	public object[] info = empty;

	/// <summary> Delegate to call when changed, if supported </summary>
	public Delegate onModifiedCallback;

	/// <summary> Delegate to call when editing finishes, if supported </summary>
	public Delegate onEndEditCallback;

	/// <summary> If present, called when the element and its children are fully loaded. </summary>
	public Action<GGUIControl> __OnReady = null;

	/// <summary> If present, called when the element and its children are fully loaded, using the RectTransform . </summary>
	public Action<RectTransform> __OnReadyRect = null;

	/// <summary> If present, attached to the root of the control, and called when the object is enabled. </summary>
	public Action<RectTransform> __OnEnable = null;

	/// <summary> Holds a delegate that will be called every frame to update a live control </summary>
	public Action<RectTransform> __Live = null;

	/// <summary> Holds a delegate that is called to render the tooltip for the item, when hover'd. Returns a Rect containing its desired size. </summary>
	public Func<Rect> __RenderTooltip = null;

	/// <summary> Flag to ignore the mouse hovering over this control. </summary>
	public bool ignoreHover = false;

	/// <summary> Style to use to style the control </summary>
	public GGUIStyle style = null;

	/// <summary> Sprite override to style the control </summary>
	public Sprite sprite = null;

	/// <summary> Nested controls, if any. </summary>
	public List<GGUIControl> children = new List<GGUIControl>();

	/// <summary> Hooks up a callback to be called when the control is ready. </summary>
	/// <param name="callback"> Callback taking the RectTransform of the control. </param>
	/// <returns> The GGUIControl that the function was called on </returns>
	public GGUIControl OnReadyRect(Action<RectTransform> callback) { __OnReadyRect += callback; return this; }

	/// <summary> Hooks up a callback to be called when the control is ready. </summary>
	/// <param name="callback"> Callback on the GGUIControl once it is ready. </param>
	/// <returns> The GGUIControl that the function was called on </returns>
	public GGUIControl OnReady(Action<GGUIControl> callback) { __OnReady += callback; return this; }

	/// <summary> Hooks up a callback to be called every time the control is enabled. </summary>
	/// <param name="callback"> Callback on the RectTransform of the control. </param>
	/// <returns> The GGUIControl that the function was called on </returns>
	public GGUIControl OnEnable(Action<RectTransform> callback) { __OnEnable += callback; return this; }

	/// <summary> Hooks up a callback to be called every frame the control exists. </summary>
	/// <param name="callback"> Callback on the RectTransform of the control. </param>
	/// <returns> The GGUIControl that the function was called on </returns>
	public GGUIControl Update(Action<RectTransform> callback) { __Live += callback; return this; }

	/// <summary> Hooks up a callback to be called when the tooltip is rendered. </summary>
	/// <param name="callback"> Callback to render the tooltip </param>
	/// <returns> The GGUIControl that the function was called on </returns>
	public GGUIControl Tooltip(Action callback) { __RenderTooltip = ()=>{callback(); return DEFAULT_TOOLTIP_SIZE; }; return this; }

	/// <summary> Hooks up a callback to be called when the tooltip is rendered. </summary>
	/// <param name="callback"> Callback to render the tooltip </param>
	/// <returns> The GGUIControl that the function was called on </returns>
	public GGUIControl Tooltip(Func<Rect> callback) { __RenderTooltip = callback; return this; }

	/// <summary> Sends a click event to a button, if it exists. </summary>
	public void Clicked() { liveObject?.GetComponent<Button>()?.onClick.Invoke(); }

	/// <summary> Focuses the relevant selectable </summary>
	public void Focus() { liveObject?.GetComponent<Selectable>()?.Focus(); }

	/// <summary> Sets the text content directly on the GUIControl's live element. </summary>
	/// <param name="str"> Text to set </param>
	public void Text(string str) {
		TextMeshProUGUI tmp = liveObject?.GetComponent<TextMeshProUGUI>();
		if (tmp != null) { tmp.text = str; }

		Text txt = liveObject?.GetComponent<Text>();
		if (txt != null) { txt.text = str; }

		InputField field = liveObject?.GetComponent<InputField>();
		if (field != null) { field.text = str; }
	}

}


/// <summary> Class holding a set of styles. </summary>
public class GGUISkin : Dictionary<string, GGUIStyle> {

	[NonSerialized] public string name = "Unnamed";

	public GGUISkin() : base() { }
	public GGUISkin(GGUIStyle style) : base() { defaultStyle = style; }
	public GGUISkin(GGUIStyle style, IDictionary<string, GGUIStyle> data) : base(data) { defaultStyle = style; }

	/// <summary> Default style to use for "missing" styles. </summary>
	public GGUIStyle defaultStyle = ScriptableObject.CreateInstance<GGUIStyle>();

	/// <summary> Indexer for string -> GGUIStyle </summary>
	/// <param name="key"> Name of style to get/set </param>
	/// <returns> style associated with key, or the <see cref="defaultStyle"/> if no style with <paramref name="key"/> exists. </returns>
	public new GGUIStyle this[string key] {
		get {
			if (ContainsKey(key)) {
				var goy = (Dictionary<string, GGUIStyle>)this;
				return goy[key];
			}
			return defaultStyle;
		}
		set {
			if (value != null) {
				var goy = (Dictionary<string, GGUIStyle>)this;
				goy[key] = value;
			} else {
				if (ContainsKey(key)) { Remove(key); }
			}

		}
	}

	public GGUISkin Duplicate() {
		var copy = new GGUISkin(ScriptableObject.Instantiate(defaultStyle));

		foreach (var pair in this) { copy[pair.Key] = ScriptableObject.Instantiate(pair.Value); }

		return copy;
	}
}

/// <summary> Class holding IMGUI like functionality for creating UGUI controls </summary>
public static partial class GGUI {

	/// <summary> Focuses a given selectable, and activates input if needed </summary>
	/// <param name="selectable"> Selectable to focus </param>
	public static void Focus(this Selectable selectable) {
		selectable.Select();
		(selectable as InputField)?.ActivateInputField();
	}

	/// <summary> Lambda for use with OnReadyRect which fixes issues with the ScrollRect's scrollbars. </summary>
	public static Action<RectTransform> fixScrollOffset = (rt) => {
		ScrollRect scroller = rt.GetComponent<ScrollRect>();
		if (scroller.verticalScrollbar != null) {
			scroller.verticalNormalizedPosition = 0;
			scroller.verticalNormalizedPosition = .90f;
		}
		if (scroller.horizontalScrollbar != null) {
			scroller.horizontalNormalizedPosition = 1;
			scroller.horizontalNormalizedPosition = 0;
		}
	};
	
	/// <summary> Creates a new GGUISkin with the default settings. </summary>
	/// <returns> Default GGUISkin </returns>
	public static GGUISkin MakeDefaultSkin() {
		missingSprite = Resources.Load<Sprite>("GGUI_Missing");

		GGUISkin sk = new GGUISkin();
		sk.name = "Default Skin";

		var style = sk.defaultStyle;
		style.name = sk.name;
		style.fontSize = 36;
		style.font = Resources.Load<TMP_FontAsset>("GGUI_Default");
		style.fontLegacy = Resources.Load<Font>("GGUI_Default");
		style.texture = Resources.Load<Texture2D>("GGUI_Default");
		style.sprite = Resources.Load<Sprite>("GGUI_Default");

		var colors = new ColorBlock();
		colors.colorMultiplier = 1f;
		colors.fadeDuration = .15f;
		colors.normalColor = new Color(.7f, .7f, .7f);
		colors.highlightedColor = new Color(.85f, .85f, .85f);
		colors.pressedColor = new Color(1f, 1f, 1f);
		colors.disabledColor = new Color(.53f, .53f, .53f);

		style.colors = colors;

		style.alignment = TextAlignmentOptions.Center;

		var empty = ScriptableObject.Instantiate(style);
		empty.sprite = null;
		empty.texture = null;
		sk["empty"] = empty;

		
		var button = ScriptableObject.Instantiate(style);
		button.colors = colors; // Note: Nullables don't get serialized :(
		button.sprite = Resources.Load<Sprite>("GGUI_Button");
		button.name += "_button";
		sk["button"] = button;
		
		
		var box = ScriptableObject.Instantiate(style);
		box.sprite = Resources.Load<Sprite>("GGUI_Default");
		box.alignment = TextAlignmentOptions.TopLeft;
		box.name += "_box";
		sk["box"] = box;

		var toggle = ScriptableObject.Instantiate(style);
		toggle.colors = colors; // Note: Nullables don't get serialized :(
		toggle.sprite = Resources.Load<Sprite>("GGUI_CheckBG");
		toggle.subSprites = new Sprite[1] { Resources.Load<Sprite>("GGUI_Check") };
		toggle.imageType = Image.Type.Simple;
		toggle.preserveAspect = true;
		toggle.alignment = TextAlignmentOptions.Left;
		toggle.overflow = TextOverflowModes.Masking;
		toggle.name += "_toggle";
		sk["toggle"] = toggle;

		var spr = ScriptableObject.Instantiate(style);
		spr.sprite = null;
		spr.alignment = TextAlignmentOptions.Center;
		spr.imageType = Image.Type.Simple;
		spr.preserveAspect = true;
		spr.alignment = TextAlignmentOptions.Center;
		spr.overflow = TextOverflowModes.Overflow;
		spr.name += "_sprite";
		sk["sprite"] = spr;

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
		slider.name += "_slider";
		sk["slider"] = slider;

		var inputField = ScriptableObject.Instantiate(style);
		inputField.colors = colors; // Note: Nullables don't get serialized :(
		inputField.imageType = Image.Type.Sliced;
		inputField.autoSize = true;
		inputField.autoScale = false;
		inputField.alignment = TextAlignmentOptions.Center; // Label gets centered
		inputField.anchorLegacy = TextAnchor.MiddleLeft; // Input field is left aligned
		inputField.horWrapLegacy = HorizontalWrapMode.Wrap;
		inputField.verWrapLegacy = VerticalWrapMode.Truncate;
		inputField.name += "_inputField";
		sk["inputField"] = inputField;

		var vertScrollView = ScriptableObject.Instantiate(style);
		vertScrollView.colors = colors; // Note: Nullables don't get serialized :(
		vertScrollView.imageType = Image.Type.Sliced; // Scrollviews use sliced and simple.
		vertScrollView.preserveAspect = false;		// Scrollbar and background use simple/stretch
		vertScrollView.fillCenter = true;			// Background uses sliced/fillcenter
		vertScrollView.subSprites = new Sprite[] {
			Resources.Load<Sprite>("GGUI_VScrollBG"),
			Resources.Load<Sprite>("GGUI_VScrollHandle"),
		};
		vertScrollView.name += "_vertScrollView";
		sk["vertScrollView"] = vertScrollView;

		style.name += "_default";
		return sk;
	}

	public static void LoadSkin(string resourcesPathToSkin) {
		GGUISkinData data = Resources.Load<GGUISkinData>(resourcesPathToSkin);
		skin = ((data != null) ? data.skin : defaultSkin);//.Duplicate(); // Uncomment this if skins clobber eachother...
		skin.name = data.name;
		if (skin.defaultStyle != null) {
			skin.defaultStyle.name = data.name + "_default";
		}
	}

	
	/// <summary> History of created controls in the current function </summary>
	public static Stack<GGUIControl> history = new Stack<GGUIControl>();

	/// <summary> Reference to a sprite to use if sprites are null (missing). </summary>
	public static Sprite missingSprite;
	/// <summary> Default skin reference. </summary>
	public static GGUISkin defaultSkin = MakeDefaultSkin();
	/// <summary> Current skin to render the next control with </summary>
	public static GGUISkin skin;

	//////////////////////////////////////////////////////////////////////
	// Overrides ////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////

	/// <summary> Color to render the next control with </summary>
	public static Color color = Color.white;
	/// <summary> Color to render the text of the next control with </summary>
	public static Color textColor = Color.white;
	/// <summary> Fontsize to use for the next control, &lt=0 means use default size. </summary>
	public static float fontSize = -1;

	/// <summary> TextAnchor to use to override legacy Text anchors. Set to null to disable overriding. </summary>
	public static TextAnchor? anchorLegacy = null;
	/// <summary> TextAlignment to use to override TMPro anchors. Set to null to disable overriding. </summary
	public static TextAlignmentOptions? alignment = null;

	/// <summary> Cached sprites for fast access </summary>
	static Dictionary<Texture2D, Sprite> spriteCache = new Dictionary<Texture2D, Sprite>();
	/// <summary> Create or get a cached sprite from a Texture2D </summary>
	static Sprite GetSprite(Texture2D tex) {
		if (spriteCache.ContainsKey(tex)) {
			if (spriteCache[tex] != null) { return spriteCache[tex]; }
		}

		var sprite = Sprite.Create(tex, unitRect, Vector2.one * .5f);
		spriteCache[tex] = sprite;

		return sprite;
	}

	/// <summary> Reference to the extant canvas... </summary>
	private static GameObject _canvas = null;
	/// <summary> Accessor that attempts to get a canvas if it exists. </summary>
	public static RectTransform canvas { 
		get { 
			if (_canvas == null) { 
				_canvas = GameObject.Find("Canvas");
				GameObject.DontDestroyOnLoad(_canvas);
				if (_canvas == null) { return null; }
			}	
			return _canvas.GetComponent<RectTransform>();
		} 
	}

	/// <summary> Reference to the extant worldspace canvas... </summary>
	private static GameObject _worldCanvas = null;
	/// <summary> Accessor that attempts to get a worldspace canvas if it exists. </summary>
	public static RectTransform worldCanvas {
		get {
			if (_worldCanvas == null) {
				_worldCanvas = GameObject.Find("WorldCanvas");
				GameObject.DontDestroyOnLoad(_worldCanvas);
				if (_worldCanvas == null) { return null; }
			}
			return _worldCanvas.GetComponent<RectTransform>();
		}
	}

	/// <summary> Extension method to make it easier to add components with just a component reference. Why isn't this standard? </summary>
	/// <typeparam name="T"> Component Type </typeparam>
	/// <param name="c"> Component to add component to </param>
	/// <returns> Newly created component </returns>
	private static T AddComponent<T>(this Component c) where T : Component { return (c == null) ? null : c.gameObject.AddComponent<T>(); }
	
	/// <summary> Resets render state, and passes over the <paramref name="guifunc"/> setting up all GGUI Controls </summary>
	/// <param name="guifunc"> Function being called to render GUI </param>
	/// <returns></returns>
	private static GGUIControl PreRender(Action guifunc, Rect? pos = null, Transform anchor = null) {
		string rootName = "func_" + guifunc.Method.Name;
		// TBD: More reset logic?
		// LOOK HERE FIRST BEFORE DUPLICATING SKINS WHEN LOADING THEM IF THEY DON'T LOAD PROPERLY
		color = Color.white;
		textColor = Color.white;
		fontSize = -1;
		alignment = null;
		anchorLegacy = null;
		skin = defaultSkin;

		// Reset history state. 
		history.Clear();
		Rect? position = pos;
		if (position == null) { position = unitRect; }

		var root = new GGUIControl() { anchorTransform = anchor, position = position, kind = rootName };
		history.Push(root);
		// Call the guifunc, which sets up all controls
		guifunc();
		// Clean up last control...
		Next();
		
		return root;
	}

	/// <summary> Render a GUI from the results of calling a given function </summary>
	/// <param name="guifunc"> Function to call to render the GUI </param>
	/// <returns> RectTransform of root of the resultant UGUI controls </returns>
	public static RectTransform Render(Action guifunc, Rect? pos = null, Transform anchor = null) {
		var root = PreRender(guifunc, pos, anchor);
		// Actually do all rendering
		return Render(root, canvas);
	}
	
	/// <summary> Render a single UGUI control (and its children, recusively), and attach it to a given parent. </summary>
	/// <param name="c"> Control data to use to render the control </param>
	/// <param name="parent"> Parent RectTransform to attach the created control to. </param>
	/// <returns> RectTransform that was created. </returns>
	public static RectTransform Render(GGUIControl c, RectTransform parent) {
		RectTransform obj = new GameObject(c.kind).AddComponent<RectTransform>();
		obj.SetParent(parent, false);
		c.liveObject = obj;
		
		var attachTo = SetUpControl(c, obj);
		
		foreach (GGUIControl child in c.children) {
			Render(child, attachTo);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// ATCHUNG ////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Below: Stupid hacky bullshit to make scrollRects work the way SHOULD IN THE FIRST PLACE
		// Come on, whoever developed UGUI probably got dropped on the head as a child :)
		// Really shouldn't have to do this kind of post-processing to create something that's measured to the proper pixel size
		// It should just use the extents of the children that are attached to the content rect!
		// If there's elements hanging off, it should automatically detect it can scroll further
		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// ATCHUNG ////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////

		if (attachTo.gameObject.name == "Content") {
			var ghostContent = AttachImage(c, attachTo.parent as RectTransform, -1, new Rect(0, 0, 1, 0) );
			var ghostImage = ghostContent.GetComponent<Image>();
			ghostImage.color = Color.clear;
			ghostImage.raycastTarget = false;

			ghostContent.gameObject.name = "GhostContent";
			var expander = ghostContent.AddComponent<GGUI_GhostContent>();
			expander.realContent = attachTo;
			expander.expandHeight = true;
			expander.ExpandHeight();

			var scrollRect = attachTo.GetComponentAbove<ScrollRect>();
			scrollRect.content = ghostContent;
			if (scrollRect.verticalScrollbar) { scrollRect.verticalScrollbar.value = 1; }
			if (scrollRect.horizontalScrollbar) { scrollRect.horizontalScrollbar.value = 0; }
			
		}

		var ctrl = obj.AddComponent<GGUI_Control>();
		ctrl.control = c;

		c.__OnReady?.Invoke(c);
		c.__OnReadyRect?.Invoke(obj);

		return obj;
	}

	/// <summary> Sets up the details of a given control. </summary>
	/// <param name="c"> Control information to use to set up the control </param>
	/// <param name="obj"> Local 'Root' object for the given control </param>
	/// <returns> 'Root' object that nested objects get attached to </returns>
	public static RectTransform SetUpControl(GGUIControl c, RectTransform obj) {
		RectTransform attachTo = obj;

		if (c.kind == "Text") { AddText(c, obj); }
		if (c.kind == "Panel") { AddImage(c, obj); }
		if (c.kind == "Sprite") { AddImage(c, obj); }

		if (c.kind == "Box") { MakeItABox(c, obj); }
		if (c.kind == "Button") { MakeItAButton(c, obj); }
		if (c.kind == "Toggle") { MakeItAToggle(c, obj); }
		if (c.kind == "Slider") { MakeItASlider(c, obj); }
		if (c.kind == "TextField") { MakeItATextField(c, obj); }
		if (c.kind == "PassField") {
			MakeItATextField(c, obj);
			obj.GetComponent<InputField>().contentType = (InputField.ContentType.Password);
		}
		if (c.kind == "VerticalScrollView") { MakeItAVerticalScrollView(c, obj); attachTo = obj.Find("Viewport/Content") as RectTransform; }

		// Position is done last because some components like to mess with it.
		Reposition(c);

		return attachTo;
	}

	/// <summary> Apply appropriate repositioning of the RectTransform to the given GGUIControl </summary>
	/// <param name="c"></param>
	public static void Reposition(this GGUIControl c) {
		if (c.liveObject != null) {
			var obj = c.liveObject;
			var pos = (c.position == null) ? unitRect : c.position.Value;
			if (c.anchorTransform == null) {
				var off = (c.offsets == null) ? zeroRect : c.offsets.Value;
				Reposition(obj, pos, off);
			} else {
				Reposition(obj, pos, c.anchorTransform.position);
			}
		}
	}

	/// <summary> Anchors <paramref name="obj"/> to the 2d point at <paramref name="wpos"/>, with normalized size and offset <paramref name="size"/>. </summary>
	/// <param name="obj"> RectTransform of object to reposition </param>
	/// <param name="size"> Screen normalized Size and offset for placement </param>
	/// <param name="wpos"> world position to anchor to </param>
	public static void Reposition(RectTransform obj, Rect size, Vector3 wpos) {
		Vector3 screenPos = Camera.main.WorldToViewportPoint(wpos);
		Vector2 point = new Vector2(screenPos.x, screenPos.y);
		obj.anchorMax = point;
		obj.anchorMin = point;

		size.x *= Screen.height;
		size.y *= Screen.height;
		size.width *= Screen.height;
		size.height *= Screen.height;
		float halfWidth = size.width / 2f;
		float halfHeight = size.height / 2f;
		float xmin = size.x - halfWidth;
		float xmax = size.x + halfWidth;
		float ymin = size.y - halfHeight;
		float ymax = size.y + halfHeight;
		obj.offsetMin = new Vector2(xmin, ymin);
		obj.offsetMax = new Vector2(xmax, ymax);
	}

	/// <summary> Repositions a control relative to its parent, via anchors. </summary>
	/// <param name="obj"> Control's RectTransform </param>
	/// <param name="pos"> Normalized anchor coordinates </param>
	/// <param name="off"> Pixel offset coordinates </param>
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

	/// <summary> Adds a TextMeshPro component to the given control </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void AddText(GGUIControl c, RectTransform obj) {
		var style = c.style;
		var tmp = obj.AddComponent<TextMeshProUGUI>();
		tmp.text = c.content;

		float fontSize = c.fontSize <= 0 ? c.style.fontSize : c.fontSize;
		if (fontSize > 0)  { tmp.fontSize = fontSize; }

		tmp.color = (!c.active && c.style.colors != null) ? c.textColor * c.style.colors.Value.disabledColor : c.textColor;

		StyleText(style, tmp);

		if (c.alignment != null) { tmp.alignment = c.alignment.Value; }
	}

	/// <summary> Styles a TextMeshPro component </summary>
	/// <param name="style"> Style to apply </param>
	/// <param name="tmp"> TextMeshPro to apply to </param>
	public static void StyleText(GGUIStyle style, TextMeshProUGUI tmp) {
		tmp.font = style.font;
		tmp.alignment = style.alignment;
		tmp.overflowMode = style.overflow;
		tmp.enableAutoSizing = style.autoSize;
		if (style.autoScale) {
			var scaler = tmp.AddComponent<GGUI_ScaleFontSize>();
			scaler.fontSize = tmp.fontSize;
		}
		if (style.mat != null) {
			tmp.fontSharedMaterial = style.mat;
		}
	}

	/// <summary> Adds a legacy Text component to the given control </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void AddTextLegacy(GGUIControl c, RectTransform obj) {
		var style = c.style;
		var text = obj.AddComponent<Text>();
		text.text = c.content;

		float fontSize = c.fontSize <= 0 ? c.style.fontSize : c.fontSize;
		if (fontSize > 0) { text.fontSize = (int)fontSize; }
		
		text.color = (!c.active && c.style.colors != null) ? c.textColor * c.style.colors.Value.disabledColor : c.textColor;
		StyleTextLegacy(style, text);

		if (c.anchorLegacy != null) { text.alignment = c.anchorLegacy.Value; }

	}

	/// <summary> Styles a legacy Text component </summary>
	/// <param name="style"> Style to apply </param>
	/// <param name="text"> Legacy Text component to apply to </param>
	public static void StyleTextLegacy(GGUIStyle style, Text text) {
		text.font = style.fontLegacy;
		text.alignment = style.anchorLegacy;
		text.horizontalOverflow = style.horWrapLegacy;
		text.verticalOverflow = style.verWrapLegacy;
		text.resizeTextForBestFit = style.autoSize;
		if (style.autoScale) {
			var scaler = text.AddComponent<GGUI_ScaleFontSize>();
			scaler.fontSize = text.fontSize;
		}
	}

	/// <summary> Adds an Image component to the given control </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	/// <param name="imageNumber"> Image number to use. Negatives mean use the 'Default' image, >= 0 means use that SubSprite of the control's style. </param>
	public static void AddImage(GGUIControl c, RectTransform obj, int imageNumber = -1) {
		var style = c.style;
		var img = obj.AddComponent<Image>();
		img.color = c.color;
		if (c.sprite != null) {
			img.sprite = c.sprite;
		}
		StyleImage(style, img, imageNumber);

	}

	/// <summary> Styles an Image component </summary>
	/// <param name="style"> Style to apply </param>
	/// <param name="img"> Image component to apply to </param>
	/// <param name="imageNumber"> Image number to use. Negatives mean use the 'Default' sprite, >= 0 means use that SubSprite of the style. </param>
	public static void StyleImage(GGUIStyle style, Image img, int imageNumber = -1) {
		if (img.sprite == null) {
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
		}

		img.preserveAspect = style.preserveAspect;
		img.fillCenter = style.fillCenter;
	}

	/// <summary> Makes the given control a box. An <see cref="Image"/>, with Text attached. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void MakeItABox(GGUIControl c, RectTransform obj) {
		AttachText(c, obj);
		AddImage(c, obj);
	}

	/// <summary> Styles <see cref="Selectable"/> components. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	/// <remarks> Applys the style's <see cref="ColorBlock"/> and active state to any <see cref="Selectable"/>. </remarks>
	public static void StyleInteractables(GGUIControl c, Component obj) {
		var sel = obj.GetComponent<Selectable>();
		sel.interactable = c.active;
		if (c.style.colors != null) { sel.colors = c.style.colors.Value; }
	}

	/// <summary> Makes the given control a button. An <see cref="Image"/> with a <see cref="Button"/> component, and some text attached. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/></param>
	public static void MakeItAButton(GGUIControl c, RectTransform obj) {
		AddImage(c, obj);
		AttachText(c, obj);

		var button = obj.AddComponent<Button>();
		StyleInteractables(c, obj);

		if (c.onModifiedCallback != null) {
			Action callback = (Action)c.onModifiedCallback;
			button.onClick.AddListener(() => callback());
		}
	}

	/// <summary> Attaches an Object with a <see cref="TextMeshProUGUI"/> component on it. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Parent <see cref="RectTransform"/> </param>
	/// <param name="positionOverride"> Optional. Normalized Rect for where to place the text. </param>
	/// <returns> <see cref="RectTransform"/> of the created control </returns>
	public static RectTransform AttachText(GGUIControl c, RectTransform obj, Rect? positionOverride = null) {
		GameObject text = new GameObject("Text");
		var rt = text.AddComponent<RectTransform>();
		AddText(c, rt);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);
		return rt;
	}

	/// <summary> Attaches an Object with a legacy Text component on it. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Parent <see cref="RectTransform"/> </param>
	/// <param name="positionOverride"> Optional. Normalized Rect for where to place the text. </param>
	/// <returns> <see cref="RectTransform"/> of the created control </returns>
	public static RectTransform AttachLegacyText(GGUIControl c, RectTransform obj, Rect? positionOverride = null) {
		GameObject text = new GameObject("Legacy Text");
		var rt = text.AddComponent<RectTransform>();
		AddTextLegacy(c, rt);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);
		return rt;
	}
	
	/// <summary> Attaches an Object with an <see cref="Image"/> component on it. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Parent <see cref="RectTransform"/> </param>
	/// <param name="imageNumberOverride"> Optional. Image to override usage with. If not provided, the default (&lt0) image is used. If &gt=0, that SubSprite of the style is used. </param>
	/// <param name="positionOverride"> Optional. Normalized Rect for where to place the Image. </param>
	/// <returns> <see cref="RectTransform"/> of the created control </returns>
	public static RectTransform AttachImage(GGUIControl c, RectTransform obj, int? imageNumberOverride = null, Rect? positionOverride = null) {
		var imageNum = (imageNumberOverride == null) ? -1 : imageNumberOverride.Value;

		GameObject img = new GameObject("Image");
		var rt = img.AddComponent<RectTransform>();
		AddImage(c, rt, imageNum);
		rt.SetParent(obj);
		Reposition(rt, ((positionOverride != null) ? positionOverride.Value : unitRect), zeroRect);

		return rt;
	}

	/// <summary> Makes the control a Toggle. </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void MakeItAToggle(GGUIControl c, RectTransform obj) {
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

	/// <summary> Makes the control a plain TextField</summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void MakeItATextField(GGUIControl c, RectTransform obj) {
		Rect rest = new Rect(0, 0, 1f, 1f);
		string initialValue = (string)c.initialValue;
		
		if (c.content.Length > 1) {
			AttachText(c, obj, new Rect(0f, 0f, .5f, 1f));
			rest = new Rect(.5f, 0f, .5f, 1f);
		}

		AddImage(c, obj, -1);
		var bg = obj.GetComponent<Image>();
		
		var entry = AttachLegacyText(c, obj, rest);
		var entryText = entry.GetComponent<Text>();
		entryText.supportRichText = false;
		
		var place = AttachLegacyText(c, obj, rest);
		var placeText = place.GetComponent<Text>();
		placeText.supportRichText = false;
		placeText.fontStyle = FontStyle.Italic;
		Color col = c.textColor; col.a *= .5f;
		placeText.color = col;

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
			
			field.onEndEdit.AddListener((s) => { 
				if (Input.GetKeyDown(KeyCode.Return)) { callback(s); }
			} );
		}
	}

	/// <summary> Makes the control a Slider </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void MakeItASlider(GGUIControl c, RectTransform obj) {
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

		
		thumb.sizeDelta = new Vector2(50, 0);

		if (c.onModifiedCallback != null) {
			Action<float> callback = (Action<float>)c.onModifiedCallback;
			slider.onValueChanged.AddListener((f)=>callback(f));
		}

	}

	/// <summary> Makes the given control a Vertical Scroll View </summary>
	/// <param name="c"> Control information </param>
	/// <param name="obj"> Control <see cref="RectTransform"/> </param>
	public static void MakeItAVerticalScrollView(GGUIControl c, RectTransform obj) {
		var scrollRect = obj.AddComponent<ScrollRect>();
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		scrollRect.movementType = ScrollRect.MovementType.Clamped;
		scrollRect.elasticity = (float) c.info[3];
		scrollRect.decelerationRate = (float) c.info[2];
		scrollRect.inertia = scrollRect.decelerationRate > 0;
		scrollRect.scrollSensitivity = (float) c.info[1];

		AddImage(c, obj);
		int width = (int) c.info[0];

		var viewport = AttachImage(c, obj);
		viewport.gameObject.name = "Viewport";
		viewport.GetComponent<Image>().color = Color.red;
		viewport.AddComponent<Mask>().showMaskGraphic = false;
		viewport.offsetMax = new Vector2(-width, 0);
		
		var content = AttachImage(c, viewport, -1, new Rect(0, 0, 1, 1));
		// Just disable the image- it ends up covering up the above image. 
		content.GetComponent<Image>().enabled = false;
		content.gameObject.name = "Content";
		
		var scrollbar = AttachImage(c, obj, 0, new Rect(1, 0, 0, 1));
		scrollbar.offsetMin = new Vector2(-width, 0);
		var scrollbarImage = scrollbar.GetComponent<Image>();
		scrollbarImage.type = Image.Type.Simple;
		scrollbar.gameObject.name = "Scrollbar";
		var scrollbarComponent = scrollbar.AddComponent<Scrollbar>();

		var handle = AttachImage(c, scrollbar, 1);
		var handleImage = handle.GetComponent<Image>();
		handle.gameObject.name = "Handle";
		scrollbarComponent.handleRect = handle;
		scrollbarComponent.targetGraphic = handleImage;
		scrollbarComponent.direction = Scrollbar.Direction.BottomToTop;
		StyleInteractables(c, scrollbarComponent);

		scrollRect.viewport = viewport;
		scrollRect.verticalScrollbar = scrollbarComponent;
		scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent; 
		scrollRect.content = content;
		
	}


	/// <summary> Just an example GUI Function that demos some features </summary>
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

		Action<int, long> procGUI = (n, seed) => {
			Rect brush = new Rect(.0f, .0f, .5f, .1f);
			SRNG fixedRNG = new SRNG(seed);

			for (int i = 0; i < n; i++) {
				var brushRight = brush;
				brushRight.x += brush.width;
				var val = i;

				int roll = fixedRNG.NextInt(4);
				if (roll == 0) {
					Text(brush, "Label " + i);
					Button(brushRight, "Button " + i, () => { Debug.Log("Cuck " + val); });
				}
				if (roll == 1) {
					Text(brush, "Label " + i);
					Toggle(brushRight, "", false, (b) => { Debug.Log("Cuck " + val + " : " + b); });
				}
				if (roll == 2) {
					Text(brush, "Label " + i);
					Slider(brushRight, "", fixedRNG.NextFloat(), 0, 1, (f) => { Debug.Log("Cuck " + val + " : " + f); });
				}
				if (roll == 3) {
					var brushFull = brush;
					brushFull.width *= 2;
					TextField(brushFull, "Label " + i, "Texts", "Placeholders", (s) => { Debug.Log("Cuck " + val + " : " + s); });
				}
				brush.y += brush.height;
			}

		};

		textColor = color = Color.white;
		//Box(new Rect(.0f, .0f, .5f, .5f), "Hello");
		NestVertScroll(new Rect(.0f, .0f, .5f, .5f), 32, () => { procGUI(20, 112315151); });
		NestVertScroll(new Rect(.5f, .0f, .5f, .5f), 32, () => { procGUI(50, 351235154); });
		

		textColor = color = color.Alpha(.55f);
		//Box(new Rect(.33f, .075f, .33f, .33f), "How Rude");

	}
	
	/// <summary> Common Rect representing (0, 0, 1, 1) </summary>
	public static readonly Rect unitRect = new Rect(0, 0, 1, 1);
	/// <summary> Common Rect representing (0, 0, 0, 0) </summary>
	public static readonly Rect zeroRect = new Rect(0, 0, 0, 0);

	/// <summary> Last control created by an IMGUI-like function </summary>
	public static GGUIControl lastControl = null;

	/// <summary> Places the last control into the top object in the history, and removes it from the <see cref="lastControl"/> field. </summary>
	public static void Next() {
		if (lastControl != null && history.Count > 0) {
			history.Peek().children.Add(lastControl);
			lastControl = null;
		}
		if (history.Count == 0) { Debug.LogWarning("GGUI.Next: Nothing to add control to!"); }
	}
	
	/// <summary> Sets the <see cref="lastControl"/>'s active state to the given boolean value. </summary>
	/// <param name="active"> True to set the control active, false to set it inactive. </param>
	public static void SetActive(bool active) { lastControl.active = active; }

	/// <summary> Adds the <see cref="lastControl"/> into the top object's children list, and pushes it onto the <see cref="history"/> stack. </summary>
	public static void Push() {
		if (lastControl != null) {
			history.Peek().children.Add(lastControl);
			history.Push(lastControl);
			lastControl = null;
		} else {
			Debug.LogWarning("GGUI.Push: Control already used, can't push it!");
		}
	}

	/// <summary> Pop the top control off of the <see cref="history"/> stack, and return to the previous control. </summary>
	public static void Pop() {
		if (history.Count > 1) {
			Next();
			history.Pop();
		} else {
			Debug.LogWarning("GGUI.Pop: Only root control remains, can't pop!");
		}
	}

	/// <summary> Nests all of the controls created by <paramref name="inside"/> into <see cref="lastControl"/> </summary>
	/// <param name="inside"> Function to call to generate controls to nest. </param>
	/// <remarks> Same as a call to <see cref="Push"/>, all of the instructions of <paramref name="inside"/>, and then a call to <see cref="Pop"/>. </remarks>
	public static void Nest(Action inside) {
		
		Push();
		inside();
		Pop();
		
	}

	/// <summary> Nests all of the controls created by <paramref name="inside"/> into a <see cref="Panel"/> </summary>
	/// <param name="inside"> Function to call to generate controls to nest. </param>
	/// <remarks> Same as a call to <see cref="Panel"/>(<paramref name="position"/>), a call to <see cref="Push"/>, all of the instructions of <paramref name="inside"/>, and then a call to <see cref="Pop"/>. </remarks>
	/// <returns> Created control </returns>
	public static GGUIControl NestPanel(Rect? position, Action inside) {
		var ct = Panel(position);
		Push();
		inside();
		Pop();
		return ct;
	}
	/// <summary> Nests all of the controls created by <paramref name="inside"/> into a <see cref="Box"/> </summary>
	/// <param name="inside"> Function to call to generate controls to nest. </param>
	/// <remarks> Same as a call to <see cref="Box"/>(<paramref name="position"/>, <paramref name="text"/>), a call to <see cref="Push"/>, all of the instructions of <paramref name="inside"/>, and then a call to <see cref="Pop"/>. </remarks>
	/// <returns> Created control </returns>
	public static GGUIControl NestBox(Rect? position, string text, Action inside) {
		var ct = Box(position, text);
		Push();
		inside();
		Pop();
		return ct;
	}

	/// <summary> Nests all of the controls created by <paramref name="inside"/> into a <see cref="VertScrollView"/> </summary>
	/// <param name="inside"> Function to call to generate controls to nest. </param>
	/// <remarks> Same as a call to <see cref="VertScrollView"/>(<paramref name="position"/>, <paramref name="scrollbarWidth"/>), a call to <see cref="Push"/>, all of the instructions of <paramref name="inside"/>, and then a call to <see cref="Pop"/>. </remarks>
	/// <returns> Created control </returns>
	public static GGUIControl NestVertScroll(Rect? position, int scrollbarWidth, Action inside) {
		var ct = VertScrollView(position, scrollbarWidth);
		Push();
		inside();
		Pop();
		return ct;
	}

	/// <summary> Creates a sprite with the current style/colors. <para> Use this to display images as elements, for example, a rectangle, or a character's portrait. </para> </summary>
	/// <param name="position"> Normalized Position Rect to create the sprite inside of </param>
	/// <param name="sprite"> Provided sprite to use </param>
	/// <returns> Created control </returns>
	public static GGUIControl DrawSprite(Rect? position = null, Sprite sprite = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, kind = "Sprite", style = skin["sprite"], sprite = sprite };
	}
	
	/// <summary> Creates a Panel with the current style/colors. <para> Use this to create containers for nesting other controls within. </para> </summary>
	/// <param name="position"> Normalized Position Rect to create the panel at </param>
	/// <returns> Created control </returns>
	public static GGUIControl Panel(Rect? position = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, kind = "Panel", style = skin["panel"] };
	}
	/// <summary> Creates a TextMeshPro with the current style/colors <para> Use this to display arbitrary text. </para> </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="text"> Text to put into the control </param>
	/// <returns> Created control </returns>
	public static GGUIControl Text(Rect? position, string text = "") {
		Next();
		return lastControl = new GGUIControl() { position = position, kind = "Text", content = text, style = skin["text"] };
	}

	/// <summary> Creates a Box <para> Use this to display only text, or brief text with buttons nested within a panel. </para> </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="text"> Text to put into the control </param>
	/// <returns> Created control </returns>
	public static GGUIControl Box(Rect? position, string text) {
		Next();
		return lastControl = new GGUIControl() { position = position, kind = "Box", content = text, style = skin["box"] };
	}

	/// <summary> Creates a Button <para> The use for this should be pretty obvious: Short text on a clickable box with an action being called when clicked. </para> </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="text"> Text to put into the control </param>
	/// <param name="callback"> Function to call when the button is clicked. </param>
	/// <returns> Created control </returns>
	public static GGUIControl Button(Rect? position, string text, Action callback = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, kind = "Button", content = text, style = skin["button"], onModifiedCallback = callback };
	}

	/// <summary> Creates a Toggle/Checkbox control </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="text"> Text to put into the control </param>
	/// <param name="value"> Initial value of the control </param>
	/// <param name="callback"> Function to call when the control is changed. </param>
	/// <returns> Created control </returns>
	public static GGUIControl Toggle(Rect? position, string text, bool value, Action<bool> callback = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, initialValue = value, kind = "Toggle", content = text, style = skin["toggle"], onModifiedCallback = callback };
	}

	/// <summary> Creates a Slider control </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="label"> Text to put into the control, optionally. (Pass "" or null for no label) </param>
	/// <param name="value"> Initial value of the slider </param>
	/// <param name="min"> Minimum value of range </param>
	/// <param name="max"> Maximum value of range </param>
	/// <param name="callback"> Function to call when the control is changed </param>
	/// <returns> Created control </returns>
	public static GGUIControl Slider(Rect? position, string label, float value, float min, float max, Action<float> callback = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, initialValue = value, info = new object[] { min, max }, kind = "Slider", content = label, style = skin["slider"], onModifiedCallback = callback };
	}

	/// <summary> Creates a TextField control </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="text"> Text to put into the control </param>
	/// <param name="value"> Initial value of control </param>
	/// <param name="placeholder"> Placeholder text (when control is empty) </param>
	/// <param name="onChanged"> Function to call when the control is changed </param>
	/// <param name="onEndEdit"> Function to call when editing is finished </param>
	/// <returns> Created control </returns>
	public static GGUIControl TextField(Rect? position, string text, string value, string placeholder, Action<string> onChanged = null, Action<string> onEndEdit = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, initialValue = value, info = new object[] { placeholder }, kind = "TextField", content = text, style = skin["inputField"], onModifiedCallback = onChanged, onEndEditCallback = onEndEdit, };
	}

	/// <summary> Creates a TextField control with a password mask </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="text"> Text to put into the control </param>
	/// <param name="value"> Initial value of control </param>
	/// <param name="placeholder"> Placeholder text (when control is empty) </param>
	/// <param name="onChanged"> Function to call when the control is changed </param>
	/// <param name="onEndEdit"> Function to call when editing is finished </param>
	/// <returns> Created control </returns>
	public static GGUIControl PassField(Rect? position, string text, string value, string placeholder, Action<string> onChanged = null, Action<string> onEndEdit = null) {
		Next();
		return lastControl = new GGUIControl() { position = position, initialValue = value, info = new object[] { placeholder }, kind = "PassField", content = text, style = skin["inputField"], onModifiedCallback = onChanged, onEndEditCallback = onEndEdit, };
	}

	/// <summary> Creates a vertical scroll view </summary>
	/// <param name="position"> Normalized Position Rect to create the text at </param>
	/// <param name="scrollbarWidth"> Width of the scroll bar, in pixels </param>
	/// <param name="sensitivity"> Optional. Sensitivity value </param>
	/// <param name="inertia"> Optional. Interia value</param>
	/// <param name="elasticity"> Optional, Elasticity value </param>
	/// <returns> Created control </returns>
	public static GGUIControl VertScrollView(Rect? position, int scrollbarWidth = 32, float sensitivity = 66, float inertia = .135f, float elasticity = .1f) {
		Next();
		return lastControl = new GGUIControl() { position = position, info = new object[] { scrollbarWidth, sensitivity, inertia, elasticity }, kind = "VerticalScrollView", style = skin["vertScrollView"] };
	}


	/// <summary> Gets the WorldSpace rect of the RectTransfrom </summary>
	/// <param name="rt"> Given RectTransform </param>
	/// <returns> Worldspace Rectangle of the control. (X/Y only, obviously) </returns>
	public static Rect GetWorldRect(this RectTransform rt) {
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		Vector3 topLeft = corners[0];
		Vector3 bottomRight = corners[2];

		return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
	}

	public static void SetHeight(this RectTransform rt, float height) {

		rt.offsetMin = new Vector2(rt.offsetMin.x, -height);
		rt.offsetMax = new Vector2(rt.offsetMax.x, 0);
		
	}
	/*
	 * 

	/// <summary> Repositions a control relative to its parent, via anchors. </summary>
	/// <param name="obj"> Control's RectTransform </param>
	/// <param name="pos"> Normalized anchor coordinates </param>
	/// <param name="off"> Pixel offset coordinates </param>
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
	 * */
}
