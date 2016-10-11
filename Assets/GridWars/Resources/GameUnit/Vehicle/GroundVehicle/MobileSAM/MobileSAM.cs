using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileSAM : GroundVehicle {

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

		if (veteranLevel == 2) {
			foreach (Weapon weapon in Weapons()) {
				weapon.SetCanTargetGround(true);
			}
		}
	}
}
