using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : GameUnit {

	public override void Start () {
		base.Start();
		thrust = 10;
		rotationThrust = 0.01f;
	}
		
	GameObject mainRotor() {
		return transform.Find("Group003").gameObject;
	}

	GameObject tailRotor() {
		return transform.Find("Group006").gameObject;
	}

		
	public override void FixedUpdate () {
		//base.FixedUpdate();

		Object_rotDY (mainRotor (), 20);
		Object_rotDY (tailRotor (), 20);

		//setY(6);
		float cruiseHeight = 15f;
		float diff = cruiseHeight - y ();

		if (y () < cruiseHeight) {
			rigidBody ().AddForce (_t.up * 6 * Mathf.Sqrt(diff));
		} 
		aimTowardsNearestEnemy();

		if (y () > 4) {
			rigidBody().AddForce(_t.forward * thrust);
		}

		/*
		if (diff > 0) {
			setRotZ ( diff * 2);
		}
		*/
		//aimTowardsNearestEnemy ();

	}
		

}
