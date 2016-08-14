using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jeep : GroundVehicle {
	public override void Start () {
		base.Start();
		thrust = 45;
		rotationThrust = 4.0f;
	}


	public override void FixedUpdate () {
		base.FixedUpdate();
		SteerTowardsTarget ();

	}

}