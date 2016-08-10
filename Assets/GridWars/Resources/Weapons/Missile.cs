using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Missile : Projectile {
	public float thrust = 10;

	public override void Start () {
		base.Start();
	}

	public override void FixedUpdate () {
		rigidBody().AddForce (transform.forward * thrust);
	}

}
