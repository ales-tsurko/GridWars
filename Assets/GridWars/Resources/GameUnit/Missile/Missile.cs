using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Missile : Projectile {
	
	public override void Start () {
		thrust = 10f;
		base.Start();
	}

	public override void FixedUpdate () {
		rigidBody().AddForce (transform.forward * thrust);
	}

}
