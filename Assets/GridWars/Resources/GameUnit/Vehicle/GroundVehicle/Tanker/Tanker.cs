using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tanker : GroundVehicle {
	public Explosion prefabBombExplosion;

	public override void Start() {
		base.Start();
	}
		
	public override void MasterFixedUpdate() {
		base.MasterFixedUpdate();

		if (IsInStandoffRange()) {
			OnDead();
		}
	}

	public void BlowUp() {
		var initialState = new GameUnitState();
		initialState.prefabGameUnit = prefabBombExplosion.GetComponent<BigBoom>();
		initialState.player = player;
		initialState.transform = transform;
		initialState.InstantiateGameUnit();
	}

	public override void OnDead() {
		base.OnDead();
		BlowUp();
	}

}