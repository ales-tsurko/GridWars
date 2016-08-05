using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jeep : GameUnit {
	void Start () {
		thrust = 20;
	}


	public override void FixedUpdate () {
		//base.FixedUpdate();
		//aimTowardsNearestEnemy ();
		rigidBody().AddForce(- forwardVector() * thrust);

	}

}