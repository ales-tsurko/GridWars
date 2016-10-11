using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GroundVehicle {

	public override List<System.Type> CountersTypes() {
		List<System.Type> counters = base.CountersTypes();
		counters.Add(typeof(MobileSAM));
		counters.Add(typeof(Tank));
		counters.Add(typeof(Tanker));
		//counters.Add(typeof(GroundVehicle));
		return counters;
	}

	/*
	public virtual void DidChangeVeternLevel() {
		
		if (veteranLevel == 1) {
			AdjustWeaponsRangeByFactor(1.25f);
			AdjustMaxHitpointsByFactor(1.5);
		}

		if (veteranLevel == 2) {
			AdjustWeaponsRangeByFactor(1.25f);
			AdjustMaxHitpointsByFactor(1.5);
		}
	}
	*/
}