using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightTank : GroundVehicle {

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 4f, 4f*2.5f, float.MaxValue };
	}

	public override void ServerInit() {
		maxHitPoints = 7f;
		base.ServerInit();
	}

	public override List<System.Type> CountersTypes() {
		List<System.Type> counters = base.CountersTypes();
		counters.Add(typeof(MobileSAM));
		counters.Add(typeof(Tank));
		counters.Add(typeof(Tanker));
		//counters.Add(typeof(GroundVehicle));
		return counters;
	}

	public override void UpgradeVeterancy() {
		base.UpgradeVeterancy();

		AdjustWeaponsFireRateByFactor(1.2f);
		AdjustWeaponsDamageByFactor(1.2f);
		AdjustMaxHitpointsByFactor(1.2f);
		//AdjustThrustByFactor(1.2f);
	}
}
