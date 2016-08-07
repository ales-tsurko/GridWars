using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightTank : GroundVehicle {
	public override void Start () {
		base.Start();
		thrust = 190;
		rotationThrust = 40;
	}

	public override void FixedUpdate () {
		base.FixedUpdate();
		steerTowardsNearestEnemy ();
	}

}