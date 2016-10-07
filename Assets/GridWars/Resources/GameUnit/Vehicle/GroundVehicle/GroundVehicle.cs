using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundVehicle : Vehicle {

	public override void ServerFixedUpdate () {
		base.ServerFixedUpdate ();

		SteerTowardsTarget ();

		// Drive!
		if (target != null && WheelsAreTouchingGround () && !IsInStandoffRange()) {
			rigidBody ().AddForce (_t.forward * AvailableThrust());
		}

		// Die if flipped
		if (IsAtWeirdAngle()) {
			Die();
		}
	}
		
	public bool WheelsAreTouchingGround() {
		return true;
		//return DiffWithWorldUpAngle() < 5f;
	}
		
	public bool IsAtWeirdAngle() {
		return DiffWithWorldUpAngle() > 70f;
	}

	public float DiffWithWorldUpAngle() {
		Vector3 worldUp = new Vector3(0, 1, 0);
		float a1 = AngleBetweenOnAxis (_t.up, worldUp, _t.forward); 
		float a2 = AngleBetweenOnAxis (_t.up, worldUp, _t.right); 
		return Mathf.Abs(a1) + Mathf.Abs(a2);
	}

}
