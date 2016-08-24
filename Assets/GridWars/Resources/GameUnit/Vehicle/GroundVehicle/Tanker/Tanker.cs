using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tanker : GroundVehicle {

	public override void Start() {
		base.Start();
	}


	public override void FixedUpdate() {
		base.FixedUpdate();

		if (IsInStandoffRange()) {
			hitPoints = 0;
		}
	}

	public void BlowUp() {


	}

	public override void OnDead() {
		base.OnDead();
		BlowUp();
	}

}