using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jeep : GameUnit {
	public override void Start () {
		base.Start();
		thrust = 45;
		rotationThrust = 10.0f;
	}


	public override void FixedUpdate () {
		base.FixedUpdate();
		aimTowardsNearestEnemy ();

	}

}