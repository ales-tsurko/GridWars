using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSpell : Spell {

	override public float Cost() {
		return 25f;
	}

	override public float LifeSpan() {
		return 1000f;
	}

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();
		factor = 1.5f;
		gameUnit.AdjustWeaponsRangeByFactor(factor);
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();
		gameUnit.AdjustWeaponsRangeByFactor(1f/factor);
	}
}
