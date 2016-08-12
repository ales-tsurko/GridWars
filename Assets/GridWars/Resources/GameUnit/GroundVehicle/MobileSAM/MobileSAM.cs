using UnityEngine;
using System.Collections;

public class MobileSAM : GroundVehicle {

	public override void FixedUpdate () {
		base.FixedUpdate();
		pickTarget ();
		steerTowardsNearestEnemy ();
	}

}
