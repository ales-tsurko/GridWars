using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCountersTrainingScenario : TrainingScenario {
	public override bool TowerCanQueueUnit(Tower tower) {
		return tower.player.units.FindAll(u => u is Vehicle).Count == 0;
	}
}
