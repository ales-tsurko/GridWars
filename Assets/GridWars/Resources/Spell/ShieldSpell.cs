using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : Spell {

	override public float Cost() {
		return 7f;
	}

	override public float LifeSpan() {
		return 3.3f;
	}

	override public void ServerInit () {
		base.ServerInit();

		gameUnit.SetArmor(0.9f);
	}

	override public void ServerStop () {
		base.ServerStop();
		gameUnit.SetArmor(0.0f);
	}

	/*
	override public void ServerFixedUpdate () {
		base.ServerFixedUpdate();
	}
	*/
}
