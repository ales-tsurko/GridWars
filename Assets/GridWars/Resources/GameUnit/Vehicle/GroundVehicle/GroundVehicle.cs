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

		if (IsAtWeirdAngle()) {
			OnDead();
		}
	}
		
	public bool WheelsAreTouchingGround() {
		return DiffWithWorldUpAngle() < 5f;
	}
		
	public bool IsAtWeirdAngle() {
		return DiffWithWorldUpAngle() > 10f;
	}

	public float DiffWithWorldUpAngle() {
		Vector3 worldUp = new Vector3(0, 1, 0);
		float a1 = AngleBetweenOnAxis (_t.up, worldUp, _t.forward); 
		float a2 = AngleBetweenOnAxis (_t.up, worldUp, _t.right); 
		return Mathf.Abs(a1) + Mathf.Abs(a2);
	}

}
