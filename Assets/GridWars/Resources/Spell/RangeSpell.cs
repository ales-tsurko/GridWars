using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSpell : Spell {

	override public float Cost() {
		return 20f;
	}

	override public float LifeSpan() {
		return 10f;
	}

	override public void ServerInit () {
		base.ServerInit();
		factor = 1.5f;

		gameUnit.AdjustWeaponsRangeByFactor(factor);
	}

	override public void ServerStop () {
		base.ServerStop();
		gameUnit.AdjustWeaponsRangeByFactor(1f/factor);
	}

	/*
	override public void ServerFixedUpdate () {
		base.ServerFixedUpdate();
	}
	*/
}
