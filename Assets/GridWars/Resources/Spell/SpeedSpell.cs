using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSpell : Spell {


	override public float Cost() {
		return 10f;
	}

	override public float LifeSpan() {
		return 2.7f;
	}

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();

		factor = 1.3f;

		gameUnit.AdjustWeaponsFireRateByFactor(factor);
		gameUnit.AdjustThrustByFactor(factor);
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();

		gameUnit.AdjustWeaponsFireRateByFactor(1.0f/factor);
		gameUnit.AdjustThrustByFactor(1.0f/factor);
	}
}
