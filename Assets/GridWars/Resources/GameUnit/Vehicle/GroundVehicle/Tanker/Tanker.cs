using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tanker : GroundVehicle {
	public Explosion prefabBombExplosion;

	public override void Start() {
		base.Start();
	}


	public override void FixedUpdate() {
		base.FixedUpdate();

		if (IsInStandoffRange()) {
			hitPoints = 0;
		}
	}

	public void BlowUp() {
		var initialState = new InitialGameUnitState();
		initialState.position = transform.position;
		initialState.rotation = transform.rotation;
		initialState.player = player;

		var projUnit = (BigBoom) prefabBombExplosion.GetComponent<BigBoom>().Instantiate(initialState);
		//projUnit.IgnoreCollisionsWith(this);
	}

	public override void OnDead() {
		base.OnDead();
		BlowUp();
	}

}