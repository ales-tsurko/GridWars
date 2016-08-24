using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundVehicle : Vehicle {

	public override void MasterFixedUpdate () {
		base.MasterFixedUpdate ();

		PickTarget ();
		SteerTowardsTarget ();

		if (WheelsAreTouchingGround () && !IsInStandoffRange()) {
			rigidBody ().AddForce (_t.forward * thrust);
		}
	}
		
	public bool WheelsAreTouchingGround() {
		return true;
		/*
		float angle = AngleBetweenOnAxis (_t.up, battlePlane.up, _t.right);
		return angle < 10.0f;
		*/
	}

}
