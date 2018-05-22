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
		RefreshList();

	}
	void RefreshList() {
		string propertyName = EditorApplication.isPlaying ?
			"runtimeList" : "list";

		list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName), true, true, true, true);

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
		
		string propertyName = EditorApplication.isPlayingOrWillChangePlaymode ?
			"runtimeList" : "list";

		GUILayout.Label("Showing " + propertyName);
		ValueMap valueMap = (ValueMap) target;
		if (EditorApplication.isPlaying && valueMap.dirtyList) {
			valueMap.RebuildRuntimeList();
		}

		serializedObject.Update();

		EditorGUI.BeginChangeCheck();
		list.DoLayoutList();
		
		serializedObject.ApplyModifiedProperties();
		if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying) {
			valueMap.RebuildRuntimeDictionary();
		}

		

	}

}

#endif



[CreateAssetMenu(fileName="NewValueMap", menuName="SOs/Create New ValueMap", order = 9000)]
public class ValueMap : ScriptableObject, IDictionary<string, object> {
	private static readonly string[] EMPTY_KEYS = { };
	private static readonly ReadOnlyCollection<string> EMPTY_KEYS_COL = new ReadOnlyCollection<string>(EMPTY_KEYS);
	private static readonly object[] EMPTY_VALS = { };
	private static readonly ReadOnlyCollection<object> EMPTY_VALS_COL = new ReadOnlyCollection<object>(EMPTY_VALS);

	public enum EntryType {
		String,
		Float,
		Int,
		Bool,
		Object,
	}

	[Serializable] public struct Entry {
		public string Key;

		public Entry(string key, string val) {
			StringValue = null; FloatValue = 0; IntValue = 0; BoolValue = false; ObjectValue = null;
			Key = key; StringValue = val;
			Type = EntryType.String;
		}
		public Entry (string key, float val) {
			StringValue = null; FloatValue = 0; IntValue = 0; BoolValue = false; ObjectValue = null;
			Key = key; FloatValue = val;
			Type = EntryType.Float;
		}
		public Entry(string key, int val) {
			StringValue = null; FloatValue = 0; IntValue = 0; BoolValue = false; ObjectValue = null;
			Key = key; IntValue = val;
			Type = EntryType.Int;
		}
		public Entry(string key, bool val) {
			StringValue = null; FloatValue = 0; IntValue = 0; BoolValue = false; ObjectValue = null;
			Key = key; BoolValue = val;
			Type = EntryType.Bool;
		}
		public Entry(string key, UnityEngine.Object val) {
			StringValue = null; FloatValue = 0; IntValue = 0; BoolValue = false; ObjectValue = null;
			Key = key; ObjectValue = val;
			Type = EntryType.Object;
		}
		public Entry(string key, object val) {
			StringValue = null; FloatValue = 0; IntValue = 0; BoolValue = false; ObjectValue = null;
			Key = key;

			if (val is UnityEngine.Object) { ObjectValue = (UnityEngine.Object)val; Type = EntryType.Object; }
			else if (val is int) { IntValue = (int) val; Type = EntryType.Int; }
			else if (val is float) { FloatValue = (float)val; Type = EntryType.Float; }
			else if (val is bool) { BoolValue = (bool)val; Type = EntryType.Bool; }
			else if (val is string) { StringValue = (string)val; Type = EntryType.String; }
			else { Type = EntryType.Object; } // Default to nulls
		}

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
	
	public List<Entry> list = new List<Entry>();
	[SerializeField] private List<Entry> runtimeList;
	[SerializeField] private bool _dirtyList;
	public bool dirtyList { get { return _dirtyList; } }
	[SerializeField] private bool _dirtyDict = false;
	public bool dirtyDict { get { return _dirtyDict; } }


	private Dictionary<string, object> data;

	public ICollection<string> Keys { get { return ((ICollection<string>)data?.Keys) ?? EMPTY_KEYS_COL; } }

	public ICollection<object> Values { get { return ((ICollection<object>)data?.Values) ?? EMPTY_VALS_COL; } }

	public int Count { get { return data != null ? data.Count : 0; } }

	public bool IsReadOnly { get { return data != null; } }

	public object this[string key] {
		get { return (data != null && data.ContainsKey(key)) ? data[key] : null; }
		set { 
			if (data != null) {
				if (value != null) {
					data[key] = value; 
				} else {
					data.Remove(key);
				}
				_dirtyList = true;
			} 
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
		}

		foreach (var entry in list) {
			data[entry.Key] = entry.Value;
		}
		
		runtimeList = new List<Entry>(list);
		//runtimeList.AddRange(list);
		
	}
	
	void OnDisable() {
		data = null;
	}

	public void Set(ValueMap source) {
		foreach (var pair in source) {
			this[pair.Key] = pair.Value;
		}
		_dirtyList = true;
	}

	public void RebuildRuntimeList() {
		RebuildList(runtimeList);
		_dirtyList = false;
	}

	public void RebuildList(List<Entry> list) {
		list.Clear();
		foreach (var pair in data) {
			list.Add(new Entry(pair.Key, pair.Value));
		}
	}

	public void RebuildRuntimeDictionary() {
		data.Clear();
		_dirtyDict = false;
		foreach (var entry in list) {
			data[entry.Key] = entry.Value;
		}
	}



	public bool ContainsKey(string key) {
		return data != null ? data.ContainsKey(key) : false;
	}

	public void Add(string key, object value) {
		if (data != null) { this[key] = value; }
	}

	public bool Remove(string key) {
		_dirtyList = true;
		return data != null ? data.Remove(key) : false;
	}

	public bool TryGetValue(string key, out object value) {
		if (data != null) { return data.TryGetValue(key, out value); }
		value = null;
		return false;
	}

	public void Add(KeyValuePair<string, object> item) {
		if (data != null) { data.Add(item.Key, item.Value); _dirtyList = true; }
	}

	public void Clear() {
		if (data != null) { data.Clear(); _dirtyList = true; }
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
		if (Contains(item)) { _dirtyList = true; return Remove(item.Key); }
		return false;
	}

	public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
		return data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return data.GetEnumerator();
	}
}
