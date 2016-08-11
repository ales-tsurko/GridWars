using UnityEngine;
using System.Collections;

public class MobileSAM : GroundVehicle {

	public override void Start () {
		base.Start();

		//thrust = 850;
		//rotationThrust = 60;
	}
	
	public override void FixedUpdate () {
		base.FixedUpdate();
		pickTarget ();
		steerTowardsNearestEnemy ();
	}
}
