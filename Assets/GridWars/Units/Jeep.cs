using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jeep : GameUnit {
	public override void Start () {
		base.Start();
		thrust = 20;
	}


	public override void FixedUpdate () {
		base.FixedUpdate();
		aimTowardsNearestEnemy ();

	}

}