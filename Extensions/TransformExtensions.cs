using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelUpper.Extensions {

	public static class TransformUtils {
	
		///<summary>
		///Copys all transform information from source and its children into dest. 
		///If objects do not exist in source, but exist in dest, they do not change their local positions.
		///</summary>
		public static void CopyLocalPositionsFrom(this Transform dest, Transform source) {
			Transform[] targets = dest.GetComponentsInChildren<Transform>();
			foreach (Transform target in targets) {
				string path = target.GetRelativePath(dest);
				Transform found = source.transform.Find(path);
				if (found != null) {
					target.localPosition = found.localPosition;
					target.localRotation = found.localRotation;
				}
			}
		
		}
	
		/// <summary> Removes all children from a given component </summary>
		/// <param name="c">Object to remove all children from underneath</param>
		public static void DeleteAllChildren(this Transform c) { 
			foreach (var child in c.GetChildren()) {
				GameObject.Destroy(child.gameObject);
			}
		}

		///<summary>
		///Get the relative path from someParent to t.
		///If someParent is not a parent of t (or is null), then it gets the path from the scene root
		///</summary>
		public static string GetRelativePath(this Transform t, Transform someParent = null) {
			if (t == null) { return ""; }
			if (t.parent == null || t.parent == someParent) { return t.gameObject.name; }
			return GetRelativePath(t.parent, someParent) + "/" + t.gameObject.name;
		}
		
	
		public static Vector3 DirectionTo(this Transform c, Vector3 position) { return position - c.position; }
		public static Vector3 DirectionTo(this Transform c, Transform other) { return other.position - c.position; }
	
		public static float DistanceTo(this Transform c, Vector3 position) { return c.DirectionTo(position).magnitude; }
		public static float DistanceTo(this Transform c, Transform other) { return c.DirectionTo(other).magnitude; }
		public static float FlatDistanceTo(this Transform c, Transform other) { 
			Vector3 dir = c.DirectionTo(other);
			dir.y = 0;
			return dir.magnitude;
		}
	
		/// <summary> Alphabetically sorts a transform's children. </summary>
		/// <param name="root"> Object to sort </param>
		public static void SortChildrenByName(this Transform root) {
			Dictionary<string, List<Transform>> children = new Dictionary<string, List<Transform>>();
			List<string> nameList = new List<string>();
		
			foreach (Transform t in root.GetChildren()) { 
				string name = t.gameObject.name;
				if (!nameList.Contains(name)) { nameList.Add(name); }
				if (children.ContainsKey(name)) {
					children[name].Add(t);
				} else {
					List<Transform> newList = new List<Transform>();
					newList.Add(t);
					children.Add(name, newList);
				}
			}
		
			nameList.Sort();
		
			int i = 0;
			foreach (string name in nameList) {
				List<Transform> transforms = children[name];
				foreach (Transform t in transforms) {
					t.SetSiblingIndex(i++);
				}
			
			}
		
		
		}

		///<summary>
		///Positions two objects ontop of eachother, by moving the parent.
		///
		///Snaps the position/rotation of the parent of the first component 
		///to line the first component up to the second component
		///Makes c face along the same vector as other.
		///with the flip option, they will face in opposite directions.
		///</summary>
		/// <param name="c"> Transform to position. The parent of this will be moved. </param>
		/// <param name="other"> Transform to snap to </param>
		/// <param name="flip"> Make c and other face opposite directions (true) or same direction (false) </param>
		public static void SnapParent(this Transform c, Transform other, bool flip = false) {
			Transform t = c;
			Transform o = other;
		
			Transform p = t.parent;
			Quaternion q = t.rotation.To(o.rotation);
		
			p.rotation *= q;
			if (flip) { p.Rotate(0, 180, 0); }
		
			p.position = o.position;
			p.position -= (t.position - p.position);
		
		
		}
	
		/// <summary> Returns an array of all of the first-level children of a given transform. </summary>
		public static Transform[] GetChildren(this Transform t) {
			int num = t.childCount;
			Transform[] list = new Transform[num];
			for (int i = 0; i < num; i++) {
				list[i] = t.GetChild(i);
			}
			return list;
		}
	
		public static void FlattenRotationZ(this Transform c) { c.FlattenZ(); }
		public static void FlattenZ(this Transform t) {
			Quaternion q = t.rotation;
			Vector3 v = q.eulerAngles;
			v.z = 0;
			q.eulerAngles = v;
			t.rotation = q;
		}
	
	
	}
}
