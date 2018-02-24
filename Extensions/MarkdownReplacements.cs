using UnityEngine;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using LevelUpper.Extensions;
using System.Runtime.CompilerServices;

namespace LevelUpper.Markdown {
	public static class Replacements {

		/// <summary> Creates the tag for a given 'sprite' with the given info. </summary>
		/// <param name="str"> StringBuilder to add to </param>
		/// <param name="sheetName"> Name of SpriteSheet to add to </param>
		/// <param name="index"> Index of sprite. &gt;=0 to use index, &lt;0 to use name </param>
		/// <param name="name"> Name of sprite, or null. </param>
		/// <param name="color"> HexColor formated string, or null to not insert color</param>
		/// <param name="tint"> Insert 'tint' directive? </param>
		/// <returns> Same string builder that was passed in, with the tag added. </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] // Gotta go FAST.
		public static StringBuilder AddTag(StringBuilder str, string sheetName = null, int index = -1, string name = null, string color = null, bool tint = false) {

			str = str + "<sprite";
			if (sheetName != null && sheetName.Length > 0) {
				str = str + "=\"" + sheetName + "\"";
			}
			if (index >= 0) {
				str = str + " index=" + index;
			} else if (name != null && name.Length > 0) {
				str = str + " name=\"" + name + "\"";
			} else {
				str = str + " index=0";
			}
			if (color != null && color.Length > 0) {
				str += " color=";
				if (!color.StartsWith("#")) { str += '#'; }
				str += color;
			}
			if (tint) {
				str += " tint";
			}
			str += ">";

			return str;
		}
		static Regex enhancedGlpyhs = new Regex(@"\[.+?\]", RegexOptions.Compiled);
		static Regex enhancedInnerGlyph = new Regex(@"([a-zA-Z]+_)?([a-zA-Z\d]+)(\#[a-fA-F0-9]{0,8})?", RegexOptions.Compiled);
		public static string GlyphToSpriteTag(this string glyph) {
			StringBuilder str = "";
			if (glyph.StartsWith("[") && glyph.EndsWith("]")) {
				string[] sprs = glyph.Split(':');
				foreach (var spr in sprs) {
					Match m = enhancedInnerGlyph.Match(spr);
					if (m.Success) {
						string sheet = m.Groups[1].Success ? m.Groups[1].Value : "";
						if (sheet.EndsWith("_")) { sheet = sheet.Substring(0, sheet.Length - 1); }
						string name = m.Groups[2].Success ? m.Groups[2].Value : "0";
						int idx; if (!int.TryParse(name, out idx)) { idx = -1; }
						string color = m.Groups[3].Success ? m.Groups[3].Value : "";
						AddTag(str, sheet, idx, name, color, false);
					}
				}


			}

			return str.ToString();
		}


		public static string EnhanceGlyphs(this string markdown) {
			if (markdown == null || markdown.Length == 0) { return ""; }
			StringBuilder str = "";
			int pos = 0;
			var match = enhancedGlpyhs.Match(markdown, pos);
			while (match.Success) {
				
				int idx = match.Index;
				int gapLength = idx - pos;
				string gapContent = markdown.Substring(pos, gapLength);

				str += gapContent;

				string val = match.Value;
				string tag = GlyphToSpriteTag(val);

				str += tag;
				pos = idx + val.Length;
				match = enhancedGlpyhs.Match(markdown, pos);
			}
			if (pos >= 0 && pos < markdown.Length) {
				str += markdown.Substring(pos);
			}
			
			return str.ToString();
		}


		static Regex glyphs = new Regex( @"\[\w+\]" );
		/// <summary>
		/// Replaces glyph placeholders with the appropriate TextMeshPro sprite XML tags
		/// 
		/// <para> Examples: </para>
		///		<para> The placeholder [abutton] will be translated to a sprite tag of 'abutton' in the default sheet. </para>
		///		<para> The placeholder [xbox_abutton] will be translated to a sprite tag for sheet 'xbox' and sprite 'abutton' </para>
		///		
		/// </summary>
		/// <param name="markdown">markdown to replace.</param>
		/// <returns>Copy of <paramref name="markdown"/> with gylph placeholders replaced with the proper TextMeshPro sprite XML tags</returns>
		public static string ReplaceGlyphs(this string markdown) {
			StringBuilder str = markdown;

			int pos = 0;
			while (glyphs.IsMatch(str, pos)) {
				var match = glyphs.Match(str, pos);
				string val = match.Value.Substring(1, match.Length - 2);
				string sheet = null;
				string sprite = null;
				if (val.IndexOf("_") >= 0) {
					sheet = val.Substring(0, val.IndexOf("_"));
					sprite = val.Substring(val.IndexOf("_") + 1);

				} else {
					sprite = val;
				}

				StringBuilder rep = "<sprite";
				if (sheet != null) { rep += "=\"" + sheet + "\""; }
				if (sprite != null) { rep += " name=\"" + sprite + "\""; }
				rep += ">";

				if (rep.Length > 8) {
					str.Replace(match.Value, rep);
					pos = match.Index + rep.Length;
				} else {
					pos = match.Index + match.Length;
				}

			}

			return str;
		}

		static Regex bolds = new Regex( @"\*\*\ *?\S+?[\S \t]*?\*\*" );
		static Regex italics = new Regex( @"\*\ *?\S+?[\S \t]*?\*" );
		
		static Regex colorCode = new Regex( @"\\[A-Za-z]" );
		public static Dictionary<char, Color32> colorCodes = Defaults();
		
		public static Dictionary<char, Color32> Defaults() {
			Dictionary<char, Color32> codes = new Dictionary<char, Color32>();
			// rainbow (roygbiv)
			codes['r'] = new Color(.95f, .1f, 0);
			codes['o'] = new Color(.8f, .6f, 0);
			codes['y'] = new Color(1, .925f, .02f);
			codes['g'] = new Color(.2f, 1f, 0f);
			codes['b'] = new Color(.1f, .1f, 1);
			codes['i'] = new Color(.35f, 0, 1);
			codes['v'] = new Color(.8f, 0, 1);
			
			// Cyan
			codes['c'] = new Color(0f, .85f, .95f);

			// White/blacK
			codes['w'] = Color.white;
			codes['k'] = Color.black;
			
			//
			codes['u'] = Colors.HSV(76f/360f, .77f, .60f);
			
			// "Half" Grey
			codes['h'] = new Color(.5f, .5f, .5f);

			codes['q'] = new Color(.8f, .8f, .8f);
			codes['e'] = new Color(.95f, .45f, 0);
			codes['t'] = new Color(.8f, 1, .8f);
			codes['p'] = new Color(.8f, 1, .8f);
			codes['j'] = new Color(.5259f, .7098f, .8508f);
			codes['m'] = new Color(.8259f, .3214f, .8109f);
			codes['l'] = new Color(.5151f, .9131f, .3212f);
			
			return codes;
		}
		
		
		public static bool debugMode = false;
		
		public static string ReplaceMarkdown(this string markdown) {
			string str = markdown;
			str = str.ReplaceColors();
			str = str.ReplaceBolds();
			str = str.ReplaceItalics();
			
			
			return str;
		}
		
		public static string ReplaceColors(this string markdown) {
			StringBuilder str = markdown;
			//gotta be able to be >implying things
			str = str.Replace(">", "\\u>");
			bool open = false;
			while (colorCode.IsMatch(str)) {
				Match match = colorCode.Match(str);
				
				string mat = match.Value;
				string rep = match.Value;
				rep = "";
				
				char ch = mat[1];
				if (colorCodes.ContainsKey(ch)) {
					string tag = "";
					if (open) {
						tag = "</color>";
					}
					tag += "<color=#";
					
					tag += colorCodes[ch].HexString();
					
					tag += ">";
					rep = tag;
					open = true;
				}
				
				
				if (debugMode) {
					Debug.Log("Matched " + mat + " : replacing with : " + rep);
				}
				str = colorCode.Replace(str, rep, 1);
				
			}
			
			if (open) { 
				str += "</color>";
			}
			return str;
		}
		
		public static string ReplaceBolds(this string markdown) {
			StringBuilder str = markdown;
			
			while (bolds.IsMatch(str)) {
				Match match = bolds.Match(str);
				string mat = match.Value;
				string rep = match.Value;
				rep = rep.Substring(2, rep.Length-4);
				rep = "<b>" + rep + "</b>";
				
				if (debugMode) {
					Debug.Log("Matched " + mat + " : replacing with : " + rep);
				}
				str = bolds.Replace(str, rep, 1);
				
			}
			
			return str;
		}
		
		
		public static string ReplaceItalics(this string markdown) {
			StringBuilder str = markdown;
			
			while (italics.IsMatch(str)) {
				Match match = italics.Match(str);
				string mat = match.Value;
				string rep = match.Value;
				rep = rep.Substring(1, rep.Length-2);
				rep = "<i>" + rep + "</i>";
				
				
				if (debugMode) {
					Debug.Log("Matched " + mat + " : replacing with : " + rep);
				}
				str = italics.Replace(str, rep, 1);
				
			}
			
			return str;
		}
		
	}
}
