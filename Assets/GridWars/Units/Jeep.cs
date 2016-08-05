using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jeep : GameUnit {
	void Start () {
		thrust = 20;
	}
		
	public override Vector3 forwardVector() {
		return transform.forward;
	}

	public override Vector3 upVector() {
		return transform.up;
	}


	public override void FixedUpdate () {
		base.FixedUpdate();
		aimTowardsNearestEnemy ();

	}

}