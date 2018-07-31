using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
[CustomEditor(typeof(ValueMap))]
public class ValueMapEditor : Editor {
	private ReorderableList list;

	static int baseWidth = 110;
	static int padding = 2;

	void OnEnable() {
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("properties"), true, true, true, true);

		list.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "ValueMap Properties");
		};
		
		list.elementHeightCallback = (index) => {
			return EditorGUIUtility.singleLineHeight + padding * 2;
		};

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += padding;
			float lineHeight = EditorGUIUtility.singleLineHeight;

			var keyProp = element.FindPropertyRelative("Key");
			var typeProp = element.FindPropertyRelative("Type");
			var type = (ValueMap.EntryType)typeProp.enumValueIndex;
			var dataProp = element.FindPropertyRelative(type + "Value");

			Rect keyRect = new Rect(rect.x, rect.y, baseWidth, lineHeight);
			Rect typeRect = new Rect(keyRect.xMax + padding, rect.y, baseWidth, lineHeight);
			Rect rest = new Rect(typeRect.xMax + padding, rect.y, rect.width - typeRect.xMax - padding, lineHeight);

			EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
			EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);
			EditorGUI.PropertyField(rest, dataProp, GUIContent.none);

		};

	}

	public override void OnInspectorGUI() {

		serializedObject.Update();

		list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();

	}

}

#endif



[CreateAssetMenu(fileName="NewValueMap", menuName="SOs/Create New ValueMap", order = 56)]
public class ValueMap : ScriptableObject, IDictionary<string, object> {
	private static readonly string[] EMPTY_STRINGS = { };
	private static readonly ReadOnlyCollection<string> EMPTY_STRING_COL = new ReadOnlyCollection<string>(EMPTY_STRINGS);
	private static readonly object[] EMPTY_VALS = { };
	private static readonly ReadOnlyCollection<object> EMPTY_VAL_COL = new ReadOnlyCollection<object>(EMPTY_VALS);

	public enum EntryType {
		String,
		Float,
		Int,
		Bool,
		Object,
	}

	[Serializable] public struct Entry {
		public string Key;

		public EntryType Type;
		public string StringValue;
		public float FloatValue;
		public int IntValue;
		public bool BoolValue;
		public UnityEngine.Object ObjectValue;

		public object Value {
			get {
				switch (Type) {
					case EntryType.String: return StringValue;
					case EntryType.Float: return FloatValue;
					case EntryType.Int: return IntValue;
					case EntryType.Bool: return BoolValue;
					case EntryType.Object: return ObjectValue;


					default: return null;
				}
				
			}
		}
	}
	
	public List<Entry> properties = new List<Entry>();
	private Dictionary<string, object> data;

	public ICollection<string> Keys { get { return ((ICollection<string>)data?.Keys) ?? EMPTY_STRING_COL; } }

	public ICollection<object> Values { get { return ((ICollection<object>)data?.Values) ?? EMPTY_VAL_COL; } }

	public int Count { get { return data != null ? data.Count : 0; } }

	public bool IsReadOnly { get { return data != null; } }

	public object this[string key] {
		get { return data != null ? data.ContainsKey(key) ? data[key] : null : null; }
		set { 
			if (data != null) {
				if (value != null) {
					data[key] = value; 
				} else {
					data.Remove(key);
				}
			} 
		}
	}

	public void Set(string key, string value) { this[key] = value; }
	public void Set(string key, float value) { this[key] = value; }
	public void Set(string key, int value) { this[key] = value; }
	public void Set(string key, bool value) { this[key] = value; }
	public void Set(string key, UnityEngine.Object value) { this[key] = value; }

	public void Set(ValueMap source) {
		foreach (var pair in source) {
			this[pair.Key] = pair.Value;
		}
	}


	public T Pull<T>(string name, T defaultVal = default(T)) {
		if (data.ContainsKey(name)) {
			object val = data[name];
			if (val is T) {
				return (T)val;
			}
		}
		return defaultVal;
	}

	void OnEnable() {

		if (data == null) {
			data = new Dictionary<string, object>();

			if (properties != null) {
				foreach (var entry in properties) {
					data[entry.Key] = entry.Value;
				}
			}
		}
	}
	
	void OnDisable() {
		data = null;
	}
	public bool ContainsKey(string key) {
		return data != null ? data.ContainsKey(key) : false;
	}

	public void Add(string key, object value) {
		if (data != null) { this[key] = value; }
	}

	public bool Remove(string key) {
		return data != null ? data.Remove(key) : false;
	}

	public bool TryGetValue(string key, out object value) {
		if (data != null) { return data.TryGetValue(key, out value); }
		value = null;
		return false;
	}

	public void Add(KeyValuePair<string, object> item) {
		if (data != null) { data.Add(item.Key, item.Value); }
	}

	public void Clear() {
		if (data != null) { data.Clear(); }
	}

	public bool Contains(KeyValuePair<string, object> item) {
		if (data != null) {
			if (data.ContainsKey(item.Key)) { return data[item.Key] == item.Value; }
		}
		return item.Value == null;
	}

	public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
		int i = 0;
		foreach (var pair in data) {
			array[i] = pair;
			i++;
		}
	}

	public bool Remove(KeyValuePair<string, object> item) {
		if (Contains(item)) { return Remove(item.Key); }
		return false;
	}

	public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
		return data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return data.GetEnumerator();
	}
}
