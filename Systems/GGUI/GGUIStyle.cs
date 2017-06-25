using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

	
[CreateAssetMenu(fileName = "NewGGUIStyle", menuName = "GGUI Style", order = 50)]
public class GGUIStyle : ScriptableObject {
	public static readonly Sprite[] empty = new Sprite[0];
	
	public TMP_FontAsset font = null;
	public Font fontLegacy = null;

	public float fontSize = -1;

	public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
	public TextOverflowModes overflow = TextOverflowModes.Overflow;

	public TextAnchor anchorLegacy = TextAnchor.MiddleCenter;
	public HorizontalWrapMode horWrapLegacy = HorizontalWrapMode.Wrap;
	public VerticalWrapMode verWrapLegacy = VerticalWrapMode.Overflow;


	public bool autoScale = true;
	public bool autoSize = false;


	public Texture2D texture = null;
	public Sprite sprite = null;

	public Sprite[] subSprites = empty;

	public Material mat = null;
	public ColorBlock? colors = null;

	public Image.Type imageType = Image.Type.Sliced;
	public bool preserveAspect = false;
	public bool fillCenter = true;

	public void CopySettings(GGUIStyle other) {
		if (other.font != null) { font = other.font; }
		if (other.fontLegacy != null) { fontLegacy = other.fontLegacy; }
		if (other.fontSize > 0) { fontSize = other.fontSize; }

		alignment = other.alignment;
		overflow = other.overflow;

		anchorLegacy = other.anchorLegacy;
		horWrapLegacy = other.horWrapLegacy;
		verWrapLegacy = other.verWrapLegacy;

		autoScale = other.autoScale;
		autoSize = other.autoSize;

		if (other.texture != null) { texture = other.texture; }
		if (other.sprite != null) { sprite = other.sprite; }

		if (other.subSprites != empty && other.subSprites != null) {
			subSprites = other.subSprites;
		}

		if (other.mat != null) { mat = other.mat; }
		if (other.colors != null) { colors = other.colors; }
		
		imageType = other.imageType;
		preserveAspect = other.preserveAspect;
		fillCenter = other.fillCenter;
		
	}

}

