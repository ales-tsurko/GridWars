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
		initialState.player = player;
		//var projUnit = (BigBoom) 
		prefabBombExplosion.GetComponent<BigBoom>().Instantiate(transform.position, transform.rotation, initialState);
		//projUnit.IgnoreCollisionsWith(this);
	}

	public override void OnDead() {
		base.OnDead();
		BlowUp();
	}

}