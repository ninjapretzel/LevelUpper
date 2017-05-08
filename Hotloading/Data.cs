using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using LevelUpper.Extensions;

namespace LevelUpper.Hotload {

#if XtoJSON
	public static class Helpers {
		/// <summary> Gets all of the static fields of a given type, both public and non-public. </summary>
		/// <param name="type"> Type to grab fields from </param>
		/// <returns> Array of all Static Fields </returns>
		public static FieldInfo[] StaticFields(this Type type) {
			return type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
		
		/// <summary> Repeatable macro for hotloading JsonObjects containing data into static fields of arbitrary types. </summary>
		/// <typeparam name="T"> Generic Type Parameter </typeparam>
		/// <param name="field"> FieldInfo to set value into </param>
		/// <param name="data"> JsonObject containing data to reflect into the given field </param>
		public static void StaticHotLoad<T>(this FieldInfo field, JsonObject data) {
			if (field.FieldType == typeof(T)) {
				field.SetValue(null, data.Get<T>(field.Name));
			}
		}
	}

#endif 

	[ExecuteInEditMode] public class Data : MonoBehaviour{

		void Start() {
			if (lastLoads == null || hotloads == null) {
				lastLoads = new Dictionary<string, DateTime>();
				hotloads = new Dictionary<string, Action<string>>();
				Debug.Log("Data.HotLoad - Hotloading initialized.\nClick for more info.\nData.CheckHotLoads needs to be called every frame (or as often as needed)");
			}
		}

		void Update() {
			CheckHotLoads();
		}

		/// <summary> Path of the Relative /Assets/Data/System/ folder </summary>
		public static string Path {
			get {

	#if UNITY_EDITOR
				return Directory.GetCurrentDirectory() + "/Assets/Data/System";
	#else
				string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory(), "*_Data");
				return dirs[0] + "/System";		
	#endif
			}

		}

		/// <summary> Record of the last timestamps of successful hotloads </summary>
		internal static Dictionary<string, DateTime> lastLoads;
		/// <summary> Mapping of filename:callback methods </summary>
		internal static Dictionary<string, Action<string> > hotloads;

		/// <summary> Checks all hotloads, and performs any that are necessary. </summary>
		public static void CheckHotLoads() {
			if (lastLoads == null || hotloads == null) {
				Debug.Log("Data.CheckHotLoads - Just a head's up, no HotLoads have been registered yet...");
				lastLoads = new Dictionary<string,DateTime>();
				hotloads = new Dictionary<string,Action<string>>();
			}

			foreach (var pair in hotloads) {
				var filename = pair.Key;
				var callback = pair.Value;
				var fullPath = Path + '/' + filename;
				var lastWrite = File.GetLastWriteTime(fullPath);

				if (lastWrite > lastLoads[filename]) {
					callback(File.ReadAllText(fullPath));

					lastLoads[filename] = lastWrite;
					Debug.Log("Hot-Reloaded " + filename);
				}

			}

		}


		/// <summary> Registers a file to hotload, and a callback to handle hotloading said file. Attempts to load the file immediately. </summary>
		/// <param name="filename"> File to check, relative to Assets/Data/System/ </param>
		/// <param name="callback"> Function to call, accepting the full text of the file. </param>
		public static void HotLoad(string filename, Action<string> callback) {
			if (lastLoads == null || hotloads == null) {
				lastLoads = new Dictionary<string,DateTime>();
				hotloads = new Dictionary<string,Action<string>>();
				Debug.Log("Data.HotLoad - Hotloading initialized.\nClick for more info.\nData.CheckHotLoads needs to be called every frame (or as often as needed)");
			}
			if (hotloads.ContainsKey(filename)) {
				Debug.LogWarning("Data.HotLoad - " + filename + " already registered. Call HotLoad once to register a callback to reload data when a file is updated. ");
				return;
			}

			var fullPath = Path + "/" + filename;
			if (File.Exists(fullPath)) {
				
				var lastWrite = File.GetLastWriteTime(fullPath);
				hotloads[filename] = callback;
				callback(File.ReadAllText(fullPath));
				lastLoads[filename] = lastWrite;
			} else {
				Debug.LogWarning("Data.HotLoad - " + filename + " was not found. Callback not registered. ");
			}

		}
		
		/// <summary> Dynamically create hotloads for all files in a given directory. Automatically ignores .meta files. </summary>
		/// <param name="directoryPath">Relative path of the directory from the Assets/Data/System/ folder. </param>
		/// <param name="callback"> Function to call with the content of each file in the directory </param>
		public static void HotLoadDirectory(string directoryPath, Action<string> callback) {
			directoryPath = directoryPath.ForwardSlashPath();
			if (!directoryPath.StartsWith("/")) { directoryPath = "/" + directoryPath; }
			if (!Directory.Exists(Path + directoryPath)) { Directory.CreateDirectory(Path + directoryPath); }
			string[] files = Directory.GetFiles(Path + directoryPath);

			foreach (var file in files) {
				if (file.EndsWith(".meta")) { continue; }
				string relativeFilePath = file.Replace(Path, "").ForwardSlashPath();

				HotLoad(relativeFilePath, callback);
			}

		}

		/// <summary> Dynamically create hotloads for all files in a given directory.</summary>
		/// <param name="directoryPath"> Relative path of the directory from the Assets/Data/System/ folder </param>
		/// <param name="callback"> Callback function that takes (filePath, fileContent), called upon loading or reloading the file. </param>
		public static void HotLoadDirectory(string directoryPath, Action<string,string> callback) {
			directoryPath = directoryPath.ForwardSlashPath();
			if (!directoryPath.StartsWith("/")) { directoryPath = "/" + directoryPath; }
			if (!Directory.Exists(Path + directoryPath)) { Directory.CreateDirectory(Path + directoryPath); }
			string[] files = Directory.GetFiles(Path + directoryPath);

			foreach (var file in files) {
				if (file.EndsWith(".meta")) { continue; }
				string relativeFilePath = file.Replace(Path, "").ForwardSlashPath();

				// Curry callback by binding the relative path to the callback. 
				HotLoad(relativeFilePath, (text)=>{ callback(relativeFilePath, text); } );
			}

		}

		/// <summary> Hotload a given directory, and all subdirectories, with a given <paramref name="callback"/> </summary>
		/// <param name="directoryPath"> Relative path to load, from /Assets/Data/System/ </param>
		/// <param name="callback"> Function to call, which accepts the content of the loaded/reloaded files </param>
		public static void HotLoadDirectoryRecursively(string directoryPath, Action<string> callback) {
			List<string> files = new List<string>();
			List<string> directories = new List<string>();

			directoryPath = directoryPath.ForwardSlashPath();
			if (!directoryPath.StartsWith("/")) { directoryPath = "/" + directoryPath; }
			if (!Directory.Exists(Path + directoryPath)) { Directory.CreateDirectory(Path + directoryPath); }

			directories.Add(Path + directoryPath);
			
			while (directories.Count > 0) {
				var directory = directories[0];
				directories.RemoveAt(0);

				foreach (var dir in Directory.GetDirectories(directory)) { directories.Add(dir); }
				foreach (var file in Directory.GetFiles(directory)) { files.Add(file); }
			}
			
			foreach (var file in files) {
				if (file.EndsWith(".meta")) { continue; }
				string relativeFilePath = file.Replace(Path, "").ForwardSlashPath();

				HotLoad(relativeFilePath, callback);
			}

		}

		/// <summary> Hotload a given directory, and all subdirectories, with a given <paramref name="callback"/> </summary>
		/// <param name="directoryPath"> Relative path to load, from /Assets/Data/System/ </param>
		/// <param name="callback"> Function to call, which takes (filePath, fileContent) of the loaded files </param>
		public static void HotLoadDirectoryRecursively(string directoryPath, Action<string,string> callback) {
			List<string> files = new List<string>();
			List<string> directories = new List<string>();

			directoryPath = directoryPath.ForwardSlashPath();
			if (!directoryPath.StartsWith("/")) { directoryPath = "/" + directoryPath; }
			if (!Directory.Exists(Path+directoryPath)) { Directory.CreateDirectory(Path+directoryPath); }

			directories.Add(Path + directoryPath);

			while (directories.Count > 0) {
				var directory = directories[0];
				directories.RemoveAt(0);

				foreach (var dir in Directory.GetDirectories(directory)) { directories.Add(dir); }
				foreach (var file in Directory.GetFiles(directory)) { files.Add(file); }
			}

			foreach (var file in files) {
				if (file.EndsWith(".meta")) { continue; }
				string relativeFilePath = file.Replace(Path, "").ForwardSlashPath();

				// Curry callback by binding the relative path to the callback. 
				HotLoad(relativeFilePath, (text)=>{ callback(relativeFilePath, text); } );
			}

		}


#if X_TO_JSON
		/// <summary> Load a JsonObject from a given file. </summary>
		/// <param name="filename"> Filename, excluding the '.' </param>
		/// <param name="ext"> File extension, including the '.' - Default is ".json" </param>
		/// <returns> The loaded JsonObject if it was loading </returns>
		public static JsonObject LoadJsonObject(string filename, string ext = ".json") {
			string fullPath = Path + "/" + filename + ext;
			if (File.Exists(fullPath)) {
				string json = File.ReadAllText(fullPath);
				
				return Json.Parse(json) as JsonObject;
			}
			return null;
		}

		/// <summary> Load a JsonArray from a given file. </summary>
		/// <param name="filename"> Filename, excluding the '.' </param>
		/// <param name="ext"> File extension, including the '.' - Default is ".json" </param>
		/// <returns> The loaded JsonArray if it was loading </returns>
		public static JsonArray LoadJsonArray(string filename, string ext = ".json") {
			string fullPath = Path + "/" + filename + ext;
			if (File.Exists(fullPath)) {
				string json = File.ReadAllText(fullPath);

				return Json.Parse(json) as JsonArray;
			}
			return null;
		}

#endif
		
	}
}
