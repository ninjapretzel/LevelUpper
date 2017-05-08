using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelUpper.InputSystem {
	
	public static class Controllers {
		/// <summary>
		/// Get the name of the primary controller that is connected
		/// </summary>
		public static string primaryController {
			get {
				foreach (string str in Input.GetJoystickNames()) {
					if (str != null && str != "") { return str; }
				}
				return "";
			}
		}

		public static int primaryControllerIndex {
			get {
				var joysticks = Input.GetJoystickNames();
				for (int i = 0; i < joysticks.Length; i++) {
					var str = joysticks[i];
					if (str != null && str != "") { return i; }
				}
				return -1;
			}
		}

		/// <summary> true if primaryController is not empty string, false otherwise. </summary>
		public static bool controllerConnected { get { return primaryController != ""; } }



	}
}
