namespace LevelUpper.InputSystem {
	/// <summary>
	/// <c>Struct</c> representing all the fields in an axis definition in Unity's Input Manager.
	/// All fields are named as they appear in Unity's serialized format.
	/// </summary>
	public struct InputAxis {
		/// <summary> Enum for different types of InputAxis. </summary>
		public enum InputAxisType {
			KeyOrMouseButton = 0,
			MouseMovement = 1,
			JoystickAxis = 2,
		}

		// Constants defining the limitations of Unity's Input Manager as of 2017.2.0. Update these if the number of supported joysticks or axes changes.
		public const int numJoysticks = 16;
		public const int numAxes = 28;

		/// <summary>The <c>string</c> that refers to the axis in the game launcher and through scripting.</summary>
		public string m_Name;
		/// <summary>A detailed definition of the Positive Button function that is displayed in the game launcher.</summary>
		public string descriptiveName;
		/// <summary>A detailed definition of the Negative Button function that is displayed in the game launcher.</summary>
		public string descriptiveNegativeName;
		/// <summary>The button that will send a negative value to the axis.</summary>
		public string negativeButton;
		/// <summary>The button that will send a positive value to the axis.</summary>
		public string positiveButton;
		/// <summary>The secondary button that will send a negative value to the axis.</summary>
		public string altNegativeButton;
		/// <summary>The secondary button that will send a positive value to the axis.</summary>
		public string altPositiveButton;
		/// <summary>How fast will the input recenter. Only used when the Type is key / mouse button.</summary>
		public float gravity;
		/// <summary>Any positive or negative values that are less than this number will register as zero. Useful for joysticks.</summary>
		public float dead;
		/// <summary>For keyboard input, a larger value will result in faster response time. A lower value will be more smooth. For Mouse delta the value will scale the actual mouse delta.</summary>
		public float sensitivity;
		/// <summary>If enabled, the axis value will be immediately reset to zero after it receives opposite inputs. Only used when the Type is key / mouse button.</summary>
		public bool snap;
		/// <summary>If enabled, the positive buttons will send negative values to the axis, and vice versa.</summary>
		public bool invert;
		/// <summary>Use Key / Mouse Button for any kind of buttons, Mouse Movement for mouse delta and scrollwheels, Joystick Axis for analog joystick axes and Window Movement for when the user shakes the window.</summary>
		public InputAxisType type;
		/// <summary>0-based axis of input from the device (joystick, mouse, gamepad, etc.)</summary>
		public int axis;
		/// <summary>1-based joystick which should be used. If set to 0, will retrieve the input from all joysticks. This is only used for input axes and not buttons.</summary>
		public int joyNum;
	}
}
