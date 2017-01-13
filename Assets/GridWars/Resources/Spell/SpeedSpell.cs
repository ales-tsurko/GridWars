using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSpell : Spell {

	override public float Cost() {
		return 20f;
	}

	override public float LifeSpan() {

		return 2.7f;
	}

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();

		factor = 1.3f;

		gameUnit.AdjustWeaponsFireRateByFactor(factor);
		gameUnit.AdjustThrustByFactor(factor);

		SetTrackColor(new Color(1f, 1f, 0f, 1f));
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();

		gameUnit.AdjustWeaponsFireRateByFactor(1.0f/factor);
		gameUnit.AdjustThrustByFactor(1.0f/factor);

		SetTrackColor(new Color(0f, 0f, 0f, 0.1f));
	}

	void SetTrackColor(Color c) {
		if (gameUnit.inheritsFrom(typeof(GroundVehicle))) {
			GroundVehicle gv = (GroundVehicle)gameUnit;
			gv.SetTrackColor(c);
		}
	}
}
