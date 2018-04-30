using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName="NewFloatRef", menuName="Vars/Create New FloatRef")]
public class FloatRef : ScriptableObject {
	public float Constant;
	public FloatVar Variable;
	public float value { get { return Variable != null ? Variable.value : Constant; } }
}
