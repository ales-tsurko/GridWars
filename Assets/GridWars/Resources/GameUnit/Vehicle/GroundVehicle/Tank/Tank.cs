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
}