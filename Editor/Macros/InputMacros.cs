#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using LevelUpper.InputSystem;

namespace LevelUpper.Editor {

	public class InputMacros {

		[MenuItem("LevelUpper/Input/Regenerate Input Manager")]
		public static void RegenerateInputManager() {
			if (EditorUtility.DisplayDialog("Warning!", "This will erase and replace all axes currently defined in the Input Manager!", "Ok", "Cancel")) {
				SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
				SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
				axesProperty.ClearArray();
				
				AddAxis(new InputAxis() { m_Name = "Null" }, axesProperty);
				AddAxis(new InputAxis() {
					m_Name = "MouseAxisX",
					sensitivity = 0.1f,
					type = InputAxis.InputAxisType.MouseMovement,
					axis = 0,
				}, axesProperty);
				AddAxis(new InputAxis() {
					m_Name = "MouseAxisY",
					sensitivity = 0.1f,
					type = InputAxis.InputAxisType.MouseMovement,
					axis = 1,
				}, axesProperty);
				AddAxis(new InputAxis() {
					m_Name = "MouseWheel",
					sensitivity = 1,
					type = InputAxis.InputAxisType.MouseMovement,
					axis = 2,
				}, axesProperty);

				// Uncomment the "sb" stuff to have a regenerated AxisCode enum printed to the debuglog.
				//StringBuilder sb = new StringBuilder();
				//sb.Append("Null = 0,\n");
				//sb.Append("MouseAxisX = 1,\n");
				//sb.Append("MouseAxisY = 2,\n");
				//sb.Append("MouseWheel = 3,\n");

				for (int i = 0; i <= InputAxis.numJoysticks; ++i) {
					for (int j = 0; j < InputAxis.numAxes; ++j) {
						InputAxis addMe = new InputAxis() {
							m_Name = "Joystick" + (i > 0 ? i.ToString() : "") + "Axis" + j.ToString(),
							gravity = 0,
							dead = 0.2f,
							sensitivity = 1,
							snap = false,
							invert = false,
							type = InputAxis.InputAxisType.JoystickAxis,
							axis = j,
							joyNum = i
						};
						//sb.Append("Joystick" + (i > 0 ? i.ToString() : "") + "Axis" + j.ToString() + " = " + (4 + (i * InputAxis.numAxes) + j) + ",\n");
						AddAxis(addMe, axesProperty);
					}
				}

				serializedObject.ApplyModifiedProperties();
				//Debug.Log(sb.ToString());
			}
		}

		private static void AddAxis(InputAxis axis, SerializedProperty axesProperty) {
			axesProperty.arraySize++;

			SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

			FieldInfo[] fields = typeof(InputAxis).GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {
				switch (field.FieldType.Name) {
					case "String": {
						GetChildProperty(axisProperty, field.Name).stringValue = (string)field.GetValue(axis);
						break;
					}
					case "Single": {
						GetChildProperty(axisProperty, field.Name).floatValue = (float)field.GetValue(axis);
						break;
					}
					case "Boolean": {
						GetChildProperty(axisProperty, field.Name).boolValue = (bool)field.GetValue(axis);
						break;
					}
					case "Int32": {
						GetChildProperty(axisProperty, field.Name).intValue = (int)field.GetValue(axis);
						break;
					}
					case "InputAxisType": {
						GetChildProperty(axisProperty, field.Name).intValue = (int)(InputAxis.InputAxisType)field.GetValue(axis);
						break;
					}
				}
			}
		}

		private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
			SerializedProperty child = parent.Copy();
			child.Next(true);
			do {
				if (child.name == name) {
					return child;
				}
			}
			while (child.Next(false));
			return null;
		}
		
	}
}
#endif
