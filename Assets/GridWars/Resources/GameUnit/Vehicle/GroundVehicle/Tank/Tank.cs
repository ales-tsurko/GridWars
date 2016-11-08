using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GroundVehicle {

	//NetworkObject

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 4.2f, 12f, float.MaxValue };
	}

	public override void ServerInit() {
		maxHitPoints = 15f;
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
		/*
		weaponDamage *= 1.5;
		energy.max *= 1.5;
		health.value += 4;
		health.max *= 2;
		*/
		base.UpgradeVeterancy();

		AdjustWeaponsFireRateByFactor(1.5f);
		AdjustWeaponsDamageByFactor(1.5f);
		AdjustMaxHitpointsByFactor(2f);
	}
}