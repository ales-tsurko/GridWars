using UnityEngine;
using System.Collections;

public class BigBomb : Projectile {

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();
	}

	public override void ServerFixedUpdate() {
	}

	public override void ServerAndClientFixedUpdate() {
	}
}

