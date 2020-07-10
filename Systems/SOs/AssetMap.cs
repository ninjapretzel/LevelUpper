using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Asset = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;

[CustomEditor(typeof(AssetMap))]
public class AssetMapEditor : Editor {
	private ReorderableList list;

	static int baseWidth = 110;
	static int padding = 2;

	void OnEnable() {
		RefreshList();
	}

	private void RefreshList() {
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);

		list.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "Asset Map Properties");
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
		
		AssetMap targetMap = target as AssetMap;

		var folder = AssetDatabase.LoadMainAssetAtPath(targetMap.folderPath);
		serializedObject.Update();

		var newFolder = EditorGUILayout.ObjectField("Folder", folder, typeof(Asset), false);
		var folderProperty = serializedObject.FindProperty("folderPath");
		folderProperty.stringValue = AssetDatabase.GetAssetPath(newFolder);
		
		list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();

		if (newFolder != folder) {
			Undo.RecordObject(target, "Update AssetMap List");
			if (!EditorApplication.isPlaying) {
				AssetMapUtils.Rebuild(targetMap, newFolder);

			}
			targetMap.folderPath = AssetDatabase.GetAssetPath(newFolder);
		}

		serializedObject.Update();
		serializedObject.ApplyModifiedProperties();
	}	

	public static Asset[] ObjectsInFolder(string path) {
		if (AssetDatabase.IsValidFolder(path)) {
			string[] paths = Directory.GetFiles(path);
			return paths
				.Where(asset => AssetDatabase.IsValidFolder(asset))
				.Select(asset => AssetDatabase.LoadMainAssetAtPath(asset))
				.Where(obj => obj != null)
				.ToArray();
		}
		return new Asset[0];
	}

}

[CustomPropertyDrawer(typeof(AssetMap.Entry))]
public class AssetMapEntryDrawer : PropertyDrawer {
	private static readonly GUIContent VALUE_CONTENT = new GUIContent("Value");
	private static readonly GUIContent KEY_CONTENT = new GUIContent("Key");
	const float LABEL_SIZE = 40;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property); {

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);
			var width = (position.width / 2);

			var keyRect = new Rect(position.x, position.y, width, position.height);
			var valueRect = new Rect(position.x + width, position.y, width, position.height);

			EditorGUIUtility.labelWidth = LABEL_SIZE;
			EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("Key"), KEY_CONTENT);
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Value"), VALUE_CONTENT);

			EditorGUI.indentLevel = indent;
		}
		EditorGUI.EndProperty();
	}
}
#endif 

public class AssetMapUtils {
	public static void Rebuild(AssetMap map, Asset folder) {
#if UNITY_EDITOR
		if (folder != null) {

			var path = AssetDatabase.GetAssetPath(folder);
			if (AssetDatabase.IsValidFolder(path)) {
				string[] paths = Directory.GetFiles(path);
				List<AssetMap.Entry> entries = new List<AssetMap.Entry>();

				foreach (var asset in paths) {
					if (AssetDatabase.IsValidFolder(asset)) {
						continue;
					}
					var obj = AssetDatabase.LoadMainAssetAtPath(asset);

					if (obj != null) {
						var entry = new AssetMap.Entry();
						entry.Key = obj.name;
						entry.Value = obj;
						entries.Add(entry);
					}

				}

				map.list = entries;

			}
		}
#endif
	}
}

[CreateAssetMenu(fileName = "New AssetMap", menuName = "SOs/Create New AssetMap", order = 56)]
public class AssetMap : ScriptableObject, IDictionary<string, Asset> {

	private static readonly string[] EMPTY_STRINGS = { };
	private static readonly ReadOnlyCollection<string> EMPTY_STRING_COL = new ReadOnlyCollection<string>(EMPTY_STRINGS);
	private static readonly Asset[] EMPTY_INTS = { };
	private static readonly ReadOnlyCollection<Asset> EMPTY_INT_COL = new ReadOnlyCollection<Asset>(EMPTY_INTS);

	[Serializable] public struct Entry {
		public string Key;
		public Asset Value;
		public Entry(string Key, Asset Value) {
			this.Key = Key;
			this.Value = Value;
		}
		public Entry(Asset Value) {
			this.Key = Value.name;
			this.Value = Value;
		}
	}
	
	public string folderPath;
	public List<Entry> list = new List<Entry>();

	private Dictionary<string, Asset> data;

	public ICollection<string> Keys { get { return ((ICollection<string>)data?.Keys) ?? EMPTY_STRING_COL; } }

	public ICollection<Asset> Values { get { return ((ICollection<Asset>)data?.Values) ?? EMPTY_INT_COL; } }

	public int Count { get { return data != null ? data.Count : 0; } }

	public bool IsReadOnly { get { return data != null; } }

	public Asset this[string key] {
		get { return (data != null && data.ContainsKey(key)) ? data[key] : null; }
		set { if (data != null) { data[key] = value; } }
	}

	public T Get<T>(string name) where T : Asset {
		return (T) data[name];
	}

	void OnEnable() {
		if (data == null) {
			data = new Dictionary<string, Asset>();
		}
		#if UNITY_EDITOR
		if (folderPath != null && folderPath != "" && AssetDatabase.IsValidFolder(folderPath)) {
			Asset folder = AssetDatabase.LoadMainAssetAtPath(folderPath);

			if (folder != null) {
				AssetMapUtils.Rebuild(this, folder);
			}
		}
		#endif
			
		foreach (var entry in list) {
			data[entry.Key] = entry.Value;
		}
	}
	

	void OnDisable() {
		data = null;
	}

	public bool ContainsKey(string key) {
		return data != null ? data.ContainsKey(key) : false;
	}

	public void Add(string key, Asset value) {
		if (data != null) { this[key] = value; }
	}

	public bool Remove(string key) {
		return data != null ? data.Remove(key) : false;
	}

	public bool TryGetValue(string key, out Asset value) {
		if (data != null) { return data.TryGetValue(key, out value); }
		value = null;
		return false;
	}

	public void Add(KeyValuePair<string, Asset> item) {
		if (data != null) { data.Add(item.Key, item.Value); }
	}

	public void Clear() {
		if (data != null) { data.Clear(); }
	}

	public bool Contains(KeyValuePair<string, Asset> item) {
		if (data != null) {
			if (data.ContainsKey(item.Key)) { return data[item.Key] == item.Value; }
		}
		return item.Value == null;
	}

	public void CopyTo(KeyValuePair<string, Asset>[] array, int arrayIndex) {
		int i = 0;
		foreach (var pair in data) {
			array[i] = pair;
			i++;
		}
	}

	public bool Remove(KeyValuePair<string, Asset> item) {
		if (Contains(item)) { return Remove(item.Key); }
		return false;
	}

	public IEnumerator<KeyValuePair<string, Asset>> GetEnumerator() {
		return data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return data.GetEnumerator();
	}
}
