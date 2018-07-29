using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> Global references to a given type. </summary>
/// <typeparam name="T"> Type of references to store. </typeparam>
public static class Globals<T> where T : class {
	
	/// <summary> Named instances of the type. </summary>
	public static Dictionary<string, T> named;
	
	/// <summary> 'Main' instance of a given type. </summary>
	public static T main;

}

/// <summary> Global datastore of a given value type. </summary>
/// <typeparam name="T"> Type of values to store. </typeparam>
public static class Values<T> where T : struct {
	/// <summary> Named set of values of the type. </summary>
	public static Dictionary<string, T> named;
}
