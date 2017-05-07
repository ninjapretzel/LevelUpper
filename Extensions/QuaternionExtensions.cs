using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LevelUpper.Extensions {

	public static class QuaternionExtensions {
		
		//Get the rotation that would be multiplied by to rotate a to b
		public static Quaternion To(this Quaternion a, Quaternion b) { return Quaternion.Inverse(a) * b; }
		
		
		/// <summary> Returns a copy of the quaternion with the eulerAngles Z component set to zero</summary>
		/// <param name="q">Source rotation</param>
		/// <returns>Rotation built from source rotation with the eulerAngles Z component set to zero</returns>
		public static Quaternion FlattenZ(this Quaternion q) {
			Vector3 e = q.eulerAngles;
			e.x = 0;
			return Quaternion.Euler(e);
		}
		
		/// <summary> Returns a copy of the quaternion with the eulerAngles Y component set to zero</summary>
		/// <param name="q">Source rotation</param>
		/// <returns>Rotation built from source rotation with the eulerAngles Y component set to zero</returns>
		public static Quaternion FlattenY(this Quaternion q) {
			Vector3 e = q.eulerAngles;
			e.x = 0;
			return Quaternion.Euler(e);
		}

		/// <summary> Returns a copy of the quaternion with the eulerAngles X component set to zero</summary>
		/// <param name="q">Source rotation</param>
		/// <returns>Rotation built from source rotation with the eulerAngles X component set to zero</returns>
		public static Quaternion FlattenX(this Quaternion q) {
			Vector3 e = q.eulerAngles;
			e.z = 0;
			return Quaternion.Euler(e);
		}
		
		public static byte[] GetBytes(this Quaternion q) {
			byte[] ret = new byte[16];
			byte[] buffer;
			
			buffer = q.x.GetBytes();
			ret[0] = buffer[0];
			ret[1] = buffer[1];
			ret[2] = buffer[2];
			ret[3] = buffer[3];
			
			buffer = q.y.GetBytes();
			ret[4] = buffer[0];
			ret[5] = buffer[1];
			ret[6] = buffer[2];
			ret[7] = buffer[3];
			
			buffer = q.z.GetBytes();
			ret[8] = buffer[0];
			ret[9] = buffer[1];
			ret[10] = buffer[2];
			ret[11] = buffer[3];
			
			buffer = q.w.GetBytes();
			ret[12] = buffer[0];
			ret[13] = buffer[1];
			ret[14] = buffer[2];
			ret[15] = buffer[3];
			
			return ret;
		}
		
		
		
	}
}
