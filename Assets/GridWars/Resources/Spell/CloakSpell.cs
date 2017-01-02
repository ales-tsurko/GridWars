using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakSpell : Spell {

	override public float Cost() {
		return 15f;
	}

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();

		gameUnit.SetIsCloaked(true);
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();

		gameUnit.SetIsCloaked(false);
	}
}
