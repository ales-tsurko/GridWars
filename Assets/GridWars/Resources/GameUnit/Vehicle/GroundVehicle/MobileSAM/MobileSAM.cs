using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileSAM : GroundVehicle {

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();

		foreach(Weapon weapon in Weapons()) {
			weapon.damageAdjustments.Add(typeof(Tower), 0.4f);
			weapon.damageAdjustments.Add(typeof(Tank), 0.4f);
		}
	}

	public override GameObject DefaultTarget() {
		return ClosestOfObjects(EnemyBuildings());
	}

	public override List<System.Type> CountersTypes() {
		List<System.Type> counters = base.CountersTypes();
		counters.Add(typeof(Chopper));
		//counters.Add(typeof(AirVehicle));
		return counters;
	}

	public override void DidChangeVeternLevel() {
		base.DidChangeVeternLevel();

		if (veteranLevel == 1) {
			foreach (Weapon weapon in Weapons()) {
				weapon.SetCanTargetGround(true);
			}
		}
	}
}
