using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : GameUnit {

	void Start () {
		thrust = 20;
	}
		
	GameObject mainRotor() {
		return transform.Find("Group003").gameObject;
	}

	GameObject tailRotor() {
		return transform.Find("Group006").gameObject;
	}

	void Object_rotDY(GameObject obj, float dy) {
		var e = obj.transform.eulerAngles;
		obj.transform.eulerAngles = new Vector3(e.x, e.y + dy, e.z);
	}

	public override void FixedUpdate () {
		//base.FixedUpdate();
		rigidBody().AddForce(transform.forward * thrust);

		Object_rotDY (mainRotor (), 20);
		Object_rotDY (tailRotor (), 20);

		setY(6);

	}
		

}
