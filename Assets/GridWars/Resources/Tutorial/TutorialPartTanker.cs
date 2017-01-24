using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartTanker : TutorialPartDelegate {
	public override void DidBegin() {
		App.shared.battlefield.player1.powerSource.power = GameUnit.Load<Tanker>().PowerCost(0);
		foreach(var tower in App.shared.battlefield.player1.fortress.towers) {
			tower.isDisabled = !(tower.unitPrefab.GameUnit() is Tanker);
		}
		PauseBattlefield();
	}
}
