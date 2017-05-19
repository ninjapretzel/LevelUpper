using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelUpper.Extensions {

	public static class RectExtensions {
		/// <summary> Rect at (0, 0) with size (1, 1) </summary>
		public static Rect unit { get { return new Rect(0, 0, 1, 1); } }

		/// <summary> Get the relative position of <paramref name="v"/> compared to <paramref name="r"/>. </summary>
		/// <param name="r"> Rect to check agains </param>
		/// <param name="v"> Vector2 to get coord of </param>
		/// <returns> Coordinate relative to the origin and size of <paramref name="r"/> </returns>
		/// <remarks>
		///		<para> This interprets the given vector in the space defined by a rectangle. </para>
		///		<para> The origin of the rectangle is (0, 0). </para>
		///		<para> The extent of the rectangle is (1, 1). </para>
		/// </remarks>
		public static Vector2 Coord(this Rect r, Vector2 v) {
			Vector2 d = v - r.TopLeft();
			d.x /= r.width;
			d.y /= r.height;
			return d;
		}

		/// <summary> Get the area on the screen that a given transform is on. </summary>
		/// <param name="t"> Transform to check </param>
		/// <param name="w"> Width of screen for result Rect </param>
		/// <param name="h"> Height of screen for result Rect </param>
		/// <returns> Rect centered around the transform, with size based on screen size and <paramref name="w"/>/<paramref name="h"/>. </returns>
		public static Rect ToScreenArea(this Transform t, float w, float h) {
			Vector3 pos = Camera.main.WorldToScreenPoint(t.position);
			pos.y = Screen.height - pos.y;
			float width = w * Screen.width;
			float height = h * Screen.height;
			return new Rect(pos.x - .5f * width, pos.y - .5f * height, width, height);
		}

		/// <summary> Get the screen point (x,y,depth) of a given transform. </summary>
		/// <param name="t"> Transform to get position of. </param>
		/// <returns> (x,y,depth) position on the screen of the given transform. </returns>
		/// <remarks> flips y so that 0 is the top of the screen, and Screen.height is the bottom </remarks>
		public static Vector3 ToScreenPosition(this Transform t) {
			Vector3 pos = Camera.main.WorldToScreenPoint(t.position);
			pos.y = Screen.height - pos.y;
			return pos;
		}
	
		/// <summary> Scales a Rectangle by the x/y components of a given vector. </summary>
		/// <param name="r"> Rect to scale </param>
		/// <param name="s"> Vector to scale by </param>
		/// <returns> Rectangle scaled component wise by the given vector </returns>
		public static Rect Scaled(this Rect r, Vector2 s) { return r.Scaled(s.x, s.y); }
		/// <summary> Scales a rectangle by the given x/y values </summary>
		/// <param name="r"> Rectangle to scale </param>
		/// <param name="x"> Scale for x/width </param>
		/// <param name="y"> Scale for y/height </param>
		/// <returns> Scaled version of the rectangle </returns>
		public static Rect Scaled(this Rect r, float x, float y) { return new Rect(r.x * x, r.y * y, r.width * x, r.height * y); }

		/// <summary> Denormalized version of a rectangle, respective to the screen size. </summary>
		/// <param name="r"> Rect to scale </param>
		/// <returns> <paramref name="r"/> scaled by (Screen.width, Screen.height) </returns>
		public static Rect Denormalized(this Rect r) { return r.Scaled(Screen.width, Screen.height); }
		/// <summary> Normalized version of a rectangle, respective to the screen size. </summary>
		/// <param name="r"> Rect to scale </param>
		/// <returns> <paramref name="r"/> scaled by (1/Screen.width, 1/Screen.height) </returns>
		public static Rect Normalized(this Rect r) { return r.Scaled(1f / Screen.width, 1f / Screen.height); }

		/// <summary> Rotates a given rectangle by 90 degrees. </summary>
		/// <param name="r"> Rectangle to rotate </param>
		/// <returns> <paramref name="r"/>, but the x/y and width/height are interchanged. </returns>
		public static Rect Rot90(this Rect r) { return new Rect(r.y, r.x, r.height, r.width); }


		public static Rect FlippedHorizontal(this Rect r) { return new Rect(Screen.width - r.xMax, r.y, r.width, r.height); }
		public static Rect FlippedVertical(this Rect r) { return new Rect(r.x, Screen.height - r.yMax, r.width, r.height); }
		public static Rect FlippedDiagonal(this Rect r) { return r.FlippedVertical().FlippedHorizontal(); }

		/// <summary> Generate a Rect from a given <paramref name="center"/>point and <paramref name="size"/></summary>
		/// <param name="center"> Center point of result </param>
		/// <param name="size"> Size (width/height) of result </param>
		/// <returns> Rect centered at <paramref name="center"/> with width/height <paramref name="size"/> </returns>
		public static Rect FromCenter(Vector2 center, Vector2 size) { return FromCenter(center, size.x, size.y); }
		/// <summary> Generate a Rect from a given <paramref name="center"/> point and width width/height of <paramref name="size"/> pixels. </summary>
		/// <param name="center"> Center point of result </param>
		/// <param name="size"> Size (both width/height) of result, in pixels. </param>
		/// <returns> Rect centered at <paramref name="center"/>, with width/height of <paramref name="size"/> pixels.</returns>
		public static Rect FromCenter(Vector2 center, float size) { return FromCenter(center, size, size); }
		/// <summary> Generate a Rect from a given <paramref name="center"/> point and width of <paramref name="w"/> and height of <paramref name="h"/>, in pixels. </summary>
		/// <param name="center"> Center point of result </param>
		/// <param name="w"> Width of result, in pixels </param>
		/// <param name="h"> Height of result, in pixels </param>
		/// <returns> Rect centered at <paramref name="center"/> with width <paramref name="w"/> and height <paramref name="h"/>, in pixels. </returns>
		public static Rect FromCenter(Vector2 center, float w, float h) { return new Rect(center.x - w / 2f, center.y - h / 2f, w, h); }
		/// <summary> Create a Rect representing a part of the whole screen, with a percent of its edges chopped off. </summary>
		/// <param name="normalized"> Normalized Rect of area. </param>
		/// <param name="overscan"> Percentage of edges of screen that get removed. </param>
		/// <returns> <paramref name="normalized"/> Scaled to the center region of the screen after <paramref name="overscan"/>% of screen width/height is removed </returns>
		public static Rect ScreenOverscan(Rect normalized, float overscan) {
			float ratio = Mathf.Clamp(overscan, 0, .05f);

			Rect overScreen = new Rect(0, 0, Screen.width, Screen.height);
			overScreen = overScreen.Trim(ratio * Screen.width, ratio * Screen.height);

			return new Rect(overScreen.x + overScreen.width * normalized.x,
							overScreen.y + overScreen.height * normalized.y,
							overScreen.width * normalized.width,
							overScreen.height * normalized.height);
		}

		/// <summary> Generate a modified Rect, shifted by pixels. </summary>
		/// <param name="r"> Rect to shift </param>
		/// <param name="v"> Distance to shift by </param>
		/// <returns> Shifted version of <paramref name="r"/> </returns>
		public static Rect Shift(this Rect r, Vector2 v) { return r.Shift(v.x, v.y); }
		/// <summary> Generate a modified Rect, shifted by pixels. </summary>
		/// <param name="r"> Rect to shift </param>
		/// <param name="x"> x-Distance to shift by </param>
		/// <param name="y"> y-Distance to shift by </param>
		/// <returns> Shifted version of <paramref name="r"/> </returns>
		public static Rect Shift(this Rect r, float x, float y) { return new Rect(r.x + x, r.y + y, r.width, r.height); }

		/// <summary> Generates a modified Rect, moved by a percentage of its width/height </summary>
		/// <param name="r"> Rect to move </param>
		/// <param name="v"> Percentages of width/height to move by </param>
		/// <returns> <paramref name="r"/> Moved by a percentage of its width/height </returns>
		public static Rect Move(this Rect r, Vector2 v) { return r.Move(v.x, v.y); }
		/// <summary> Generates a modified Rect, moved by a percentage of its width/height </summary>
		/// <param name="r"> Rect to move </param>
		/// <param name="x"> Percentage of width to move by </param>
		/// <param name="y"> Percentage of height to move by </param>
		/// <returns> <paramref name="r"/>, Moved by a percentage of its width/height </returns>
		public static Rect Move(this Rect r, float x, float y) { return new Rect(r.x + x * r.width, r.y + y * r.height, r.width, r.height); }

		/// <summary> Shrink <paramref name="r"/> so it could fit within <paramref name="t"/> </summary>
		/// <param name="r"> Rectangle to shrink </param>
		/// <param name="t"> Maximum size of given rectangle </param>
		/// <returns> <paramref name="r"/>, shrunk so it is at maximum size of <paramref name="t"/> </returns>
		public static Rect ShrinkTo(this Rect r, Rect t) {
			if (r.width < t.width && r.height < t.height) { return r; }
			float hRatio = t.height / r.height;
			float wRatio = t.width / r.width;
			if (hRatio < wRatio) {
				return FromCenter(r.MiddleCenter(), hRatio * r.width, hRatio * r.height);
			} else {
				return FromCenter(r.MiddleCenter(), wRatio * r.width, wRatio * r.height);
			}
		}

		/// <summary> Get the aspect ratio for a given Rect </summary>
		/// <param name="r"> Rect to get aspect of </param>
		/// <returns> width/height of <paramref name="r"/> </returns>
		public static float Aspect(this Rect r) { return r.width / r.height; }
	
		//Resize a rect's width based on its existant height
		/// <summary> Generate a modified Rect, Resized based on height </summary>
		/// <param name="r"> Rect to modify  </param>
		/// <param name="ratio"> Ratio of height for the result's width </param>
		/// <returns> A rectangle with the same center, but with width of <paramref name="ratio"/> * <paramref name="r"/>.height </returns>
		public static Rect Aspect(this Rect r, float ratio) { return FromCenter(r.center, r.height * ratio, r.height); }

		/// <summary> Generates Rect moved by (-1, 0) </summary>
		/// <param name="r"> Rect to move </param>
		/// <returns> Modified version of <paramref name="r"/> </returns>
		public static Rect MoveLeft(this Rect r) { return r.MoveLeft(1); }
		/// <summary> Generates Rect moved by (-<paramref name="val"/>, 0) </summary>
		/// <param name="r"> Rect to move </param>
		/// <param name="val"> Percentage of width to move left by </param>
		/// <returns> Modified version of <paramref name="r"/> </returns>
		public static Rect MoveLeft(this Rect r, float val) { return r.Move(-val, 0); }

		/// <summary> Generates Rect moved by (1, 0) </summary>
		/// <param name="r"> Rect to move </param>
		/// <returns> Modified version of <paramref name="r"/> </return>
		public static Rect MoveRight(this Rect r) { return r.MoveRight(1); }
		/// <summary> Generates Rect moved by (<paramref name="val"/>, 0) </summary>
		/// <param name="r"> Rect to move </param>
		/// <param name="val"> Percentage of width to move right by </param>
		/// <returns> Modified version of <paramref name="r"/> </returns>
		public static Rect MoveRight(this Rect r, float val) { return r.Move(val, 0); }

		/// <summary> Generates Rect moved by (0, -1) </summary>
		/// <param name="r"> Rect to move </param>
		/// <returns> Modified version of <paramref name="r"/> </return>
		public static Rect MoveUp(this Rect r) { return r.MoveUp(1); }
		/// <summary> Generates Rect moved by (0, -<paramref name="val"/>) </summary>
		/// <param name="r"> Rect to move </param>
		/// <param name="val"> Percentage of height to move up by </param>
		/// <returns> Modified version of <paramref name="r"/> </returns>
		public static Rect MoveUp(this Rect r, float val) { return r.Move(0, -val); }

		/// <summary> Generates Rect moved by (0, 1) </summary>
		/// <param name="r"> Rect to move </param>
		/// <returns> Modified version of <paramref name="r"/> </return>
		public static Rect MoveDown(this Rect r) { return r.MoveDown(1); }
		/// <summary> Generates Rect moved by (0, <paramref name="val"/>) </summary>
		/// <param name="r"> Rect to move </param>
		/// <param name="val"> Percentage of height to move down by </param>
		/// <returns> Modified version of <paramref name="r"/> </returns>
		public static Rect MoveDown(this Rect r, float val) { return r.Move(0, val); }

		/// <summary> Generate a modified Rect with <paramref name="p"/> pixels added to each side. </summary>
		/// <param name="r"> Rect to modify </param>
		/// <param name="p"> Number of pixels to pad each edge by </param>
		/// <returns> Modified Rect with a slightly larger width/height </returns>
		public static Rect Pad(this Rect r, float p = 2) { return r.Pad(p, p); }
		/// <summary> Generate modified Rect with some pixels added to each side </summary>
		/// <param name="r"> Rect to modify </param>
		/// <param name="v"> Pixels to add to (left/right, top/bottom) sides</param>
		/// <returns> Modified Rect with a slightly larger width/height </returns>
		public static Rect Pad(this Rect r, Vector2 v) { return r.Pad(v.x, v.y); }
		/// <summary> Generates a modified Rect with <paramref name="x"/> pixels added to left/right edges and <paramref name="y"/> pixels added to top/bottom edges </summary>
		/// <param name="r"> Rect to modify </param>
		/// <param name="x"> Pixels to pad left/right edges with </param>
		/// <param name="y"> Pixels to pad top/bottom edges with </param>
		/// <returns> Modified Rect with a slightly larger width/height </returns>
		public static Rect Pad(this Rect r, float x, float y) { return new Rect(r.x - x, r.y - y, r.width + 2 * x, r.height + 2 * y); }

		/// <summary> Generates a modified Rect with <paramref name="p"/> pixels removed from every side. </summary>
		/// <param name="r"> Rect to modify </param>
		/// <param name="p"> Number of pixels to remove from each side </param>
		/// <returns> Modified Rect with a slightly smaller width/height </returns>
		public static Rect Trim(this Rect r, float p) { return r.Trim(p, p); }
		/// <summary> Generates a modified Rect with some pixels removed from each side </summary>
		/// <param name="r"> Rect to modify </param>
		/// <param name="v"> Pixels to remove from (left/right, top/bottom) sides </param>
		/// <returns> Modified Rect with a slightly smaller width/height </returns>
		public static Rect Trim(this Rect r, Vector2 v) { return r.Trim(v.x, v.y); }
		/// <summary> Generates a modified Rect with <paramref name="x"/> pixels removed from left/right edges and <paramref name="y"/> pixels removed from top/bottom edges </summary>
		/// <param name="r"> Rect to modify </param>
		/// <param name="x"> Pixels to remove from left/right edges </param>
		/// <param name="y"> Pixels to remove from top/bottom edges </param>
		/// <returns> Modified Rect with a slightly smaller size </returns>
		public static Rect Trim(this Rect r, float x, float y) { return new Rect(r.x + x, r.y + y, r.width - 2 * x, r.height - 2 * y); }

		/// <summary> Get a point based on the space defined by a Rect. </summary>
		/// <param name="r"> Rect that defines the space </param>
		/// <param name="v"> Normalized point to get </param>
		/// <returns> Point at position <paramref name="v"/> relative to <paramref name="r"/> </returns>
		/// <remarks> Shorthand for (r.x + r.width * v.x, r.y + r.height * v.y) </remarks>
		public static Vector2 Point(this Rect r, Vector2 v) { return r.Point(v.x, v.y); }
		/// <summary> Get a point based on the space defined by a Rect. </summary>
		/// <param name="r"> Rect that defines the space </param>
		///	<param name="x"> X-position relative to <paramref name="r"/> </param>
		///	<param name="y"> Y-position relative to <paramref name="r"/> </param>
		/// <returns> Point at position (<paramref name="x"/>,<paramref name="y"/>) relative to <paramref name="r"/> </returns>
		/// <remarks> Shorthand for (r.x + r.width * x, r.y + r.height * y) </remarks>
		public static Vector2 Point(this Rect r, float x, float y) { return new Vector2(r.x + r.width * x, r.y + r.height * y); }
	
		/// <summary> Create a Rect from two Vector2s </summary>
		/// <param name="pos"> Position of top-left corner </param>
		/// <param name="size"> Width/height of result </param>
		/// <returns> Rect created at <paramref name="pos"/> with size <paramref name="size"/> </returns>
		public static Rect Craft(Vector2 pos, Vector2 size) { return new Rect(pos.x, pos.y, size.x, size.y); }

		/// <summary> Create a Rect centered at <paramref name="pos"/> with size <paramref name="size"/> </summary>
		/// <param name="pos"> Center position of result </param>
		/// <param name="size"> Size of result </param>
		/// <returns> Rect centered on <paramref name="pos"/> with size <paramref name="size"/> </returns>
		public static Rect Centered(Vector2 pos, Vector2 size) { return new Rect(pos.x - size.x/2f, pos.y -size.y/2f, size.x, size.y); }

		/// <summary> Get the area of the top percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to <paramref name="p"/>% down from the edge top of <paramref name="r"/> </returns>
		public static Rect Top(this Rect r, float p) { return r.TopLeft(1, p); }
		/// <summary> Get the area of the top percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to <paramref name="p"/>% down from the edge top of <paramref name="r"/> </returns>
		public static Rect Upper(this Rect r, float p) { return r.TopLeft(1, p); }
		/// <summary> Get the area of the middle percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to (<paramref name="p"/>/2)% up/down from the middle of <paramref name="r"/> </returns>
		public static Rect Middle(this Rect r, float p) { return r.MiddleLeft(1, p); }
		/// <summary> Get the area of the bottom percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to <paramref name="p"/>% up from the bottom edge of <paramref name="r"/> </returns>
		public static Rect Bottom(this Rect r, float p) { return r.BottomLeft(1, p); }
		/// <summary> Get the area of the bottom percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to <paramref name="p"/>% up from the bottom edge of <paramref name="r"/> </returns>
		public static Rect Lower(this Rect r, float p) { return r.BottomLeft(1, p); }

		/// <summary> Get the area of the left percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to <paramref name="p"/>% right from the left edge of <paramref name="r"/> </returns>
		public static Rect Left(this Rect r, float p) { return r.TopLeft(p, 1); }
		/// <summary> Get the area of the center percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to (<paramref name="p"/>/2)% left/right from the center of <paramref name="r"/> </returns>
		public static Rect Center(this Rect r, float p) { return r.TopCenter(p, 1); }
		/// <summary> Get the area of the right percentage from a Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="p"> Percentage of area to get </param>
		/// <returns> Rect of area up to <paramref name="p"/>% left from the right edge of <paramref name="r"/> </returns>
		public static Rect Right(this Rect r, float p) { return r.TopRight(p, 1); }
	
		/// <summary> Get the upper-left point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of upper-left corner </returns>
		public static Vector2 TopLeft(this Rect r) { return new Vector2(r.x, r.y); }
		/// <summary> Get an upper-left region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Upper left region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect TopLeft(this Rect r, Vector2 v) { return r.TopLeft(v.x, v.y); }
		/// <summary> Get an upper-left region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Upper left region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect TopLeft(this Rect r, float width, float height) { return new Rect(r.x, r.y, width * r.width, height * r.height); }

		/// <summary> Get the upper-center point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of upper-center point </returns>
		public static Vector2 TopCenter(this Rect r) { return r.Point(.5f, 0); }
		/// <summary> Get an upper-center region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Upper center region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect TopCenter(this Rect r, Vector2 v) { return r.TopCenter(v.x, v.y); }
		/// <summary> Get an upper-center region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Upper center region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect TopCenter(this Rect r, float width, float height) { 
			return new Rect(r.x + r.width / 2.0f - r.width * width / 2.0f, r.y, r.width * width, r.height * height); 
		}

		/// <summary> Get the upper-right point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of upper-right point </returns>
		public static Vector2 TopRight(this Rect r) { return r.Point(1, 0); }
		/// <summary> Get an upper-right region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Upper right region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect TopRight(this Rect r, Vector2 v) { return r.TopRight(v.x, v.y); }
		/// <summary> Get an upper-right  region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Upper right region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect TopRight(this Rect r, float width, float height) {
			return new Rect(r.x + r.width - r.width * width, r.y, r.width * width, r.height * height);
		}

		/// <summary> Get the middle-left point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of middle-left point </returns>
		public static Vector2 MiddleLeft(this Rect r) { return r.Point(0, .5f); }
		/// <summary> Get an middle-left region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Middle left region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect MiddleLeft(this Rect r, Vector2 v) { return r.MiddleLeft(v.x, v.y); }
		/// <summary> Get an middle-left region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Middle left region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect MiddleLeft(this Rect r, float width, float height) {
			return new Rect(r.x, r.y + r.height / 2.0f - r.height * height / 2.0f, r.width * width, r.height * height);
		}

		/// <summary> Get the middle-center point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of middle-center point </returns>
		public static Vector2 MiddleCenter(this Rect r) { return r.Point(.5f, .5f); }
		/// <summary> Get an middle-center region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Middle center region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect MiddleCenter(this Rect r, Vector2 v) { return r.MiddleCenter(v.x, v.y); }
		/// <summary> Get an middle-center region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Middle center region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect MiddleCenter(this Rect r, float width, float height) {
			return new Rect(r.x + r.width / 2.0f - r.width * width / 2.0f, r.y + r.height / 2.0f - r.height * height / 2.0f, r.width * width, r.height * height);
		}

		/// <summary> Get the middle-right point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of middle-right point </returns>
		public static Vector2 MiddleRight(this Rect r) { return r.Point(1, .5f); }
		/// <summary> Get an middle-right region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Middle right region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect MiddleRight(this Rect r, Vector2 v) { return r.MiddleRight(v.x, v.y); }
		/// <summary> Get an middle-right region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Middle right region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect MiddleRight(this Rect r, float width, float height) {
			return new Rect(r.x + r.width - r.width * width, r.y + r.height / 2.0f - r.height * height / 2.0f, r.width * width, r.height * height);
		}

		/// <summary> Get the bottom-left point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of bottom-left point </returns>
		public static Vector2 BottomLeft(this Rect r) { return r.Point(0, 1); }
		/// <summary> Get an bottom-left region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Bottom left region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect BottomLeft(this Rect r, Vector2 v) { return r.BottomLeft(v.x, v.y); }
		/// <summary> Get an bottom-left region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Bottom left region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect BottomLeft(this Rect r, float width, float height) {
			return new Rect(r.x, r.y + r.height - r.height * height, r.width * width, r.height * height);
		}

		/// <summary> Get the bottom-center point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of bottom-center point </returns>
		public static Vector2 BottomCenter(this Rect r) { return r.Point(.5f, 1); }
		/// <summary> Get an bottom-center region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Bottom center region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect BottomCenter(this Rect r, Vector2 v) { return r.BottomCenter(v.x, v.y); }
		/// <summary> Get an bottom-center region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Bottom center region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect BottomCenter(this Rect r, float width, float height) {
			return new Rect(r.x + r.width / 2.0f - r.width * width / 2.0f,
							r.y + r.height - r.height * height,
							r.width * width,
							r.height * height);
		}

		/// <summary> Get the bottom-right point of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <returns> Pixel position of bottom-right point </returns>
		public static Vector2 BottomRight(this Rect r) { return r.Point(1, 1); }
		/// <summary> Get an bottom-right region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="v"> X/Y percentage of region to get </param>
		/// <returns> Bottom right region of <paramref name="r"/>, with size based on <paramref name="v"/>'s X and Y components  </returns>
		public static Rect BottomRight(this Rect r, Vector2 v) { return r.BottomRight(v.x, v.y); }
		/// <summary> Get an bottom-right region of a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="width"> X Percentage of region to get </param>
		/// <param name="height"> Y Percentage of region to get </param>
		/// <returns> Bottom right region of <paramref name="r"/>, with size based on <paramref name="height"/> and <paramref name="width"/>. </returns>
		public static Rect BottomRight(this Rect r, float width, float height) { 
			return new Rect(r.x + r.width - r.width * width, r.y + r.height - r.height * height, r.width * width, r.height * height); 
		}

		/// <summary> Get the upper left 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect UpperLeftSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x, r.y, sz, sz); 
		}
		/// <summary> Get the upper center 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect UpperCenterSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x + r.width / 2.0f - sz / 2.0f, r.y, sz, sz);
		}
		/// <summary> Get the upper right 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect UpperRightSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x + r.width - sz, r.y, sz, sz);
		}

		/// <summary> Get the middle left 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect MiddleLeftSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x, r.y + r.height / 2.0f - sz / 2.0f, sz, sz);
		}

		/// <summary> Get the middle center 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect MiddleCenterSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x + r.width / 2.0f - sz / 2.0f, r.y + r.height / 2.0f - sz / 2.0f, sz, sz);
		}

		/// <summary> Get the middle right 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect MiddleRightSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x + r.width - sz, r.y + r.height / 2.0f - sz / 2.0f, sz, sz);
		}

		/// <summary> Get the bottom left 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect BottomLeftSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x, r.y + r.height - sz, sz, sz);
		}

		/// <summary> Get the bottom center 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect BottomCenterSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x + r.width / 2.0f - sz / 2.0f, r.y + r.height - sz, sz, sz);
		}

		/// <summary> Get the bottom right 'square' region from a given Rect </summary>
		/// <param name="r"> Rect to get from </param>
		/// <param name="size"> Percentage of side length that becomes the width/height for the result </param>
		/// <param name="useWidth"> true to use width, false to use height. Default is false (height)  </param>
		/// <returns> Rect with square dimensions, based on a percentage of the height or width of <paramref name="r"/> </returns>
		public static Rect BottomRightSquare(this Rect r, float size, bool useWidth = false) {
			float sz = (useWidth ? r.width : r.height) * size;
			return new Rect(r.x + r.width - sz, r.y + r.height - sz, sz, sz);
		}



	}
}
