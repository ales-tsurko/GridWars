using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Missile : Projectile {
	
	public override void ServerJoinedGame () {
		thrust = 10f;
		base.ServerJoinedGame();
	}

	public override void ServerFixedUpdate () {
		rigidBody().AddForce (transform.forward * thrust);
	}

}
