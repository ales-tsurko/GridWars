using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Missile : Projectile {
	
	public override void MasterStart () {
		thrust = 10f;
		base.MasterStart();
	}

	public override void MasterFixedUpdate () {
		rigidBody().AddForce (transform.forward * thrust);
	}

}
