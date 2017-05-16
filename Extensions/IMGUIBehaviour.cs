using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LevelUpper.Extensions.IMGUI {

	public class IMGUIBehaviour : MonoBehaviour {

		#region Copy-Paste GUI extension Bullshit
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// General GUI Stuff
		// Most of this is because C# 4.0 doesn't have 'using static'
		// And Unity doesn't want to upgrade mono to latest C#
		// TBD: As soon as Unity allows for 'using static', move all of this stuff into its own class.
		#region General GUI Stuff
		/// <summary> Unfocuses the currently focused GUI elemnt by creating a new focusable control offscreen, and focusing it. </summary>
		public static void UnfocusControl() {
			string name = "Control" + Guid.NewGuid().ToString();
			GUI.SetNextControlName(name);
			GUI.Button(new Rect(-100, -100, 5, 5), "");
			GUI.FocusControl(name);
		}

		/// <summary> Alternate between colors. Defaults to <see cref="Color.white"/> and a .8 gray. </summary>
		/// <param name="i"> Current 'position' </param>
		/// <returns> Current position + 1</returns>
		public static int AlternateColor(int i) { return AlternateColor(i, Color.white, new Color(.8f, .8f, .8f, 1)); }
		/// <summary> Alternate between colors. Uses <paramref name="a"/> for even positions, and <paramref name="b"/> for odd positions. </summary>
		/// <param name="i"> Current 'position' </param>
		/// <returns> Current position + 1</returns>
		public static int AlternateColor(int i, Color a, Color b) {
			GUI.color = (i % 2 == 0) ? a : b;
			return ++i;
		}
		/// <summary> Alternate color between a given array of colors. Repeats from the begining once the end is reached. </summary>
		/// <param name="i"> Current 'position'. </param>
		/// <param name="colors"> Colors to use. </param>
		/// <returns> Current position + 1</returns>
		public static int AlternateColor(int i, Color[] colors) {
			GUI.color = colors[i % colors.Length];
			return ++i;
		}

		/// <summary> Create an auto-layout collapsable area. </summary>
		/// <param name="expanded"> Is the control expanded? </param>
		/// <param name="label"> Label to place next to the expander button. </param>
		/// <param name="style"> Style to draw around the inside part. </param>
		/// <param name="func"> Function to call to draw the inside part. </param>
		/// <returns> Updated state of the expansion of the collapsable area. </returns>
		public static bool Collapsable(bool expanded, string label, Action func = null, string style = "box") {
			bool retval = expanded;
			BeginVertical(style);
			{
				BeginHorizontal("box");
				{
					if (Button(expanded ? "V" : ">", FixedWidth(20))) { retval = !retval; }
					Label(label, ExpandWidth(true));
				}
				EndHorizontal();

				if (retval && func != null) { func(); }
			}
			EndVertical();
			return retval;
		}
		#endregion

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Misc Wrapper Stuff
		#region MISC Wrapper Stuff

		/// <summary> Shortcut to check if the game is playing </summary>
		public bool isPlaying { get { return Application.isPlaying; } }

		#endregion
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// GUI Event Wrapper Stuff
		#region GUI Event Wrappers
		/// <summary> Check the current event to see if it is a KeyDown of the key for <paramref name="code"/> .</summary>
		/// <param name="code"> Keycode to check for </param>
		/// <returns> True if the current event is a KeyDown for <paramref name="code"/>, false otherwise. </returns>
		public static bool KeyDown(KeyCode code) { return Event.current.type == EventType.KeyDown && Event.current.keyCode == code; }

		/// <summary> Check the current event to see if it is a KeyUp of the key for <paramref name="code"/> .</summary>
		/// <param name="code"> Keycode to check for </param>
		/// <returns> True if the current event is a KeyUp for <paramref name="code"/>, false otherwise. </returns>
		public static bool KeyUp(KeyCode code) { return Event.current.type == EventType.KeyUp && Event.current.keyCode == code; }

		/// <summary> Check the current event to see if it is a MouseDown of the given mouse <paramref name="button"/> </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool MouseDown(int button) { return Event.current.type == EventType.MouseDown && Event.current.button == button; }

		/// <summary> Check the current event to see if it is a MouseUp of the given mouse <paramref name="button"/> </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool MouseUp(int button) { return Event.current.type == EventType.MouseUp && Event.current.button == button; }

		/// <summary> Get the current event's mouse position </summary>
		public static Vector2 mousePosition { get { return Event.current.mousePosition; } }

		#endregion

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// GUI Wrapper

		#region GUI wrappers
		
		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Labels

		/// <summary> Draw a label over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		public static void Label(Rect area, string content) { GUI.Label(area, content); }
		/// <summary> Draw a label over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		public static void Label(Rect area, Texture content) { GUI.Label(area, content); }
		/// <summary> Draw a label over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		public static void Label(Rect area, GUIContent content) { GUI.Label(area, content); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Boxes

		/// <summary> Draw a box over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		public static void Box(Rect area, string content) { GUI.Box(area, content); }
		/// <summary> Draw a box over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		public static void Box(Rect area, Texture content) { GUI.Box(area, content); }
		/// <summary> Draw a box over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		public static void Box(Rect area, GUIContent content) { GUI.Box(area, content); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Buttons

		/// <summary> Draw a button and check for a click over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		/// <returns> If the button was clicked or not </returns>
		public static bool Button(Rect area, string content) { return GUI.Button(area, content); }
		/// <summary> Draw a button and check for a click over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		/// <returns> If the button was clicked or not </returns>
		public static bool Button(Rect area, Texture content) { return GUI.Button(area, content); }
		/// <summary> Draw a button and check for a click over a given <paramref name="area"/> with a given <paramref name="content"/></summary>
		/// <param name="area"> Area to draw the display </param>
		/// <param name="content"> Content to draw </param>
		/// <returns> If the button was clicked or not </returns>
		public static bool Button(Rect area, GUIContent content) { return GUI.Button(area, content); }

		#endregion
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// GUILayout wrapper 

		#region GUILayout wrappers
		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Labels

		/// <summary> Make an auto-layout label with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Label(string content, params GUILayoutOption[] options) { GUILayout.Label(content, options); }
		/// <summary> Make an auto-layout label with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Label(Texture content, params GUILayoutOption[] options) { GUILayout.Label(content, options); }
		/// <summary> Make an auto-layout label with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Label(GUIContent content, params GUILayoutOption[] options) { GUILayout.Label(content, options); }
		/// <summary> Make an auto-layout label in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Label(string content, string style, params GUILayoutOption[] options) { GUILayout.Label(content, style, options); }
		/// <summary> Make an auto-layout label in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Label(Texture content, string style, params GUILayoutOption[] options) { GUILayout.Label(content, style, options); }
		/// <summary> Make an auto-layout label in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Label(GUIContent content, string style, params GUILayoutOption[] options) { GUILayout.Label(content, style, options); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Boxes

		/// <summary> Make an auto-layout box with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Box(string content, params GUILayoutOption[] options) { GUILayout.Box(content, options); }
		/// <summary> Make an auto-layout box with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Box(Texture content, params GUILayoutOption[] options) { GUILayout.Box(content, options); }
		/// <summary> Make an auto-layout box with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Box(GUIContent content, params GUILayoutOption[] options) { GUILayout.Box(content, options); }
		/// <summary> Make an auto-layout box in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Box(string content, string style, params GUILayoutOption[] options) { GUILayout.Box(content, style, options); }
		/// <summary> Make an auto-layout box in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Box(Texture content, string style, params GUILayoutOption[] options) { GUILayout.Box(content, style, options); }
		/// <summary> Make an auto-layout box in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		public static void Box(GUIContent content, string style, params GUILayoutOption[] options) { GUILayout.Box(content, style, options); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Buttons (normal) 

		/// <summary> Make an auto-layout button with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(string content, params GUILayoutOption[] options) { return GUILayout.Button(content, options); }
		/// <summary> Make an auto-layout button with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(Texture content, params GUILayoutOption[] options) { return GUILayout.Button(content, options); }
		/// <summary> Make an auto-layout button with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(GUIContent content, params GUILayoutOption[] options) { return GUILayout.Button(content, options); }
		/// <summary> Make an auto-layout button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(string content, string style, params GUILayoutOption[] options) { return GUILayout.Button(content, style, options); }
		/// <summary> Make an auto-layout button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(Texture content, string style, params GUILayoutOption[] options) { return GUILayout.Button(content, style, options); }
		/// <summary> Make an auto-layout button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(GUIContent content, string style, params GUILayoutOption[] options) { return GUILayout.Button(content, style, options); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Buttons (Action callback)

		/// <summary> Make an auto-layout button with the given <paramref name="content"/> and <paramref name="options"/>. Calls the given <paramref name="action"/> if the button is hit. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="action"> Function to run if the button is hit. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(string content, Action action, params GUILayoutOption[] options) { if (Button(content, options)) { action(); return true; } return false; }
		/// <summary> Make an auto-layout button with the given <paramref name="content"/> and <paramref name="options"/>. Calls the given <paramref name="action"/> if the button is hit. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="action"> Function to run if the button is hit. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(Texture content, Action action, params GUILayoutOption[] options) { if (Button(content, options)) { action(); return true; } return false; }
		/// <summary> Make an auto-layout button with the given <paramref name="content"/> and <paramref name="options"/>. Calls the given <paramref name="action"/> if the button is hit. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="action"> Function to run if the button is hit. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(GUIContent content, Action action, params GUILayoutOption[] options) { if (Button(content, options)) { action(); return true; } return false; }
		/// <summary> Make an auto-layout button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. Calls the given <paramref name="action"/> if the button is hit. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="action"> Function to run if the button is hit. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(string content, string style, Action action, params GUILayoutOption[] options) { if (Button(content, style, options)) { action(); return true; } return false; }
		/// <summary> Make an auto-layout button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. Calls the given <paramref name="action"/> if the button is hit. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="action"> Function to run if the button is hit. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(Texture content, string style, Action action, params GUILayoutOption[] options) { if (Button(content, style, options)) { action(); return true; } return false; }
		/// <summary> Make an auto-layout button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. Calls the given <paramref name="action"/> if the button is hit. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="action"> Function to run if the button is hit. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, if the button was hit, or false, if it was not. </returns>
		public static bool Button(GUIContent content, string style, Action action, params GUILayoutOption[] options) { if (Button(content, style, options)) { action(); return true; } return false; }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Buttons (Repeat)

		/// <summary> Make an auto-layout repeat button with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, every frame the button is held, or false, if it is not. </returns>
		public static bool RepeatButton(string content, params GUILayoutOption[] options) { return GUILayout.RepeatButton(content, options); }
		/// <summary> Make an auto-layout repeat button with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, every frame the button is held, or false, if it is not. </returns>
		public static bool RepeatButton(Texture content, params GUILayoutOption[] options) { return GUILayout.RepeatButton(content, options); }
		/// <summary> Make an auto-layout repeat button with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, every frame the button is held, or false, if it is not. </returns>
		public static bool RepeatButton(GUIContent content, params GUILayoutOption[] options) { return GUILayout.RepeatButton(content, options); }
		/// <summary> Make an auto-layout repeat button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, every frame the button is held, or false, if it is not. </returns>
		public static bool RepeatButton(string content, string style, params GUILayoutOption[] options) { return GUILayout.RepeatButton(content, style, options); }
		/// <summary> Make an auto-layout repeat button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, every frame the button is held, or false, if it is not. </returns>
		public static bool RepeatButton(Texture content, string style, params GUILayoutOption[] options) { return GUILayout.RepeatButton(content, style, options); }
		/// <summary> Make an auto-layout repeat button in the given <paramref name="style"/> with the given <paramref name="content"/> and <paramref name="options"/>. </summary>
		/// <param name="content"> Content to display. </param>
		/// <param name="style"> Style to use for display. </param>
		/// <param name="options"> Options to change the display. </param>
		/// <returns> True, every frame the button is held, or false, if it is not. </returns>
		public static bool RepeatButton(GUIContent content, string style, params GUILayoutOption[] options) { return GUILayout.RepeatButton(content, style, options); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Toggles

		/// <summary> Make an auto-layout toggle, with the given <paramref name="label"/>, and the given <paramref name="options"/> </summary>
		/// <param name="v"> Current value of the toggle. </param>
		/// <param name="label"> Content to display. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> State of the toggle, potentially having changed if it was clicked. </returns>
		public static bool Toggle(bool v, string label, params GUILayoutOption[] options) { return GUILayout.Toggle(v, label, options); }
		/// <summary> Make an auto-layout toggle, with the given <paramref name="label"/>, and the given <paramref name="options"/> </summary>
		/// <param name="v"> Current value of the toggle. </param>
		/// <param name="label"> Content to display. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> State of the toggle, potentially having changed if it was clicked. </returns>
		public static bool Toggle(bool v, Texture label, params GUILayoutOption[] options) { return GUILayout.Toggle(v, label, options); }
		/// <summary> Make an auto-layout toggle, with the given <paramref name="label"/>, and the given <paramref name="options"/> </summary>
		/// <param name="v"> Current value of the toggle. </param>
		/// <param name="label"> Content to display. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> State of the toggle, potentially having changed if it was clicked. </returns>
		public static bool Toggle(bool v, GUIContent label, params GUILayoutOption[] options) { return GUILayout.Toggle(v, label, options); }
		/// <summary> Make an auto-layout toggle, in the given <paramref name="style"/> with the given <paramref name="label"/>, and the given <paramref name="options"/> </summary>
		/// <param name="v"> Current value of the toggle. </param>
		/// <param name="label"> Content to display. </param>
		/// <param name="style"> Style to display the toggle with. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> State of the toggle, potentially having changed if it was clicked. </returns>
		public static bool Toggle(bool v, string label, string style, params GUILayoutOption[] options) { return GUILayout.Toggle(v, label, style, options); }
		/// <summary> Make an auto-layout toggle, in the given <paramref name="style"/> with the given <paramref name="label"/>, and the given <paramref name="options"/> </summary>
		/// <param name="v"> Current value of the toggle. </param>
		/// <param name="label"> Content to display. </param>
		/// <param name="style"> Style to display the toggle with. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> State of the toggle, potentially having changed if it was clicked. </returns>
		public static bool Toggle(bool v, Texture label, string style, params GUILayoutOption[] options) { return GUILayout.Toggle(v, label, style, options); }
		/// <summary> Make an auto-layout toggle, in the given <paramref name="style"/> with the given <paramref name="label"/>, and the given <paramref name="options"/> </summary>
		/// <param name="v"> Current value of the toggle. </param>
		/// <param name="label"> Content to display. </param>
		/// <param name="style"> Style to display the toggle with. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> State of the toggle, potentially having changed if it was clicked. </returns>
		public static bool Toggle(bool v, GUIContent label, string style, params GUILayoutOption[] options) { return GUILayout.Toggle(v, label, style, options); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Sliders

		/// <summary> Make an auto layout slider, with the given <paramref name="left"/> and <paramref name="right"/> extents, and <paramref name="options"/>. </summary>
		/// <param name="value"> Current value of the slider. </param>
		/// <param name="left"> Left extent of the slider's value. </param>
		/// <param name="right"> Right extent of the slider's value. </param>
		/// <param name="options"> Options to change the display</param>
		/// <returns> The updated value of the slider, potentially having changed if it was clicked/dragged. </returns>
		public static float HorizontalSlider(float value, float left, float right, params GUILayoutOption[] options) { return GUILayout.HorizontalSlider(value, left, right, options); }

		/// <summary> Make an auto layout slider, with the given <paramref name="left"/> and <paramref name="right"/> extents, and <paramref name="options"/>. Calls the given <paramref name="callback"/> if the value changes. </summary>
		/// <param name="value"> Current value of the slider. </param>
		/// <param name="left"> Left extent of the slider's value. </param>
		/// <param name="right"> Right extent of the slider's value. </param>
		/// <param name="options"> Options to change the display</param>
		/// <returns> The updated value of the slider, potentially having changed if it was clicked/dragged. </returns>
		public static float HorizontalSlider(float value, Action<float> callback, float left, float right, params GUILayoutOption[] options) {
			float val = GUILayout.HorizontalSlider(value, left, right, options);
			if (val != value) { callback(val); }
			return val;
		}

		/// <summary> Make an auto layout slider, with the given <paramref name="bottom"/> and <paramref name="top"/> extents, and <paramref name="options"/>. </summary>
		/// <param name="value"> Current value of the slider. </param>
		/// <param name="bottom"> Bottom extent of the slider's value. </param>
		/// <param name="top"> Top extent of the slider's value. </param>
		/// <param name="options"> Options to change the display</param>
		/// <returns> The updated value of the slider, potentially having changed if it was clicked/dragged. </returns>
		public static float VerticalSlider(float value, float bottom, float top, params GUILayoutOption[] options) { return GUILayout.VerticalSlider(value, bottom, top, options); }

		/// <summary> Make an auto layout slider, with the given <paramref name="bottom"/> and <paramref name="top"/> extents, and <paramref name="options"/>. Calls the given <paramref name="callback"/> if the value changes. </summary>
		/// <param name="value"> Current value of the slider. </param>
		/// <param name="bottom"> Left extent of the slider's value. </param>
		/// <param name="top"> Right extent of the slider's value. </param>
		/// <param name="options"> Options to change the display</param>
		/// <returns> The updated value of the slider, potentially having changed if it was clicked/dragged. </returns>
		public static float VerticalSlider(float value, Action<float> callback, float bottom, float top, params GUILayoutOption[] options) {
			float val = GUILayout.VerticalSlider(value, bottom, top, options);
			if (val != value) { callback(val); }
			return val;
		}

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Fields

		/// <summary> Make an auto layout text field, displaying <paramref name="text"/> with given <paramref name="options"/>. </summary>
		/// <param name="text"> Text to display. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> Updated value of <paramref name="text"/>. </returns>
		public static string TextField(string text, params GUILayoutOption[] options) { return GUILayout.TextField(text, options); }
		/// <summary> Make an auto layout password-text field, displaying <paramref name="mask"/>ed <paramref name="text"/> with given <paramref name="options"/>. </summary>
		/// <param name="text"> Text to display. </param>
		/// <param name="mask"> Character to use to mask the content of the textbox </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> Updated value of <paramref name="text"/>. </returns>
		public static string PasswordField(string s, char mask = '*', params GUILayoutOption[] options) { return GUILayout.PasswordField(s, mask, options); }
		
		/// <summary> Make an auto layout text field, displaying a float <paramref name="value"/>, with given <paramref name="options"/>. </summary>
		/// <param name="value"> Value to display. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> Updated <paramref name="value"/>, based on user input. </returns>
		public static float FloatField(float value, params GUILayoutOption[] options) {
			float val = value;
			string str = TextField("" + val, options);
			try { val = str.ParseFloat(); } 
			catch { return value; }
			return val;
		}

		/// <summary> Make an auto layout text field, displaying an integer <paramref name="value"/>, with given <paramref name="options"/>. </summary>
		/// <param name="value"> Value to display. </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> Updated <paramref name="value"/>, based on user input. </returns>
		public static int IntField(int value, params GUILayoutOption[] options) {
			int val = value;
			string str = TextField("" + val, options);
			try { val = str.ParseInt(); } catch { return value; }
			return val;
		}

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// Layout Stuff

		/// <summary> Generate a flexible (auto-expanding) space </summary>
		public static void FlexibleSpace() { GUILayout.FlexibleSpace(); }
		/// <summary> Generate a fixed space. </summary>
		/// <param name="size"> Pixels for the space. </param>
		public static void Space(float size) { GUILayout.Space(size); }

		/// <summary> Begin a ScrollView area, with the given scroll <paramref name="pos"/>, and with the given <paramref name="options"/>. </summary>
		/// <param name="pos"> Current Scroll Position </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> Updated Scroll Position, potentially having changed if the user scrolled. </returns>
		/// <remarks> Must be matched with a <see cref="EndScrollView"/> call. </remarks>
		public static Vector2 BeginScrollView(Vector2 pos, params GUILayoutOption[] options) { return GUILayout.BeginScrollView(pos, options); }
		/// <summary> 
		///		<para> Begin a ScrollView area, with the given scroll <paramref name="pos"/>, and with the given <paramref name="options"/>. </para>
		///		<para> Can disable the <paramref name="h"/> and <paramref name="v"/> scrollbars if they are not needed. They will still show up depending on the size of the internal content. </para>
		///	</summary>
		/// <param name="pos"> Current Scroll Position </param>
		/// <param name="h"> Is the horizontal scroll bar always present? </param>
		/// <param name="v"> Is the vertical scroll bar always present? </param>
		/// <param name="options"> Options to change the display </param>
		/// <returns> Updated Scroll Position, potentially having changed if the user scrolled. </returns>
		/// <remarks> Must be matched with a <see cref="EndScrollView"/> call. </remarks>
		public static Vector2 BeginScrollView(Vector2 pos, bool h, bool v, params GUILayoutOption[] options) { return GUILayout.BeginScrollView(pos, h, v, options); }
		/// <summary> Stop drawing inside of a scroll view, and return to the previous context. </summary>
		/// <remarks> Must match one of the <see cref="BeginScrollView"/> method calls. </remarks>
		public static void EndScrollView() { GUILayout.EndScrollView(); }

		/// <summary> Begin layouting inside of a nested Area.</summary>
		/// <param name="area"> Area in the current context to nest inside of. </param>
		/// <remarks> Must be matched with a <see cref="EndArea"/> call. </remarks>
		public static void BeginArea(Rect area) { GUILayout.BeginArea(area); }
		/// <summary> Begin layouting inside of a nested Area, with a given <paramref name="style"/></summary>
		/// <param name="area"> Area in the current context to nest inside of. </param>
		/// <param name="style"> Name of style to use in the current GUISkin. </param>
		/// <remarks> Must be matched with a <see cref="EndArea"/> call. </remarks>
		public static void BeginArea(Rect area, string style) { GUILayout.BeginArea(area, style); }
		/// <summary> End drawing the given nested area, and return to the previous context. </summary>
		/// <remarks> Must match one of the <see cref="BeginArea"/> method calls. </remarks>
		public static void EndArea() { GUILayout.EndArea(); }

		/// <summary> Start laying out controls vertically. </summary>
		/// <param name="options"> Options to change the display. </param>
		/// <remarks> Must be matched with a call to <see cref="EndVertical"/>. </remarks>
		public static void BeginVertical(params GUILayoutOption[] options) { GUILayout.BeginVertical(options); }
		/// <summary> Start laying out controls vertically. </summary>
		/// <param name="style"> Name of style to use for this area (eg, box, label, button etc) </param>
		/// <param name="options"> Options to change the display. </param>
		/// <remarks> Must be matched with a call to <see cref="EndVertical"/>. </remarks>
		public static void BeginVertical(string style, params GUILayoutOption[] options) { GUILayout.BeginVertical(style, options); }
		/// <summary> Stops laying out controls in a vertical area. </summary>
		/// <remarks> Must be matched with one of the calls to <see cref="BeginVertical"/></remarks>
		public static void EndVertical() { GUILayout.EndVertical(); }

		/// <summary> Start laying out controls horizontally. </summary>
		/// <param name="options"> Options to change the display. </param>
		/// <remarks> Must be matched with a call to <see cref="EndHorizontal"/>. </remarks>
		public static void BeginHorizontal(params GUILayoutOption[] options) { GUILayout.BeginHorizontal(options); }
		/// <summary> Start laying out controls horizontally. </summary>
		/// <param name="style"> Name of style to use for this area (eg, box, label, button etc) </param>
		/// <param name="options"> Options to change the display. </param>
		/// <remarks> Must be matched with a call to <see cref="EndHorizontal"/>. </remarks>
		public static void BeginHorizontal(string style, params GUILayoutOption[] options) { GUILayout.BeginHorizontal(style, options); }
		/// <remarks> Must be matched with one of the calls to <see cref="BeginHorizontal"/></remarks>
		public static void EndHorizontal() { GUILayout.EndHorizontal(); }

		//////////////////////////////////
		/////////////////////////////////
		////////////////////////////////
		// GUILayoutOption Wrappers

		/// <summary> Create a Height Option </summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption Height(float size) { return GUILayout.Height(size); }
		/// <summary> Create a MinHeight Option </summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption MinHeight(float val) { return GUILayout.MinHeight(val); }
		/// <summary> Create a MaxHeight Option </summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption MaxHeight(float val) { return GUILayout.MaxHeight(val); }
		/// <summary> Create an ExpandHeight Option </summary>
		/// <param name="expand"> If the control is allowed to expand, or not. </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption ExpandHeight(bool expand) { return GUILayout.ExpandHeight(expand); }


		/// <summary> Create a Width Option </summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption Width(float size) { return GUILayout.Width(size); }
		/// <summary> Create a MinWidth Option </summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption MinWidth(float val) { return GUILayout.MinWidth(val); }
		/// <summary> Create a MaxWidth Option </summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption MaxWidth(float val) { return GUILayout.MaxWidth(val); }
		/// <summary> Create an ExpandHeight Option </summary>
		/// <param name="expand"> If the control is allowed to expand, or not. </param>
		/// <returns> The representitive GUILayoutOption object </returns>
		public static GUILayoutOption ExpandWidth(bool expand) { return GUILayout.ExpandWidth(expand); }

		/// <summary> Create an array for a fixed width (Width of size, and not allowed to expand)</summary>
		/// <param name="size"> Size in pixels </param>
		/// <returns> Array of two GUILayoutOptions. </returns>
		public static GUILayoutOption[] FixedWidth(float size) { return new GUILayoutOption[] { Width(size), ExpandWidth(false) }; }

		#endregion

		#endregion


	}
}
