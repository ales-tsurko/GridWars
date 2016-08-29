using UnityEngine;
using System.Collections;

public class MobileSAM : GroundVehicle {


	public override GameObject DefaultTarget() {
		return ClosestOfObjects(EnemyBuildings());
	}

}
