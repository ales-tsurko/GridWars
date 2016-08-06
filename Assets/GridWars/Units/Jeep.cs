using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jeep : GameUnit {
	public override void Start () {
		base.Start();
		thrust = 35;
		rotationThrust = 4.0f;
	}


	public override void FixedUpdate () {
		base.FixedUpdate();
		steerTowardsNearestEnemy ();

	}

}