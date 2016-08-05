using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightTank : GameUnit {
	void Start () {
		thrust = 18;
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