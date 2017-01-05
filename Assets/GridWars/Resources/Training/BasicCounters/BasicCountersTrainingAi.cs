using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCountersTrainingAi : TrainingAi {
	public override void AI() {
		base.AI();

		foreach (var tower in player.fortress.towers) {
			if (player.EnemyUnitsOfType(tower.unitPrefab.GameUnit().CountersTypes()[0]).Count > 0) {
				tower.SendAttemptQueueUnit(0);
				return;
			}
		}

		player.fortress.towers.PickRandom<Tower>().SendAttemptQueueUnit(0);
	}
}
