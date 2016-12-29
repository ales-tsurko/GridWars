using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakSpell : Spell {

	public float Cost() {
		return 15f;
	}

	override public void ServerInit () {
		base.ServerInit();

		gameUnit.SetIsCloaked(true);
	}

	override public void ServerStop () {
		base.ServerStop();

		gameUnit.SetIsCloaked(false);
	}


	/*
	override public void ServerFixedUpdate () {
		base.ServerFixedUpdate();
	}
	*/
}
