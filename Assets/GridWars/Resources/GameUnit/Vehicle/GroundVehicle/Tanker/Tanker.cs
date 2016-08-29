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


	public virtual List <GameObject> PossibleDefaultTargets() {
		return NonAirEnemyVehicles();
	}

	public override GameObject DefaultTarget() {
		return ClosestOfObjects(PossibleDefaultTargets());
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

	public override void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);
		if (!enabled) {
			return;
		}

		GameUnit otherUnit = collision.gameObject.GetComponent<GameUnit> ();

		if (otherUnit != null && !IsFriendOf(otherUnit)) {
			if (!otherUnit.IsOfType(typeof(Projectile))) {
				OnDead();
			}
		}
			/*
			if (collision.relativeVelocity.) {
				OnDead()
			}
			*/
	}

}