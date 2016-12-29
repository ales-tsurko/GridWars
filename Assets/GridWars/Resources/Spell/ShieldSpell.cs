using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : Spell {

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
