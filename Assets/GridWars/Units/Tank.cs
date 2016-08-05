using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GameUnit {
	public override void Start () {
		base.Start();
		thrust = 14;
	}

	GameObject turret() {
		return transform.Find("headdus1").gameObject;
	}


	public override void FixedUpdate () {
		base.FixedUpdate();

		Object_rotDY (turret (), 0.1f);
		aimTowardsNearestEnemy ();
	}

}