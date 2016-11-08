using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileSAM : GroundVehicle {

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 8f, 8f*2f, float.MaxValue };
	}

	//NetworkObject

	public override void ServerInit() {
		maxHitPoints = 11f;
		base.ServerInit();
	}

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();

		foreach(Weapon weapon in Weapons()) {
			weapon.damageAdjustments.Add(typeof(Tower), 0.2f);
			weapon.damageAdjustments.Add(typeof(Tank), 0.4f);
			weapon.SetCanTargetGroundBuildings(true);
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

	public override void UpgradeVeterancy() {
		/*
		weaponRange *= 1.5;
		energy.gen *= 1.5;
		*/

		base.UpgradeVeterancy();
		AdjustWeaponsRangeByFactor(1.2f);
		AdjustWeaponsFireRateByFactor(1.5f);
		AdjustHitPointGenByFactor(1.5f);
	}
}
