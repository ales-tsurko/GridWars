using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSpell : Spell {

	public float Cost() {
		return 5f;
	}

	public float LifeSpan() {
		return 10f;
	}

	override public void ServerInit () {
		base.ServerInit();

		gameUnit.AdjustWeaponsFireRateByFactor(1.5f);
		gameUnit.AdjustThrustByFactor(1.5f);
	}

	override public void ServerStop () {
		base.ServerStop();

		gameUnit.AdjustWeaponsFireRateByFactor(1.0f/1.5f);
		gameUnit.AdjustThrustByFactor(1.0f/1.5f);
	}
	
	/*
	override public void ServerFixedUpdate () {
		base.ServerFixedUpdate();
	}
	*/
}
