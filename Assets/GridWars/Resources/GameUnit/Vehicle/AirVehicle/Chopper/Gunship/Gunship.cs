using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gunship : Chopper {

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 10f, 18f, float.MaxValue };
	}

	public override void ServerInit() {
		base.ServerInit();
		maxHitPoints = 35f;
		hitPoints = maxHitPoints;
	}

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();

		Weapon w = Weapons()[0];

		if (w) {
			w.damageAdjustments.Add(typeof(MobileSAM), 0.7f);
			w.damageAdjustments.Add(typeof(Tower), 0.4f);
		}
	}

	public override List<System.Type> CountersTypes() {
		List<System.Type> counters = base.CountersTypes();
		counters.Add(typeof(Tank));
		counters.Add(typeof(Tanker));
		counters.Add(typeof(MobileSAM));
		return counters;
	}

	public override void UpgradeVeterancy() {
		//base.UpgradeVeterancy(); // commenting this out is a hack
		veteranLevel ++;

		AdjustWeaponsRangeByFactor(1.2f);

		//AdjustWeaponsFireRateByFactor(1.1f);
		//AdjustWeaponsDamageByFactor(1.1f);
		//AdjustMaxHitpointsByFactor(1.1f);
	}
}
