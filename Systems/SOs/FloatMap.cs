using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
[CustomEditor(typeof(FloatMap))]
public class FloatMapEditor : Editor {
	private ReorderableList list;

	static int baseWidth = 110;
	static int padding = 2;

	void OnEnable() {
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);
		
		list.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "FloatMap Properties");
		};

		list.elementHeightCallback = (index) => {
			return EditorGUIUtility.singleLineHeight + padding * 2;
		};

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += padding;

			var keyProp = element.FindPropertyRelative("Key");
			var valueProp = element.FindPropertyRelative("Value");
			float lineHeight = EditorGUIUtility.singleLineHeight;

			Rect keyRect = new Rect(rect.x, rect.y, baseWidth, lineHeight);
			Rect valueRect = new Rect(keyRect.xMax + padding, rect.y, rect.width - keyRect.xMax - padding, lineHeight);

			EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
			EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

		};
	}
	public override void OnInspectorGUI() {
		serializedObject.Update();

		list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}
}

[CustomPropertyDrawer(typeof(FloatMap.Entry))]
public class FloatMapEntryDrawer : PropertyDrawer {
	private static readonly GUIContent VALUE_CONTENT = new GUIContent("Value");
	private static readonly GUIContent KEY_CONTENT = new GUIContent("Key");
	const float LABEL_SIZE = 40;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property); {

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("DIIICKS"));
			var width = (position.width / 2);
			var keyRect = new Rect(position.x, position.y, width, position.height);
			var valueRect = new Rect(position.x + width, position.y, width, position.height);

			EditorGUIUtility.labelWidth = LABEL_SIZE;
			EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("Key"), KEY_CONTENT);
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Value"), VALUE_CONTENT);

			EditorGUI.indentLevel = indent;
		} EditorGUI.EndProperty();
	}
}

#endif

[CreateAssetMenu(fileName = "New FloatMap", menuName = "SOs/Create New FloatMap", order = 9000)]
public class FloatMap : ScriptableObject, IDictionary<string, float> {

	[Serializable]
	public struct Entry {
		public string Key;
		public float Value;
	}

	private static readonly string[] EMPTY_STRINGS = { };
	private static readonly ReadOnlyCollection<string> EMPTY_STRING_COL = new ReadOnlyCollection<string>(EMPTY_STRINGS);
	private static readonly float[] EMPTY_INTS = { };
	private static readonly ReadOnlyCollection<float> EMPTY_INT_COL = new ReadOnlyCollection<float>(EMPTY_INTS);

	public List<Entry> list;

	private Dictionary<string, float> data;

	public ICollection<string> Keys { get { return ((ICollection<string>)data?.Keys) ?? EMPTY_STRING_COL; } }

	public ICollection<float> Values { get { return ((ICollection<float>)data?.Values) ?? EMPTY_INT_COL; } }

	public int Count { get { return data != null ? data.Count : 0; } }

	public bool IsReadOnly { get { return data != null; } }

	public float this[string key] {
		get { return data != null ? data[key] : 0f; }
		set { if (data != null) { data[key] = value; } }
	}

	void Awake() {

	}

	void OnEnable() {
		if (data == null) {
			data = new Dictionary<string, float>();

			if (list != null) {
				foreach (var entry in list) {
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

	public void Add(string key, float value) {
		if (data != null) { this[key] = value; }
	}

	public bool Remove(string key) {
		return data != null ? data.Remove(key) : false;
	}

	public bool TryGetValue(string key, out float value) {
		if (data != null) { return data.TryGetValue(key, out value); }
		value = 0;
		return false;
	}

	public void Add(KeyValuePair<string, float> item) {
		if (data != null) { data.Add(item.Key, item.Value); }
	}

	public void Clear() {
		if (data != null) { data.Clear(); }
	}

	public bool Contains(KeyValuePair<string, float> item) {
		if (data != null) {
			if (data.ContainsKey(item.Key)) { return data[item.Key] == item.Value; }
		}
		return item.Value == 0;
	}

	public void CopyTo(KeyValuePair<string, float>[] array, int arrayIndex) {
		int i = 0;
		foreach (var pair in data) {
			array[i] = pair;
			i++;
		}
	}

	public bool Remove(KeyValuePair<string, float> item) {
		if (Contains(item)) { return Remove(item.Key); }
		return false;
	}

	public IEnumerator<KeyValuePair<string, float>> GetEnumerator() {
		return data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return data.GetEnumerator();
	}
}
