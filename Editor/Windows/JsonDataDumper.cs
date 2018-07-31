#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace LevelUpper.Editor {

	public class JsonDataDumper : EditorWindowLevelUpper {
		
		[MenuItem("Window/Custom/JsonDataDumper")]
		public static void ShowWindow() { 
			EditorWindow.GetWindow(typeof(JsonDataDumper)); 
		}
		
		public UnityEngine.Object obj;
		public UnityEngine.Object folder;
		public string assetName;

		public JsonDataDumper() {
			titleContent = new GUIContent("Json Data Dumper");
		}
		
		static readonly Type UNITYOBJECT = typeof(UnityEngine.Object);

		void OnGUI() { 

			obj = EditorGUILayout.ObjectField("Object", obj, UNITYOBJECT, true);
			folder = EditorGUILayout.ObjectField("Folder", folder, UNITYOBJECT, false);
			var folderPath = AssetDatabase.GetAssetPath(folder);

			assetName = EditorGUILayout.TextField("Name", assetName);

			if (!AssetDatabase.IsValidFolder(folderPath)) {
				folder = null;
			}


			if (Button("Yeah, do it") && folder != null) {
				JsonValue val = Json.Reflect(obj);


				if (val.isObject) {
					val["useGUILayout"] = null;
					val["runInEditMode"] = null;
					val["enabled"] = null;
					val["tag"] = null;
					val["name"] = null;
					val["hideFlags"] = null;

					string assetPath = $"{folderPath}/{assetName}.asset";

					//AssetDatabase.LoadAssetAtPath(assetPath, UNITYOBJECT);

					JsonData data = ScriptableObject.CreateInstance<JsonData>();
					data.json = val.ToString();
					
					AssetDatabase.CreateAsset(data, assetPath);

					AssetDatabase.Refresh();
										
				}

			}

		}
		
		void Update() { }
		void OnInspectorUpdate() { }
		
		void OnFocus() { }
		void OnLostFocus() { }

		void OnSelectionChange() { }
		void OnHierarchyChange() { }
		void OnProjectChange() { }
		
		void OnDestroy() { }
		
	}
	
}
#endif
