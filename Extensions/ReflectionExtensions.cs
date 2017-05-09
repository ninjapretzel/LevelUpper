using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

///Class containing various common reflection things.
///

namespace LevelUpper.Extensions.Reflection {
	/// <summary> Holds extensions on object for reflection </summary>
	public static class ReflectionExtensions {

		/// <summary> Cache to make repeated use of this class faster. </summary>
		private static Dictionary<string, Type> _cachedTypes = new Dictionary<string,Type>();

		/// <summary> Get the 'code' name of a type ('float' instead of 'System.Single') </summary>
		/// <param name="t"> Type </param>
		/// <returns> Short name for the given type </returns>
		public static string ShortName(this Type t) {
			if (t == typeof(void)) { return "void"; }
			else if (t == typeof(string)) { return "string"; }
			else if (t == typeof(float)) { return "float"; }
			else if (t == typeof(bool)) { return "bool"; }
			else if (t == typeof(int)) { return "int"; }
			else if (t == typeof(double)) { return "double"; }
			else if (t == typeof(long)) { return "long"; }
			else if (t == typeof(System.Object)) { return "object"; }
			else if (t == typeof(UnityEngine.Object)) { return "UnityEngine.Object"; }
			else if (t == typeof(Event)) { return "Event"; }
			return t.ToString().FromLast('.').Replace('+', '.');
		}

		/// <summary> Check that an object can be cast to a type. </summary>
		/// <param name="o"> Object to check </param>
		/// <param name="target"> Target type </param>
		/// <returns> True if the target type is assignable on the given object's type. </returns>
		public static bool CanCastTo(this System.Object o, Type target) { return target.IsAssignableFrom(o.GetType()); }

		/// <summary> Does <paramref name="obj"/> have a property called <paramref name="name"/>? </summary>
		/// <param name="obj"> Object to check on </param>
		/// <param name="name"> Name of property to check on </param>
		/// <returns> True if a property called <paramref name="name"/> exists on <paramref name="obj"/>, false otherwise.  </returns>
		public static bool HasProperty(this System.Object obj, string name) { return obj.GetProperty(name) != null; }

		/// <summary> Does <paramref name="type"/> have a static property called <paramref name="name"/>? </summary>
		/// <param name="type"> Type to check on </param>
		/// <param name="name"> Name of property to check on </param>
		/// <returns> True if a static property called <paramref name="name"/> exists on <paramref name="type"/>, false otherwise. </returns>
		public static bool HasStaticProperty(this System.Type type, string name) { return type.GetStaticProperty(name) != null; }

		/// <summary> Get <paramref name="obj"/>'s property called <paramref name="name"/></summary>
		/// <param name="obj"> Object to get PropertyInfo from </param>
		/// <param name="name"> Name of property to grab </param>
		/// <returns> PropertyInfo of <paramref name="obj"/>'s property named <paramref name="name"/> </returns>
		public static PropertyInfo GetProperty(this System.Object obj, string name) { return obj.GetType().GetProperty(name); }
		/// <summary> Get <paramref name="obj"/>'s property called <paramref name="name"/>, of a given <paramref name="type"/> </summary>
		/// <param name="obj"> Object to get PropertyInfo from </param>
		/// <param name="name"> Name of property to grab </param>
		/// <param name="type"> Type of property </param>
		/// <returns> PropertyInfo of <paramref name="obj"/>, of type <paramref name="type"/> named <paramref name="name"/> </returns>
		public static PropertyInfo GetProperty(this System.Object obj, string name, Type type) { return obj.GetType().GetProperty(name, type); }
		/// <summary> Get <paramref name="obj"/>'s property called <paramref name="name"/>, bound with given <paramref name="flags"/>. </summary>
		/// <param name="obj"> Object to get PropertyInfo from </param>
		/// <param name="name"> Name of property to grab </param>
		/// <param name="flags"> BindingFlags to use to narrow search </param>
		/// <returns> PropertyInfo of <paramref name="obj"/> named <paramref name="name"/> with given <paramref name="flags"/> </returns>
		public static PropertyInfo GetProperty(this System.Object obj, string name, BindingFlags flags) { return obj.GetType().GetProperty(name, flags); }
		
		/// <summary> Gets a static property named <paramref name="name"/> from <paramref name="type"/> </summary>
		/// <param name="type"> Type to get property from </param>
		/// <param name="name"> Name of property to get </param>
		/// <returns> Static PropertyInfo for <paramref name="type"/> for property named <paramref name="name"/> </returns>
		public static PropertyInfo GetStaticProperty(this System.Type type, string name) { return type.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty); }
		
		/// <summary> Does <paramref name="obj"/> have a field called <paramref name="name"/>? </summary>
		/// <param name="obj"> Object to check on </param>
		/// <param name="name"> Name of field to check for </param>
		/// <returns> True if <paramref name="obj"/> has a field called <paramref name="name"/>, false otherwise. </returns>
		public static bool HasField(this System.Object obj, string name) { return obj.GetField(name) != null; }

		/// <summary> Does <paramref name="type"/> have a static field called <paramref name="name"/>? </summary>
		/// <param name="type"> Type to check on </param>
		/// <param name="name"> Name of field to check for </param>
		/// <returns> True if <paramref name="type"/> has a static field called <paramref name="name"/>, false otherwise </returns>
		public static bool HasStaticField(this System.Type type, string name) { return type.GetStaticField(name) != null; }


		/// <summary> Get <paramref name="obj"/>'s field named <paramref name="name"/> </summary>
		/// <param name="obj"> Object to check on </param>
		/// <param name="name"> Name of field to check for </param>
		/// <returns> FieldInfo object for <paramref name="obj"/>'s field named <paramref name="name"/>, if it exists. </returns>
		public static FieldInfo GetField(this System.Object obj, string name) { return obj.GetType().GetField(name); }
		/// <summary> Get <paramref name="obj"/>'s field named <paramref name="name"/>, given it is for a <paramref name="type"/> </summary>
		/// <param name="obj"> Object to check on </param>
		/// <param name="name"> Name of field to check for </param>
		/// <param name="type"> Type of field to check for </param>
		/// <returns> FieldInfo object for <paramref name="obj"/>'s field named <paramref name="name"/>, of a <paramref name="type"/> </returns>
		public static FieldInfo GetField(this System.Object obj, string name, Type type) { return obj.GetType().GetField(name, type); }
		/// <summary> Get <paramref name="obj"/>'s field named <paramref name="name"/>, given it matches <paramref name="flags"/> </summary>
		/// <param name="obj"> Object to check on </param>
		/// <param name="name"> Name of field to check for </param>
		/// <param name="flags"> BindingFlags that should be matched </param>
		/// <returns> FieldInfo object for <paramref name="obj"/>'s field named <paramref name="name"/>, given it matches <paramref name="flags"/> </returns>
		public static FieldInfo GetField(this System.Object obj, string name, BindingFlags flags) { return obj.GetType().GetField(name, flags); }
		
		/// <summary> Get a static field named <paramref name="name"/> from <paramref name="type"/> </summary>
		/// <param name="type"> Type to check on </param>
		/// <param name="name"> Name of field to check for </param>
		/// <returns> FieldInfo for a static field named <paramref name="name"/> from <paramref name="type"/> </returns>
		public static FieldInfo GetStaticField(this System.Type type, string name) { return type.GetField(name, BindingFlags.Static); }

		/// <summary> Check for the existance of a method named <paramref name="name"/> on the type of <paramref name="obj"/> </summary>
		/// <param name="obj"> Object to check for method on </param>
		/// <param name="name"> Name of method to check for </param>
		/// <returns> True if <paramref name="obj"/>'s type has a method called <paramref name="name"/> </returns>
		public static bool HasMethod(this System.Object obj, string name) { return obj.GetMethod(name) != null; }
		/// <summary> Check for a static method on <paramref name="type"/> named <paramref name="name"/> </summary>
		/// <param name="type"> Type to check for method on </param>
		/// <param name="name"> Name of method to check for </param>
		/// <returns> True if <paramref name="type"/> has a static method called <paramref name="name"/>, false otherwise. </returns>
		public static bool HasStaticMethod(this System.Type type, string name) { return type.GetMethod(name, BindingFlags.Static) != null; }

		/// <summary> Check <paramref name="obj"/> for a method called <paramref name="name"/> with Void return type </summary>
		/// <param name="obj"> Object to check for method on </param>
		/// <param name="name"> Name of method to check for </param>
		/// <returns> True if <paramref name="obj"/>'s type has a method called <paramref name="name"/> with Void return type </returns>
		public static bool HasAction(this System.Object obj, string name) {
			MethodInfo info = obj.GetMethod(name);
			if (info != null) { return (info.ReturnType == typeof(void)); }
			return false;
		}
		
		/// <summary> Does <paramref name="obj"/> have a method named <paramref name="name"/> with return type <typeparamref name="T"/>? </summary>
		/// <typeparam name="T"> Type of return type to check for </typeparam>
		/// <param name="obj"> Object to check on </param>
		/// <param name="name"> Name of method to check for </param>
		/// <returns> True if <paramref name="obj"/> has a method named <paramref name="name"/> that returns a <typeparamref name="T"/>, false otherwise </returns>
		public static bool HasMethod<T>(this System.Object obj, string name) {
			MethodInfo info = obj.GetMethod(name);
			if (info != null) { return (info.ReturnType == typeof(T)); }
			return false;
		}
		
		/// <summary> Invokes method named <paramref name="name"/> on <paramref name="obj"/> with given <paramref name="parameters"/>. </summary>
		/// <param name="obj"> Object to invoke on </param>
		/// <param name="name"> Name of method to Invoke </param>
		/// <param name="parameters"> Parameters to pass into the given method </param>
		/// <remarks> Less safe than CallAction(), but faster. Only use this when it is KNOWN that name exists, and the constructed params match the target method's signature. </remarks>
		public static void CallActionQ(this System.Object obj, string name, params System.Object[] parameters) { obj.GetMethod(name).Invoke(obj, parameters); }
		
		/// <summary> Call a method named <paramref name="name"/> on <paramref name="obj"/>, with given <paramref name="parameters"/>, safely. </summary>
		/// <param name="obj"> Object to invoke on </param>
		/// <param name="name"> Name of method to Invoke </param>
		/// <param name="parameters"> Parameters to pass into the given method </param>
		/// <remarks> Call a function on an object by name. Does not return any information. Safely looks up if the function exists before calling it. </remarks>
		public static void CallAction(this System.Object obj, string name, params System.Object[] parameters) {
			MethodInfo info = obj.GetMethod(name);
			if (info != null) {
				ParameterInfo[] signature = info.GetParameters();
				if (signature.Length == parameters.Length) {
					for (int i = 0; i < signature.Length; i++) {
						if (!parameters[i].CanCastTo(signature[i].ParameterType)) {
							Debug.LogWarning("ReflectionExtensions.CallAction: Function " +  name + " on instance of " + obj.GetType().ShortName() + " does not match given parameters.");
							return;
						}
					}
					info.Invoke(obj, parameters);
					
				} else {
					Debug.LogWarning("ReflectionExtensions.CallAction: Function " +  name + " on instance of " + obj.GetType().ShortName() + " does not match given parameters.");
				}
			} else {
				Debug.LogWarning("ReflectionExtensions.CallAction: Function " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			}
		}
		
		/// <summary> Get method named <paramref name="name"/> from <paramref name="obj"/>. </summary>
		/// <param name="obj"> Object to get method from </param>
		/// <param name="name"> Name of method to get </param>
		/// <returns> MethodInfo for method named <paramref name="name"/> from <paramref name="obj"/> </returns>
		//Get this object's method called name, optionally matching a Type or BindingFlags.
		public static MethodInfo GetMethod(this System.Object obj, string name) { return obj.GetType().GetMethod(name); }
		/// <summary> Get method named <paramref name="name"/> from <paramref name="obj"/> with return type <paramref name="type"/> </summary>
		/// <param name="obj"> Object to get method from </param>
		/// <param name="name"> Name of method to get </param>
		/// <param name="type"> Expected return type of method </param>
		/// <returns> Method info for method named <paramref name="name"/> with return type <paramref name="type"/> from <paramref name="obj"/> </returns>
		public static MethodInfo GetMethod(this System.Object obj, string name, Type type) { return obj.GetType().GetMethod(name, type); }
		/// <summary> Get method named <paramref name="name"/> with binding flags <paramref name="flags"/> from <paramref name="obj"/> </summary>
		/// <param name="obj"> Object to get method from </param>
		/// <param name="name"> Name of method to get </param>
		/// <param name="flags"> BindingFlags to match </param>
		/// <returns> MethodInfo from <paramref name="obj"/> for method named <paramref name="name"/> matching <paramref name="flags"/> </returns>
		public static MethodInfo GetMethod(this System.Object obj, string name, BindingFlags flags) { return obj.GetType().GetMethod(name, flags); }
		
		/// <summary> Gets a static method from <paramref name="type"/> named <paramref name="name"/> </summary>
		/// <param name="type"> Type to get method from </param>
		/// <param name="name"> Name of method to get </param>
		/// <returns> MethodInfo from <paramref name="type"/> for method named <paramref name="name"/> </returns>
		public static MethodInfo GetStaticMethod(this System.Type type, string name) { return type.GetMethod(name, BindingFlags.Static); }
		
		/// <summary> Sets a property or field named <paramref name="name"/> on <paramref name="obj"/> to <paramref name="value"/> </summary>
		/// <param name="obj"> Object to set property or field on </param>
		/// <param name="name"> Name of property or field </param>
		/// <param name="value"> Value to set into property or field </param>
		/// <returns> True, if the set occurred, false if it did not. </returns>
		public static bool SetObjectValue(this System.Object obj, string name, System.Object value) {
			if (obj.GetField(name) != null) { return obj.SetFieldValue(name, value); } 
			else if (obj.GetProperty(name) != null) { return obj.SetPropertyValue(name, value); }
			
			Debug.LogWarning("ReflectionExtensions.SetObjectValue: No field or property named " + name + " exists on instance of " + obj.GetType().ShortName());
			return false;
		}
		
		//Get an object's Property or Field value by a provided name and type.
		/// <summary> Get the value of a property or field named <paramref name="name"/> on <paramref name="obj"/>, with the given <typeparamref name="T"/> </summary>
		/// <typeparam name="T"> Type of value to get back </typeparam>
		/// <param name="obj"> Object to get from </param>
		/// <param name="name"> Name of property or field to get from </param>
		/// <returns> <typeparamref name="T"/> value of field or property named <paramref name="name"/> on <paramref name="obj"/>. default(T) if it does not exist. </returns>
		public static T GetObjectValue<T>(this System.Object obj, string name) {
			if (obj.GetField(name) != null) { return obj.GetFieldValue<T>(name); } 
			else if (obj.GetProperty(name) != null) { return obj.GetPropertyValue<T>(name); }
			
			Debug.LogWarning("ReflectionExtensions.GetObjectValue: No field or property named " + name + " exists on instance of " + obj.GetType().ShortName());
			return default(T);
		}
		
		/// <summary> Get the System.Object value of a property or field named <paramref name="name"/> on <paramref name="obj"/> </summary>
		/// <param name="obj"> Object to get from </param>
		/// <param name="name"> Name of property or field to get </param>
		/// <returns> Value of property or field named <paramref name="name"/> on <paramref name="obj"/> as a System.Object </returns>
		public static System.Object GetRawObjectValue(this System.Object obj, string name) {
			if (obj.GetField(name) != null) { return obj.GetRawFieldValue(name); }
			else if (obj.GetProperty(name) != null) { return obj.GetRawPropertyValue(name); }
			
			Debug.LogWarning("ReflectionExtensions.GetRawObjectValue: No field or property named " + name + " exists on instance of " + obj.GetType().ShortName());
			return null;
		}
		
		/// <summary> Get the value of the property on <paramref name="obj"/> named <paramref name="name"/>, with type <typeparamref name="T"/> </summary>
		/// <typeparam name="T"> Type of value to withdraw </typeparam>
		/// <param name="obj"> Object to check property on </param>
		/// <param name="name"> Name of property to check </param>
		/// <returns> <typeparamref name="T"/> value of property named <paramref name="name"/> on <paramref name="obj"/>, or default(T) if it does not exist. </returns>
		public static T GetPropertyValue<T>(this System.Object obj, string name) {
			PropertyInfo prop = obj.GetProperty(name);
			if (prop != null) {
				if (prop.PropertyType.IsAssignableFrom(typeof(T))) {
					MethodInfo method = prop.GetGetMethod();
					if (method != null) { return (T) method.Invoke(obj, null); }
					Debug.LogWarning("ReflectionExtensions.GetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not have get method.");
					return default(T);
				}
				Debug.LogWarning("ReflectionExtensions.GetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
				return default(T);
			}
			Debug.LogWarning("ReflectionExtensions.GetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			return default(T);
		}
		
		/// <summary> Get the System.Object value of a property or field named <paramref name="name"/> on <paramref name="obj"/> </summary>
		/// <param name="obj"> Object to get property value from </param>
		/// <param name="name"> Name of property to get </param>
		/// <returns> Value of property named <paramref name="name"/> on <paramref name="obj"/>, or null if it does not exist. </returns>
		public static System.Object GetRawPropertyValue(this System.Object obj, string name) {
			PropertyInfo prop = obj.GetProperty(name);
			if (prop != null) {
				if (prop.PropertyType.IsAssignableFrom(typeof(System.Object))) {
					MethodInfo method = prop.GetGetMethod();
					if (method != null) { return method.Invoke(obj, null); }
					
					Debug.LogWarning("ReflectionExtensions.GetRawPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not have get method.");
					return null;
				}
				Debug.LogWarning("ReflectionExtensions.GetRawPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
				return null;
			}
			Debug.LogWarning("ReflectionExtensions.GetRawPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			return null;
		}
		
		/// <summary> Get the value of a static property named <paramref name="name"/> from <paramref name="type"/> with type <typeparamref name="T"/> </summary>
		/// <typeparam name="T"> Type of value to get </typeparam>
		/// <param name="type"> Type to check property on </param>
		/// <param name="name"> Name of property to check </param>
		/// <returns> <typeparamref name="T"/> value of property named <paramref name="name"/> on type <paramref name="type"/>, or default(T) if it does not exist. </returns>
		public static T GetStaticPropertyValue<T>(this System.Type type, string name) {
			PropertyInfo prop = type.GetStaticProperty(name);
			if (prop != null) {
				if (prop.PropertyType.IsAssignableFrom(typeof(T))) {
					MethodInfo method = prop.GetGetMethod();
					if (method != null) { return (T) method.Invoke(null, null); }

					Debug.LogWarning("ReflectionExtensions.GetStaticPropertyValue: Property " + name + " in " + type.ShortName() + " does not have get method.");
					return default(T);
				}
				Debug.LogWarning("ReflectionExtensions.GetStaticPropertyValue: Property " + name + " in " + type.ShortName() + " does not match expected type.");
				return default(T);
			}
			Debug.LogWarning("ReflectionExtensions.GetStaticPropertyValue: Property " + name + " in " + type.ShortName() + " does not exist.");
			return default(T);
		}
		
		/// <summary> Sets the value of the property named <paramref name="name"/> on <paramref name="obj"/> to <paramref name="value"/> </summary>
		/// <param name="obj"> Object to set property on </param>
		/// <param name="name"> Name of property to set </param>
		/// <param name="value"> Value to set property to </param>
		/// <returns> True, if the set occurred, false otherwise </returns>
		public static bool SetPropertyValue(this System.Object obj, string name, System.Object value) {
			PropertyInfo prop = obj.GetProperty(name);
			if (prop != null) {
				MethodInfo method = prop.GetSetMethod();
				if (method != null) {
					if (prop.PropertyType.IsAssignableFrom(value.GetType())) {
						method.Invoke(obj, new System.Object[] { value } );
						return true;
					}					
					Debug.LogWarning("ReflectionExtensions.SetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
					return false;
				}
				Debug.LogWarning("ReflectionExtensions.SetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not have set method.");
				return false;
			}
			Debug.LogWarning("ReflectionExtensions.SetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			return false;
		}
		
		/// <summary> Get the value of the field named <paramref name="name"/> on <paramref name="obj"/>, with value of type <typeparamref name="T"/> </summary>
		/// <typeparam name="T"> Type of value to get back </typeparam>
		/// <param name="obj"> Object to get field value from </param>
		/// <param name="name"> Name of field to get </param>
		/// <returns> <typeparamref name="T"/> value of field named <paramref name="name"/> on <paramref name="obj"/>, or default(T) if it does not exist </returns>
		public static T GetFieldValue<T>(this System.Object obj, string name) {
			FieldInfo field = obj.GetField(name);
			if (field != null) {
				if (field.FieldType.IsAssignableFrom(typeof(T))) { return (T) field.GetValue(obj); }
				
				Debug.LogWarning("ReflectionExtensions.GetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
				return default(T);
			}
			Debug.LogWarning("ReflectionExtensions.GetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			return default(T);
		}
		
		/// <summary> Get the System.Object value of a field named <paramref name="name"/> on <paramref name="obj"/> </summary>
		/// <param name="obj"> Object to get field value from </param>
		/// <param name="name"> Name of field to get </param>
		/// <returns> Value of the field named <paramref name="name"/> on <paramref name="obj"/>, or null if it does not exist. </returns>
		public static System.Object GetRawFieldValue(this System.Object obj, string name) {
			FieldInfo field = obj.GetField(name);
			if (field != null) {
				if (field.FieldType.IsAssignableFrom(typeof(System.Object))) {return field.GetValue(obj); }
				
				Debug.LogWarning("ReflectionExtensions.GetRawFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
				return null;
			}
			Debug.LogWarning("ReflectionExtensions.GetRawFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			return null;
		}
		
		/// <summary> Set the field named <paramref name="name"/> on <paramref name="obj"/> to <paramref name="value"/> </summary>
		/// <param name="obj"> Object to set field value on </param>
		/// <param name="name"> Name of field to set </param>
		/// <param name="value"> Value to set field to </param>
		/// <returns> True if the set occurred, false otherwise </returns>
		public static bool SetFieldValue(this System.Object obj, string name, System.Object value) {
			FieldInfo field = obj.GetField(name);
			if (field != null) {
				if (field.FieldType.IsAssignableFrom(value.GetType())) {
					field.SetValue(obj, value);
					return true;
				}
				
				Debug.LogWarning("ReflectionExtensions.SetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
				return true;
			}
			
			Debug.LogWarning("ReflectionExtensions.SetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
			return false;
		}
		/// <summary> Check if <paramref name="info"/> is declared in a different type than the one it was reflected from </summary>
		/// <param name="info"> MethodInfo object to check </param>
		/// <returns> True if this <paramref name="info"/> was declared in a different type than the one it was reflected from </returns>
		public static bool IsInherited(this MemberInfo info) { return info.DeclaringType != info.ReflectedType; }

		/// <summary> Check if <paramref name="info"/> is a public get/set property </summary>
		/// <param name="info"> PropertyInfo to check </param>
		/// <returns> True if all of the get/set methods that are present are public </returns>
		public static bool IsPublic(this PropertyInfo info) {
			MethodInfo getter = info.GetGetMethod();
			MethodInfo setter = info.GetSetMethod();
			if (getter == null && setter == null) { return false; }
			
			if (getter != null && setter != null) {
				return getter.IsPublic && setter.IsPublic;
				
			} else if (setter == null) {
				return getter.IsPublic;
			} else {
				return setter.IsPublic;
			}
		}

		/// <summary> Check if <paramref name="info"/> is a private get/set property </summary>
		/// <param name="info"> PropertyInfo to check </param>
		/// <returns> True if all of the get/set methods that are present are private </returns>
		public static bool IsPrivate(this PropertyInfo info) {
			MethodInfo getter = info.GetGetMethod();
			MethodInfo setter = info.GetSetMethod();
			if (getter == null && setter == null) { return false; }
			
			if (getter != null && setter != null) {
				return getter.IsPrivate && setter.IsPrivate;
				
			} else if (setter == null) {
				return getter.IsPrivate;
			} else {
				return setter.IsPrivate;
			}
		}

		/// <summary> Check if <paramref name="info"/> is a static get/set property </summary>
		/// <param name="info"> PropertyInfo to check </param>
		/// <returns> True if all of the get/set methods that are present are static </returns>
		public static bool IsStatic(this PropertyInfo info) {
			MethodInfo getter = info.GetGetMethod();
			MethodInfo setter = info.GetSetMethod();
			if (getter == null && setter == null) { return false; }
			
			if (getter != null && setter != null) {
				return getter.IsStatic && setter.IsStatic;
				
			} else if (setter == null) {
				return getter.IsStatic;
			} else {
				return setter.IsStatic;
			}
		}

		/// <summary> Cached GetType() method to speed up access </summary>
		/// <param name="typeName"> Name of Type to check for </param>
		/// <returns> Type object for <paramref name="typeName"/> </returns>
		public static Type GetType(string typeName) {
			if (_cachedTypes.ContainsKey(typeName)) { return _cachedTypes[typeName]; }
			Type type = Type.GetType(typeName);
			_cachedTypes[typeName] = type;
			return type;
		}

		/// <summary> Searches all assemblies in the <c>assemblies</c> IEnumerable for a type with the specified <paramref name="targetTypeName"/>. </summary>
		/// <param name="targetTypeName">The name of the <c>System.Type</c> to search for.</param>
		/// <returns>The <c>System.Type</c> corresponding to <paramref name="targetTypeName"/>, or <c>null</c> if no such <c>Type</c> was found.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="targetTypeName"/> was <c>null</c>.</exception>
		public static Type GetTypeInUnityAssemblies(string targetTypeName) {
			if (targetTypeName == null) { throw new ArgumentNullException(); }
			foreach (string assembly in ReflectionExtensions.assemblies) {
				Type targetClass = assembly.Length > 0 ? GetType(targetTypeName + assembly) : GetType(targetTypeName);
				if (targetClass != null) {
					return targetClass;
				}
			}
			return null;
		}
		
		/// <summary> List all the members in a given <paramref name="type"/> in Unity's console </summary>
		/// <param name="type"> Type to list </param>
		/// <param name="showPrivate"> Show private members? </param>
		/// <param name="showHidden"> Show hidden members? </param>
		public static void ListAllMembers(this Type type, bool showPrivate = false, bool showHidden = false) { Debug.Log(type.Summary(showPrivate, showHidden)); }

		/// <summary> Get a string holding the summary of the given <paramref name="type"/> </summary>
		/// <param name="type"> Type to get summary for </param>
		/// <param name="showPrivate"> Show private members? </param>
		/// <param name="showHidden"> Show hidden members? </param>
		/// <returns> Details of <paramref name="type"/> as a string </returns>
		public static string Summary(this Type type, bool showPrivate = false, bool showHidden = false) {
			//BindingFlags allPublic = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty;
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
			if (showPrivate) { flags = flags | BindingFlags.NonPublic; }
			
			List<FieldInfo> fields = type.GetFields(flags).ToList();
			List<ConstructorInfo> constructors = type.GetConstructors(flags).ToList();
			List<PropertyInfo> properties = type.GetProperties(flags).ToList();
			List<MethodInfo> methods = type.GetMethods(flags).ToList();
			
			fields.Sort(CompareFields);
			methods.Sort(CompareMethods);
			
			Type lastInheritedType = null;
			StringBuilder output = new StringBuilder("");
			
			if (type.IsPublic) { output += "public "; }
			if (type.IsInterface) {
				output += "interface ";
			} else {
				if (type.IsAbstract) { output += "abstract "; }
				output += "class ";
			}
			output += (type.ShortName());
			
			Type[] interfaces = type.GetInterfaces();
			if (type.BaseType == typeof(System.Object) || type.BaseType == null) {
				if (interfaces.Length > 0) { output += " : "; }
			} else {
				output += " : " + type.BaseType.ShortName();
				if (interfaces.Length > 0) { output += ", "; }
			}

			for (int i = 0; i < interfaces.Length; i++) {
				output += (interfaces[i].ShortName());
				if (i != interfaces.Length-1) { output += ", "; }
			}
			
			output += " {";
			
			output += "\n\n\t//Fields:----------------------------------------------\n";
			foreach (FieldInfo info in fields) {
				if (info.IsInherited()) {
					if (info.DeclaringType != lastInheritedType) {
						lastInheritedType = info.DeclaringType;
						output += "\n\t//Inherited from <" + info.DeclaringType.ToString() + ">\n";
						
					}
				}
				output += "\t" + info.Summary() + "\n";
			}
			
			lastInheritedType = null;
			output += "\n\n\t//Properties:----------------------------------------------\n";
			foreach (PropertyInfo info in properties) {
				if (info.IsInherited()) {
					if (info.DeclaringType != lastInheritedType) {
						lastInheritedType = info.DeclaringType;
						output += "\n\t//Inherited from <" + info.DeclaringType.ToString() + ">\n";
						
					}
				}
				output += "\t" + info.Summary() + "\n";
			}
			
			output += "\n\n\t//Constructors:----------------------------------------------\n";
			foreach (ConstructorInfo info in constructors) {
				output += "\t" + info.Summary() + "\n";
			}
			
			//Give summary of each method info
			lastInheritedType = null;
			output += "\n\n\t//Methods:----------------------------------------------\n";
			foreach (MethodInfo info in methods) {
				if (info.IsSpecialName && !showHidden) { continue; }
				if (info.IsInherited()) {
					if (info.DeclaringType != lastInheritedType) {
						lastInheritedType = info.DeclaringType;
						output += "\n\t//Inherited from <" + info.DeclaringType.ToString() + ">\n";
						
					}
				}
				output += "\t" + info.Summary() + "\n";
			}
			
			output += "\n}";
			
			return output.ToString();
		}

		/// <summary> Iterable of assembly names to check </summary>
		public static IEnumerable<string> assemblies {
			get {
				yield return "";
				yield return ",UnityEngine";
	#if UNITY_EDITOR
				yield return ",UnityEditor";
	#endif
				yield return ",Assembly-UnityScript";
				yield return ",Assembly-CSharp";
	#if UNITY_EDITOR
				yield return ",Assembly-UnityScript-Editor";
				yield return ",Assembly-CSharp-Editor";
	#endif
				yield return ",Assembly-UnityScript-firstpass";
				yield return ",Assembly-CSharp-firstpass";
	#if UNITY_EDITOR
				yield return ",Assembly-UnityScript-Editor-firstpass";
				yield return ",Assembly-CSharp-Editor-firstpass";
	#endif
			}
		}
		
		/// <summary> Compare two method infos to order them </summary>
		/// <param name="info"> first info </param>
		/// <param name="other"> second info </param>
		/// <returns> int describing order of <paramref name="info"/> and <paramref name="other"/> </returns>
		public static int CompareMethods(this MethodInfo info, MethodInfo other) {
			if (info.IsInherited() == other.IsInherited()) {
				if (info.DeclaringType == other.DeclaringType) {
					if (info.IsStatic == other.IsStatic) { 
						if (info.IsPublic == other.IsPublic) {
							if (info.IsPrivate == other.IsPrivate) {
								if (info.IsAbstract == other.IsAbstract) {
									if (info.IsVirtual == other.IsVirtual) {
										if (info.ReturnType == other.ReturnType) {
											
											return 0;
											
										} else { return info.ReturnType.ShortName().CompareTo(other.ReturnType.ShortName()); }
									} else { return (info.IsVirtual ? 1 : -1); }
								} else { return (info.IsAbstract ? 1 : -1); }
							} else { return (info.IsPrivate ? -1 : 1); }
						} else { return (info.IsPublic ? -1 : 1); }
					} else { return (info.IsStatic ? -1 : 1); }
				} else { return info.DeclaringType.ShortName().CompareTo(other.DeclaringType.ShortName()); }
			} else { return (info.IsInherited() ? 1 : -1); }
			//return 0;
		}
		/// <summary> Compare two field infos to order them </summary>
		/// <param name="info"> first info </param>
		/// <param name="other"> second info </param>
		/// <returns> int describing order of <paramref name="info"/> and <paramref name="other"/> </returns>
		public static int CompareFields(this FieldInfo info, FieldInfo other) {
			if (info.IsInherited() == other.IsInherited()) {
				if (info.DeclaringType == other.DeclaringType) {
					if (info.IsStatic == other.IsStatic) { 
						if (info.IsPublic == other.IsPublic) {
							if (info.IsNotSerialized == other.IsNotSerialized) {
								if (info.IsPrivate == other.IsPrivate) {
									
									return 0;
									
								} else { return (info.IsPrivate ? -1 : 1); }
							} else { return (info.IsNotSerialized ? -1 : 1); }
						} else { return (info.IsPublic ? -1 : 1); }
					} else { return (info.IsStatic ? -1 : 1); }
				} else { return info.DeclaringType.ShortName().CompareTo(other.DeclaringType.ShortName()); }
			} else { return (info.IsInherited() ? 1 : -1); }
		}
		
		/// <summary> Get a summary of the Attributes of a given MemberInfo </summary>
		/// <param name="info"> Info to get Attribute summary for </param>
		/// <returns> Attribute summary for <paramref name="info"/> </returns>
		public static string AttributeSummary(this MemberInfo info) {
			StringBuilder str = new StringBuilder();

			var atts = info.GetCustomAttributes(typeof(Attribute), true);
			foreach (var attInfo in atts) {
				str += "[" + attInfo.GetType().FullName + "]" + (atts.Length > 1 ? "\n" : "");
			}

			return str.ToString();
		}

		/// <summary> Get a nicely formatted summary for parameters </summary>
		/// <param name="pinfos"> Parameter infos to format </param>
		/// <returns> Nicely formatted summary of given parameters </returns>
		public static string ParameterSummary(this ParameterInfo[] pinfos) {
			StringBuilder str = new StringBuilder();

			for (int i = 0; i < pinfos.Length; i++) {
				ParameterInfo pinfo = pinfos[i];
				if (pinfo.IsOut) { str += "out "; }
				str += (pinfo.ParameterType.ShortName() + " " + pinfo.Name);
				/*
				//Unity's Mono does not have this functionality... :(
				if (pinfo.HasDefaultValue) {
					str += " = " + pinfo.DefaultValue.ToString().RemoveAll("\n"));
				}
				//*/
				//

				if (i < pinfos.Length - 1) { str += ", "; }

			}
			return str.ToString();
		}

		/// <summary> Nicely formatted summary for ConstructorInfo</summary>
		/// <param name="info"> info to format </param>
		/// <returns> Nicely formatted summary of given <paramref name="info"/> </returns>
		public static string Summary(this ConstructorInfo info) {
			StringBuilder str = new StringBuilder();
			str += info.AttributeSummary();

			if (info.IsPublic) { str += "public "; }
			else if (info.IsPrivate) { str += "private "; } 

			if (info.IsFamily) { str += "protected "; }
			if (info.IsAssembly) { str += "internal "; }
			
			str += (info.DeclaringType.Name + "(");
			str += info.GetParameters().ParameterSummary();
			str += ");";
			
			return str.ToString();
		}

		/// <summary> Nicely formatted summary for MethodInfo </summary>
		/// <param name="info"> MethodInfo to format </param>
		/// <returns> Nicely formatted summary of given <paramref name="info"/> </returns>
		public static string Summary(this MethodInfo info) {
			StringBuilder str = new StringBuilder();
			str += info.AttributeSummary();

			if (info.IsPublic) { str += "public "; }
			else if (info.IsPrivate) { str += "private "; } 
			
			if (info.IsFamily) { str += "protected "; }
			if (info.IsAssembly) { str += "internal "; }
			if (info.IsSpecialName) { str += "hidden "; }
			
			if (info.IsStatic) {
				str += "static ";
			} else {
				if (info.IsVirtual) { str += "virtual "; }
				if (info.IsAbstract) { str += "abstract "; }
			}
			
			str += (info.ReturnType.ShortName() + " " + info.Name + "(");
			str += info.GetParameters().ParameterSummary();			
			str += ");";
			
			return str.ToString();
		}
		
		/// <summary> Nicely formatted summary for PropertyInfo </summary>
		/// <param name="info"> info to format </param>
		/// <returns> Nicely formatted summary of given <paramref name="info"/> </returns>
		public static string Summary(this PropertyInfo info) {
			StringBuilder str = new StringBuilder("");
			str += info.AttributeSummary();
			
			if (info.IsPublic()) { str += "public "; }
			else if (info.IsPrivate()) { str += "private "; } 
			/*
			if (info.IsFamily) { str += "protected "); }
			if (info.IsAssembly) { str += "internal "); }
			*/
			
			if (info.IsSpecialName) { str += "hidden "; }
			if (info.IsStatic()) { str += "static "; }
			
			str += (info.PropertyType.ShortName() + " " + info.Name + " {");
			
			MethodInfo getter = info.GetGetMethod();
			MethodInfo setter = info.GetSetMethod();
			
			if (getter != null) { str += " get; "; }
			if (setter != null) { str += " set; "; }
			
			str += "}";
			
			return str.ToString();
		}
		
		/// <summary> Nicely formatted summary for FieldInfo </summary>
		/// <param name="info"> info to format </param>
		/// <returns> Nicely formatted summary of given <paramref name="info"/> </returns>
		public static string Summary(this FieldInfo info) {
			StringBuilder str = new StringBuilder("");
			str += info.AttributeSummary();

			if (info.IsPublic) { str += "public "; }
			else if (info.IsPrivate) { str += "private "; } 
			
			if (info.IsFamily) { str += "protected "; }
			if (info.IsAssembly) { str += "internal "; }
			if (info.IsSpecialName) { str += "hidden "; }
			if (info.IsStatic) { str += "static "; }
			
			str += (info.FieldType.ShortName() + " " + info.Name + ";");
			
			return str.ToString();
		}
		
	}
}
