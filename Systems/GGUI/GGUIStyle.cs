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


	public bool autoSize = false;


	public Texture2D texture = null;
	public Sprite sprite = null;

	public Sprite[] subSprites = empty;

	public Material mat = null;
	public ColorBlock? colors = null;

	public Image.Type imageType = Image.Type.Sliced;
	public bool preserveAspect = false;
	public bool fillCenter = true;

}

