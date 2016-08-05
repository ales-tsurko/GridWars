using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GameUnit {
	void Start () {
		thrust = 14;
	}

	GameObject turret() {
		return transform.Find("headdus1").gameObject;
	}

	public override Vector3 forwardVector() {
		return transform.forward;
	}

	public override Vector3 upVector() {
		return transform.up;
	}


	public override void FixedUpdate () {
		//base.FixedUpdate();
		rigidBody().AddForce(forwardVector() * thrust);

		Object_rotDY (turret (), 0.1f);
		aimTowardsNearestEnemy ();

	}

}