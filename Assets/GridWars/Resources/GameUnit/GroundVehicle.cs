using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundVehicle : GameUnit {

	public override void FixedUpdate () {
		base.FixedUpdate ();

		if (WheelsAreTouchingGround () && !IsInStandoffRange()) {
			rigidBody ().AddForce (_t.forward * thrust);
		}

		RemoveIfOutOfBounds ();
	}

	public bool IsInStandoffRange() {
		//return false;
		return target && (targetDistance() < standOffDistance);
	}

	public bool WheelsAreTouchingGround() {
		return true;
		/*
		float angle = AngleBetweenOnAxis (_t.up, battlePlane.up, _t.right);
		return angle < 10.0f;
		*/
	}

}
