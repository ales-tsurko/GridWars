using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSpell : Spell {

	override public void ServerInit () {
		base.ServerInit();
		gameUnit.AdjustWeaponsRangeByFactor(1.5f);
	}

	override public void ServerStop () {
		base.ServerStop();
		gameUnit.SetArmor(1f/1.5f);
	}

	/*
	override public void ServerFixedUpdate () {
		base.ServerFixedUpdate();
	}
	*/
}
