using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSpell : Spell {


	override public float Cost() {
		return 7f;
	}

	override public float LifeSpan() {
		return 2.7f;
	}

	override public void ServerInit () {
		base.ServerInit();

		factor = 1.3f;

		gameUnit.AdjustWeaponsFireRateByFactor(factor);
		gameUnit.AdjustThrustByFactor(factor);
	}

	override public void ServerStop () {
		base.ServerStop();

		gameUnit.AdjustWeaponsFireRateByFactor(1.0f/factor);
		gameUnit.AdjustThrustByFactor(1.0f/factor);
	}
	
	/*
	override public void ServerFixedUpdate () {
		base.ServerFixedUpdate();
	}
	*/
}
